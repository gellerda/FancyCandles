using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FancyCandles;

namespace CandlesSourceProviderExample
{
    public class Candle : ICandle
    {
        public DateTime t { get; set; }
        public double O { get; set; }
        public double H { get; set; }
        public double L { get; set; }
        public double C { get; set; }
        public double V { get; set; }

        public Candle(DateTime t, double O, double H, double L, double C, double V)
        {
            this.t = t;
            this.O = O;
            this.H = H;
            this.L = L;
            this.C = C;
            this.V = V;
        }
    }
    //**************************************************************************************************************************
    public static class ICandleExtensionMethods
    {
        public static bool IsEqualByValue(this ICandle cndl1, ICandle cndl2)
        {
            return cndl1.O == cndl2.O && cndl1.H == cndl2.H && cndl1.L == cndl2.L && cndl1.C == cndl2.C && cndl1.V == cndl2.V && cndl1.t == cndl2.t;
        }
    }
}
