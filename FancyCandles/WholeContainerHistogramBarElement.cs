using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace FancyCandles
{
    class WholeContainerHistogramBarElement : FrameworkElement
    {
        public WholeContainerHistogramBarElement()
        {
            ToolTip tt = new ToolTip() { FontSize = CandleChart.candleToolTipFontSize, BorderBrush = Brushes.Beige };
            tt.Content = "";
            ToolTip = tt;

            // Зададим время задержки появления подсказок здесь, а расположение подсказок (если его нужно поменять) зададим в XAML:
            ToolTipService.SetShowDuration(this, int.MaxValue);
            ToolTipService.SetInitialShowDelay(this, 0);
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        static WholeContainerHistogramBarElement()
        {
            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata(new WholeContainerCandle(), new PropertyChangedCallback(OnCandleDataChanged)) { AffectsRender = true };
            CandleDataProperty = DependencyProperty.Register("CandleData", typeof(WholeContainerCandle), typeof(WholeContainerHistogramBarElement), metadata);

            metadata = new FrameworkPropertyMetadata(1.0) { AffectsRender = true };
            VolumeBarWidthToCandleWidthRatioProperty = DependencyProperty.Register("VolumeBarWidthToCandleWidthRatio", typeof(double), typeof(WholeContainerHistogramBarElement), metadata);

            metadata = new FrameworkPropertyMetadata(CandleChart.DefaultBullishVolumeBarBrush) { AffectsRender = true };
            BullishVolumeBarBrushProperty =  DependencyProperty.Register("BullishVolumeBarBrush", typeof(Brush), typeof(WholeContainerHistogramBarElement), metadata);

            metadata = new FrameworkPropertyMetadata(CandleChart.DefaultBearishVolumeBarBrush) { AffectsRender = true };
            BearishVolumeBarBrushProperty = DependencyProperty.Register("BearishVolumeBarBrush", typeof(Brush), typeof(WholeContainerHistogramBarElement), metadata);
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the color of the bullish volume bar (when the close price is higher than the open price).</summary>
        public Brush BullishVolumeBarBrush
        {
            get { return (Brush)GetValue(BullishVolumeBarBrushProperty); }
            set { SetValue(BullishVolumeBarBrushProperty, value); }
        }
        public static readonly DependencyProperty BullishVolumeBarBrushProperty;
        //---------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the color of the bearish volume bar (when the close price is lower than the open price).</summary>
        public Brush BearishVolumeBarBrush
        {
            get { return (Brush)GetValue(BearishVolumeBarBrushProperty); }
            set { SetValue(BearishVolumeBarBrushProperty, value); }
        }
        public static readonly DependencyProperty BearishVolumeBarBrushProperty;
        //---------------------------------------------------------------------------------------------------------------------------------------
        public static readonly DependencyProperty CandleDataProperty;
        public WholeContainerCandle CandleData
        {
            get { return (WholeContainerCandle)GetValue(CandleDataProperty); }
            set { SetValue(CandleDataProperty, value); }
        }

        private static void OnCandleDataChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            WholeContainerHistogramBarElement thisElement = (WholeContainerHistogramBarElement)obj;
            ((ToolTip)thisElement.ToolTip).Content = thisElement.CandleData.VolumeToolTipText;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public static readonly DependencyProperty VolumeBarWidthToCandleWidthRatioProperty;
        public double VolumeBarWidthToCandleWidthRatio
        {
            get { return (double)GetValue(VolumeBarWidthToCandleWidthRatioProperty); }
            set { SetValue(VolumeBarWidthToCandleWidthRatioProperty, value); }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        protected override void OnRender(DrawingContext drawingContext)
        {
            Brush transparentBrush = Brushes.Transparent;
            Pen transparentPen = new Pen(transparentBrush, 0);
            Brush brush = CandleData.C > CandleData.O ? BullishVolumeBarBrush : BearishVolumeBarBrush;
            double penThickness = (VolumeBarWidthToCandleWidthRatio == 1.0) ? CandleData.BodyWidth : (VolumeBarWidthToCandleWidthRatio == 0.0 ? 1.0 : System.Math.Max(CandleData.BodyWidth*VolumeBarWidthToCandleWidthRatio, 1.0));
            Pen pen = new Pen(brush, penThickness) { StartLineCap=PenLineCap.Flat, EndLineCap=PenLineCap.Flat };

            // Нарисуем бар гистограммы:
            double volBarHeighth = CandleData.VolumeBarHeight * RenderSize.Height;
            double y = RenderSize.Height - volBarHeighth;
            double x = CandleData.LeftMargin + CandleData.BodyWidth / 2.0;
            drawingContext.DrawLine(pen, new Point(x, y), new Point(x, RenderSize.Height));

            // Нарисуем прозрачный прямоугольник, накрывающий всю свечку, для ToolTip:
            drawingContext.DrawRectangle(transparentBrush, transparentPen, new Rect(CandleData.LeftMargin, y, CandleData.BodyWidth, volBarHeighth));
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
    }
}
