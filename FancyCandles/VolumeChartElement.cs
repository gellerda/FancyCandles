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
    class VolumeChartElement : FrameworkElement
    {
        public VolumeChartElement()
        {
            ToolTip tt = new ToolTip() { FontSize = CandleChart.ToolTipFontSize, BorderBrush = Brushes.Beige };
            tt.Content = "";
            ToolTip = tt;

            // Зададим время задержки появления подсказок здесь, а расположение подсказок (если его нужно поменять) зададим в XAML:
            ToolTipService.SetShowDuration(this, int.MaxValue);
            ToolTipService.SetInitialShowDelay(this, 0);

            if (bullishBarPen == null)
            {
                bullishBarPen = new Pen(CandleChart.DefaultBullishVolumeBarFill, 1);
                if (!bullishBarPen.IsFrozen)
                    bullishBarPen.Freeze();
            }

            if (bearishBarPen == null)
            {
                bearishBarPen = new Pen(CandleChart.DefaultBearishVolumeBarFill, 1);
                if (!bearishBarPen.IsFrozen)
                    bearishBarPen.Freeze();
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public static readonly DependencyProperty CandlesSourceProperty
             = DependencyProperty.Register("CandlesSource", typeof(ObservableCollection<ICandle>), typeof(VolumeChartElement), new FrameworkPropertyMetadata(null));
        public ObservableCollection<ICandle> CandlesSource
        {
            get { return (ObservableCollection<ICandle>)GetValue(CandlesSourceProperty); }
            set { SetValue(CandlesSourceProperty, value); }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        private Pen bullishBarPen;

        public static readonly DependencyProperty BullishBarFillProperty
            = DependencyProperty.Register("BullishBarFill", typeof(Brush), typeof(VolumeChartElement), 
                new FrameworkPropertyMetadata(CandleChart.DefaultBullishVolumeBarFill, null, CoerceBullishCandleFill) { AffectsRender = true });
        public Brush BullishBarFill
        {
            get { return (Brush)GetValue(BullishBarFillProperty); }
            set { SetValue(BullishBarFillProperty, value); }
        }

        private static object CoerceBullishCandleFill(DependencyObject objWithOldDP, object newDPValue)
        {
            VolumeChartElement thisElement = (VolumeChartElement)objWithOldDP;
            Brush newBrushValue = (Brush)newDPValue;

            if (newBrushValue.IsFrozen)
            {
                Pen p = new Pen(newBrushValue, 1.0);
                p.Freeze();
                thisElement.bullishBarPen = p;
                return newDPValue;
            }
            else
            {
                Brush b = (Brush)newBrushValue.GetCurrentValueAsFrozen();
                Pen p = new Pen(b, 1.0);
                p.Freeze();
                thisElement.bullishBarPen = p;
                return b;
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        private Pen bearishBarPen;

        public static readonly DependencyProperty BearishBarFillProperty
            = DependencyProperty.Register("BearishBarFill", typeof(Brush), typeof(VolumeChartElement), 
                new FrameworkPropertyMetadata(CandleChart.DefaultBearishVolumeBarFill, null, CoerceBearishCandleFill) { AffectsRender = true });
        public Brush BearishBarFill
        {
            get { return (Brush)GetValue(BearishBarFillProperty); }
            set { SetValue(BearishBarFillProperty, value); }
        }

        private static object CoerceBearishCandleFill(DependencyObject objWithOldDP, object newDPValue)
        {
            VolumeChartElement thisElement = (VolumeChartElement)objWithOldDP;
            Brush newBrushValue = (Brush)newDPValue;

            if (newBrushValue.IsFrozen)
            {
                Pen p = new Pen(newBrushValue, 1.0);
                p.Freeze();
                thisElement.bearishBarPen = p;
                return newDPValue;
            }
            else
            {
                Brush b = (Brush)newBrushValue.GetCurrentValueAsFrozen();
                Pen p = new Pen(b, 1.0);
                p.Freeze();
                thisElement.bearishBarPen = p;
                return b;
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public static readonly DependencyProperty VisibleCandlesExtremumsProperty
            = DependencyProperty.Register("VisibleCandlesExtremums", typeof(CandleExtremums), typeof(VolumeChartElement), new FrameworkPropertyMetadata(new CandleExtremums(0.0, 0.0, 0L, 0L)) { AffectsRender = true });
        public CandleExtremums VisibleCandlesExtremums
        {
            get { return (CandleExtremums)GetValue(VisibleCandlesExtremumsProperty); }
            set { SetValue(VisibleCandlesExtremumsProperty, value); }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public static readonly DependencyProperty VisibleCandlesRangeProperty
             = DependencyProperty.Register("VisibleCandlesRange", typeof(IntRange), typeof(VolumeChartElement), new FrameworkPropertyMetadata(IntRange.Undefined));
        public IntRange VisibleCandlesRange
        {
            get { return (IntRange)GetValue(VisibleCandlesRangeProperty); }
            set { SetValue(VisibleCandlesRangeProperty, value); }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public static readonly DependencyProperty CandleWidthAndGapProperty
             = DependencyProperty.Register("CandleWidthAndGap", typeof(CandleDrawingParameters), typeof(VolumeChartElement),
                 new FrameworkPropertyMetadata(new CandleDrawingParameters()));
        public CandleDrawingParameters CandleWidthAndGap
        {
            get { return (CandleDrawingParameters)GetValue(CandleWidthAndGapProperty); }
            set { SetValue(CandleWidthAndGapProperty, value); }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public double VolumeBarWidthToCandleWidthRatio
        {
            get { return (double)GetValue(VolumeBarWidthToCandleWidthRatioProperty); }
            set { SetValue(VolumeBarWidthToCandleWidthRatioProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VolumeBarWidthToCandleWidthRatio.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolumeBarWidthToCandleWidthRatioProperty =
            DependencyProperty.Register("VolumeBarWidthToCandleWidthRatio", typeof(double), typeof(VolumeChartElement), 
                new FrameworkPropertyMetadata(1.0) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        protected override void OnRender(DrawingContext drawingContext)
        {
            //drawingContext.DrawRectangle(Brushes.Aquamarine, transparentPen, new Rect(0, 0, RenderSize.Width, RenderSize.Height));
            double volumeBarWidth = VolumeBarWidthToCandleWidthRatio * CandleWidthAndGap.Width;
            double volumeBarGap = (1.0 - VolumeBarWidthToCandleWidthRatio) * CandleWidthAndGap.Width + CandleWidthAndGap.Gap;
            double halfDWidth = 0.5 * (CandleWidthAndGap.Width - volumeBarWidth);

            for (int i = 0; i < VisibleCandlesRange.Count; i++)
            {
                ICandle cndl = CandlesSource[VisibleCandlesRange.Start_i + i];
                Brush cndlBrush = (cndl.C > cndl.O) ? BullishBarFill : BearishBarFill;
                Pen cndlPen = (cndl.C > cndl.O) ? bullishBarPen : bearishBarPen;

                double wnd_V = (1.0 - cndl.V / (double)VisibleCandlesExtremums.VolumeHigh) * RenderSize.Height;

                double volumeBarLeftX = halfDWidth + i * (volumeBarWidth + volumeBarGap);
                if (cndl.V > 0L)
                    drawingContext.DrawRectangle(cndlBrush, null, new Rect(volumeBarLeftX, wnd_V, volumeBarWidth, RenderSize.Height - wnd_V));
                else
                    drawingContext.DrawLine(cndlPen, new Point(volumeBarLeftX, 0.0), new Point(volumeBarLeftX + volumeBarWidth, 0.0));
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(this);
            //Vector uv = new Vector(mousePos.X/ RenderSize.Width, mousePos.Y / RenderSize.Height);
            int cndl_i = VisibleCandlesRange.Start_i + (int)(mousePos.X / (CandleWidthAndGap.Width + CandleWidthAndGap.Gap));
            ICandle cndl = CandlesSource[cndl_i];
            string tooltipText = $"{cndl.t.ToString("d.MM.yyyy H:mm")}\nV={cndl.V}";
            ((ToolTip)ToolTip).Content = tooltipText;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
    }
}
