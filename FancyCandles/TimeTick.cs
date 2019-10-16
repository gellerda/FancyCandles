using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyCandles
{
    struct TimeTick
    {
        static string[] MonthsOfYear = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        public int csi; // - CandlesSource Index
        public bool isHourStart; // Является ли данная свечка первой в своем часе. Такие метки отображаются иначе.

        public TimeTick(int csi, bool isHourStart, DateTime t, double rightMargin)
        {
            this.csi = csi;
            this.isHourStart = isHourStart;
            Text = ConvertDateTimeToTimeTickText(isHourStart, t);
            RightMargin = rightMargin;
        }

        public string Text;
        public double RightMargin;
        //---------------------------------------------------------------------------------------------------------------------------------------
        public static TimeTick Undefined { get { return new TimeTick(-1, false, new DateTime(), 0); } }
        public static bool IsUndefined(TimeTick tt)
        {
            return (tt.csi == -1 && !tt.isHourStart);
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public static string ConvertDateTimeToTimeTickText(bool isHourStart, DateTime t)
        {
            if (isHourStart)
                return $"{t.Hour}h";
            else
            {
                if (t.Minute > 9)
                    return $":{t.Minute}";
                else
                    return $":0{t.Minute}";
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public static string ConvertDateTimeToDateTickText(bool isYearStart, bool isMonthStart, DateTime t)
        {
            if (isYearStart)
                return t.Year.ToString();
            else if (isMonthStart)
                return MonthsOfYear[t.Month - 1];
            else
                return t.Day.ToString();
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public static string ConvertDateTimeToMonthTickText(bool isYearStart, DateTime t)
        {
            if (isYearStart)
                return t.Year.ToString();
            else
                return MonthsOfYear[t.Month - 1];
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
    }
}
