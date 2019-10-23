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
using System.Globalization;

namespace FancyCandles
{
    // Расширенный Candle. Содержит свой индекс в каком-либо IList.
    struct WholeContainerCandle
    {
        public DateTime t; // Момент времени включая дату и время
        public double O;
        public double H;
        public double L;
        public double C;
        public long V;

        public int Index;
        public double LeftMargin;
        public double BodyWidth;

        // Далее как доля от (High-Low) видимых свечей. Т.е. данные значения нужно домножить на высоту видимой области:
        public double ShadowsHeight;
        public double BodyHeight;
        public double ShadowsBottomMargin;
        public double BodyBottomMargin;

        public double VolumeBarHeight; // Как доля от максимального значения Volume видимых свечей.

        // Текст всплывающей подсказки для свечки: 
        public string ToolTipText;
        public string VolumeToolTipText;

        // Логическая сумма всех DateTimeMilestones для данной свечи:
        public byte DateTimeMilestonesBitwiseSum;

        public WholeContainerCandle(DateTime t, double O, double H, double L, double C, long V, int index, double visibleCandlesRangeLH, double visibleCandlesLow, double bodyWidth, double betweenCandlesWidth, long visibleCandlesMaxVolume, byte dateTimeMilestonesBitwiseSum)
        {
            this.t = t;
            this.O = O;
            this.H = H;
            this.L = L;
            this.C = C;
            this.V = V;
            Index = index;
            ShadowsHeight = (H - L) / visibleCandlesRangeLH;
            BodyHeight = Math.Abs(O - C) / visibleCandlesRangeLH;
            ShadowsBottomMargin = (L - visibleCandlesLow) / visibleCandlesRangeLH;
            BodyBottomMargin = (Math.Min(O, C) - visibleCandlesLow) / visibleCandlesRangeLH;
            ToolTipText = $"{t.ToString("g", CultureInfo.CurrentCulture)}\nO={O}\nH={H}\nL={L}\nC={C}\nV={V}"; //"d.MM.yyyy H:mm"
            VolumeToolTipText = $"{t.ToString("g", CultureInfo.CurrentCulture)}\nV={V}";
            BodyWidth = bodyWidth;
            LeftMargin = (bodyWidth + betweenCandlesWidth) * index;
            VolumeBarHeight = V / (double)visibleCandlesMaxVolume;
            DateTimeMilestonesBitwiseSum = dateTimeMilestonesBitwiseSum;
        }

        public WholeContainerCandle(ICandle cndl, int index, double visibleCandlesRangeLH, double visibleCandlesLow, double bodyWidth, double betweenCandlesWidth, long visibleCandlesMaxVolume, byte dateTimeMilestonesBitwiseSum)
        {
            t = cndl.t;
            O = cndl.O;
            H = cndl.H;
            L = cndl.L;
            C = cndl.C;
            V = cndl.V;
            Index = index;
            ShadowsHeight = (H - L) / visibleCandlesRangeLH;
            BodyHeight = Math.Abs(O - C) / visibleCandlesRangeLH;
            ShadowsBottomMargin = (L - visibleCandlesLow) / visibleCandlesRangeLH;
            BodyBottomMargin = (Math.Min(O, C) - visibleCandlesLow) / visibleCandlesRangeLH;
            ToolTipText = $"{t.ToString("d.MM.yyyy H:mm")}\nO={O}\nH={H}\nL={L}\nC={C}\nV={V}";
            VolumeToolTipText = $"{t.ToString("d.MM.yyyy H:mm")}\nV={V}";
            BodyWidth = bodyWidth;
            LeftMargin = (bodyWidth + betweenCandlesWidth) * index;
            VolumeBarHeight = V / (double)visibleCandlesMaxVolume;
            DateTimeMilestonesBitwiseSum = dateTimeMilestonesBitwiseSum;
        }
    }
}
