using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FancyCandles
{
    class PriceTicksElement : FrameworkElement
    {
        public static double TICK_LINE_WIDTH = 3.0;
        public static double TICK_LEFT_MARGIN = 2.0;
        public static double TICK_RIGHT_MARGIN = 1.0;
        //---------------------------------------------------------------------------------------------------------------------------------------
        static PriceTicksElement()
        {
            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnPriceTickFontSizeChanged)) { AffectsRender = true };
            PriceTickFontSizeProperty = CandleChart.PriceTickFontSizeProperty.AddOwner(typeof(PriceTicksElement), metadata);

            metadata = new FrameworkPropertyMetadata(0.0) { AffectsRender = true };
            PricePanelWidthProperty = DependencyProperty.Register("PriceAxisWidth", typeof(double), typeof(PriceTicksElement), metadata);

            metadata = new FrameworkPropertyMetadata(new Vector(1, 1)) { AffectsRender = true };
            CandlesLHProperty = DependencyProperty.Register("CandlesLH", typeof(Vector), typeof(PriceTicksElement), metadata);

            metadata = new FrameworkPropertyMetadata(15.0) { AffectsRender = true };
            ChartBottomMarginProperty = DependencyProperty.Register("ChartBottomMargin", typeof(double), typeof(PriceTicksElement), metadata);
            //MyCandleChart3.ChartBottomMarginProperty.AddOwner(typeof(PriceTicksElement), metadata);

            metadata = new FrameworkPropertyMetadata(15.0) { AffectsRender = true };
            ChartTopMarginProperty = DependencyProperty.Register("ChartTopMargin", typeof(double), typeof(PriceTicksElement), metadata);
            //MyCandleChart3.ChartTopMarginProperty.AddOwner(typeof(PriceTicksElement), metadata);

            metadata = new FrameworkPropertyMetadata(0.0) { AffectsRender = true };
            GapBetweenTickLabelsProperty = DependencyProperty.Register("GapBetweenTickLabels", typeof(double), typeof(PriceTicksElement), metadata);

            metadata = new FrameworkPropertyMetadata(true) { AffectsRender = true };
            IsGridlinesEnabledProperty = DependencyProperty.Register("IsGridlinesEnabled", typeof(bool), typeof(PriceTicksElement), metadata);

            Pen defaultPen = new Pen(new SolidColorBrush(Color.FromArgb(50, 105, 42, 0)), 1); // { DashStyle = new DashStyle(new double[] { 2, 3 }, 0) };
            metadata = new FrameworkPropertyMetadata(defaultPen) { AffectsRender = true };
            GridlinesPenProperty = DependencyProperty.Register("GridlinesPen", typeof(Pen), typeof(PriceTicksElement), metadata);
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
        public Vector CandlesLH
        {
            get { return (Vector)GetValue(CandlesLHProperty); }
            set { SetValue(CandlesLHProperty, value); }
        }
        public static readonly DependencyProperty CandlesLHProperty;
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
            //PriceTicksElement thisElement = (PriceTicksElement)obj;
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
            Pen pen = new Pen(Brushes.Black, 1);
            double textHeight = (new FormattedText("123", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), PriceTickFontSize, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip)).Height;
            double halfTextHeight = textHeight / 2.0;
            double candlePanelWidth = ActualWidth - PriceAxisWidth;
            double tick_text_X = candlePanelWidth + TICK_LINE_WIDTH + TICK_LEFT_MARGIN;
            double tick_line_endX = candlePanelWidth + TICK_LINE_WIDTH;

            double chartHeight = ActualHeight - ChartBottomMargin - ChartTopMargin;
            double stepInRubles = (CandlesLH.Y - CandlesLH.X) / chartHeight * (textHeight + GapBetweenTickLabels);
            double stepInRubles_maxDigit = MyWpfMath.MaxDigit(stepInRubles);
            stepInRubles = Math.Ceiling(stepInRubles / stepInRubles_maxDigit) * stepInRubles_maxDigit;
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            double chartHeight_candlesLHRange_Ratio = chartHeight / (CandlesLH.Y - CandlesLH.X);

            void DrawPriceTick(double price)
            {
                FormattedText priceTickFormattedText = new FormattedText(price.ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), PriceTickFontSize, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                double y = ChartTopMargin + (CandlesLH.Y - price) * chartHeight_candlesLHRange_Ratio;
                drawingContext.DrawText(priceTickFormattedText, new Point(tick_text_X, y - halfTextHeight));
                drawingContext.DrawLine(pen, new Point(candlePanelWidth, y), new Point(tick_line_endX, y));

                if (IsGridlinesEnabled && GridlinesPen != null)
                    drawingContext.DrawLine(GridlinesPen, new Point(0, y), new Point(candlePanelWidth, y));
            }
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            double theMostRoundPrice = MyWpfMath.TheMostRoundValueInsideRange(CandlesLH.X, CandlesLH.Y);
            DrawPriceTick(theMostRoundPrice);

            double maxPriceThreshold = CandlesLH.Y + (ChartTopMargin - halfTextHeight) / chartHeight_candlesLHRange_Ratio;
            double minPriceThreshold = CandlesLH.Y + (ChartTopMargin - ActualHeight + halfTextHeight) / chartHeight_candlesLHRange_Ratio;

            int step_i = 1;
            double next_tick = theMostRoundPrice + step_i * stepInRubles;
            while (next_tick < maxPriceThreshold)
            {
                DrawPriceTick(next_tick);
                step_i++;
                next_tick = theMostRoundPrice + step_i * stepInRubles;
            }

            step_i = 1;
            next_tick = theMostRoundPrice - step_i * stepInRubles;
            while (next_tick > minPriceThreshold)
            {
                DrawPriceTick(next_tick);
                step_i++;
                next_tick = theMostRoundPrice - step_i * stepInRubles;
            }

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
