using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HwModule.Utils
{
    /// <summary>
    /// 확장 메서드 모음
    /// </summary>
    public static class ExpansionMethods
    {
        public static string ToSearchSring(this DateTime searchDate)
        {
            return searchDate.ToString("yyyyMMdd");
        }

        public static string ToSearchLongString(this DateTime searchDate)
        {
            return searchDate.ToString("yyyyMMddHHmmss");
        }

        public static DateTime ToDateTime(this string searchDate)
        {
            return DateTime.ParseExact(searchDate, "yyyyMMdd", CultureInfo.InvariantCulture);
        }

        public static string ToPrintSring(this DateTime searchDate)
        {
            return searchDate.ToString("yyyy-MM-dd");
        }

        public static string ToDecimalString(this int value)
        {
            return string.Format("{0:#,0}", value);
        }

        public static double InchToPixel(this int value)
        {
            return (value * 96) / 100;
        }

        public static double MillimeterToPixel(this double value)
        {
            //밀리미터에서 바로 Pixel로 변환하면 Inch -> Pixel로 변환 했을때와 소수점이 안맞기 때문에 인치로 변환하고 픽셀로 변환함.
            var inch = Math.Round(value / 25.4, 2);
            var pixel = inch * 96;
            return pixel;
        }

        public static double MillimeterToPixel(this int value)
        {
            //밀리미터에서 바로 Pixel로 변환하면 Inch -> Pixel로 변환 했을때와 소수점이 안맞기 때문에 인치로 변환하고 픽셀로 변환함.
            var inch = Math.Round(value / 25.4, 2);
            var pixel = inch * 96;
            return pixel;
        }
    }
}
