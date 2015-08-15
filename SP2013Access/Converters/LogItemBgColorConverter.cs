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
            switch (value.ToString())
            {
                case "Warn":
                    return Brushes.Orange;
                case "Error":
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