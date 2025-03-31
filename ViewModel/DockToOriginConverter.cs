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
    class DockToOriginConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Dock dock)
            {
                switch (dock)
                {
                    case Dock.Left:
                        return new System.Windows.Point(0, 0);
                    case Dock.Top:
                        return new System.Windows.Point(0, 0);
                    case Dock.Right:
                        return new System.Windows.Point(1, 0);
                    case Dock.Bottom:
                        return new System.Windows.Point(0, 1);
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
