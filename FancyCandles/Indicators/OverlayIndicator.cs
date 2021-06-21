using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices; // [CallerMemberName]
using FancyPrimitives;
using Newtonsoft.Json;

namespace FancyCandles.Indicators
{
    ///<summary>Provides a base class for Overlay technical indicators, such as Moving Average, Bollinger Bands, etc.</summary>
    ///<remarks>Overlays - Technical indicators that use the same scale as prices and plotted on a price chart area.</remarks>
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class OverlayIndicator: DependencyObject, INotifyPropertyChanged
    {
        ///<summary>Gets the short name of this OverlayIndicator object.</summary>
        ///<value>The short name of this OverlayIndicator object.</value>
        ///<remarks>
        ///The short name of an OverlayIndicator object usually contains no instance parameter values.
        ///</remarks>
        public abstract string ShortName { get; }

        /// <summary>Gets the full name of this OverlayIndicator object.</summary>
        ///<value>The full name of this OverlayIndicator object.</value>
        ///<remarks>
        ///The full name of an OverlayIndicator object usually contains some of its property values.
        ///</remarks>
        public abstract string FullName { get; }

        /// <summary>Gets the XAML snippet representing a property editor for this OverlayIndicator object.</summary>
        ///<value>The String of XAML snippet representing a property editor for this OverlayIndicator object.</value>
        ///<remarks>
        ///This XAML snippet can be loaded dynamically to instantiate a new element - the OverlayIndicator property editor. 
        ///</remarks>
        public abstract string PropertiesEditorXAML { get; }

        ///<summary>Returns this OverlayIndicator value at a specified time period.</summary>
        ///<value>The OverlayIndicator value at a specified time period.</value>
        ///<param name="candle_i">Specifies the time period at which the OverlayIndicator value is calculated. Usually indicator value is calculated at the candle closing time.</param>
        public abstract double GetIndicatorValue(int candle_i);

        ///<summary>Draws this OverlayIndicator chart for a specified time span.</summary>
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
        public abstract void OnRender(DrawingContext drawingContext, IntRange visibleCandlesRange, CandleExtremums visibleCandlesExtremums,
                                      double candleWidth, double gapBetweenCandles, double RenderHeight);

#pragma warning  disable CS1591
        protected abstract void ReCalcAllIndicatorValues();
        protected abstract void OnLastCandleChanged();
        protected abstract void OnNewCandleAdded();
#pragma warning  restore CS1591
        //---------------------------------------------------------------------------------------------------------------------------------------
        ///<summary>Gets the type name of this OverlayIndicator object.</summary>
        ///<value>The type name of this OverlayIndicator object.</value>
        ///<remarks>
        ///This property is used by serialization system.
        ///</remarks>
        [JsonProperty]
        public string TypeName { get { return GetType().FullName; } }
        //---------------------------------------------------------------------------------------------------------------------------------------
        private IList<ICandle> candlesSource;
        /// <summary>Gets or sets the underlying candle data collection.</summary>
        ///<value>The underlying candle data collection.</value>
        ///<remarks>
        ///Each overlay indicator calculation is based on an underlying price data series. Usually you need to set the <see cref="OverlayIndicator.CandlesSource"/> property to the same collection that you use for the <see cref="CandleChart.CandlesSource"/> property of the underlying <see cref="CandleChart"/> object.
        ///</remarks>
        public IList<ICandle> CandlesSource 
        {
            get { return candlesSource; }
            set
            {
                if (candlesSource == value) return;

                if (candlesSource!=null && candlesSource is INotifyCollectionChanged)
                    ((INotifyCollectionChanged)candlesSource).CollectionChanged -= OnCandlesSourceChanged;

                candlesSource = value;

                if (candlesSource != null && candlesSource is INotifyCollectionChanged)
                    ((INotifyCollectionChanged)candlesSource).CollectionChanged += OnCandlesSourceChanged;

                ReCalcAllIndicatorValues();
            }
        }

        private void OnCandlesSourceChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add) // New element has been added: (e.NewItems[0] as ICandle)
                OnNewCandleAdded();
            else if (e.Action == NotifyCollectionChangedAction.Replace) // Element (e.OldItems[0] as ICandle) has been replaced with (e.NewItems[0] as ICandle)
                OnLastCandleChanged();
            else if (e.Action == NotifyCollectionChangedAction.Remove) // Element has been removed: (e.OldItems[0] as ICandle)
            { }
        }
        //---------------- INotifyPropertyChanged ----------------------------------------------------------
#pragma warning  disable CS1591
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
#pragma warning  restore CS1591
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
        /*
        ///<summary>Removes portions of line segment outside the drawing area area.</summary>
        ///<value>True if some portion of line segment is inside the drawing area. False - you don't need to draw anything.</value>
        ///<param name="x0">Coordinate X of the first ending point of the line segment to clip.</param>
        ///<param name="y0">Coordinate Y of the first ending point of the line segment to clip.</param>
        ///<param name="x1">Coordinate X of the second ending point of the line segment to clip.</param>
        ///<param name="y1">Coordinate Y of the second ending point of the line segment to clip.</param>
        ///<param name="renderHeight">The Height of the drawing area.</param>
        ///<param name="newPoint0">First new ending point of line segment to draw.</param>
        ///<param name="newPoint1">Second new ending point of line segment to draw.</param>
        ///<remarks>
        ///Drawing area: 0 &lt;= y &lt;= RenderHeight. 
        ///Line segment: (x0,y0) - (x1,y1).
        ///</remarks>
        protected static bool ClipLineSegment(double x0, double y0, double x1, double y1, double renderHeight, out Point newPoint0, out Point newPoint1)
        {
            if (y0 >= 0.0 && y1 >= 0.0 && y0 <= renderHeight && y1 <= renderHeight)
            {
                newPoint0 = new Point(x0, y0);
                newPoint1 = new Point(x1, y1);
                return true;
            }

            if ((y0 < 0.0 && y1 < 0.0) || (y0 > renderHeight && y1 > renderHeight))
            {
                newPoint0 = newPoint1 = new Point();
                return false;
            }

            if (y0 < 0)
                newPoint0 = new Point(x0 - y0 * (x1 - x0) / (y1 - y0), 0.0);
            else if (y0 > renderHeight)
                newPoint0 = new Point(x0 - (y0 - renderHeight) * (x1 - x0) / (y1 - y0), renderHeight);
            else 
                newPoint0 = new Point(x0, y0);

            if (y1 < 0)
                newPoint1 = new Point(x1 - y1 * (x0 - x1) / (y0 - y1), 0.0);
            else if (y1 > renderHeight)
                newPoint1 = new Point(x1 - (y1 - renderHeight) * (x0 - x1) / (y0 - y1), renderHeight);
            else
                newPoint1 = new Point(x1, y1);

            return true;
        }*/
        //---------------------------------------------------------------------------------------------------------------------------------------
    }
}
