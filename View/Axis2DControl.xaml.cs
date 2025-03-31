using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for Axis2DControl.xaml
    /// </summary>
    public partial class Axis2DControl : UserControl
    {
        public static readonly DependencyProperty XValueProperty = DependencyProperty.Register("XValue", typeof(double), typeof(Axis2DControl), new PropertyMetadata(0.0));
        public static readonly DependencyProperty MinXValueProperty = DependencyProperty.Register("MinXValue", typeof(double), typeof(Axis2DControl), new PropertyMetadata(0.0));
        public static readonly DependencyProperty MaxXValueProperty = DependencyProperty.Register("MaxXValue", typeof(double), typeof(Axis2DControl), new PropertyMetadata(1.0));

        public static readonly DependencyProperty YValueProperty = DependencyProperty.Register("YValue", typeof(double), typeof(Axis2DControl), new PropertyMetadata(0.0));
        public static readonly DependencyProperty MinYValueProperty = DependencyProperty.Register("MinYValue", typeof(double), typeof(Axis2DControl), new PropertyMetadata(0.0));
        public static readonly DependencyProperty MaxYValueProperty = DependencyProperty.Register("MaxYValue", typeof(double), typeof(Axis2DControl), new PropertyMetadata(1.0));

        public static readonly DependencyProperty AxisNameProperty = DependencyProperty.Register("AxisName", typeof(string), typeof(Axis2DControl));

        public double XValue
        {
            get { return (double)GetValue(XValueProperty); }
            set
            {
                SetValue(XValueProperty, value);
            }
        }
        public double MinXValue
        {
            get { return (double)GetValue(MinXValueProperty); }
            set { SetValue(MinXValueProperty, value); }
        }
        public double MaxXValue
        {
            get { return (double)GetValue(MaxXValueProperty); }
            set { SetValue(MaxXValueProperty, value); }
        }

        public double YValue
        {
            get { return (double)GetValue(YValueProperty); }
            set
            {
                SetValue(YValueProperty, value);
            }
        }
        public double MinYValue
        {
            get { return (double)GetValue(MinYValueProperty); }
            set { SetValue(MinYValueProperty, value); }
        }
        public double MaxYValue
        {
            get { return (double)GetValue(MaxYValueProperty); }
            set { SetValue(MaxYValueProperty, value); }
        }

        public string AxisName
        {
            get { return (string)GetValue(AxisNameProperty); }
            set { SetValue(AxisNameProperty, value); }
        }

        public double NormalizedXValue
        {
            get
            {
                return (XValue - MinXValue) / (MaxXValue - MinXValue);
            }
        }

        public double NormalizedYValue
        {
            get
            {
                return (YValue - MinYValue) / (MaxYValue - MinYValue);
            }
        }
        public Axis2DControl()
        {
            InitializeComponent();
        }
    }
}
