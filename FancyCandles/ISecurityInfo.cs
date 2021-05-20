using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyCandles
{
    public enum SecurityTypes
    { 
        Undefined,
        Stock,
        Futures,
        Option,
        CurrencyPair,
        Commodity,
        Index,
        Bond,
        Cryptocurrency
    }

    public interface ISecurityInfo
    {
        SecurityTypes SecurityType { get; }
        string ExchangeName { get; }
        string Ticker { get; }
        string SecurityName { get; }
    }
}
