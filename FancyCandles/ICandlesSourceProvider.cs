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
        ICandlesSource GetCandlesSource(string secID, TimeFrame timeFrame);
    }
}
