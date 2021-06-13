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
        //ReadOnlyObservableCollection<ICandle> GetCandlesSource(int secCatalog_i, TimeFrame timeFrame);
        CandlesSourceFromProvider GetCandlesSource(int secCatalog_i, TimeFrame timeFrame);
    }
    //*****************************************************************************************************************************************************************************
    public class CandlesSourceFromProvider : ReadOnlyObservableCollection<ICandle>, ICandlesSource
    {
        //---------------------------------------------------------------------------------------------------------------------------------------
        public CandlesSourceFromProvider(ObservableCollection<ICandle> candlesSource, ICandlesSourceProvider parentProvider, int secCatalogIndex, TimeFrame timeFrame) : base(candlesSource)
        {
            this.parentProvider = parentProvider;
            this.secCatalogIndex = secCatalogIndex; 
            this.timeFrame = timeFrame;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        private readonly ICandlesSourceProvider parentProvider;
        public ICandlesSourceProvider ParentProvider
        {
            get { return parentProvider; }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        private readonly int secCatalogIndex;
        public int SecCatalogIndex
        {
            get { return secCatalogIndex; }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        private readonly TimeFrame timeFrame;
        public TimeFrame TimeFrame
        {
            get { return timeFrame; }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
    }
    //*****************************************************************************************************************************************************************************
}
