using System.Configuration;
using System.Data;
using System.Windows;
using Vader4ProReader.Device;

namespace Vader4ProReader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var devs =  Vader4ProDevice.EnumerateDevices();
            GC.KeepAlive(devs);
        }
    }

}
