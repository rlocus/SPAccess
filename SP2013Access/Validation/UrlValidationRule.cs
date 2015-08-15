using System;
using System.Globalization;
using System.Windows.Controls;

namespace SP2013Access.Validation
{
    public class UrlValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var isValid = true;
            string error = null;

            if (value != null)
            {
                try
                {
                    new Uri(value.ToString());
                }
                catch (UriFormatException ex)
                {
                    isValid = false;
                    error = ex.Message;
                }
            }

            return new ValidationResult(isValid, error);
        }
    }
}