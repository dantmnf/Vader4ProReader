using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Vader4ProReader.View
{
    /// <summary>
    /// Interaction logic for Axis1DControl.xaml
    /// </summary>
    public partial class Axis1DControl : UserControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(Axis1DControl), new PropertyMetadata(0.0));
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(Axis1DControl), new PropertyMetadata(0.0));
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(Axis1DControl), new PropertyMetadata(1.0));

        public static readonly DependencyProperty DockProperty = DependencyProperty.Register("Dock", typeof(Dock), typeof(Axis1DControl), new PropertyMetadata(Dock.Left));

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set {
                SetValue(ValueProperty, value);
                
            }
        }
        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public Dock Dock
        {
            get { return (Dock)GetValue(DockProperty); }
            set { SetValue(DockProperty, value); }
        }

        public Axis1DControl()
        {
            InitializeComponent();
        }
    }
}
