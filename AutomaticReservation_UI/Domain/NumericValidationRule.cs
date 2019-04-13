using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace AutomaticReservation_UI.Domain
{
    public class NumericValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return ((string)value).Any(c => !char.IsNumber(c))
                ? new ValidationResult(false, "数字のみ")
                : ValidationResult.ValidResult;
        }
    }
}