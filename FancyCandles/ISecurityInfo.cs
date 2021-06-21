using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyCandles
{
    ///<summary>Represents the supported security types.</summary>
    public enum SecurityTypes
    {
#pragma warning  disable CS1591
        Undefined,
        Stock,
        Futures,
        Option,
        CurrencyPair,
        Commodity,
        Index,
        Bond,
        CryptocurrencyPair
#pragma warning  restore CS1591
    }

    ///<summary>Represents the descriptive information about one security.</summary>
    public interface ISecurityInfo
    {
        ///<summary>Gets the unique identifier of this security.</summary>
        string SecID { get; }

        ///<summary>Gets the type of this security.</summary>
        SecurityTypes SecurityType { get; }

        ///<summary>Gets the exchange name this security is listed on.</summary>
        string ExchangeName { get; }

        ///<summary>Gets the ticker of this security on the <see cref="ISecurityInfo.ExchangeName"/> exchange.</summary>
        string Ticker { get; }

        ///<summary>Gets the name of this security.</summary>
        string SecurityName { get; }
    }
}
