using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CAFEHOLIC.service
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisible = value is bool b && b;
            System.Diagnostics.Debug.WriteLine($"[BooleanToVisibilityConverter] Input: {value}, Output: {(isVisible ? Visibility.Visible : Visibility.Collapsed)}");
            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}