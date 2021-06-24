using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyCandles
{
    /// <summary>Represents the time frames you can use in your <see cref="CandleChart"/> charts.</summary>
    public enum TimeFrame
    {
        ///<summary>1-second time frame.</summary>
        S1 = -1,
        ///<summary>2-second time frame.</summary>
        S2 = -2,
        ///<summary>3-second time frame.</summary>
        S3 = -3,
        ///<summary>5-second time frame.</summary>
        S5 = -5,
        ///<summary>10-second time frame.</summary>
        S10 = -10,
        ///<summary>15-second time frame.</summary>
        S15 = -15,
        ///<summary>20-second time frame.</summary>
        S20 = -20,
        ///<summary>30-second time frame.</summary>
        S30 = -30,
        //Tick = 0, Not supported yet. Let me know if you need it.
        ///<summary>1-minute time frame.</summary>
        M1 = 1,
        ///<summary>2-minute time frame.</summary>
        M2 = 2,
        ///<summary>3-minute time frame.</summary>
        M3 = 3,
        ///<summary>5-minute time frame.</summary>
        M5 = 5,
        ///<summary>10-minute time frame.</summary>
        M10 = 10,
        ///<summary>15-minute time frame.</summary>
        M15 = 15,
        ///<summary>20-minute time frame.</summary>
        M20 = 20,
        ///<summary>30-minute time frame.</summary>
        M30 = 30,
        ///<summary>1-hour time frame.</summary>
        H1 = 60,
        ///<summary>2-hour time frame.</summary>
        H2 = 120,
        ///<summary>3-hour time frame.</summary>
        H3 = 180,
        ///<summary>4-hour time frame.</summary>
        H4 = 240,
        ///<summary>1-day time frame.</summary>
        Daily = 1440,
        ///<summary>1-week time frame.</summary>
        Weekly = 10080,
        ///<summary>1-month time frame.</summary>
        Monthly = 43200
    }

    /// <summary>Extension methods for the <see cref="TimeFrame"/> enum.</summary>
    public static class TimeFrameExtensionMethods
    {
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Converts a <see cref="TimeFrame"/> value to a value in minutes.</summary>
        public static double ToMinutes(this TimeFrame tf)
        {
            return ((int)tf >= 0) ? (double)tf : (-(double)tf / 60.0);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Converts a <see cref="TimeFrame"/> value to a value in seconds.</summary>
        public static int ToSeconds(this TimeFrame tf)
        {
            return ((int)tf >= 0) ? 60*((int)tf) : (-(int)tf);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
    }
}
