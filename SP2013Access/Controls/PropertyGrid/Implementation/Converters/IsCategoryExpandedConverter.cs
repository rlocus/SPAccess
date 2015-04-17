using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace SP2013Access.Controls.PropertyGrid.Converters
{
    public class IsCategoryExpandedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is IList<object>))
            {
                return true;
            }
            IList<object> list = value as IList<object>;
            if (list.Count == 0)
            {
                return true;
            }
            if (list[0] is CustomPropertyItem)
            {
                return ((CustomPropertyItem)list[0]).IsCategoryExpanded;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}