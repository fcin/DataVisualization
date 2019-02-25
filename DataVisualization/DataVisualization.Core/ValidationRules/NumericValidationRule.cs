using System.Globalization;
using System.Windows.Controls;

namespace DataVisualization.Core.ValidationRules
{
    public class NumericValidationRule : ValidationRule
    {
        private const NumberStyles NumericStyles = NumberStyles.Float;
        private readonly NumberFormatInfo _numericInfo = new NumberFormatInfo();

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return value != null && double.TryParse(value.ToString(), NumericStyles, _numericInfo, out var _) ?
                ValidationResult.ValidResult :
                new ValidationResult(false, "Not a number");
        }
    }
}
