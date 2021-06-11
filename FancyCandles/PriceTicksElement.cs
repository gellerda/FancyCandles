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
using System.Windows;
using System.Windows.Media;
using System.Globalization;

namespace FancyCandles
{
    class PriceTicksElement : FrameworkElement
    {
        public static double TICK_LINE_WIDTH = 3.0;
        public static double TICK_HORIZ_MARGIN = 2.0;

        //---------------------------------------------------------------------------------------------------------------------------------------
        static PriceTicksElement()
        {
            Pen defaultPen = new Pen(CandleChart.DefaultHorizontalGridlinesBrush, CandleChart.DefaultHorizontalGridlinesThickness);
            defaultPen.Freeze();
            GridlinesPenProperty = DependencyProperty.Register("GridlinesPen", typeof(Pen), typeof(PriceTicksElement), 
                new FrameworkPropertyMetadata(defaultPen, null, CoerceGridlinesPen) { AffectsRender = true });
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public PriceTicksElement()
        {
            if (tickPen == null)
            {
                tickPen = new Pen(CandleChart.DefaultAxisTickColor, 1.0);
                if (!tickPen.IsFrozen)
                    tickPen.Freeze();
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public CultureInfo Culture
        {
            get { return (CultureInfo)GetValue(CultureProperty); }
            set { SetValue(CultureProperty, value); }
        }
        public static readonly DependencyProperty CultureProperty =
            DependencyProperty.Register("Culture", typeof(CultureInfo), typeof(PriceTicksElement), new FrameworkPropertyMetadata(CultureInfo.CurrentCulture) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public double CurrentPrice
        {
            get { return (double)GetValue(CurrentPriceProperty); }
            set { SetValue(CurrentPriceProperty, value); }
        }
        public static readonly DependencyProperty CurrentPriceProperty =
            DependencyProperty.Register("CurrentPrice", typeof(double), typeof(PriceTicksElement), new FrameworkPropertyMetadata(0.0) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public bool IsCurrentPriceLabelVisible
        {
            get { return (bool)GetValue(IsCurrentPriceLabelVisibleProperty); }
            set { SetValue(IsCurrentPriceLabelVisibleProperty, value); }
        }
        public static readonly DependencyProperty IsCurrentPriceLabelVisibleProperty =
            DependencyProperty.Register("IsCurrentPriceLabelVisible", typeof(bool), typeof(PriceTicksElement), new FrameworkPropertyMetadata(true) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        private Pen currentPriceLabelForegroundPen;

        public Brush CurrentPriceLabelForeground
        {
            get { return (Brush)GetValue(CurrentPriceLabelForegroundProperty); }
            set { SetValue(CurrentPriceLabelForegroundProperty, value); }
        }
        public static readonly DependencyProperty CurrentPriceLabelForegroundProperty =
            DependencyProperty.Register("CurrentPriceLabelForeground", typeof(Brush), typeof(PriceTicksElement), 
                new FrameworkPropertyMetadata(CandleChart.DefaultCurrentPriceLabelForeground, null, CoerceCurrentPriceLabelForeground) { AffectsRender = true });

        private static object CoerceCurrentPriceLabelForeground(DependencyObject objWithOldDP, object newDPValue)
        {
            PriceTicksElement thisElement = (PriceTicksElement)objWithOldDP;
            Brush newBrushValue = (Brush)newDPValue;

            if (newBrushValue.IsFrozen)
            {
                Pen p = new Pen(newBrushValue, 1.0);
                p.Freeze();
                thisElement.currentPriceLabelForegroundPen = p;
                return newDPValue;
            }
            else
            {
                Brush b = (Brush)newBrushValue.GetCurrentValueAsFrozen();
                Pen p = new Pen(b, 1.0);
                p.Freeze();
                thisElement.currentPriceLabelForegroundPen = p;
                return b;
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        public Brush CurrentPriceLabelBackground
        {
            get { return (Brush)GetValue(CurrentPriceLabelBackgroundProperty); }
            set { SetValue(CurrentPriceLabelBackgroundProperty, value); }
        }
        public static readonly DependencyProperty CurrentPriceLabelBackgroundProperty =
            DependencyProperty.Register("CurrentPriceLabelBackground", typeof(Brush), typeof(PriceTicksElement), 
                new FrameworkPropertyMetadata(CandleChart.DefaultCurrentPriceLabelBackground, null, CoerceCurrentPriceLabelBackground) { AffectsRender = true });

        private static object CoerceCurrentPriceLabelBackground(DependencyObject objWithOldDP, object newDPValue)
        {
            PriceTicksElement thisElement = (PriceTicksElement)objWithOldDP;
            Brush newBrushValue = (Brush)newDPValue;

            if (newBrushValue.IsFrozen)
                return newDPValue;
            else
                return (Brush)newBrushValue.GetCurrentValueAsFrozen();
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        private Pen tickPen;

        public Brush TickColor
        {
            get { return (Brush)GetValue(TickColorProperty); }
            set { SetValue(TickColorProperty, value); }
        }
        public static readonly DependencyProperty TickColorProperty
            = DependencyProperty.Register("TickColor", typeof(Brush), typeof(PriceTicksElement),
                new FrameworkPropertyMetadata(CandleChart.DefaultAxisTickColor, null, CoerceTickColor) { AffectsRender = true });

        private static object CoerceTickColor(DependencyObject objWithOldDP, object newDPValue)
        {
            PriceTicksElement thisElement = (PriceTicksElement)objWithOldDP;
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
        private Typeface currentTypeFace = new Typeface(SystemFonts.MessageFontFamily.ToString());

        public FontFamily TickLabelFontFamily
        {
            get { return (FontFamily)GetValue(TickLabelFontFamilyProperty); }
            set { SetValue(TickLabelFontFamilyProperty, value); }
        }
        public static readonly DependencyProperty TickLabelFontFamilyProperty =
            DependencyProperty.Register("TickLabelFontFamily", typeof(FontFamily), typeof(PriceTicksElement), new FrameworkPropertyMetadata(SystemFonts.MessageFontFamily, OnTickLabelFontFamilyChanged));

        static void OnTickLabelFontFamilyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            PriceTicksElement thisElement = obj as PriceTicksElement;
            if (thisElement == null) return;
            thisElement.currentTypeFace = new Typeface(thisElement.TickLabelFontFamily.ToString());
        }
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
        public bool IsGridlinesEnabled
        {
            get { return (bool)GetValue(IsGridlinesEnabledProperty); }
            set { SetValue(IsGridlinesEnabledProperty, value); }
        }
        public static readonly DependencyProperty IsGridlinesEnabledProperty
            = DependencyProperty.Register("IsGridlinesEnabled", typeof(bool), typeof(PriceTicksElement), new FrameworkPropertyMetadata(true) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public CandleExtremums VisibleCandlesExtremums
        {
            get { return (CandleExtremums)GetValue(VisibleCandlesExtremumsProperty); }
            set { SetValue(VisibleCandlesExtremumsProperty, value); }
        }
        public static readonly DependencyProperty VisibleCandlesExtremumsProperty
            = DependencyProperty.Register("VisibleCandlesExtremums", typeof(CandleExtremums), typeof(PriceTicksElement), new FrameworkPropertyMetadata(new CandleExtremums(1.0, 1.0, 0L, 0L)) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public double GapBetweenTickLabels
        {
            get { return (double)GetValue(GapBetweenTickLabelsProperty); }
            set { SetValue(GapBetweenTickLabelsProperty, value); }
        }
        public static readonly DependencyProperty GapBetweenTickLabelsProperty
            = DependencyProperty.Register("GapBetweenTickLabels", typeof(double), typeof(PriceTicksElement), new FrameworkPropertyMetadata(0.0) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public double ChartBottomMargin
        {
            get { return (double)GetValue(ChartBottomMarginProperty); }
            set { SetValue(ChartBottomMarginProperty, value); }
        }
        public static readonly DependencyProperty ChartBottomMarginProperty
            = DependencyProperty.Register("ChartBottomMargin", typeof(double), typeof(PriceTicksElement), new FrameworkPropertyMetadata(15.0) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public double ChartTopMargin
        {
            get { return (double)GetValue(ChartTopMarginProperty); }
            set { SetValue(ChartTopMarginProperty, value); }
        }
        public static readonly DependencyProperty ChartTopMarginProperty
            = DependencyProperty.Register("ChartTopMargin", typeof(double), typeof(PriceTicksElement), new FrameworkPropertyMetadata(15.0) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public double TickLabelFontSize
        {
            get { return (double)GetValue(TickLabelFontSizeProperty); }
            set { SetValue(TickLabelFontSizeProperty, value); }
        }
        public static readonly DependencyProperty TickLabelFontSizeProperty
            = DependencyProperty.Register("TickLabelFontSize", typeof(double), typeof(PriceTicksElement), new FrameworkPropertyMetadata(9.0) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public double PriceAxisWidth
        {
            get { return (double)GetValue(PricePanelWidthProperty); }
            set { SetValue(PricePanelWidthProperty, value); }
        }
        public static readonly DependencyProperty PricePanelWidthProperty
            = DependencyProperty.Register("PriceAxisWidth", typeof(double), typeof(PriceTicksElement), new FrameworkPropertyMetadata(0.0) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public int MaxNumberOfFractionalDigitsInPrice
        {
            get { return (int)GetValue(MaxNumberOfFractionalDigitsInPriceProperty); }
            set { SetValue(MaxNumberOfFractionalDigitsInPriceProperty, value); }
        }
        public static readonly DependencyProperty MaxNumberOfFractionalDigitsInPriceProperty =
            DependencyProperty.Register("MaxNumberOfFractionalDigitsInPrice", typeof(int), typeof(PriceTicksElement), new FrameworkPropertyMetadata(0));
        //---------------------------------------------------------------------------------------------------------------------------------------
        protected override void OnRender(DrawingContext drawingContext)
        {
            double textHeight = (new FormattedText("123", Culture, FlowDirection.LeftToRight, currentTypeFace, TickLabelFontSize, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip)).Height;
            double halfTextHeight = textHeight / 2.0;
            double chartPanelWidth = ActualWidth - PriceAxisWidth;
            double tickLabelX = chartPanelWidth + TICK_LINE_WIDTH + TICK_HORIZ_MARGIN;
            double tickLineEndX = chartPanelWidth + TICK_LINE_WIDTH;
            double chartHeight = ActualHeight - ChartBottomMargin - ChartTopMargin;

            double stepInRubles = (VisibleCandlesExtremums.PriceHigh - VisibleCandlesExtremums.PriceLow) / chartHeight * (textHeight + GapBetweenTickLabels);
            double stepInRubles_HPlace = MyWpfMath.HighestDecimalPlace(stepInRubles, out int stepInRubles_HPow);
            stepInRubles = Math.Ceiling(stepInRubles / stepInRubles_HPlace) * stepInRubles_HPlace;
            MyWpfMath.HighestDecimalPlace(stepInRubles, out int stepInRublesHighestDecimalPow);
            string priceTickLabelNumberFormat = (stepInRubles_HPow >= 0) ? "N0" : $"N{-stepInRubles_HPow}";
            string currentPriceLabelNumberFormat = $"N{MaxNumberOfFractionalDigitsInPrice}";

            double chartHeight_candlesLHRange_Ratio = chartHeight / (VisibleCandlesExtremums.PriceHigh - VisibleCandlesExtremums.PriceLow);

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            string decimalSeparator = Culture.NumberFormat.NumberDecimalSeparator;
            char[] decimalSeparatorArray = decimalSeparator.ToCharArray();

            void DrawPriceTickLabel(double price, int priceStepHighestDecimalPow)
            {
                string s = MyNumberFormatting.PriceToString(price, priceTickLabelNumberFormat, Culture, decimalSeparator, decimalSeparatorArray);
                FormattedText priceTickFormattedText = new FormattedText(s, Culture, FlowDirection.LeftToRight, currentTypeFace, TickLabelFontSize, TickColor, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                double y = ChartTopMargin + (VisibleCandlesExtremums.PriceHigh - price) * chartHeight_candlesLHRange_Ratio;
                drawingContext.DrawText(priceTickFormattedText, new Point(tickLabelX, y - halfTextHeight));
                drawingContext.DrawLine(tickPen, new Point(chartPanelWidth, y), new Point(tickLineEndX, y));

                if (IsGridlinesEnabled && GridlinesPen != null)
                    drawingContext.DrawLine(GridlinesPen, new Point(0, y), new Point(chartPanelWidth, y));
            }
            
            void DrawCurrentPriceLabel()
            {
                string currentPriceString = MyNumberFormatting.PriceToString(CurrentPrice, currentPriceLabelNumberFormat, Culture, decimalSeparator, decimalSeparatorArray);
                FormattedText formattedText = new FormattedText(currentPriceString, Culture, FlowDirection.LeftToRight, currentTypeFace, TickLabelFontSize, CurrentPriceLabelForeground, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                double formattedTextWidth = formattedText.Width;
                double y = ChartTopMargin + (VisibleCandlesExtremums.PriceHigh - CurrentPrice) * chartHeight_candlesLHRange_Ratio;
                drawingContext.DrawRectangle(CurrentPriceLabelBackground, currentPriceLabelForegroundPen, 
                                             new Rect(chartPanelWidth, y - halfTextHeight, formattedTextWidth + TICK_LINE_WIDTH + 2 * TICK_HORIZ_MARGIN, textHeight + 1.0));
                drawingContext.DrawLine(currentPriceLabelForegroundPen, new Point(chartPanelWidth, y), new Point(tickLineEndX, y));
                drawingContext.DrawText(formattedText, new Point(tickLabelX, y - halfTextHeight));
            }
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            double theMostRoundPrice = MyWpfMath.TheMostRoundValueInsideRange(VisibleCandlesExtremums.PriceLow, VisibleCandlesExtremums.PriceHigh);
            DrawPriceTickLabel(theMostRoundPrice, stepInRublesHighestDecimalPow);

            double maxPriceThreshold = VisibleCandlesExtremums.PriceHigh + (ChartTopMargin - halfTextHeight) / chartHeight_candlesLHRange_Ratio;
            double minPriceThreshold = VisibleCandlesExtremums.PriceHigh + (ChartTopMargin - ActualHeight + halfTextHeight) / chartHeight_candlesLHRange_Ratio;

            int step_i = 1;
            double next_tick;
            while ((next_tick = theMostRoundPrice + step_i * stepInRubles) < maxPriceThreshold)
            {
                DrawPriceTickLabel(next_tick, stepInRublesHighestDecimalPow);
                step_i++;
            }

            step_i = 1;
            while ((next_tick = theMostRoundPrice - step_i * stepInRubles) > minPriceThreshold)
            {
                DrawPriceTickLabel(next_tick, stepInRublesHighestDecimalPow);
                step_i++;
            }

            if (IsCurrentPriceLabelVisible && CurrentPrice >= VisibleCandlesExtremums.PriceLow && CurrentPrice <= VisibleCandlesExtremums.PriceHigh)
                DrawCurrentPriceLabel();

            // Горизонтальные линии на всю ширину разделяющая и окаймляющая панели времени и даты:
            //drawingContext.DrawLine(pen, new Point(0, 0), new Point(RenderSize.Width, 0));
            //drawingContext.DrawLine(pen, new Point(0, halfRenderSizeHeight), new Point(RenderSize.Width, halfRenderSizeHeight));
            //drawingContext.DrawLine(pen, new Point(0, RenderSize.Height), new Point(RenderSize.Width, RenderSize.Height));
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
    }
}
