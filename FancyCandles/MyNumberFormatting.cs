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
        //----------------------------------------------------------------------------------------------------------------------------------
        public static readonly int MaxVolumeStringLength = 6;

        private static string volumeStringFormat0 = "N4"; // the number is less than 1
        private static string volumeStringFormat1 = "N2"; // 1 digit before decimal point
        private static string volumeStringFormat2 = "N1"; // 2 digits before decimal point
        private static string volumeStringFormat3 = "N0"; // 3 digits before decimal point

        public static string VolumeToLimitedLengthString(double volume, CultureInfo culture, string decimalSeparator, char[] decimalSeparatorArray)
        {
            if (volume < 0.0) return "0";

            string stringFormat;
            string s;

            if (volume < 1.0)
            {
                if (volume < Math.Pow(10, 2 - MaxVolumeStringLength))
                {
                    stringFormat = "e0";
                    s = volume.ToString(stringFormat, culture);
                }
                else
                {
                    stringFormat = volumeStringFormat0;
                    s = volume.ToString(stringFormat, culture);
                    if (s.Contains(decimalSeparator))
                        s = s.TrimEnd('0').TrimEnd(decimalSeparatorArray);
                }
            }
            else if (volume < 1000.0)
            {
                if (volume < 10.0)
                    stringFormat = volumeStringFormat1;
                else if (volume < 100.0)
                    stringFormat = volumeStringFormat2;
                else // if (volume < 1000.0)
                    stringFormat = volumeStringFormat3;

                s = volume.ToString(stringFormat, culture);
                if (s.Contains(decimalSeparator))
                    s = s.TrimEnd('0').TrimEnd(decimalSeparatorArray);
            }
            else if (volume < 1_000_000.0)
            {
                if (volume < 10_000.0)
                    stringFormat = volumeStringFormat1;
                else if (volume < 100_000.0)
                    stringFormat = volumeStringFormat2;
                else // if (volume < 1_000_000.0)
                    stringFormat = volumeStringFormat3;

                s = (volume / 1_000.0).ToString(stringFormat, culture);
                if (s.Contains(decimalSeparator))
                    s = s.TrimEnd('0').TrimEnd(decimalSeparatorArray);
                s += "K";
            }
            else if (volume < 1_000_000_000.0)
            {
                if (volume < 10_000_000.0)
                    stringFormat = volumeStringFormat1;
                else if (volume < 100_000_000.0)
                    stringFormat = volumeStringFormat2;
                else // if (volume < 1_000_000_000.0)
                    stringFormat = volumeStringFormat3;

                s = (volume / 1_000_000.0).ToString(stringFormat, culture);
                if (s.Contains(decimalSeparator))
                    s = s.TrimEnd('0').TrimEnd(decimalSeparatorArray);
                s += "M";
            }
            else if (volume < 1_000_000_000_000.0)
            {
                if (volume < 10_000_000_000.0)
                    stringFormat = volumeStringFormat1;
                else if (volume < 100_000_000_000.0)
                    stringFormat = volumeStringFormat2;
                else // if (volume < 1_000_000_000_000.0)
                    stringFormat = volumeStringFormat3;

                s = (volume / 1_000_000_000.0).ToString(stringFormat, culture);
                if (s.Contains(decimalSeparator))
                    s = s.TrimEnd('0').TrimEnd(decimalSeparatorArray);
                s += "B";
            }
            else // if (volume < 1_000_000_000_000_000.0)
            {
                if (volume < 10_000_000_000_000.0)
                    stringFormat = volumeStringFormat1;
                else if (volume < 100_000_000_000_000.0)
                    stringFormat = volumeStringFormat2;
                else // if (volume < 1_000_000_000_000_000.0)
                    stringFormat = volumeStringFormat3;

                s = (volume / 1_000_000_000_000.0).ToString(stringFormat, culture);
                if (s.Contains(decimalSeparator))
                    s = s.TrimEnd('0').TrimEnd(decimalSeparatorArray);
                s += "T";
            }

            return s;
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        public static string VolumeToString(double volume, CultureInfo culture, string decimalSeparator, char[] decimalSeparatorArray)
        {
            if (volume <= 0.0) return "0";

            string stringFormat;
            string s;

            if (volume < 1.0)
            {
                MyWpfMath.HighestDecimalPlace(volume, out int highestDecimalPow);

                if (highestDecimalPow == int.MinValue)
                    return "0";

                stringFormat = "N" + (2 - highestDecimalPow).ToString();
                s = volume.ToString(stringFormat, culture).TrimEnd('0');
            }
            else // if (volume >= 1.0)
            {
                if (volume < 10.0)
                    stringFormat = volumeStringFormat1;
                else if (volume < 100.0)
                    stringFormat = volumeStringFormat2;
                else // if (volume >= 100.0)
                    stringFormat = volumeStringFormat3;

                s = volume.ToString(stringFormat, culture);
                if (s.Contains(decimalSeparator))
                    s = s.TrimEnd('0').TrimEnd(decimalSeparatorArray);
            }

            return s;
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        public static string PriceToString(double price, string numberFormat, CultureInfo culture, string decimalSeparator, char[] decimalSeparatorArray)
        {
            string s = price.ToString(numberFormat, culture);
            if (s.Contains(decimalSeparator))
                s = s.TrimEnd('0').TrimEnd(decimalSeparatorArray);

            return s;
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        public static int NumberOfFractionalDigits(double d)
        {
            string str = d.ToString(cultureEnUS);
            int pointPosition = str.LastIndexOf('.');

            if (pointPosition < 0) 
                return 0;

            return str.Length - pointPosition - 1;
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------
    }
}
