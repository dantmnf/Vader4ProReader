using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Vader4ProReader.ViewModel
{
    internal class Axis2DConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double scale = 1.0;
            if (values.Length == 4 && values[3] is double actualSize)
            {
                scale = actualSize - 2.0;
            }
            if ((values.Length == 3 || values.Length == 4) && values[0] is double value && values[1] is double minValue && values[2] is double maxValue)
            {
                if (maxValue == minValue)
                    return 0.0;

                return (value - minValue) / (maxValue - minValue) * scale;
            }

            return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
