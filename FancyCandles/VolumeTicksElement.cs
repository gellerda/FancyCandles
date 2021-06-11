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
using System;

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
            if (tickPen == null)
            {
                tickPen = new Pen(CandleChart.DefaultAxisTickColor, 1.0);
                if (!tickPen.IsFrozen)
                    tickPen.Freeze();
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
                new FrameworkPropertyMetadata(new CandleExtremums()) { AffectsRender = true });
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
        private Typeface currentTypeFace = new Typeface(SystemFonts.MessageFontFamily.ToString());

        public FontFamily TickLabelFontFamily
        {
            get { return (FontFamily)GetValue(TickLabelFontFamilyProperty); }
            set { SetValue(TickLabelFontFamilyProperty, value); }
        }
        public static readonly DependencyProperty TickLabelFontFamilyProperty =
            DependencyProperty.Register("TickLabelFontFamily", typeof(FontFamily), typeof(VolumeTicksElement), new FrameworkPropertyMetadata(SystemFonts.MessageFontFamily, OnTickLabelFontFamilyChanged));

        static void OnTickLabelFontFamilyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            VolumeTicksElement thisElement = obj as VolumeTicksElement;
            if (thisElement == null) return;
            thisElement.currentTypeFace = new Typeface(thisElement.TickLabelFontFamily.ToString());
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public double TickLabelFontSize
        {
            get { return (double)GetValue(TickLabelFontSizeProperty); }
            set { SetValue(TickLabelFontSizeProperty, value); }
        }
        public static readonly DependencyProperty TickLabelFontSizeProperty
            = DependencyProperty.Register("TickLabelFontSize", typeof(double), typeof(VolumeTicksElement), new FrameworkPropertyMetadata(9.0) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        private Pen tickPen;

        public Brush TickColor
        {
            get { return (Brush)GetValue(TickColorProperty); }
            set { SetValue(TickColorProperty, value); }
        }
        public static readonly DependencyProperty TickColorProperty
            = DependencyProperty.Register("TickColor", typeof(Brush), typeof(VolumeTicksElement),
                new FrameworkPropertyMetadata(CandleChart.DefaultAxisTickColor, null, CoerceTickColor) { AffectsRender = true });

        private static object CoerceTickColor(DependencyObject objWithOldDP, object newDPValue)
        {
            VolumeTicksElement thisElement = (VolumeTicksElement)objWithOldDP;
            Brush newBrushValue = (Brush)newDPValue;

            if (newBrushValue.IsFrozen)
            {
                Pen p = new Pen(newBrushValue, 1.0);
                p.Freeze();
                thisElement.tickPen = p;
                return newDPValue;
            }
            else
            {
                Brush b = (Brush)newBrushValue.GetCurrentValueAsFrozen();
                Pen p = new Pen(b, 1.0);
                p.Freeze();
                thisElement.tickPen = p;
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
            //if (VisibleCandlesExtremums.VolumeHigh == long.MinValue) return;

            double textHeight = (new FormattedText("1,23", Culture, FlowDirection.LeftToRight, currentTypeFace, TickLabelFontSize, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip)).Height;
            double halfTextHeight = textHeight / 2.0;
            double chartPanelWidth = ActualWidth - PriceAxisWidth;
            double tickLabelX = chartPanelWidth + TICK_LINE_WIDTH + TICK_LEFT_MARGIN;
            double tickLineEndX = chartPanelWidth + TICK_LINE_WIDTH;
            double chartHeight = ActualHeight - ChartBottomMargin - ChartTopMargin;
            if (chartHeight <= 0) return;

            double stepInVolumeUnits = VisibleCandlesExtremums.VolumeHigh * ((textHeight + GapBetweenTickLabels) / chartHeight);
            double stepInVolumeUnits_HPlace = MyWpfMath.HighestDecimalPlace(stepInVolumeUnits, out _);
            stepInVolumeUnits = Math.Ceiling(stepInVolumeUnits / stepInVolumeUnits_HPlace) * stepInVolumeUnits_HPlace;
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            double chartHeight_candlesLHRange_Ratio = chartHeight / VisibleCandlesExtremums.VolumeHigh;

            string decimalSeparator = Culture.NumberFormat.NumberDecimalSeparator;
            char[] decimalSeparatorArray = decimalSeparator.ToCharArray();

            void DrawVolumeTick(double volume)
            {
                string s = MyNumberFormatting.VolumeToLimitedLengthString(volume, Culture, decimalSeparator, decimalSeparatorArray);
                FormattedText priceTickFormattedText = new FormattedText(s, Culture, FlowDirection.LeftToRight, currentTypeFace, TickLabelFontSize, TickColor, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                double y = ChartTopMargin + (VisibleCandlesExtremums.VolumeHigh - volume) * chartHeight_candlesLHRange_Ratio;
                drawingContext.DrawText(priceTickFormattedText, new Point(tickLabelX, y - halfTextHeight));
                drawingContext.DrawLine(tickPen, new Point(chartPanelWidth, y), new Point(tickLineEndX, y));

                if (IsGridlinesEnabled && GridlinesPen != null)
                    drawingContext.DrawLine(GridlinesPen, new Point(0, y), new Point(chartPanelWidth, y));
            }
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            double theMostRoundVolume = MyWpfMath.HighestDecimalPlace(VisibleCandlesExtremums.VolumeHigh, out _);
            DrawVolumeTick(theMostRoundVolume);

            double maxVolumeThreshold = (VisibleCandlesExtremums.VolumeHigh + (ChartTopMargin - halfTextHeight) / chartHeight_candlesLHRange_Ratio);
            double minVolumeThreshold = (VisibleCandlesExtremums.VolumeHigh + (ChartTopMargin - ActualHeight + halfTextHeight) / chartHeight_candlesLHRange_Ratio);

            int step_i = 1;
            double next_tick;
            while ((next_tick = theMostRoundVolume + step_i * stepInVolumeUnits) < maxVolumeThreshold)
            {
                DrawVolumeTick(next_tick);
                step_i++;
            }

            step_i = 1;
            while ((next_tick = theMostRoundVolume - step_i * stepInVolumeUnits) > minVolumeThreshold)
            {
                DrawVolumeTick(next_tick);
                step_i++;
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
    }
}
