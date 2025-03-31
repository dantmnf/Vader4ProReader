using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Vader4ProReader.Util;
using Windows.Win32;
using Windows.Win32.Devices.DeviceAndDriverInstallation;
using Windows.Win32.Foundation;
using Windows.Win32.Storage.FileSystem;

namespace Vader4ProReader.Device
{
    public class Vader4ProDevice : IDisposable
    {
        private static readonly Guid GUID_DEVINTERFACE_HID = new Guid("4d1e55b2-f16f-11cf-88cb-001111000030");

        private static unsafe string? GetUsbProductName(string instid)
        {
            CONFIGRET cr;
            cr = NativeMethods.CM_Locate_DevNode(out var devInst, instid, 0);
            if (cr != CONFIGRET.CR_SUCCESS)
            {
                return null;
            }

            uint parentsize = 0;
            cr = PInvoke.CM_Get_DevNode_Property(devInst, PInvoke.DEVPKEY_Device_Parent, out _, null, ref parentsize, 0);
            if (cr != CONFIGRET.CR_BUFFER_SMALL || parentsize == 0)
            {
                return null;
            }

            var parentDevInstBuf = new byte[parentsize];
            cr = NativeMethods.CM_Get_DevNode_Property(devInst, PInvoke.DEVPKEY_Device_Parent, out _, ref parentDevInstBuf[0], ref parentsize, 0);
            if (cr != CONFIGRET.CR_SUCCESS)
            {
                return null;
            }

            // var parentInstId = new string(MemoryMarshal.Cast<byte, char>(parentDevInstBuf.AsSpan()).TrimEnd('\0'));

            cr = NativeMethods.CM_Locate_DevNode(out var parentDevInst, ref parentDevInstBuf[0], 0);
            if (cr != CONFIGRET.CR_SUCCESS)
            {
                return null;
            }

            uint requiredSize = 0;
            cr = PInvoke.CM_Get_DevNode_Property(parentDevInst, PInvoke.DEVPKEY_Device_BusReportedDeviceDesc, out _, null, ref requiredSize, 0);
            if (cr != CONFIGRET.CR_BUFFER_SMALL || requiredSize == 0)
            {
                return null;
            }

            var buffer = new byte[requiredSize];
            cr = NativeMethods.CM_Get_DevNode_Property(parentDevInst, PInvoke.DEVPKEY_Device_BusReportedDeviceDesc, out _, ref buffer[0], ref requiredSize, 0);
            if (cr != CONFIGRET.CR_SUCCESS)
            {
                return null;
            }

            return new string(MemoryMarshal.Cast<byte, char>(buffer.AsSpan())[..^1]);
        }
        public static unsafe string[] EnumerateDevices()
        {
            const string hwid = "HID\\VID_04B4&PID_2412&MI_02";

            var cr = PInvoke.CM_Get_Device_ID_List_Size(out var idListLen, hwid, PInvoke.CM_GETIDLIST_FILTER_PRESENT | PInvoke.CM_GETIDLIST_FILTER_ENUMERATOR);
            if (cr != CONFIGRET.CR_SUCCESS)
            {
                return [];
            }

            var idList = new char[idListLen];
            cr = NativeMethods.CM_Get_Device_ID_List(hwid, ref idList[0], idListLen, PInvoke.CM_GETIDLIST_FILTER_PRESENT | PInvoke.CM_GETIDLIST_FILTER_ENUMERATOR);
            if (cr != CONFIGRET.CR_SUCCESS)
            {
                return [];
            }

            var idListStr = new string(idList);
            var idListArr = idListStr.Split('\0', StringSplitOptions.RemoveEmptyEntries);

            var intfList = new List<string>(idList.Length);
            foreach (var instid in idListArr)
            {
                // All Flydigi devices have the same hardware ID, we need to check the product name to filter out other devices.
                if (string.CompareOrdinal(GetUsbProductName(instid), "Flydigi VADER4") != 0)
                {
                    continue;
                }
                cr = NativeMethods.CM_Get_Device_Interface_List_Size(out var intfListLen, GUID_DEVINTERFACE_HID, instid, CM_GET_DEVICE_INTERFACE_LIST_FLAGS.CM_GET_DEVICE_INTERFACE_LIST_PRESENT);
                if (cr != CONFIGRET.CR_SUCCESS)
                {
                    continue;
                }
                var intflistbuf = new char[intfListLen];
                cr = NativeMethods.CM_Get_Device_Interface_List(GUID_DEVINTERFACE_HID, instid, ref intflistbuf[0], intfListLen, CM_GET_DEVICE_INTERFACE_LIST_FLAGS.CM_GET_DEVICE_INTERFACE_LIST_PRESENT);
                if (cr != CONFIGRET.CR_SUCCESS)
                {
                    continue;
                }
                var intfListStr = new string(intflistbuf);
                var intfListArr = intfListStr.Split('\0', StringSplitOptions.RemoveEmptyEntries);
                intfList.AddRange(intfListArr);
            }

            return intfList.ToArray();
        }

        public static Vader4ProDevice OpenDevice(string devicePath)
        {
            var handle = PInvoke.CreateFile(devicePath, (uint)(GENERIC_ACCESS_RIGHTS.GENERIC_READ | GENERIC_ACCESS_RIGHTS.GENERIC_WRITE), FILE_SHARE_MODE.FILE_SHARE_READ | FILE_SHARE_MODE.FILE_SHARE_WRITE, null, FILE_CREATION_DISPOSITION.OPEN_EXISTING, FILE_FLAGS_AND_ATTRIBUTES.FILE_FLAG_OVERLAPPED, null);
            if (handle.IsInvalid)
            {
                throw new IOException("failed to open device " + devicePath);
            }
            return new Vader4ProDevice(handle);
        }

        private readonly SafeFileHandle device;
        private readonly CancellationTokenSource cts;
        private readonly CancellationToken ct;
        private readonly Thread thr;
        private bool disposed;

        public bool Error { get; private set; }
        public event EventHandler<Vader4ProReport>? ReportReceived;

        private Vader4ProDevice(SafeFileHandle device)
        {
            this.device = device;
            SetLed(0, 0, 0);
            SetRumble(0, 0);
            cts = new CancellationTokenSource();
            ct = cts.Token;
            thr = new Thread(ReadReportThread);
            thr.Start(this);
        }

        ~Vader4ProDevice()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (disposed)
                return;
            cts.Cancel();
            thr.Join();
            cts.Dispose();
            device.Dispose();
            Error = true;
            disposed = true;
            GC.SuppressFinalize(this);
        }

        private static unsafe void ReadReportThread(object? arg)
        {
            if (arg is not Vader4ProDevice thiz)
                return;
            var ct = thiz.ct;
            using var ev = new ManualResetEvent(false);
            NativeOverlapped ov = new() { EventHandle = ev.SafeWaitHandle.DangerousGetHandle() };
            WaitHandle[] waitHandles = [ev, ct.WaitHandle];
            bool pending = false;
            while (!ct.IsCancellationRequested)
            {
                var buf = new byte[32];
                PInvoke.ReadFile(thiz.device, buf, null, &ov);
                var err = Marshal.GetLastWin32Error();
                if (err == 0 || err == (int)WIN32_ERROR.ERROR_IO_PENDING)
                {
                    if (err == (int)WIN32_ERROR.ERROR_IO_PENDING)
                    {
                        var n = WaitHandle.WaitAny(waitHandles);
                        if (n == 1)
                        {
                            // cancelled
                            NativeMethods.CancelIoEx(thiz.device, &ov);
                            break;
                        }
                    }
                    if (NativeMethods.GetOverlappedResult(thiz.device, &ov, out var xferd, false))
                    {
                        pending = false;
                        if (xferd != 32)
                        {
                            Debug.WriteLine($"HID short read: {xferd} bytes transferred");
                            thiz.Error = true;
                            break;
                        }
                        thiz.ReportReceived?.Invoke(thiz, new Vader4ProReport(buf));
                    }
                    else
                    {
                        err = Marshal.GetLastWin32Error();
                        Debug.WriteLine("overlapped ReadFile failed: " + err);
                        thiz.Error = true;
                        break;
                    }
                }
                else
                {
                    Debug.WriteLine("ReadFile failed: " + err);
                    thiz.Error = true;
                    break;
                }
            }
            if (pending)
            {
                NativeMethods.GetOverlappedResult(thiz.device, &ov, out _, true);
            }
        }

        private unsafe void WriteReport(ReadOnlySpan<byte> report)
        {
            ObjectDisposedException.ThrowIf(disposed, this);
            if (report.Length < 32)
            {
                var buf = new byte[32];
                report.CopyTo(buf);
                report = buf;
            }
            using var ev = new ManualResetEvent(false);
            NativeOverlapped ov = new() { EventHandle = ev.SafeWaitHandle.DangerousGetHandle() };
            WaitHandle[] waitHandles = [ev, ct.WaitHandle];
            PInvoke.WriteFile(device, report, null, &ov);
            bool pending = false;
            var err = Marshal.GetLastWin32Error();
            if (err == (int)WIN32_ERROR.ERROR_IO_PENDING)
            {
                pending = true;
                var n = WaitHandle.WaitAny(waitHandles);
                if (n == 1)
                {
                    NativeMethods.CancelIoEx(device, &ov);
                }
                else
                {
                    if (NativeMethods.GetOverlappedResult(device, &ov, out var xferd, false))
                    {
                        pending = false;
                    }
                    else
                    {
                        err = Marshal.GetLastWin32Error();
                        Debug.WriteLine("WriteFile failed: " + err);
                    }
                }
            }

            if (pending)
            {
                NativeMethods.GetOverlappedResult(device, &ov, out _, true);
            }
        }

        /// <summary>
        /// Disable gyro to mouse emulation. FN key to emulation switch is still tracked and will take effect after <see cref="EnableAirMouse"/> call.
        /// </summary>
        public void DisableAirMouse()
        {
            WriteReport([0x05, 0x10, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00]);
        }

        public void EnableAirMouse()
        {
            WriteReport([0x05, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00]);
        }

        public void SetLed(byte r, byte g, byte b)
        {
            WriteReport([0x05, 0xe0, r, g, b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00]);
        }

        /// <summary>
        /// Set rumble values. Will stop after a few seconds, repeat as needed.
        /// </summary>
        public void SetRumble(byte leftMain, byte rightMain, byte leftTrigger = 0, byte rightTrigger = 0)
        {
            WriteReport([0x05, 0x0f, leftMain, rightMain, leftTrigger, rightTrigger, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00]);
        }

    }
}
