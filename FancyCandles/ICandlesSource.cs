/* 
    Copyright 2019 Dennis Geller.

    This file is part of FancyCandles.

    FancyCandles is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    FancyCandles is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with FancyCandles.  If not, see<https://www.gnu.org/licenses/>. */

using System.Collections.Generic;

namespace FancyCandles
{
    ///<summary>Represents a collection of candles, that can be used as a value for the <see cref="CandleChart.CandlesSource"/> property of the <see cref="CandleChart"/> class.</summary>
    ///<seealso cref = "ICandle">Interface ICandle</seealso>
    public interface ICandlesSource : IList<ICandle> 
    {
        ///<summary>Gets the time frame of the candle collection.</summary>
        ///<value>The time frame of the candle collection.</value>
        TimeFrame TimeFrame { get; }
    }

    ///<summary>Represents a collection of candles obtained from <see cref="ICandlesSourceProvider"/>, that can be used as a value for the <see cref="CandleChart.CandlesSource"/> property of the <see cref="CandleChart"/> class.</summary>
    ///<seealso cref = "ICandlesSourceProvider">Interface ICandlesSourceProvider</seealso>
    public interface ICandlesSourceFromProvider : ICandlesSource
    {
        ///<summary>Gets the unique ID of the security, this candle collection relates to.</summary>
        ///<value>The unique ID of the security, this candle collection relates to.</value>
        ///<remarks>
        ///<see cref="ICandlesSourceProvider"/> widely operates with the <see cref="ISecurityInfo.SecID"/> value.
        ///<see cref="ISecurityInfo.SecID"/> allows you to uniquely identify one security among others this <see cref="ICandlesSourceProvider"/> provide you access to.
        ///</remarks>
        string SecID { get; }
    }

    ///<summary>Represents some resource that a counter of its users.</summary>
    public interface IResourceWithUserCounter
    {
        ///<summary>Gets the number of users of this resource.</summary>
        ///<value>Gets the number of users of this resource.</value>
        int UserCount { get; }

        ///<summary>Increments the number of users of this resource.</summary>
        void IncreaseUserCount();

        ///<summary>Decrements the number of users of this resource.</summary>
        void DecreaseUserCount();
    }
}
