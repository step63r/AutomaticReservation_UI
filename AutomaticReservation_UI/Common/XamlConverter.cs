using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace AutomaticReservation_UI.Common
{
    /// <summary>
    /// int ⇔ bool
    /// </summary>
    public class BoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)value >= 1) ? true : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // いらない
            return ((bool)value) ? 1 : 0;
        }
    }

    /// <summary>
    /// bool ⇒ Vilibility
    /// </summary>
    public class BoolVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((bool?)value == true) ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// bool and bool ⇒ true
    /// </summary>
    public class BoolAndMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is null || values[1] is null)
            {
                return false;
            }
            else
            {
                return ((bool)values[0]) && ((bool)values[1]);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 日付が今日かどうか
    /// </summary>
    public class DateTodayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return false;
            }
            else
            {
                return ((DateTime)value) == DateTime.Today;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 日付が今日かどうか
    ///     values[0]   対象日付
    ///     values[1]   本日日付
    /// </summary>
    public class DateTodayMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is null || values[1] is null)
            {
                return false;
            }
            else
            {
                return ((DateTime)values[0]) == ((DateTime)values[1]);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 日付が明日かどうか
    /// </summary>
    public class DateTomorrowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return false;
            }
            else
            {
                return ((DateTime)value) == DateTime.Today.AddDays(1);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 日付が明日かどうか
    ///     values[0]   対象日付
    ///     values[1]   本日日付
    /// </summary>
    public class DateTomorrowMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is null || values[1] is null)
            {
                return false;
            }
            else
            {
                return ((DateTime)values[0]) == ((DateTime)values[1]).AddDays(1);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 日付が過去かどうか
    /// </summary>
    public class DateOverConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return false;
            }
            else
            {
                return ((DateTime)value) < DateTime.Today;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 日付が過去かどうか
    ///     values[0]   対象日付
    ///     values[1]   本日日付
    /// </summary>
    public class DatePastMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is null || values[1] is null)
            {
                return false;
            }
            else
            {
                return ((DateTime)values[0]) < ((DateTime)values[1]);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /****************************************************************
     *                                                              *
     * ソース                                                       *
     * http://y-maeyama.hatenablog.com/entry/20101104/1288867106    *
     *                                                              *
     ****************************************************************/
    /// <summary>
    /// 負値を赤色にするConverter
    /// </summary>
    public class BlushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.TryParse(value as string, out var number))
            {
                if (number >= 0)
                {
                    // 負の数だった場合、黒を返す
                    return new SolidColorBrush(Colors.Black);
                }
                else
                {
                    // 負の数だった場合、赤を返す
                    return new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                return new SolidColorBrush(Colors.Black);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // BindingModeがOneWayを想定しているので未実装で問題ない。
            throw new NotImplementedException();
        }
    }
}
