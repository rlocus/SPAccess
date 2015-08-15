using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SP2013Access.Converters
{
    public class LogItemFgColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value.ToString())
            {
                case "Error":
                    return Brushes.White;
                case "Warn":
                    return Brushes.White;
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}