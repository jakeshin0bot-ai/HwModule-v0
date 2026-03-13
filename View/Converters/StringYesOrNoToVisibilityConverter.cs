using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HwModule.View.Converters
{
    public class StringYesOrNoToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string stringValue = value as string;
            return string.IsNullOrEmpty(stringValue) || stringValue != "Y" ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}