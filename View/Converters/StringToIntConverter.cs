using System.Globalization;
using System.Windows.Data;
using System;

namespace HwModule.View.Converters
{
    public class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 여기서 필요한 변환 로직을 구현합니다.
            if (value == null || !(value is string))
                return 0;

            string stringValue = (string)value;
            if (int.TryParse(stringValue, out int result))
                return result;
            else
                return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}