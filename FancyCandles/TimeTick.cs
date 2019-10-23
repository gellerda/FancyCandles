/* 
    Copyright 2019 Dennis Geller.

    This file is part of FancyCandles.

    FancyCandles is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    FancyCandles is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with FancyCandles.  If not, see<https://www.gnu.org/licenses/>. */

using System;

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
