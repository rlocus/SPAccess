using SP2013Access.Controls.Utilities;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;

namespace SP2013Access.Controls.PropertyGrid
{
    internal class CommonPropertyExceptionValidationRule : ValidationRule
    {
        private readonly TypeConverter _propertyTypeConverter;
        private readonly Type _type;
        internal CommonPropertyExceptionValidationRule(Type type)
        {
            this._propertyTypeConverter = TypeDescriptor.GetConverter(type);
            this._type = type;
        }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = new ValidationResult(true, null);
            if (GeneralUtilities.CanConvertValue(value, this._type))
            {
                try
                {
                    this._propertyTypeConverter.ConvertFrom(value);
                }
                catch (Exception ex)
                {
                    result = new ValidationResult(false, ex.Message);
                }
            }
            return result;
        }
    }
}