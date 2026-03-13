using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HwModule.View.Converters
{
    public class ValueMatchToVisibilityConverter : IValueConverter
    {
        public bool Collapse { get; set; } = true;
        public bool IsMatch { get; set; } = true;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var hideCondition = Collapse == false ? Visibility.Hidden : Visibility.Collapsed;

            if (value == null || parameter == null)
                return hideCondition;

            try
            {
                string checkValue = value.ToString();
                string targetValue = parameter.ToString();

                if (IsMatch == true)
                {

                    if (checkValue == targetValue)
                        return Visibility.Visible;
                    else
                        return hideCondition;
                }
                else
                {
                    if (checkValue != targetValue)
                        return Visibility.Visible;
                    else
                        return hideCondition;
                }
            }
            catch
            {
                return hideCondition;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
