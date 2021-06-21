using System.Collections.Generic;
using System.Windows.Media;
using System.Windows;
using Newtonsoft.Json;
using FancyCandles;
using FancyCandles.Indicators;

namespace FancyCandles.Indicators
{
    ///<summary>Represents the Exponential Moving Average overlay technical indicator.</summary>
    ///<remarks>An Exponential Moving Average is the overlay technical indicator calculated by the following formulae:
    ///<para>EMA(i, Smoothing) = C(i) * Smoothing + EMA(i-1, Smoothing) * (1 - Smoothing),  i>0<br/>
    ///EMA(0, Smoothing) = C(0)</para> 
    ///where:<br/>
    ///<list type="bullet">
    ///<item><term>EMA(i, Smoothing) is the Exponential Moving Average value at a time period i;</term></item>
    ///<item><term>Smoothing is the constant weighting factor between 0 and 1. A higher Smoothing discounts older observations faster.</term></item>
    ///<item><term>C(i) is the Close price value at a time period i.</term></item>
    ///</list>
    ///</remarks>
    ///<seealso cref="OverlayIndicator"/>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class ExponentialMovingAverage : OverlayIndicator
    {
        private List<double> indicatorValues;

        //---------------------------------------------------------------------------------------------------------------------------------------
        ///<summary>Gets the static name of this OverlayIndicator.</summary>
        ///<value>The static name of this OverlayIndicator.</value>
        public static string StaticName { get { return "Exponential Moving Average"; } }

        ///<summary>Gets the short name of this OverlayIndicator object. Implements the <see cref="OverlayIndicator.ShortName"/> abstract property inherited from the <see cref="OverlayIndicator"/> base class.</summary>
        ///<value>The short name of this OverlayIndicator object.</value>
        ///<remarks>
        ///The short name of an OverlayIndicator object usually contains no instance parameter values.
        ///</remarks>
        public override string ShortName { get { return "Exponential MA"; } }

        ///<summary>Implements the <see cref="OverlayIndicator.FullName"/> abstract property inherited from the <see cref="OverlayIndicator"/> base class.</summary>
        ///<value>The full name of this OverlayIndicator object.</value>
        ///<remarks>
        ///The full name of an OverlayIndicator object usually contains some of its property values.
        ///</remarks>
        public override string FullName { get { return $"Exponential Moving Average, a={Smoothing}"; } }
        //---------------------------------------------------------------------------------------------------------------------------------------
        private double smoothing = 0.25;

        ///<summary>Gets or sets the Smoothing weighting factor.</summary>
        ///<value>The Smoothing weighting factor. The default value is <c>0.25</c>.</value>
        ///<remarks>
        ///A higher Smoothing discounts older observations faster. A <see cref="Smoothing"/> value must be between <c>0.0</c> and <c>1.0</c>. 
        ///</remarks>
        [JsonProperty]
        public double Smoothing
        {
            get { return smoothing; }
            set
            {
                if (smoothing == value) return;
                smoothing = value;
                ReCalcAllIndicatorValues();
                OnPropertyChanged();
                OnPropertyChanged("FullName");
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        private Pen pen = new Pen(Brushes.Black, 1);

        ///<summary>Gets or sets the <see href="https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.pen?view=netframework-4.7.2">Pen</see> to draw the indicator chart.</summary>
        ///<value>The <see href="https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.pen?view=netframework-4.7.2">Pen</see> to draw the indicator chart. The default value is <c>Pen(SolidColorBrush=Black, Thickness=1)</c>.</value>
        [JsonProperty]
        public Pen Pen
        {
            get { return pen; }
            set
            {
                pen = (Pen)value.GetCurrentValueAsFrozen();
                OnPropertyChanged();
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        ///<summary>Implements the <see cref="OverlayIndicator.GetIndicatorValue"/> abstract method inherited from the <see cref="OverlayIndicator"/> base class.</summary>
        ///<value>The OverlayIndicator value at a specified time period.</value>
        ///<param name="candle_i">Specifies the time period at which the OverlayIndicator value is calculated.</param>
        public override double GetIndicatorValue(int candle_i)
        {
            if (indicatorValues == null) return 0;

            return indicatorValues[candle_i];
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        private double CalcIndicatorValue(int candle_i)
        {
            if (candle_i == 0)
                return CandlesSource[0].C;

            double ema = CandlesSource[candle_i].C * Smoothing + GetIndicatorValue(candle_i - 1) * (1 - Smoothing);
            return ema;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
#pragma warning  disable CS1591
        protected override void ReCalcAllIndicatorValues()
        {
            indicatorValues = new List<double>();
            if (CandlesSource == null || CandlesSource.Count==0) return;

            indicatorValues.Add(CandlesSource[0].C);
            for (int candle_i = 1; candle_i < CandlesSource.Count; candle_i++)
                indicatorValues.Add(CalcIndicatorValue(candle_i));
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        protected override void OnLastCandleChanged()
        {
            //if (CandlesSource.Count == 1) return;
            indicatorValues[indicatorValues.Count - 1] = CalcIndicatorValue(CandlesSource.Count - 1);
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        protected override void OnNewCandleAdded()
        {
            //if (CandlesSource.Count == 1) return;
            indicatorValues.Add(CalcIndicatorValue(CandlesSource.Count - 1));
        }
#pragma warning  restore CS1591
        //---------------------------------------------------------------------------------------------------------------------------------------
        ///<summary>Implements the <see cref="OverlayIndicator.OnRender"/> abstract method inherited from the <see cref="OverlayIndicator"/> base class.</summary>
        ///<param name="drawingContext">Provides methods for drawing lines and shapes on the price chart area.</param>
        ///<param name="visibleCandlesRange">Specifies the time span currently shown on the price chart area. The range of the candles currently visible on the price chart.</param>
        ///<param name="visibleCandlesExtremums">The maximal High and minimal Low of the candles in visibleCandlesRange.</param>
        ///<param name="candleWidth">The Width of candle of the price chart, in device-independent units.</param>
        ///<param name="gapBetweenCandles">The Gap between candles of the price chart, in device-independent units.</param>
        ///<param name="RenderHeight">The height of the price chart area, in device-independent units.</param>
        ///<remarks>
        ///This is an analog of the <see href="https://docs.microsoft.com/en-za/dotnet/api/system.windows.uielement.onrender?view=netframework-4.7.2">UIElement.OnRender()</see> method. 
        ///Participates in rendering operations that are directed by the layout system. The rendering instructions for this indicator are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        ///</remarks>
        public override void OnRender(DrawingContext drawingContext, IntRange visibleCandlesRange, CandleExtremums visibleCandlesExtremums,
                                      double candleWidth, double gapBetweenCandles, double RenderHeight)
        {
            if (visibleCandlesRange.Count < 2 || visibleCandlesRange.Start_i < 0) return;

            double candleWidthPlusGap = candleWidth + gapBetweenCandles;
            double range = visibleCandlesExtremums.PriceHigh - visibleCandlesExtremums.PriceLow;
            double prevCndlCenterX = 0;
            double prevLocalIndicatorValue = 0;

            prevCndlCenterX = 0.5 * candleWidth;
            double prevIndicatorValue = GetIndicatorValue(visibleCandlesRange.Start_i);
            prevLocalIndicatorValue = (1.0 - (prevIndicatorValue - visibleCandlesExtremums.PriceLow) / range) * RenderHeight;

            for (int cndl_i = 1; cndl_i < visibleCandlesRange.Count; cndl_i++)
            {
                double indicatorValue = GetIndicatorValue(visibleCandlesRange.Start_i + cndl_i);
                double localIndicatorValue = (1.0 - (indicatorValue - visibleCandlesExtremums.PriceLow) / range) * RenderHeight;
                double cndlCenterX = cndl_i * candleWidthPlusGap + 0.5 * candleWidth;

                //ClipLineSegment(prevCndlCenterX, prevLocalIndicatorValue, cndlCenterX, localIndicatorValue, RenderHeight, out Point newPoint0, out Point newPoint1);
                //drawingContext.DrawLine(Pen, newPoint0, newPoint1);
                drawingContext.DrawLine(Pen, new Point(prevCndlCenterX, prevLocalIndicatorValue), new Point(cndlCenterX, localIndicatorValue));

                prevCndlCenterX = cndlCenterX;
                prevLocalIndicatorValue = localIndicatorValue;
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        ///<summary>Gets the XAML snippet representing a property editor for this OverlayIndicator object. Implements the <see cref="OverlayIndicator.PropertiesEditorXAML"/> abstract property inherited from the <see cref="OverlayIndicator"/> base class.</summary>
        ///<value>The String of XAML snippet representing a property editor for this OverlayIndicator object.</value>
        ///<remarks>
        ///This XAML snippet can be loaded dynamically to instantiate a new element - the OverlayIndicator property editor. 
        ///</remarks>
        public override string PropertiesEditorXAML
        {
            get
            {
                string xaml = $@"
                    <StackPanel>
                        <StackPanel.Resources>
                            <fp:SymStringToNumberConverter x:Key=""symStringToNumberConverter""/>

                            <Style x:Key=""horizontalCaption_"" TargetType=""TextBlock"">
                                <Setter Property=""Margin"" Value=""0 0 5 2""/>
                                <Setter Property=""VerticalAlignment"" Value=""Bottom""/>
                            </Style>

                            <Style x:Key=""settingsItem_"" TargetType=""StackPanel"">
                                <Setter Property=""Orientation"" Value=""Horizontal""/>
                                <Setter Property=""FrameworkElement.HorizontalAlignment"" Value=""Left""/>
                                <Setter Property=""FrameworkElement.Margin"" Value=""0 8 0 0""/>
                            </Style>
                        </StackPanel.Resources>

                        <StackPanel Style=""{{StaticResource settingsItem_}}"">
                            <TextBlock Style=""{{StaticResource horizontalCaption_}}"" ToolTip=""Smoothing parameter (between 0 and 1 inclusive)."">a=</TextBlock>
                            <fp:DoubleTextBox MinValue=""0"" MaxValue=""1"" VerticalAlignment=""Bottom"" Width=""50""
                                              Text=""{{Binding Smoothing, Converter={{StaticResource symStringToNumberConverter}}, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}}""/>
                        </StackPanel>

                        <StackPanel Style=""{{StaticResource settingsItem_}}"">
                            <TextBlock Style=""{{StaticResource horizontalCaption_}}"">Line:</TextBlock>
                            <fp:PenSelector SelectedPen=""{{Binding Pen, Mode = TwoWay}}"" VerticalAlignment=""Bottom""/>
                        </StackPanel>

                    </StackPanel>";

                return xaml;
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
    }
}
