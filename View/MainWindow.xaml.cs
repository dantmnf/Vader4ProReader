using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Vader4ProReader.Device;

namespace Vader4ProReader.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private class DebouncedReportReceiver(Vader4ProViewModel vm)
        {
            private volatile DispatcherOperation? lastOperation;

            public void OnReportReceived(object? sender, Vader4ProReport report)
            {
                if (lastOperation != null && lastOperation.Status != DispatcherOperationStatus.Completed)
                {
                    return;
                }
                lastOperation = Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    vm.UpdateFromReport(report);
                }, DispatcherPriority.Input);
            }
        }


        private DebouncedReportReceiver? receiver;
        private Vader4ProDevice? dev;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var devs = Vader4ProDevice.EnumerateDevices();
            if (devs.Length > 0)
            {
                dev = Vader4ProDevice.OpenDevice(devs[0]);
                var vm = new Vader4ProViewModel(dev);
                DataContext = vm;
                receiver = new DebouncedReportReceiver(vm);
                dev.ReportReceived += receiver.OnReportReceived;
            }
            else
            {
                MessageBox.Show("No Vader 4 Pro devices found.");
            }

        }


        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (dev != null)
            {
                if (receiver != null)
                {
                    dev.ReportReceived -= receiver.OnReportReceived;
                }
                dev.Dispose();
            }
            Binding x;
            
        }
    }
}