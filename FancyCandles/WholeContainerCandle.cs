using System;

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
            ToolTipText = $"{t.ToString("d.MM.yyyy H:mm")}\nO={O}\nH={H}\nL={L}\nC={C}\nV={V}";
            VolumeToolTipText = $"{t.ToString("d.MM.yyyy H:mm")}\nV={V}";
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
