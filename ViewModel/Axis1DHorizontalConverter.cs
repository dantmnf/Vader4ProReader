using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Vader4ProReader.ViewModel
{
    internal class Axis1DHorizontalConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            if (values.Length == 4 && values[0] is double value && values[1] is double minValue && values[2] is double maxValue && values[3] is Dock dock)
            {
                if (maxValue == minValue)
                    return 0.0;

                if (dock == Dock.Left || dock == Dock.Right)
                    return (value - minValue) / (maxValue - minValue);

                return 1.0;
            }

            return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
