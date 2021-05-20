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

using System.Windows;
using System.Windows.Media;
using System.Globalization;

namespace FancyCandles
{
    class VolumeTicksElement : FrameworkElement
    {
        public static double TICK_LINE_WIDTH = 3.0;
        public static double TICK_LEFT_MARGIN = 2.0;
        public static double TICK_RIGHT_MARGIN = 1.0;
        //---------------------------------------------------------------------------------------------------------------------------------------
        static VolumeTicksElement()
        {
            Pen defaultPen = new Pen(CandleChart.DefaultHorizontalGridlinesBrush, CandleChart.DefaultHorizontalGridlinesThickness);
            defaultPen.Freeze();
            GridlinesPenProperty = DependencyProperty.Register("GridlinesPen", typeof(Pen), typeof(VolumeTicksElement),
                new FrameworkPropertyMetadata(defaultPen, null, CoerceGridlinesPen) { AffectsRender = true });
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public CultureInfo Culture
        {
            get { return (CultureInfo)GetValue(CultureProperty); }
            set { SetValue(CultureProperty, value); }
        }
        public static readonly DependencyProperty CultureProperty =
            DependencyProperty.Register("Culture", typeof(CultureInfo), typeof(VolumeTicksElement), new FrameworkPropertyMetadata(CultureInfo.CurrentCulture) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public VolumeTicksElement()
        {
            if (axisTickPen == null)
            {
                axisTickPen = new Pen(CandleChart.DefaultAxisTickColor, 1.0);
                if (!axisTickPen.IsFrozen)
                    axisTickPen.Freeze();
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public bool IsGridlinesEnabled
        {
            get { return (bool)GetValue(IsGridlinesEnabledProperty); }
            set { SetValue(IsGridlinesEnabledProperty, value); }
        }
        public static readonly DependencyProperty IsGridlinesEnabledProperty
            = DependencyProperty.Register("IsGridlinesEnabled", typeof(bool), typeof(VolumeTicksElement), new FrameworkPropertyMetadata(true) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public Pen GridlinesPen
        {
            get { return (Pen)GetValue(GridlinesPenProperty); }
            set { SetValue(GridlinesPenProperty, value); }
        }
        public static readonly DependencyProperty GridlinesPenProperty;

        private static object CoerceGridlinesPen(DependencyObject objWithOldDP, object newDPValue)
        {
            Pen newPenValue = (Pen)newDPValue;
            return newPenValue.IsFrozen ? newDPValue : newPenValue.GetCurrentValueAsFrozen();
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public CandleExtremums VisibleCandlesExtremums
        {
            get { return (CandleExtremums)GetValue(VisibleCandlesExtremumsProperty); }
            set { SetValue(VisibleCandlesExtremumsProperty, value); }
        }
        public static readonly DependencyProperty VisibleCandlesExtremumsProperty
            = DependencyProperty.Register("VisibleCandlesExtremums", typeof(CandleExtremums), typeof(VolumeTicksElement),
                new FrameworkPropertyMetadata(new CandleExtremums(0.0, 0.0, long.MinValue, long.MinValue)) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public double GapBetweenTickLabels
        {
            get { return (double)GetValue(GapBetweenTickLabelsProperty); }
            set { SetValue(GapBetweenTickLabelsProperty, value); }
        }
        public static readonly DependencyProperty GapBetweenTickLabelsProperty
            = DependencyProperty.Register("GapBetweenTickLabels", typeof(double), typeof(VolumeTicksElement), new FrameworkPropertyMetadata(0.0) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public double ChartBottomMargin
        {
            get { return (double)GetValue(ChartBottomMarginProperty); }
            set { SetValue(ChartBottomMarginProperty, value); }
        }
        public static readonly DependencyProperty ChartBottomMarginProperty
             = DependencyProperty.Register("ChartBottomMargin", typeof(double), typeof(VolumeTicksElement), new FrameworkPropertyMetadata(15.0) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public double ChartTopMargin
        {
            get { return (double)GetValue(ChartTopMarginProperty); }
            set { SetValue(ChartTopMarginProperty, value); }
        }
        public static readonly DependencyProperty ChartTopMarginProperty
            = DependencyProperty.Register("ChartTopMargin", typeof(double), typeof(VolumeTicksElement), new FrameworkPropertyMetadata(15.0) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public double PriceTickFontSize
        {
            get { return (double)GetValue(PriceTickFontSizeProperty); }
            set { SetValue(PriceTickFontSizeProperty, value); }
        }
        public static readonly DependencyProperty PriceTickFontSizeProperty 
            = CandleChart.PriceTickFontSizeProperty.AddOwner(typeof(VolumeTicksElement),
                new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnPriceTickFontSizeChanged)) { AffectsRender = true });
        private static void OnPriceTickFontSizeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            //VolumeTicksElement thisElement = (VolumeTicksElement)obj;
            //thisElement.InvalidateMeasure();
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        private Pen axisTickPen;

        public Brush AxisTickColor
        {
            get { return (Brush)GetValue(AxisTickColorProperty); }
            set { SetValue(AxisTickColorProperty, value); }
        }
        public static readonly DependencyProperty AxisTickColorProperty
            = DependencyProperty.Register("AxisTickColor", typeof(Brush), typeof(VolumeTicksElement),
                new FrameworkPropertyMetadata(CandleChart.DefaultAxisTickColor, null, CoerceAxisTickColor) { AffectsRender = true });

        private static object CoerceAxisTickColor(DependencyObject objWithOldDP, object newDPValue)
        {
            VolumeTicksElement thisElement = (VolumeTicksElement)objWithOldDP;
            Brush newBrushValue = (Brush)newDPValue;

            if (newBrushValue.IsFrozen)
            {
                Pen p = new Pen(newBrushValue, 1.0);
                p.Freeze();
                thisElement.axisTickPen = p;
                return newDPValue;
            }
            else
            {
                Brush b = (Brush)newBrushValue.GetCurrentValueAsFrozen();
                Pen p = new Pen(b, 1.0);
                p.Freeze();
                thisElement.axisTickPen = p;
                return b;
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public double PriceAxisWidth
        {
            get { return (double)GetValue(PricePanelWidthProperty); }
            set { SetValue(PricePanelWidthProperty, value); }
        }
        public static readonly DependencyProperty PricePanelWidthProperty 
            = DependencyProperty.Register("PriceAxisWidth", typeof(double), typeof(VolumeTicksElement), new FrameworkPropertyMetadata(0.0) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (VisibleCandlesExtremums.VolumeHigh == long.MinValue) return;

            double textHeight = (new FormattedText("123", Culture, FlowDirection.LeftToRight, new Typeface("Verdana"), PriceTickFontSize, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip)).Height;
            double halfTextHeight = textHeight / 2.0;
            double volumeHistogramPanelWidth = ActualWidth - PriceAxisWidth;
            double tick_text_X = volumeHistogramPanelWidth + TICK_LINE_WIDTH + TICK_LEFT_MARGIN;
            double tick_line_endX = volumeHistogramPanelWidth + TICK_LINE_WIDTH;

            double chartHeight = ActualHeight - ChartBottomMargin - ChartTopMargin;
            if (chartHeight <= 0) return;
            long stepInVolumeLots = (long)(VisibleCandlesExtremums.VolumeHigh * ((textHeight + GapBetweenTickLabels) / chartHeight)) + 1;
            long stepInVolumeLots_maxDigit = MyWpfMath.MaxDigit(stepInVolumeLots);
            stepInVolumeLots = (stepInVolumeLots % stepInVolumeLots_maxDigit) == 0 ? stepInVolumeLots : (stepInVolumeLots / stepInVolumeLots_maxDigit + 1) * stepInVolumeLots_maxDigit;
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            double chartHeight_candlesLHRange_Ratio = chartHeight / VisibleCandlesExtremums.VolumeHigh;

            string decimalSeparator = Culture.NumberFormat.NumberDecimalSeparator;
            char[] decimalSeparatorArray = decimalSeparator.ToCharArray();

            void DrawPriceTick(long volume)
            {
                string s = volume.MyToString(Culture, decimalSeparator, decimalSeparatorArray);
                FormattedText priceTickFormattedText = new FormattedText(s, Culture, FlowDirection.LeftToRight, new Typeface("Verdana"), PriceTickFontSize, AxisTickColor, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                double y = ChartTopMargin + (VisibleCandlesExtremums.VolumeHigh - volume) * chartHeight_candlesLHRange_Ratio;
                drawingContext.DrawText(priceTickFormattedText, new Point(tick_text_X, y - halfTextHeight));
                drawingContext.DrawLine(axisTickPen, new Point(volumeHistogramPanelWidth, y), new Point(tick_line_endX, y));

                if (IsGridlinesEnabled && GridlinesPen != null)
                    drawingContext.DrawLine(GridlinesPen, new Point(0, y), new Point(volumeHistogramPanelWidth, y));
            }
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            long theMostRoundVolume = MyWpfMath.MaxDigit(VisibleCandlesExtremums.VolumeHigh);
            DrawPriceTick(theMostRoundVolume);

            long maxVolumeThreshold = (long)(VisibleCandlesExtremums.VolumeHigh + (ChartTopMargin - halfTextHeight) / chartHeight_candlesLHRange_Ratio);
            long minVolumeThreshold = (long)(VisibleCandlesExtremums.VolumeHigh + (ChartTopMargin - ActualHeight + halfTextHeight) / chartHeight_candlesLHRange_Ratio);

            int step_i = 1;
            long next_tick = theMostRoundVolume + step_i * stepInVolumeLots;
            while (next_tick < maxVolumeThreshold)
            {
                DrawPriceTick(next_tick);
                step_i++;
                next_tick = theMostRoundVolume + step_i * stepInVolumeLots;
            }

            step_i = 1;
            next_tick = theMostRoundVolume - step_i * stepInVolumeLots;
            while (next_tick > minVolumeThreshold)
            {
                DrawPriceTick(next_tick);
                step_i++;
                next_tick = theMostRoundVolume - step_i * stepInVolumeLots;
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
    }
}
