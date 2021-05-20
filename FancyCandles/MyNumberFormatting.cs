using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace FancyCandles
{
    internal static class MyNumberFormatting
    {
        private static CultureInfo cultureEnUS = CultureInfo.CreateSpecificCulture("en-US");

        public static string MyToString(this double num, CultureInfo culture, string decimalSeparator, char[] decimalSeparatorArray)
        {
            string s = num.ToString("N15", culture);
            if (s.Contains(decimalSeparator))
                s = s.TrimEnd('0').TrimEnd(decimalSeparatorArray);

            return s;
        }

        public static string MyToString(this long num, CultureInfo culture, string decimalSeparator, char[] decimalSeparatorArray)
        {
            string s = num.ToString("N15", culture);
            if (s.Contains(decimalSeparator))
                s = s.TrimEnd('0').TrimEnd(decimalSeparatorArray);

            return s;
        }

        public static int NumberOfFractionalDigits(this double d)
        {
            string str = d.ToString(cultureEnUS);
            int pointPosition = str.LastIndexOf('.');

            if (pointPosition < 0) 
                return 0;

            return str.Length - pointPosition - 1;
        }

    }
}
