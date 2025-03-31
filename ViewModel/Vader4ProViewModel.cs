using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Vader4ProReader.Device;

namespace Vader4ProReader
{
    public partial class Vader4ProViewModel : ObservableObject
    {
        // Properties for ButtonControl bindings
        [ObservableProperty] public partial bool IsAPressed { get; set; }
        [ObservableProperty] public partial bool IsBPressed { get; set; }
        [ObservableProperty] public partial bool IsXPressed { get; set; }
        [ObservableProperty] public partial bool IsYPressed { get; set; }
        [ObservableProperty] public partial bool IsLBPressed { get; set; }
        [ObservableProperty] public partial bool IsRBPressed { get; set; }
        [ObservableProperty] public partial bool IsLTPressed { get; set; }
        [ObservableProperty] public partial bool IsRTPressed { get; set; }
        [ObservableProperty] public partial bool IsLSPressed { get; set; }
        [ObservableProperty] public partial bool IsRSPressed { get; set; }
        [ObservableProperty] public partial bool IsSelectPressed { get; set; }
        [ObservableProperty] public partial bool IsStartPressed { get; set; }
        [ObservableProperty] public partial bool IsHomePressed { get; set; }
        [ObservableProperty] public partial bool IsLeftPressed { get; set; }
        [ObservableProperty] public partial bool IsDownPressed { get; set; }
        [ObservableProperty] public partial bool IsUpPressed { get; set; }
        [ObservableProperty] public partial bool IsRightPressed { get; set; }
        [ObservableProperty] public partial bool IsCPressed { get; set; }
        [ObservableProperty] public partial bool IsZPressed { get; set; }
        [ObservableProperty] public partial bool IsM2Pressed { get; set; }
        [ObservableProperty] public partial bool IsM4Pressed { get; set; }
        [ObservableProperty] public partial bool IsM3Pressed { get; set; }
        [ObservableProperty] public partial bool IsM1Pressed { get; set; }
        [ObservableProperty] public partial bool IsFNPressed { get; set; }

        // Properties for Axis2DControl bindings
        [ObservableProperty] public partial double LS_X { get; set; }
        public double LS_XMin => 0.0;
        public double LS_XMax => 255.0;
        [ObservableProperty] public partial double LS_Y { get; set; }
        public double LS_YMin => 0.0;
        public double LS_YMax => 255.0;
        [ObservableProperty] public partial double RS_X { get; set; }
        public double RS_XMin => 0.0;
        public double RS_XMax => 255.0;
        [ObservableProperty] public partial double RS_Y { get; set; }
        public double RS_YMin => 0.0;
        public double RS_YMax => 255.0;

        // Properties for Axis1DControl bindings
        [ObservableProperty] public partial double LT { get; set; }
        public double LTMin => 0.0;
        public double LTMax => 255.0;
        [ObservableProperty] public partial double RT { get; set; }
        public double RTMin => 0.0;
        public double RTMax => 255.0;
        [ObservableProperty] public partial double Yaw { get; set; }
        public double YawMin => -1.0;
        public double YawMax => 1.0;
        [ObservableProperty] public partial double Pitch { get; set; }
        public double PitchMin => -1.0;
        public double PitchMax => 1.0;
        [ObservableProperty] public partial double Roll { get; set; }
        public double RollMin => -1.0;
        public double RollMax => 1.0;
        [ObservableProperty] public partial double AccelX { get; set; }
        public double AccelXMin => -40;
        public double AccelXMax => 40;
        [ObservableProperty] public partial double AccelY { get; set; }
        public double AccelYMin => -40;
        public double AccelYMax => 40;
        [ObservableProperty] public partial double AccelZ { get; set; }


        public double AccelZMin => -40;
        public double AccelZMax => 40;

        // Properties for Rumble bindings
        public int RumbleMin => 0;
        public int RumbleMax => 255;

        [ObservableProperty] public partial int RumbleL { get; set; }
        [ObservableProperty] public partial int RumbleR { get; set; }
        [ObservableProperty] public partial int RumbleLT { get; set; }
        [ObservableProperty] public partial int RumbleRT { get; set; }

        // Properties for LED bindings
        [ObservableProperty] public partial int LedR { get; set; }
        [ObservableProperty] public partial int LedG { get; set; }
        [ObservableProperty] public partial int LedB { get; set; }
        public int LedMin => 0;
        public int LedMax => 255;


        [ObservableProperty] public partial double YawMinRecorded { get; set; }
        [ObservableProperty] public partial double YawMaxRecorded { get; set; }
        [ObservableProperty] public partial double PitchMinRecorded { get; set; }
        [ObservableProperty] public partial double PitchMaxRecorded { get; set; }
        [ObservableProperty] public partial double RollMinRecorded { get; set; }
        [ObservableProperty] public partial double RollMaxRecorded { get; set; }


        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == nameof(RumbleL) || e.PropertyName == nameof(RumbleR) || e.PropertyName == nameof(RumbleLT) || e.PropertyName == nameof(RumbleRT))
            {
                SetRumble(RumbleL, RumbleR, RumbleLT, RumbleRT);
            }
            else if (e.PropertyName == nameof(LedR) || e.PropertyName == nameof(LedG) || e.PropertyName == nameof(LedB))
            {
                SetLed(LedR, LedG, LedB);
            }
        }


        public ICommand DisableAirMouseCommand { get; }

        public ICommand EnableAirMouseCommand { get; }


        private Vader4ProDevice dev;

        public Vader4ProViewModel(Vader4ProDevice dev)
        {
            // Set default values
            LS_X = 127.0;
            LS_Y = 127.0;
            RS_X = 127.0;
            RS_Y = 127.0;
            LT = 0.0;
            RT = 0.0;
            Yaw = 0.0;
            Pitch = 0.0;
            Roll = 0.0;
            AccelX = 0.0;
            AccelY = 0.0;
            AccelZ = 0.0;

            this.dev = dev;
            DisableAirMouseCommand = new RelayCommand(DisableAirMouse);
            EnableAirMouseCommand = new RelayCommand(EnableAirMouse);
        }

        private void DisableAirMouse()
        {
            dev.DisableAirMouse();
        }

        private void EnableAirMouse()
        {
            dev.EnableAirMouse();
        }

        private void SetRumble(int left, int right, int lt, int rt)
        {
            dev.SetRumble((byte)left, (byte)right, (byte)lt, (byte)rt);
        }
        private void SetLed(int ledR, int ledG, int ledB)
        {
            dev.SetLed((byte)ledR, (byte)ledG, (byte)ledB);
        }

        internal void UpdateFromReport(Vader4ProReport report)
        {
            IsAPressed = report.IsAPressed;
            IsBPressed = report.IsBPressed;
            IsXPressed = report.IsXPressed;
            IsYPressed = report.IsYPressed;
            IsLBPressed = report.IsLBPressed;
            IsRBPressed = report.IsRBPressed;
            IsLTPressed = report.IsLTPressed;
            IsRTPressed = report.IsRTPressed;
            IsLSPressed = report.IsLSPressed;
            IsRSPressed = report.IsRSPressed;
            IsSelectPressed = report.IsSelectPressed;
            IsStartPressed = report.IsStartPressed;
            IsHomePressed = report.IsHOMEPressed;
            IsLeftPressed = report.IsDPadLeftPressed;
            IsDownPressed = report.IsDPadDownPressed;
            IsUpPressed = report.IsDPadUpPressed;
            IsRightPressed = report.IsDPadRightPressed;
            IsCPressed = report.IsCPressed;
            IsZPressed = report.IsZPressed;
            IsM2Pressed = report.IsM2Pressed;
            IsM4Pressed = report.IsM4Pressed;
            IsM3Pressed = report.IsM3Pressed;
            IsM1Pressed = report.IsM1Pressed;
            IsFNPressed = report.IsFNPressed;

            LS_X = report.LS_X;
            LS_Y = report.LS_Y;
            RS_X = report.RS_X;
            RS_Y = report.RS_Y;

            LT = report.LT;
            RT = report.RT;
            Yaw = report.YawFloat;
            Pitch = report.PitchFloat;
            Roll = report.RollFloat;
            AccelX = report.AccelXCalibrated;
            AccelY = report.AccelYCalibrated;
            AccelZ = report.AccelZCalibrated;

            if (report.YawRaw < YawMinRecorded) YawMinRecorded = report.YawRaw;
            if (report.YawRaw > YawMaxRecorded) YawMaxRecorded = report.YawRaw;
            if (report.PitchRaw < PitchMinRecorded) PitchMinRecorded = report.PitchRaw;
            if (report.PitchRaw > PitchMaxRecorded) PitchMaxRecorded = report.PitchRaw;
            if (report.RollRaw < RollMinRecorded) RollMinRecorded = report.RollRaw;
            if (report.RollRaw > RollMaxRecorded) RollMaxRecorded = report.RollRaw;

        }
    }
}
