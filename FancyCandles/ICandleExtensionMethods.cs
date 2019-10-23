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
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
