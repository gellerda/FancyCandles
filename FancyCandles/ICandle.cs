using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyCandles
{
    public interface ICandle
    {
        DateTime t { get; } // Момент времени включая дату и время
        double O { get;}
        double H { get;}
        double L { get;}
        double C { get;}
        long V { get;}
    }
}
