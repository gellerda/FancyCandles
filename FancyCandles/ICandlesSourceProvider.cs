using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace FancyCandles
{
    public interface ICandlesSourceProvider
    {
        List<ISecurityInfo> SecCatalog { get; }
        //ReadOnlyObservableCollection<ICandle> GetCandlesSource(int secCatalog_i, int timeFrameInMinutes);
        CandlesSourceFromProvider GetCandlesSource(string secID, int timeFrameInMinutes);
    }
    //*****************************************************************************************************************************************************************************
    public class CandlesSourceFromProvider : ReadOnlyObservableCollection<ICandle>, ICandlesSource
    {
        //---------------------------------------------------------------------------------------------------------------------------------------
        public CandlesSourceFromProvider(ObservableCollection<ICandle> candlesSource, ICandlesSourceProvider parentProvider, string secID, int timeFrameInMinutes) : base(candlesSource)
        {
            this.parentProvider = parentProvider;
            this.secID = secID; 
            this.timeFrameInMinutes = timeFrameInMinutes;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        private readonly ICandlesSourceProvider parentProvider;
        public ICandlesSourceProvider ParentProvider
        {
            get { return parentProvider; }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        private readonly string secID;
        public string SecID
        {
            get { return secID; }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        private readonly int timeFrameInMinutes;
        public int TimeFrameInMinutes
        {
            get { return timeFrameInMinutes; }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
    }
    //*****************************************************************************************************************************************************************************
}
