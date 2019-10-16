using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyCandles
{
    enum DateTimeMilestones : byte
    {
        NewHour = 1,
        NewDay = 2,
        NewMonth = 4,
        NewYear = 8,
        NewTimeFrame = 128
    }
    //************************************************************************************************************************************************
    static class DateTimeMilestonesHelper
    {
        // Возвращает логическую сумму всех DateTimeMilestones для двух соседних DateTime.
        public static byte GetMilestones(DateTime t1, DateTime t2) // t1 и t2 - два соседних DateTime.
        {
            // Побитовая сумма не определена для Byte. Поэтому используем uint:
            uint isNewYear = t1.Year == t2.Year ? 0 : (uint)DateTimeMilestones.NewYear;
            uint isNewMonth = t1.Month == t2.Month ? 0 : (uint)DateTimeMilestones.NewMonth;
            uint isNewDay = t1.Day == t2.Day ? 0 : (uint)DateTimeMilestones.NewDay;
            uint isNewHour = t1.Hour == t2.Hour ? 0 : (uint)DateTimeMilestones.NewHour;
            return (byte)(isNewYear | isNewMonth | isNewDay | isNewHour);
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------
    }
    //************************************************************************************************************************************************
    static class DateTimeMilestonesExtension
    {
        public static bool IsNewYear(this byte milestones_bitwise_sum)
        {
            return (milestones_bitwise_sum & (uint)DateTimeMilestones.NewYear) > 0;
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        public static bool IsNewMonth(this byte milestones_bitwise_sum)
        {
            return (milestones_bitwise_sum & (uint)DateTimeMilestones.NewMonth) > 0;
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        public static bool IsNewDay(this byte milestones_bitwise_sum)
        {
            return (milestones_bitwise_sum & (uint)DateTimeMilestones.NewDay) > 0;
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        public static bool IsNewHour(this byte milestones_bitwise_sum)
        {
            return (milestones_bitwise_sum & (uint)DateTimeMilestones.NewHour) > 0;
        }
    }
}
