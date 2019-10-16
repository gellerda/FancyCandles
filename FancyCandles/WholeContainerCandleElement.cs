using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace FancyCandles
{
    class WholeContainerCandleElement : FrameworkElement
    {
        public WholeContainerCandleElement()
        {
            ToolTip tt = new ToolTip() { FontSize = CandleChart.candleToolTipFontSize, BorderBrush = Brushes.Beige };
            tt.Content = "";
            ToolTip = tt;

            // Зададим время задержки появления подсказок здесь, а расположение подсказок (если его нужно поменять) зададим в XAML:
            ToolTipService.SetShowDuration(this, int.MaxValue);
            ToolTipService.SetInitialShowDelay(this, 0);
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        static WholeContainerCandleElement()
        {
            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata(new WholeContainerCandle(), new PropertyChangedCallback(OnCandleDataChanged)) { AffectsRender = true };
            CandleDataProperty = DependencyProperty.Register("CandleData", typeof(WholeContainerCandle), typeof(WholeContainerCandleElement), metadata);

            metadata = new FrameworkPropertyMetadata(CandleChart.DefaultBearishCandleBrush) { AffectsRender=true };
            BearishCandleBrushProperty = DependencyProperty.Register("BearishCandleBrush", typeof(Brush), typeof(WholeContainerCandleElement), metadata);

            metadata = new FrameworkPropertyMetadata(CandleChart.DefaultBullishCandleBrush) { AffectsRender = true };
            BullishCandleBrushProperty = DependencyProperty.Register("BullishCandleBrush", typeof(Brush), typeof(WholeContainerCandleElement), metadata);
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        private static void OnCandleDataChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            WholeContainerCandleElement thisElement = (WholeContainerCandleElement)obj;
            ((ToolTip)thisElement.ToolTip).Content = thisElement.CandleData.ToolTipText;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the color of the bullish candle (when the Close is higher than the Open).</summary>
        public Brush BullishCandleBrush
        {
            get { return (Brush)GetValue(BullishCandleBrushProperty); }
            set { SetValue(BullishCandleBrushProperty, value); }
        }
        public static readonly DependencyProperty BullishCandleBrushProperty;
        //---------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the color of the bearish candle (when the Close is lower than the Open).</summary>
        public Brush BearishCandleBrush
        {
            get { return (Brush)GetValue(BearishCandleBrushProperty); }
            set { SetValue(BearishCandleBrushProperty, value); }
        }
        public static readonly DependencyProperty BearishCandleBrushProperty;
        //---------------------------------------------------------------------------------------------------------------------------------------
        public static readonly DependencyProperty CandleDataProperty;
        public WholeContainerCandle CandleData
        {
            get { return (WholeContainerCandle)GetValue(CandleDataProperty); }
            set { SetValue(CandleDataProperty, value); }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        protected override void OnRender(DrawingContext drawingContext)
        {
            Brush transparentBrush = Brushes.Transparent;
            Pen transparentPen = new Pen(transparentBrush, 0);
            Brush brush = CandleData.C > CandleData.O ? BullishCandleBrush : BearishCandleBrush;
            Pen pen = new Pen(brush, 1);

            // Нарисуем тело свечи:
            if (CandleData.BodyWidth >= 2.5)
            {
                double bodyHeighth = CandleData.BodyHeight * RenderSize.Height;
                if (bodyHeighth == 0.0) bodyHeighth = 1.0;
                double bodyBottomMargin = CandleData.BodyBottomMargin * RenderSize.Height;
                drawingContext.DrawRectangle(brush, transparentPen, new Rect(CandleData.LeftMargin, RenderSize.Height - bodyHeighth - bodyBottomMargin, CandleData.BodyWidth, bodyHeighth));
            }

            // Нарисуем тени свечи:
            double shadowsHeighth = CandleData.ShadowsHeight * RenderSize.Height;
            double shadowsBottomMargin = CandleData.ShadowsBottomMargin * RenderSize.Height;
            double y1 = RenderSize.Height - shadowsBottomMargin;
            double y2 = y1 - shadowsHeighth;
            double x = CandleData.LeftMargin + CandleData.BodyWidth / 2.0;
            drawingContext.DrawLine(pen, new Point(x, y1), new Point(x, y2));

            // Нарисуем прозрачный прямоугольник, накрывающий всю свечку, для ToolTip:
            drawingContext.DrawRectangle(transparentBrush, transparentPen, new Rect(CandleData.LeftMargin, y2, CandleData.BodyWidth, shadowsHeighth));
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
    }
}
