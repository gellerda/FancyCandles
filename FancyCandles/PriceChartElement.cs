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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.ComponentModel;
using System.Runtime.CompilerServices; // [CallerMemberName]
using System.Diagnostics;

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
        public static readonly DependencyProperty CandlesSourceProperty
            = DependencyProperty.Register("CandlesSource", typeof(ObservableCollection<ICandle>), typeof(PriceChartElement), new UIPropertyMetadata(null));
        public ObservableCollection<ICandle> CandlesSource
        {
            get { return (ObservableCollection<ICandle>)GetValue(CandlesSourceProperty); }
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
            double range = VisibleCandlesExtremums.PriceHigh - VisibleCandlesExtremums.PriceLow;
            double correctedCndlWidth = CandleWidthAndGap.Width - 1.0;

            for (int i = 0; i < VisibleCandlesRange.Count; i++)
            {
                ICandle cndl = CandlesSource[VisibleCandlesRange.Start_i + i];
                Brush cndlBrush = (cndl.C > cndl.O) ? BullishCandleFill : BearishCandleFill;
                Pen cndlPen = (cndl.C > cndl.O) ? bullishCandleStrokePen : bearishCandleStrokePen;

                double wnd_L = (1.0 - (cndl.L - VisibleCandlesExtremums.PriceLow) / range) * RenderSize.Height;
                double wnd_H = (1.0 - (cndl.H - VisibleCandlesExtremums.PriceLow) / range) * RenderSize.Height;
                double wnd_O = (1.0 - (cndl.O - VisibleCandlesExtremums.PriceLow) / range) * RenderSize.Height;
                double wnd_C = (1.0 - (cndl.C - VisibleCandlesExtremums.PriceLow) / range) * RenderSize.Height;

                double cndlLeftX = i * (CandleWidthAndGap.Width + CandleWidthAndGap.Gap);
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
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(this);
            //Vector uv = new Vector(mousePos.X/ RenderSize.Width, mousePos.Y / RenderSize.Height);
            int cndl_i = VisibleCandlesRange.Start_i + (int)(mousePos.X / (CandleWidthAndGap.Width + CandleWidthAndGap.Gap));
            ICandle cndl = CandlesSource[cndl_i];
            string tooltipText = $"{cndl.t.ToString("d.MM.yyyy H:mm")}\nO={cndl.O}\nH={cndl.H}\nL={cndl.L}\nC={cndl.C}\nV={cndl.V}";
            ((ToolTip)ToolTip).Content = tooltipText;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
    }
}
