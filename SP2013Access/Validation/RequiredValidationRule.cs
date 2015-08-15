using System.Globalization;
using System.Windows.Controls;

namespace SP2013Access.Validation
{
    public class RequiredValidationRule : ValidationRule
    {
        public RequiredValidationRule()
        {
            ErrorMessage = "Value cannot be null.";
        }

        public string ErrorMessage { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var isValid = true;
            string error = null;

            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                isValid = false;
                error = ErrorMessage;
            }

            return new ValidationResult(isValid, error);
        }
    }
}