using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
