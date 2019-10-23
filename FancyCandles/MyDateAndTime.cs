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
    //**************************************************************************************************************************
    class MyDateAndTime
    {
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Являются ли дни yymmdd_1 и int yymmdd_2 следующими друг за другом днями. Порядок важен.
        public static bool IsDayByDay(int yymmdd_1, int yymmdd_2)
        {
            DateTime dt1 = YYMMDD_to_Datetime(yymmdd_1);
            DateTime dt2 = YYMMDD_to_Datetime(yymmdd_2);
            DateTime d = dt1.AddDays(1);
            return d.Year == dt2.Year && d.Month == dt2.Month && d.Day == dt2.Day;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static int CeilMinutesToConventionalTimeFrame(double minutes)
        {
            if (minutes > 240.0) return 1440;
            else if (minutes > 180.0) return 240;
            else if (minutes > 120.0) return 180;
            else if (minutes > 60.0) return 120;
            else if (minutes > 30.0) return 60;
            else if (minutes > 20.0) return 30;
            else if (minutes > 15.0) return 20;
            else if (minutes > 10.0) return 15;
            else if (minutes > 5.0) return 10;
            else return 5;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static int CeilMinutesToConventionalTimeFrameMultiplesToBase(double minutes, int baseTimeFrame)
        {
            if (minutes > 240.0) return 1440;
            else if (minutes > 180.0) return 240;
            else if (minutes > 120.0) return 180;
            else if (minutes > 60.0) return 120;
            else if (minutes > 30.0) return 60;
            else if (minutes > 20.0) return 30;
            else if (minutes > 15.0) return 20;
            else if (minutes > 10.0) return 15;
            else if (minutes > 5.0) return 10;
            else return 5;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Если временной отрезок [t0, t1] принять за единичный, то возвращает значение параметра, соответствующего моменту времени t. 
        // Момент времени t может может лежать за пределами [t0, t1]. Тогда значение параметра будет либо < 0, либо > 1.
        public static double GetLinearParameter(DateTime t0, DateTime t1, DateTime t)
        {
            if (t == t0) return 0;
            if (t == t1) return 1;
            return (t - t0).TotalMinutes / (t1 - t0).TotalMinutes;
        }

        public static DateTime Lerp(DateTime t0, DateTime t1, double t)
        {
            if (t == 0.0) return t0;
            if (t == 1.0) return t1;
            return t0 + TimeSpan.FromMinutes(t * (t1 - t0).TotalMinutes);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static DateTime DaySessionStart(DateTime t)
        {
            return new DateTime(t.Year, t.Month, t.Day, 10, 0, 0);
        }

        public static DateTime DaySessionEnd(DateTime t)
        {
            return new DateTime(t.Year, t.Month, t.Day, 18, 40, 0);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Возвращает ближайший к t момент времени, кратный timeFrameInMinutes.
        public static DateTime RoundTime(DateTime t, int timeFrameInMinutes)
        {
            int m = 60 * (t.Hour) + t.Minute;
            int ost = m % timeFrameInMinutes;
            if (ost == 0)
                return new DateTime(t.Year, t.Month, t.Day, t.Hour, t.Minute, 0);
            else if (ost >= (timeFrameInMinutes / 2.0))
                m = m - ost + timeFrameInMinutes;
            else
                m = m - ost;

            int hh = m / 60;
            return new DateTime(t.Year, t.Month, t.Day, hh, m - hh * 60, 0);
        }

        // t и результат округления должны лежать в диапазоне [fromHour:fromMinute, toHour:toMinute] - обязательно from < to
        public static DateTime RoundTime(DateTime t, int timeFrameInMinutes, int fromHour, int fromMinute, int toHour, int toMinute)
        {
            int m = 60 * (t.Hour) + t.Minute;
            int ost = m % timeFrameInMinutes;
            if (ost == 0)
                return new DateTime(t.Year, t.Month, t.Day, t.Hour, t.Minute, 0);

            int prev_m = m - ost;
            int prev_h = prev_m / 60;
            prev_m = prev_m - prev_h * 60;
            DateTime prev_t;
            prev_t = (prev_h < fromHour || (prev_h == fromHour && prev_m < fromMinute))
                        ? (new DateTime(t.Year, t.Month, t.Day, toHour, toMinute, 0)).AddDays(-1)
                        : (new DateTime(t.Year, t.Month, t.Day, prev_h, prev_m, 0));

            int next_m = m - ost + timeFrameInMinutes;
            int next_h = next_m / 60;
            next_m = next_m - next_h * 60;
            DateTime next_t;
            next_t = (next_h > toHour || (next_h == toHour && next_m > toMinute))
                        ? (new DateTime(t.Year, t.Month, t.Day, fromHour, fromMinute, 0)).AddDays(1)
                        : (new DateTime(t.Year, t.Month, t.Day, next_h, next_m, 0));

            if ((next_t - t) > (t - prev_t))
                return prev_t;
            else
                return next_t;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Находит в будущем ближайший DateTime, кратный timeFrameInMinutes. Если t кратен timeFrameInMinutes, то возвращает t.
        public static DateTime CeilTime(DateTime t, int timeFrameInMinutes)
        {
            int m = 60 * (t.Hour) + t.Minute;
            if (m % timeFrameInMinutes == 0) return t;
            m = (m / timeFrameInMinutes + 1) * timeFrameInMinutes;
            return (new DateTime(t.Year, t.Month, t.Day, 0, 0, 0)).AddMinutes(m);
            /*int hh = m / 60;
            return new DateTime(t.Year, t.Month, t.Day, hh, m - hh * 60, 0);*/
        }

        // t и результат округления должны лежать в диапазоне [fromHour:fromMinute, toHour:toMinute] - обязательно from < to
        public static DateTime CeilTime(DateTime t, int timeFrameInMinutes, int fromHour, int fromMinute, int toHour, int toMinute)
        {
            DateTime return_t = CeilTime(t, timeFrameInMinutes);

            if (return_t.Hour > toHour || (return_t.Hour == toHour && return_t.Minute > toMinute))
                return (new DateTime(return_t.Year, return_t.Month, return_t.Day, fromHour, fromMinute, 0)).AddDays(1);

            if (return_t.Hour < fromHour || (return_t.Hour == fromHour && return_t.Minute < fromMinute))
                return new DateTime(return_t.Year, return_t.Month, return_t.Day, fromHour, fromMinute, 0);
            return return_t;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Находит в прошлом ближайший DateTime, кратный timeFrameInMinutes. Если t кратен timeFrameInMinutes, то возвращает t.
        public static DateTime FloorTime(DateTime t, int timeFrameInMinutes)
        {
            int m = 60 * (t.Hour) + t.Minute;
            if (m % timeFrameInMinutes == 0) return t;
            m = (m / timeFrameInMinutes) * timeFrameInMinutes;
            return (new DateTime(t.Year, t.Month, t.Day, 0, 0, 0)).AddMinutes(m);
            /*int hh = m / 60;
            return new DateTime(t.Year, t.Month, t.Day, hh, m - hh * 60, 0);*/
        }

        // t и результат округления должны лежать в диапазоне [fromHour:fromMinute, toHour:toMinute] - обязательно from < to
        public static DateTime FloorTime(DateTime t, int timeFrameInMinutes, int fromHour, int fromMinute, int toHour, int toMinute)
        {
            DateTime return_t = FloorTime(t, timeFrameInMinutes);

            if (return_t.Hour < fromHour || (return_t.Hour == fromHour && return_t.Minute < fromMinute))
                return (new DateTime(return_t.Year, return_t.Month, return_t.Day, toHour, toMinute, 0)).AddDays(-1);

            if (return_t.Hour > toHour || (return_t.Hour == toHour && return_t.Minute > toMinute))
                return new DateTime(return_t.Year, return_t.Month, return_t.Day, toHour, toMinute, 0);

            return return_t;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool IsInSameHour(int HHMM_1, int HHMM_2)
        {
            int HH_1 = HHMM_1 / 100;
            int HH_2 = HHMM_2 / 100;
            return HH_1 == HH_2;
        }

        public static bool IsInSameHour(DateTime t1, DateTime t2)
        {
            return (t1.Date == t2.Date && t1.Hour == t2.Hour);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Кратно ли время HHMM количеству минут timeStep.
        public static bool IsTimeMultipleOf(int HHMM, int timeStep)
        {
            DateTime t = HHMM_to_Datetime(HHMM);
            int m = 60 * (t.Hour - 10) + t.Minute;
            return m % timeStep == 0 ? true : false;
        }

        public static bool IsTimeMultipleOf(DateTime t, int timeStep)
        {
            int m = 60 * (t.Hour - 10) + t.Minute;
            return m % timeStep == 0 ? true : false;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        /* Вариант с использованием строковых функций. Предположительно более медленный
        public static DateTime YYMMDDHHMM_to_Datetime(int YYMMDD, int HHMM)
        {
            int hour, min;
            if (HHMM >= 1000)
            {
                hour = int.Parse(HHMM.ToString().Substring(0, 2));
                min = int.Parse(HHMM.ToString().Substring(2, 2));
            }
            else if (HHMM >= 100)
            {
                hour = int.Parse(HHMM.ToString().Substring(0, 1));
                min = int.Parse(HHMM.ToString().Substring(1, 2));
            }
            else
            {
                hour = 0;
                min = HHMM;
            }

            int year, month, day;
            if (YYMMDD >= 100000)
            {
                year = int.Parse("20" + YYMMDD.ToString().Substring(0, 2));
                month = int.Parse(YYMMDD.ToString().Substring(2, 2));
                day = int.Parse(YYMMDD.ToString().Substring(4, 2));
            }
            else
            {
                year = int.Parse("200" + YYMMDD.ToString().Substring(0, 1));
                month = int.Parse(YYMMDD.ToString().Substring(1, 2));
                day = int.Parse(YYMMDD.ToString().Substring(3, 2));
            }

            return new DateTime(year, month, day, hour, min, 0);
        }*/
        public static DateTime YYMMDDHHMM_to_Datetime(int YYMMDD, int HHMM)
        {
            int hour = HHMM / 100;
            int min = HHMM - hour * 100;

            int year = YYMMDD / 10000;
            int minus_year = YYMMDD - year * 10000;
            int month = minus_year / 100;
            int day = minus_year - month * 100;
            year += 2000;

            return new DateTime(year, month, day, hour, min, 0);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static DateTime HHMM_to_Datetime(int HHMM)
        {
            int hour, min;
            if (HHMM >= 1000)
            {
                hour = int.Parse(HHMM.ToString().Substring(0, 2));
                min = int.Parse(HHMM.ToString().Substring(2, 2));
            }
            else if (HHMM >= 100)
            {
                hour = int.Parse(HHMM.ToString().Substring(0, 1));
                min = int.Parse(HHMM.ToString().Substring(1, 2));
            }
            else
            {
                hour = 0;
                min = HHMM;
            }

            return new DateTime(1, 1, 1, hour, min, 0);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static DateTime YYMMDD_to_Datetime(int YYMMDD)
        {
            int year, month, day;
            if (YYMMDD >= 100000)
            {
                year = int.Parse("20" + YYMMDD.ToString().Substring(0, 2));
                month = int.Parse(YYMMDD.ToString().Substring(2, 2));
                day = int.Parse(YYMMDD.ToString().Substring(4, 2));
            }
            else
            {
                year = int.Parse("200" + YYMMDD.ToString().Substring(0, 1));
                month = int.Parse(YYMMDD.ToString().Substring(1, 2));
                day = int.Parse(YYMMDD.ToString().Substring(3, 2));
            }

            return new DateTime(year, month, day);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static int Datetime_to_YYMMDD(DateTime t)
        {
            return (t.Year - 2000) * 10000 + t.Month * 100 + t.Day;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static int Datetime_to_HHMM(DateTime t)
        {
            return t.Hour * 100 + t.Minute;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
    }
}
