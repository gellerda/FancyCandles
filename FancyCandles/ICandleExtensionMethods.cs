using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Diagnostics;

namespace FancyCandles
{
    //**************************************************************************************************************************
    class CandleComparerByDatetime : Comparer<ICandle>
    {
        public override int Compare(ICandle c1, ICandle c2)
        {
            if (c1.t == c2.t)
                return 0;
            else if (c1.t > c2.t)
                return 1;
            else
                return -1;
        }
    }
    //**************************************************************************************************************************
    static class ICandleCollectionExtensionMethods
    {
        public static int BinarySearchOfExistingCandleInObservableCollection(this ObservableCollection<ICandle> candles, ICandle candleToFind)
        {
            CandleComparerByDatetime comparer = new CandleComparerByDatetime();

            int i0 = 0, i1 = candles.Count - 1;

            int res = comparer.Compare(candleToFind, candles[i0]);
            if (res == 0) return i0;
            else if (res < 0) return -1;

            res = comparer.Compare(candleToFind, candles[i1]);
            if (res == 0) return i1;
            else if (res > 0) return -1;

            while (true)
            {
                if ((i0 + 1) == i1) return i1;

                int i = (i0 + i1) / 2;
                res = comparer.Compare(candleToFind, candles[i]);
                if (res == 0) return i;
                else if (res > 0)
                    i0 = i;
                else
                    i1 = i;
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        // В упорядоченной по возрастанию t коллекции IList<ICandle> находит свечку с t. Возвращает ее индекс. Если такой свечки нет, то ближайшую. 
        public static int FindCandleByDatetime(this IList<ICandle> candles, DateTime t)
        {
            int i0 = 0, i1 = candles.Count - 1;
            DateTime t_ = candles[i0].t;
            if (t <= t_) return i0;
            //else if (t < t_) return -1;

            t_ = candles[i1].t;
            if (t >= t_) return i1;
            //else if (t > t_) return -1;

            while (true)
            {
                if ((i0 + 1) == i1) return i1;

                int i = (i0 + i1) / 2;
                t_ = candles[i].t;
                if (t == t_) return i;
                else if (t > t_)
                    i0 = i;
                else
                    i1 = i;
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------
    }
}
