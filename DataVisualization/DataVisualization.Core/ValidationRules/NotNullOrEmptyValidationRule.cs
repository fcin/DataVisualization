using System.Globalization;
using System.Windows.Controls;

namespace DataVisualization.Core.ValidationRules
{
    public class NotNullOrEmptyValidationRule : ValidationRule
    {

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return value == null || string.IsNullOrEmpty(value.ToString()) ?
                new ValidationResult(false, "Field cannot be empty") :
                ValidationResult.ValidResult;
        }
    }
}
