using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SP2013Access.Converters
{
    public class LogItemBgColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ("Warn" == value.ToString())
            {
                return Brushes.Orange;
            }
            if ("Error" == value.ToString())
            {
                return Brushes.Tomato;
            }
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
