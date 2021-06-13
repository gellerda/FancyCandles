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
        public CandlesSource(TimeFrame timeFrame)
        {
            this.timeFrame = timeFrame;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        private readonly TimeFrame timeFrame;
        public TimeFrame TimeFrame { get { return timeFrame; } }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
    }

    /* Or you can make it read only, which is much cleaner, IMHO!
    public class CandlesSource : ReadOnlyObservableCollection<ICandle>, ICandlesSource
    {
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        public CandlesSource(ObservableCollection<ICandle> list, TimeFrame timeFrame) : base(list)
        {
            this.timeFrame = timeFrame;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        readonly TimeFrame timeFrame;
        public TimeFrame TimeFrame { get { return timeFrame; } }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
    }*/
}
