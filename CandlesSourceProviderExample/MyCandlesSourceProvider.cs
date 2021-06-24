using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FancyCandles;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Windows.Threading;
using System.Diagnostics;

namespace CandlesSourceProviderExample
{
    public class MyCandlesSourceProvider : ICandlesSourceProvider
    {
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        public MyCandlesSourceProvider()
        { }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        private readonly IList<ISecurityInfo> secCatalog = new List<ISecurityInfo>() { 
                    new MySecurityInfo("coinbase-pro", "btcusd", "Bitcoin"), 
                    new MySecurityInfo("coinbase-pro", "ethusd", "Etherium"), 
                    new MySecurityInfo("coinbase-pro", "dogeusd", "Dogecoin"), 
                    new MySecurityInfo("coinbase-pro", "adausd", "Cardano") };
        public IList<ISecurityInfo> SecCatalog { get { return secCatalog; } }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        private readonly IList<TimeFrame> supportedTimeFrames = 
            new List<TimeFrame>() { TimeFrame.M1, TimeFrame.M5, TimeFrame.M10, TimeFrame.M15, TimeFrame.M20, TimeFrame.M30, TimeFrame.H1, TimeFrame.Daily };
        public IList<TimeFrame> SupportedTimeFrames { get {return supportedTimeFrames;} }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        private readonly Dictionary<string, MyCandlesSource> candlesSources = new Dictionary<string, MyCandlesSource>(); // Key - "SecID_timeFrame"

        public ICandlesSourceFromProvider GetCandlesSource(string secID, TimeFrame timeFrame)
        {
            string candlesSources_key = $"{secID}_{(int)timeFrame}";

            if (!candlesSources.ContainsKey(candlesSources_key))
            {
                ISecurityInfo secInfo = GetSecFromCatalog(secID);
                MyCandlesSource candlesSource = new MyCandlesSource(secInfo.ExchangeName, secInfo.Ticker, timeFrame, EndCandlesSourceUsing);
                candlesSources.Add(candlesSources_key, candlesSource);
                return candlesSource;
            }
            else
                return candlesSources[candlesSources_key];
        }

        private void EndCandlesSourceUsing(string candlesSources_key)
        {
            if (!candlesSources.ContainsKey(candlesSources_key))
                throw new ArgumentException($"There is no item with key={candlesSources_key} in the candlesSources dictionary.");

            candlesSources.Remove(candlesSources_key);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        public ISecurityInfo GetSecFromCatalog(string secID)
        {
            foreach (ISecurityInfo secInfo in SecCatalog)
            {
                if (secInfo.SecID == secID)
                    return secInfo;
            }

            throw new ArgumentException($"There is no security with SecID={secID} in SecCatalog.");
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
    }
}
