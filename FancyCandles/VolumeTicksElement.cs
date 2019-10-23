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
            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnPriceTickFontSizeChanged)) { AffectsRender = true };
            PriceTickFontSizeProperty = CandleChart.PriceTickFontSizeProperty.AddOwner(typeof(VolumeTicksElement), metadata);

            metadata = new FrameworkPropertyMetadata(0.0) { AffectsRender = true };
            PricePanelWidthProperty = DependencyProperty.Register("PriceAxisWidth", typeof(double), typeof(VolumeTicksElement), metadata);

            metadata = new FrameworkPropertyMetadata(long.MinValue) { AffectsRender = true };
            CandlesMaxVolumeProperty = DependencyProperty.Register("CandlesMaxVolume", typeof(long), typeof(VolumeTicksElement), metadata);

            metadata = new FrameworkPropertyMetadata(15.0) { AffectsRender = true };
            ChartBottomMarginProperty = DependencyProperty.Register("ChartBottomMargin", typeof(double), typeof(VolumeTicksElement), metadata);

            metadata = new FrameworkPropertyMetadata(15.0) { AffectsRender = true };
            ChartTopMarginProperty = DependencyProperty.Register("ChartTopMargin", typeof(double), typeof(VolumeTicksElement), metadata);

            metadata = new FrameworkPropertyMetadata(0.0) { AffectsRender = true };
            GapBetweenTickLabelsProperty = DependencyProperty.Register("GapBetweenTickLabels", typeof(double), typeof(VolumeTicksElement), metadata);

            metadata = new FrameworkPropertyMetadata(true) { AffectsRender = true };
            IsGridlinesEnabledProperty = DependencyProperty.Register("IsGridlinesEnabled", typeof(bool), typeof(VolumeTicksElement), metadata);

            Pen defaultPen = new Pen(new SolidColorBrush(Color.FromArgb(50, 105, 42, 0)), 1); // { DashStyle = new DashStyle(new double[] { 2, 3 }, 0) };
            metadata = new FrameworkPropertyMetadata(defaultPen) { AffectsRender = true };
            GridlinesPenProperty = DependencyProperty.Register("GridlinesPen", typeof(Pen), typeof(VolumeTicksElement), metadata);
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public bool IsGridlinesEnabled
        {
            get { return (bool)GetValue(IsGridlinesEnabledProperty); }
            set { SetValue(IsGridlinesEnabledProperty, value); }
        }
        public static readonly DependencyProperty IsGridlinesEnabledProperty;
        //---------------------------------------------------------------------------------------------------------------------------------------
        public Pen GridlinesPen
        {
            get { return (Pen)GetValue(GridlinesPenProperty); }
            set { SetValue(GridlinesPenProperty, value); }
        }
        public static readonly DependencyProperty GridlinesPenProperty;
        //---------------------------------------------------------------------------------------------------------------------------------------
        public long CandlesMaxVolume
        {
            get { return (long)GetValue(CandlesMaxVolumeProperty); }
            set { SetValue(CandlesMaxVolumeProperty, value); }
        }
        public static readonly DependencyProperty CandlesMaxVolumeProperty;
        //---------------------------------------------------------------------------------------------------------------------------------------
        public double GapBetweenTickLabels
        {
            get { return (double)GetValue(GapBetweenTickLabelsProperty); }
            set { SetValue(GapBetweenTickLabelsProperty, value); }
        }
        public static readonly DependencyProperty GapBetweenTickLabelsProperty;
        //---------------------------------------------------------------------------------------------------------------------------------------
        public double ChartBottomMargin
        {
            get { return (double)GetValue(ChartBottomMarginProperty); }
            set { SetValue(ChartBottomMarginProperty, value); }
        }
        public static readonly DependencyProperty ChartBottomMarginProperty;
        //---------------------------------------------------------------------------------------------------------------------------------------
        public double ChartTopMargin
        {
            get { return (double)GetValue(ChartTopMarginProperty); }
            set { SetValue(ChartTopMarginProperty, value); }
        }
        public static readonly DependencyProperty ChartTopMarginProperty;
        //---------------------------------------------------------------------------------------------------------------------------------------
        public double PriceTickFontSize
        {
            get { return (double)GetValue(PriceTickFontSizeProperty); }
            set { SetValue(PriceTickFontSizeProperty, value); }
        }
        public static readonly DependencyProperty PriceTickFontSizeProperty;
        private static void OnPriceTickFontSizeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            //VolumeTicksElement thisElement = (VolumeTicksElement)obj;
            //thisElement.InvalidateMeasure();
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public double PriceAxisWidth
        {
            get { return (double)GetValue(PricePanelWidthProperty); }
            set { SetValue(PricePanelWidthProperty, value); }
        }
        public static readonly DependencyProperty PricePanelWidthProperty;
        //---------------------------------------------------------------------------------------------------------------------------------------
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (CandlesMaxVolume == long.MinValue) return;

            Pen pen = new Pen(Brushes.Black, 1);
            double textHeight = (new FormattedText("123", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), PriceTickFontSize, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip)).Height;
            double halfTextHeight = textHeight / 2.0;
            double volumeHistogramPanelWidth = ActualWidth - PriceAxisWidth;
            double tick_text_X = volumeHistogramPanelWidth + TICK_LINE_WIDTH + TICK_LEFT_MARGIN;
            double tick_line_endX = volumeHistogramPanelWidth + TICK_LINE_WIDTH;

            double chartHeight = ActualHeight - ChartBottomMargin - ChartTopMargin;
            if (chartHeight <= 0) return;
            long stepInVolumeLots = (long)(CandlesMaxVolume * ((textHeight + GapBetweenTickLabels) / chartHeight)) + 1;
            long stepInVolumeLots_maxDigit = MyWpfMath.MaxDigit(stepInVolumeLots);
            stepInVolumeLots = (stepInVolumeLots % stepInVolumeLots_maxDigit) == 0 ? stepInVolumeLots : (stepInVolumeLots / stepInVolumeLots_maxDigit + 1) * stepInVolumeLots_maxDigit;
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            double chartHeight_candlesLHRange_Ratio = chartHeight / CandlesMaxVolume;

            void DrawPriceTick(long volume)
            {
                FormattedText priceTickFormattedText = new FormattedText(volume.ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), PriceTickFontSize, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                double y = ChartTopMargin + (CandlesMaxVolume - volume) * chartHeight_candlesLHRange_Ratio;
                drawingContext.DrawText(priceTickFormattedText, new Point(tick_text_X, y - halfTextHeight));
                drawingContext.DrawLine(pen, new Point(volumeHistogramPanelWidth, y), new Point(tick_line_endX, y));

                if (IsGridlinesEnabled && GridlinesPen != null)
                    drawingContext.DrawLine(GridlinesPen, new Point(0, y), new Point(volumeHistogramPanelWidth, y));
            }
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            long theMostRoundVolume = MyWpfMath.MaxDigit(CandlesMaxVolume);
            DrawPriceTick(theMostRoundVolume);

            long maxVolumeThreshold = (long)(CandlesMaxVolume + (ChartTopMargin - halfTextHeight) / chartHeight_candlesLHRange_Ratio);
            long minVolumeThreshold = (long)(CandlesMaxVolume + (ChartTopMargin - ActualHeight + halfTextHeight) / chartHeight_candlesLHRange_Ratio);

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
