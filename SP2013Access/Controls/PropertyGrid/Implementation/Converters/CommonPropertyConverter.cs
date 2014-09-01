using SP2013Access.Controls.Utilities;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Data;

namespace SP2013Access.Controls.PropertyGrid.Converters
{
    public class CommonPropertyConverter : IMultiValueConverter
    {
        private readonly TypeConverter _propertyTypeConverter;

        internal CommonPropertyConverter(Type type)
        {
            this._propertyTypeConverter = TypeDescriptor.GetConverter(type);
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Distinct<object>().Count<object>() > 1)
            {
                return null;
            }
            return values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            object element = value;
            if (GeneralUtilities.CanConvertValue(value, targetTypes[0]))
            {
                if (!this._propertyTypeConverter.CanConvertFrom(value.GetType()))
                {
                    throw new InvalidDataException("Cannot convert from targetType.");
                }
                element = this._propertyTypeConverter.ConvertFrom(value);
            }
            return Enumerable.Repeat<object>(element, targetTypes.Count<Type>()).ToArray<object>();
        }
    }
}