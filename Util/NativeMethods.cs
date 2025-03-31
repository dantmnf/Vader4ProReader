using System.Runtime.InteropServices;
using Windows.Win32.Devices.Properties;
using Windows.Win32.Devices.DeviceAndDriverInstallation;
using Microsoft.Win32.SafeHandles;

namespace Vader4ProReader.Util;

public static class NativeMethods
{
	    [DllImport("CFGMGR32.dll", ExactSpelling = true, EntryPoint = "CM_Locate_DevNodeW", CharSet = CharSet.Unicode)]
		internal static extern CONFIGRET CM_Locate_DevNode(out uint pdnDevInst, string? pDeviceID, CM_LOCATE_DEVNODE_FLAGS ulFlags);

        [DllImport("CFGMGR32.dll", ExactSpelling = true, EntryPoint = "CM_Locate_DevNodeW")]
		internal static extern CONFIGRET CM_Locate_DevNode(out uint pdnDevInst, ref byte pDeviceID, CM_LOCATE_DEVNODE_FLAGS ulFlags);

        [DllImport("CFGMGR32.dll", ExactSpelling = true, EntryPoint = "CM_Get_DevNode_PropertyW", CharSet = CharSet.Unicode)]
		internal static extern CONFIGRET CM_Get_DevNode_Property(uint dnDevInst, in DEVPROPKEY PropertyKey, out DEVPROPTYPE PropertyType, ref byte PropertyBuffer, ref uint PropertyBufferSize, uint ulFlags);

        [DllImport("CFGMGR32.dll", ExactSpelling = true, EntryPoint = "CM_Get_Device_ID_ListW", CharSet = CharSet.Unicode)]
		internal static extern CONFIGRET CM_Get_Device_ID_List(string pszFilter, ref char Buffer, uint BufferLen, uint ulFlags);

        [DllImport("CFGMGR32.dll", ExactSpelling = true, EntryPoint = "CM_Get_Device_Interface_ListW", CharSet = CharSet.Unicode)]
		internal static extern unsafe CONFIGRET CM_Get_Device_Interface_List(in Guid InterfaceClassGuid, string pDeviceID, ref char Buffer, uint BufferLen, CM_GET_DEVICE_INTERFACE_LIST_FLAGS ulFlags);
		
        [DllImport("CFGMGR32.dll", ExactSpelling = true, EntryPoint = "CM_Get_Device_Interface_List_SizeW", CharSet = CharSet.Unicode)]
		internal static extern unsafe CONFIGRET CM_Get_Device_Interface_List_Size(out uint pulLen, in Guid InterfaceClassGuid, string pDeviceID, CM_GET_DEVICE_INTERFACE_LIST_FLAGS ulFlags);
        
        [DllImport("KERNEL32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern unsafe Windows.Win32.Foundation.BOOL CancelIoEx(SafeFileHandle hFile, NativeOverlapped* lpOverlapped);

        [DllImport("KERNEL32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern unsafe Windows.Win32.Foundation.BOOL GetOverlappedResult(SafeFileHandle hFile, NativeOverlapped* lpOverlapped, out uint lpNumberOfBytesTransferred, Windows.Win32.Foundation.BOOL bWait);
}