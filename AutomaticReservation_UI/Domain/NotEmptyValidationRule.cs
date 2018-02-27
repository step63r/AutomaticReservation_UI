using System.Globalization;
using System.Windows.Controls;

namespace AutomaticReservation_UI.Domain
{
    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrWhiteSpace((value ?? "").ToString())
                ? new ValidationResult(false, "必須入力")
                : ValidationResult.ValidResult;
        }
    }
}
