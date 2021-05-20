using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace FancyCandles
{
    public interface ICandleProvider
    {
        List<ISecurityInfo> SecCatalog { get; }
        ReadOnlyObservableCollection<ICandle> GetCandleSource(int secCatalog_i, int timeFrameInMinutes);
    }
}
