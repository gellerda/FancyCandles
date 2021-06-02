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
        ///<summary>Gets the timeframe (in minutes) of the candle collection.</summary>
        ///<value>The timeframe (in minutes) of the candle collection.</value>
        int TimeFrameInMinutes { get; }
    }
}
