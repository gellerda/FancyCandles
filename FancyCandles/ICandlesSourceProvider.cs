using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace FancyCandles
{

    ///<summary>Represents a provider of candle collections for some set of securities.</summary>
    public interface ICandlesSourceProvider
    {
        ///<summary>Gets the list of securities for which this <see cref="ICandlesSourceProvider"/> can provide the candle collection.</summary>
        ///<value>The list of securities for which this <see cref="ICandlesSourceProvider"/> can provide the candle collection.</value>
        IList<ISecurityInfo> SecCatalog { get; }

        ///<summary>Returns the information about one security from <see cref="ICandlesSourceProvider.SecCatalog"/>.</summary>
        ///<param name="secID">The unique identifier of the security to get the information about.</param>
        ISecurityInfo GetSecFromCatalog(string secID);

        ///<summary>Returns the collection of candles of one security.</summary>
        ///<param name="secID">The unique identifier of the security to get the candle collection of.</param>
        ///<param name="timeFrame">The time frame of the candle collection to get.</param>
        ICandlesSourceFromProvider GetCandlesSource(string secID, TimeFrame timeFrame);

        ///<summary>Gets the list of supported time frames.</summary>
        ///<remarks>You can get candle collections of supported time frames only.</remarks>
        IList<TimeFrame> SupportedTimeFrames { get; }
    }
}
