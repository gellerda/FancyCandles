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
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ICandle'
    public interface ICandle
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ICandle'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ICandle.t'
        DateTime t { get; } // Момент времени включая дату и время
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ICandle.t'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ICandle.O'
        double O { get;}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ICandle.O'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ICandle.H'
        double H { get;}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ICandle.H'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ICandle.L'
        double L { get;}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ICandle.L'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ICandle.C'
        double C { get;}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ICandle.C'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ICandle.V'
        long V { get;}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ICandle.V'
    }
}
