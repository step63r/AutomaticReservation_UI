using System;
using System.Globalization;
using System.Windows.Controls;

namespace AutomaticReservation_UI.Domain
{
    public class FutureDateValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!DateTime.TryParse((value ?? "").ToString(),
                CultureInfo.CurrentCulture,
                DateTimeStyles.AssumeLocal | DateTimeStyles.AllowWhiteSpaces,
                out var time))
            {
                return new ValidationResult(false, "日付が不正です");
            }

            return time.Date < DateTime.Now.Date
                ? new ValidationResult(false, "過去日は入力できません")
                : ValidationResult.ValidResult;
        }
    }
}
