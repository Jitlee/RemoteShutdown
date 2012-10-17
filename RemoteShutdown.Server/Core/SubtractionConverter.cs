using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace RemoteShutdown.Core
{
    public class SubtractionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Convert(value, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Convert(value, parameter);
        }

        private object Convert(object value, object parameter)
        {
            int a, b;
            if (null != value
                && null != parameter
                && int.TryParse(value.ToString(), out a)
                && int.TryParse(parameter.ToString(), out b))
            {
                return b - a;
            }
            return 0;
        }
    }
}
