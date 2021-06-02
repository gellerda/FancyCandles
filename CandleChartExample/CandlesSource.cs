using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using FancyCandles;

namespace CandleChartExample
{

    public class CandlesSource : ObservableCollection<ICandle>, ICandlesSource
    {
        public CandlesSource(int timeFrameInMinutes)
        {
            this.timeFrameInMinutes = timeFrameInMinutes;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        private readonly int timeFrameInMinutes;
        public int TimeFrameInMinutes { get { return timeFrameInMinutes; } }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
    }

    /* Or you can make it read only, which is much cleaner, IMHO!
    public class CandlesSource : ReadOnlyObservableCollection<ICandle>, ICandlesSource
    {
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        public CandlesSource(ObservableCollection<ICandle> list, int timeFrameInMinutes) : base(list)
        {
            this.timeFrameInMinutes = timeFrameInMinutes;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        readonly int timeFrameInMinutes;
        public int TimeFrameInMinutes { get { return timeFrameInMinutes; } }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
    }*/
}
