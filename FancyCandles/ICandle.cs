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

using System;

namespace FancyCandles
{
    ///<summary>Represents the most widely used candlestick parameters.</summary>
    public interface ICandle
    {
        ///<summary>Gets the time of the candlestick.</summary>
        ///<value>The time of the candlestick.</value>
        DateTime t { get; } // Момент времени включая дату и время

        ///<summary>Gets the Open of the candlestick (opening price).</summary>
        ///<value>The Open of the candlestick (opening price).</value>
        double O { get;}

        ///<summary>Gets the High of the candlestick (price maximum).</summary>
        ///<value>The High of the candlestick (price maximum).</value>
        double H { get;}

        ///<summary>Gets the Low of the candlestick (price minimum).</summary>
        ///<value>The Low of the candlestick (price minimum).</value>
        double L { get;}

        ///<summary>Gets the Close of the candlestick (closing price).</summary>
        ///<value>The Close of the candlestick (closing price).</value>
        double C { get;}

        ///<summary>Gets the Volume of the candlestick.</summary>
        ///<value>The Volume of the candlestick.</value>
        double V { get;}
    }
}
