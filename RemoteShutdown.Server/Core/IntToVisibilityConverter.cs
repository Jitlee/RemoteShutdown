using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using RemoteShutdown.Utilities;

namespace RemoteShutdown.Core
{
    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Convert(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Convert(value);
        }

        public Visibility Convert(object value)
        {
            var i = Converter.ToInt(value);
            return i == 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
