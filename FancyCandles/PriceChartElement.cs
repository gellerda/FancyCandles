/* 
    Copyright 2020 Dennis Geller.

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
using System.Windows.Controls;
using System;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections.Generic;
using FancyCandles.Indicators;
using System.Globalization;

namespace FancyCandles
{
    class PriceChartElement : FrameworkElement
    {
        //---------------------------------------------------------------------------------------------------------------------------------------
        public PriceChartElement() 
        {
            ToolTip tt = new ToolTip() { FontSize = CandleChart.ToolTipFontSize, BorderBrush = Brushes.Beige };
            tt.Content = "";
            ToolTip = tt;

            // Зададим время задержки появления подсказок здесь, а расположение подсказок (если его нужно поменять) зададим в XAML:
            ToolTipService.SetShowDuration(this, int.MaxValue);
            ToolTipService.SetInitialShowDelay(this, 0);

            if (bullishCandleStrokePen == null)
            {
                bullishCandleStrokePen = new Pen(CandleChart.DefaultBullishCandleFill, 1);
                if (!bullishCandleStrokePen.IsFrozen)
                    bullishCandleStrokePen.Freeze();
            }

            if (bearishCandleStrokePen == null)
            {
                bearishCandleStrokePen = new Pen(CandleChart.DefaultBearishCandleFill, 1);
                if (!bearishCandleStrokePen.IsFrozen)
                    bearishCandleStrokePen.Freeze();
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public int MaxNumberOfFractionalDigitsInPrice
        {
            get { return (int)GetValue(MaxNumberOfFractionalDigitsInPriceProperty); }
            set { SetValue(MaxNumberOfFractionalDigitsInPriceProperty, value); }
        }
        public static readonly DependencyProperty MaxNumberOfFractionalDigitsInPriceProperty =
            DependencyProperty.Register("MaxNumberOfFractionalDigitsInPrice", typeof(int), typeof(PriceChartElement), new FrameworkPropertyMetadata(0));
        //---------------------------------------------------------------------------------------------------------------------------------------
        public CultureInfo Culture
        {
            get { return (CultureInfo)GetValue(CultureProperty); }
            set { SetValue(CultureProperty, value); }
        }
        public static readonly DependencyProperty CultureProperty =
            DependencyProperty.Register("Culture", typeof(CultureInfo), typeof(PriceChartElement), new FrameworkPropertyMetadata(CultureInfo.CurrentCulture) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        //private Brush transparentFrozenBrush = (Brush)(new SolidColorBrush(Colors.Transparent)).GetCurrentValueAsFrozen();
        //---------------------------------------------------------------------------------------------------------------------------------------
        public static readonly DependencyProperty IndicatorsProperty
            = DependencyProperty.Register("Indicators", typeof(ObservableCollection<OverlayIndicator>), typeof(PriceChartElement), 
                new FrameworkPropertyMetadata(null, OnIndicatorsChanged) { AffectsRender = true });
        public ObservableCollection<OverlayIndicator> Indicators
        {
            get { return (ObservableCollection<OverlayIndicator>)GetValue(IndicatorsProperty); }
            set { SetValue(IndicatorsProperty, value); }
        }

        static void OnIndicatorsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            PriceChartElement thisPriceChartElement = obj as PriceChartElement;
            if (thisPriceChartElement == null) return;

            ObservableCollection<OverlayIndicator> old_obsCollection = e.OldValue as ObservableCollection<OverlayIndicator>;
            if (old_obsCollection != null)
            {
                old_obsCollection.CollectionChanged -= thisPriceChartElement.OnIndicatorsCollectionChanged;

                foreach (OverlayIndicator indicator in old_obsCollection)
                    indicator.PropertyChanged -= thisPriceChartElement.OnIndicatorsCollectionItemChanged;
            }

            ObservableCollection<OverlayIndicator> new_obsCollection = e.NewValue as ObservableCollection<OverlayIndicator>;
            if (new_obsCollection != null)
            {
                new_obsCollection.CollectionChanged += thisPriceChartElement.OnIndicatorsCollectionChanged;

                foreach (OverlayIndicator indicator in new_obsCollection)
                    indicator.PropertyChanged += thisPriceChartElement.OnIndicatorsCollectionItemChanged;
            }
        }

        private void OnIndicatorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (OverlayIndicator indicator in e.NewItems)
                    indicator.PropertyChanged += OnIndicatorsCollectionItemChanged;
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (OverlayIndicator indicator in e.NewItems)
                    indicator.PropertyChanged += OnIndicatorsCollectionItemChanged;

                foreach (OverlayIndicator indicator in e.OldItems)
                    indicator.PropertyChanged -= OnIndicatorsCollectionItemChanged;
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (OverlayIndicator indicator in e.OldItems)
                    indicator.PropertyChanged -= OnIndicatorsCollectionItemChanged;
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (OverlayIndicator indicator in (sender as IEnumerable<OverlayIndicator>))
                    indicator.PropertyChanged += OnIndicatorsCollectionItemChanged;
            }
            else if (e.Action == NotifyCollectionChangedAction.Move) {}

            InvalidateVisual();
        }

        private void OnIndicatorsCollectionItemChanged(object source, PropertyChangedEventArgs args)
        {
            InvalidateVisual();
        }        
        //---------------------------------------------------------------------------------------------------------------------------------------
        public static readonly DependencyProperty CandlesSourceProperty
            = DependencyProperty.Register("CandlesSource", typeof(ICandlesSource), typeof(PriceChartElement), new UIPropertyMetadata(null));
        public ICandlesSource CandlesSource
        {
            get { return (ICandlesSource)GetValue(CandlesSourceProperty); }
            set { SetValue(CandlesSourceProperty, value); }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public static readonly DependencyProperty BullishCandleFillProperty
            = DependencyProperty.Register("BullishCandleFill", typeof(Brush), typeof(PriceChartElement), 
                new FrameworkPropertyMetadata(CandleChart.DefaultBullishCandleFill, null, CoerceBullishCandleFill) { AffectsRender = true });
        public Brush BullishCandleFill
        {
            get { return (Brush)GetValue(BullishCandleFillProperty); }
            set { SetValue(BullishCandleFillProperty, value); }
        }

        private static object CoerceBullishCandleFill(DependencyObject objWithOldDP, object newDPValue)
        {
            Brush newBrushValue = (Brush)newDPValue;
            return newBrushValue.IsFrozen ? newDPValue : newBrushValue.GetCurrentValueAsFrozen();
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public static readonly DependencyProperty BullishCandleStrokeProperty
            = DependencyProperty.Register("BullishCandleStroke", typeof(Brush), typeof(PriceChartElement),
                new FrameworkPropertyMetadata(CandleChart.DefaultBullishCandleFill, null, CoerceBullishCandleStroke) { AffectsRender = true });
        public Brush BullishCandleStroke
        {
            get { return (Brush)GetValue(BullishCandleStrokeProperty); }
            set { SetValue(BullishCandleStrokeProperty, value); }
        }

        private Pen bullishCandleStrokePen;

        private static object CoerceBullishCandleStroke(DependencyObject objWithOldDP, object newDPValue)
        {
            PriceChartElement thisElement = (PriceChartElement)objWithOldDP;
            Brush newBrushValue = (Brush)newDPValue;

            if (newBrushValue.IsFrozen)
            {
                Pen p = new Pen(newBrushValue, 1.0);
                p.Freeze();
                thisElement.bullishCandleStrokePen = p;
                return newDPValue;
            }
            else
            {
                Brush b = (Brush)newBrushValue.GetCurrentValueAsFrozen();
                Pen p = new Pen(b, 1.0);
                p.Freeze();
                thisElement.bullishCandleStrokePen = p;
                return b;
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public static readonly DependencyProperty BearishCandleFillProperty
            = DependencyProperty.Register("BearishCandleFill", typeof(Brush), typeof(PriceChartElement), 
                new FrameworkPropertyMetadata(CandleChart.DefaultBearishCandleFill, null, CoerceBearishCandleFill) { AffectsRender = true });
        public Brush BearishCandleFill
        {
            get { return (Brush)GetValue(BearishCandleFillProperty); }
            set { SetValue(BearishCandleFillProperty, value); }
        }

        private static object CoerceBearishCandleFill(DependencyObject objWithOldDP, object newDPValue)
        {
            Brush newBrushValue = (Brush)newDPValue;
            return newBrushValue.IsFrozen ? newDPValue : newBrushValue.GetCurrentValueAsFrozen();
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public static readonly DependencyProperty BearishCandleStrokeProperty
            = DependencyProperty.Register("BearishCandleStroke", typeof(Brush), typeof(PriceChartElement),
                new FrameworkPropertyMetadata(CandleChart.DefaultBearishCandleFill, null, CoerceBearishCandleStroke) { AffectsRender = true });
        public Brush BearishCandleStroke
        {
            get { return (Brush)GetValue(BearishCandleStrokeProperty); }
            set { SetValue(BearishCandleStrokeProperty, value); }
        }

        private Pen bearishCandleStrokePen;

        private static object CoerceBearishCandleStroke(DependencyObject objWithOldDP, object newDPValue)
        {
            PriceChartElement thisElement = (PriceChartElement)objWithOldDP;
            Brush newBrushValue = (Brush)newDPValue;

            if (newBrushValue.IsFrozen)
            {
                Pen p = new Pen(newBrushValue, 1.0);
                p.Freeze();
                thisElement.bearishCandleStrokePen = p;
                return newDPValue;
            }
            else
            {
                Brush b = (Brush)newBrushValue.GetCurrentValueAsFrozen();
                Pen p = new Pen(b, 1.0);
                p.Freeze();
                thisElement.bearishCandleStrokePen = p;
                return b;
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public static readonly DependencyProperty VisibleCandlesExtremumsProperty
            = DependencyProperty.Register("VisibleCandlesExtremums", typeof(CandleExtremums), typeof(PriceChartElement), new FrameworkPropertyMetadata(new CandleExtremums(0.0, 0.0, 0L, 0L)) { AffectsRender = true });
        public CandleExtremums VisibleCandlesExtremums
        {
            get { return (CandleExtremums)GetValue(VisibleCandlesExtremumsProperty); }
            set { SetValue(VisibleCandlesExtremumsProperty, value); }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public static readonly DependencyProperty VisibleCandlesRangeProperty
            = DependencyProperty.Register("VisibleCandlesRange", typeof(IntRange), typeof(PriceChartElement), new FrameworkPropertyMetadata(IntRange.Undefined));
        public IntRange VisibleCandlesRange
        {
            get { return (IntRange)GetValue(VisibleCandlesRangeProperty); }
            set { SetValue(VisibleCandlesRangeProperty, value); }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public static readonly DependencyProperty CandleWidthAndGapProperty
            = DependencyProperty.Register("CandleWidthAndGap", typeof(CandleDrawingParameters), typeof(PriceChartElement), new FrameworkPropertyMetadata(new CandleDrawingParameters()));
        public CandleDrawingParameters CandleWidthAndGap
        {
            get { return (CandleDrawingParameters)GetValue(CandleWidthAndGapProperty); }
            set { SetValue(CandleWidthAndGapProperty, value); }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        protected override void OnRender(DrawingContext drawingContext)
        {
            //drawingContext.DrawRectangle(transparentFrozenBrush, null, new Rect(0, 0, RenderSize.Width, RenderSize.Height));
            double range = VisibleCandlesExtremums.PriceHigh - VisibleCandlesExtremums.PriceLow;
            double correctedCndlWidth = CandleWidthAndGap.Width - 1.0;
            double candleWidthPlusGap = CandleWidthAndGap.Width + CandleWidthAndGap.Gap;

            for (int i = 0; i < VisibleCandlesRange.Count; i++)
            {
                ICandle cndl = CandlesSource[VisibleCandlesRange.Start_i + i];
                Brush cndlBrush = (cndl.C > cndl.O) ? BullishCandleFill : BearishCandleFill;
                Pen cndlPen = (cndl.C > cndl.O) ? bullishCandleStrokePen : bearishCandleStrokePen;

                double wnd_L = (1.0 - (cndl.L - VisibleCandlesExtremums.PriceLow) / range) * RenderSize.Height;
                double wnd_H = (1.0 - (cndl.H - VisibleCandlesExtremums.PriceLow) / range) * RenderSize.Height;
                double wnd_O = (1.0 - (cndl.O - VisibleCandlesExtremums.PriceLow) / range) * RenderSize.Height;
                double wnd_C = (1.0 - (cndl.C - VisibleCandlesExtremums.PriceLow) / range) * RenderSize.Height;

                double cndlLeftX = i * candleWidthPlusGap;
                double cndlCenterX = cndlLeftX + 0.5 * CandleWidthAndGap.Width;
                drawingContext.DrawLine(cndlPen, new Point(cndlCenterX, wnd_L), new Point(cndlCenterX, wnd_H));
                double cndlBodyH = Math.Abs(wnd_O - wnd_C);
                if (cndlBodyH > 1.0)
                {
                    drawingContext.DrawRectangle(cndlBrush, cndlPen, new Rect(cndlLeftX + 0.5, Math.Min(wnd_O, wnd_C) + 0.5, correctedCndlWidth, cndlBodyH - 1.0));
                }
                else
                    drawingContext.DrawLine(cndlPen, new Point(cndlLeftX, wnd_O), new Point(cndlLeftX + CandleWidthAndGap.Width, wnd_O));
            }

            for (int i = 0; i < Indicators.Count; i++)
                Indicators[i].OnRender(drawingContext, VisibleCandlesRange, VisibleCandlesExtremums, CandleWidthAndGap.Width, CandleWidthAndGap.Gap, RenderSize.Height);
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        protected override void OnMouseMove(MouseEventArgs e)
        {
            string decimalSeparator = Culture.NumberFormat.NumberDecimalSeparator;
            char[] decimalSeparatorArray = decimalSeparator.ToCharArray();
            string priceNumberFormat = $"N{MaxNumberOfFractionalDigitsInPrice}";

            Point mousePos = e.GetPosition(this);
            //Vector uv = new Vector(mousePos.X/ RenderSize.Width, mousePos.Y / RenderSize.Height);
            int cndl_i = VisibleCandlesRange.Start_i + (int)(mousePos.X / (CandleWidthAndGap.Width + CandleWidthAndGap.Gap));
            ICandle cndl = CandlesSource[cndl_i];
            string strO = MyNumberFormatting.PriceToString(cndl.O, priceNumberFormat, Culture, decimalSeparator, decimalSeparatorArray);
            string strH = MyNumberFormatting.PriceToString(cndl.H, priceNumberFormat, Culture, decimalSeparator, decimalSeparatorArray);
            string strL = MyNumberFormatting.PriceToString(cndl.L, priceNumberFormat, Culture, decimalSeparator, decimalSeparatorArray);
            string strC = MyNumberFormatting.PriceToString(cndl.C, priceNumberFormat, Culture, decimalSeparator, decimalSeparatorArray);
            string strV = MyNumberFormatting.VolumeToString(cndl.V, Culture, decimalSeparator, decimalSeparatorArray);
            string strT = cndl.t.ToString((CandlesSource.TimeFrame < 0) ? "G" : "g", Culture);
            string tooltipText = $"{strT}\nO= {strO}\nH= {strH}\nL= {strL}\nC= {strC}\nV= {strV}";
            ((ToolTip)ToolTip).Content = tooltipText;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
    }
}
