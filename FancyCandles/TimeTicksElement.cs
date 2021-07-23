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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Globalization;
using System.Diagnostics; // Debug.WriteLine("Error...");
using System.Collections.Specialized;

namespace FancyCandles
{
    class TimeTicksElement : FrameworkElement
    {
        private static double TICK_LEFT_MARGIN = 2.0;
        private static double TICK_RIGHT_MARGIN = 2.0;

        private static CultureInfo cultureEnUS = CultureInfo.GetCultureInfo("en-us");
        //---------------------------------------------------------------------------------------------------------------------------------------
        static TimeTicksElement()
        {
            Pen defaultPen = new Pen(CandleChart.DefaultVerticalGridlinesBrush, CandleChart.DefaultVerticalGridlinesThickness); // { DashStyle = new DashStyle(new double[] { 2, 3 }, 0) };
            defaultPen.Freeze();
            GridlinesPenProperty = DependencyProperty.Register("GridlinesPen", typeof(Pen), typeof(TimeTicksElement),
                new FrameworkPropertyMetadata(defaultPen, null, CoerceGridlinesPen) { AffectsRender = true });
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public TimeTicksElement()
        {
            Loaded += new RoutedEventHandler(OnTimeTicksElementLoaded);

            if (axisTickPen == null)
            {
                axisTickPen = new Pen(CandleChart.DefaultAxisTickColor, 1.0);
                if (!axisTickPen.IsFrozen)
                    axisTickPen.Freeze();
            }
        }

        void OnTimeTicksElementLoaded(object sender, RoutedEventArgs e)
        {
            if (threeCharTickLabelWidth==0.0)
                OnTickLabelFontSizeChanged(this, new DependencyPropertyChangedEventArgs());
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public bool HideMinorGridlines
        {
            get { return (bool)GetValue(HideMinorGridlinesProperty); }
            set { SetValue(HideMinorGridlinesProperty, value); }
        }
        public static readonly DependencyProperty HideMinorGridlinesProperty 
            = DependencyProperty.Register("HideMinorGridlines", typeof(bool), typeof(TimeTicksElement), new FrameworkPropertyMetadata(true) { AffectsRender = true });
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
            = DependencyProperty.Register("IsGridlinesEnabled", typeof(bool), typeof(TimeTicksElement),
                new FrameworkPropertyMetadata(true) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        private Pen axisTickPen;

        public Brush AxisTickColor
        {
            get { return (Brush)GetValue(AxisTickColorProperty); }
            set { SetValue(AxisTickColorProperty, value); }
        }
        public static readonly DependencyProperty AxisTickColorProperty
            = DependencyProperty.Register("AxisTickColor", typeof(Brush), typeof(TimeTicksElement),
                new FrameworkPropertyMetadata(CandleChart.DefaultAxisTickColor, null, CoerceAxisTickColor) { AffectsRender = true });

        private static object CoerceAxisTickColor(DependencyObject objWithOldDP, object newDPValue)
        {
            TimeTicksElement thisElement = (TimeTicksElement)objWithOldDP;
            Brush newBrushValue = (Brush)newDPValue;

            if (newBrushValue.IsFrozen)
            {
                Pen p = new Pen(newBrushValue, 1.0);
                p.Freeze();
                thisElement.axisTickPen = p;
                return newDPValue;
            }
            else
            {
                Brush b = (Brush)newBrushValue.GetCurrentValueAsFrozen();
                Pen p = new Pen(b, 1.0);
                p.Freeze();
                thisElement.axisTickPen = p;
                return b;
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public double TimeAxisHeight
        {
            get { return (double)GetValue(TimePanelHeightProperty); }
            set { SetValue(TimePanelHeightProperty, value); }
        }
        public static readonly DependencyProperty TimePanelHeightProperty
            = DependencyProperty.Register("TimeAxisHeight", typeof(double), typeof(TimeTicksElement),
                new FrameworkPropertyMetadata(0.0) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        private Typeface currentTypeFace = new Typeface(SystemFonts.MessageFontFamily.ToString());

        public FontFamily TickLabelFontFamily
        {
            get { return (FontFamily)GetValue(TickLabelFontFamilyProperty); }
            set { SetValue(TickLabelFontFamilyProperty, value); }
        }
        public static readonly DependencyProperty TickLabelFontFamilyProperty =
            DependencyProperty.Register("TickLabelFontFamily", typeof(FontFamily), typeof(TimeTicksElement), new FrameworkPropertyMetadata(SystemFonts.MessageFontFamily, OnTickLabelFontFamilyChanged));

        static void OnTickLabelFontFamilyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TimeTicksElement thisElement = obj as TimeTicksElement;
            if (thisElement == null) return;
            thisElement.currentTypeFace = new Typeface(thisElement.TickLabelFontFamily.ToString());
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public double TickLabelFontSize
        {
            get { return (double)GetValue(TickLabelFontSizeProperty); }
            set { SetValue(TickLabelFontSizeProperty, value); }
        }
        public static readonly DependencyProperty TickLabelFontSizeProperty
            = DependencyProperty.Register("TickLabelFontSize", typeof(double), typeof(TimeTicksElement), new FrameworkPropertyMetadata(16.0,OnTickLabelFontSizeChanged) { AffectsRender = true, AffectsMeasure = true });

        private static void OnTickLabelFontSizeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TimeTicksElement thisElement = (TimeTicksElement)obj;
            thisElement.ReCalc_TickLabelWidths();
            thisElement.ReCalc_TimeTicksTimeFrame();
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        private double twoCharTickLabelWidth = 0;
        private double threeCharTickLabelWidth = 0.0;
        private double fourCharTickLabelWidth = 0;

        void ReCalc_TickLabelWidths()
        {
            FormattedText txt = new FormattedText("23H", cultureEnUS, FlowDirection.LeftToRight, currentTypeFace, TickLabelFontSize, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);
            threeCharTickLabelWidth = txt.Width + TICK_RIGHT_MARGIN + TICK_LEFT_MARGIN;

            txt = new FormattedText("2020", cultureEnUS, FlowDirection.LeftToRight, currentTypeFace, TickLabelFontSize, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);
            fourCharTickLabelWidth = txt.Width + TICK_RIGHT_MARGIN + TICK_LEFT_MARGIN;

            txt = new FormattedText("30", cultureEnUS, FlowDirection.LeftToRight, currentTypeFace, TickLabelFontSize, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);
            twoCharTickLabelWidth = txt.Width + TICK_RIGHT_MARGIN + TICK_LEFT_MARGIN;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public IntRange VisibleCandlesRange
        {
            get { return (IntRange)GetValue(VisibleCandlesRangeProperty); }
            set { SetValue(VisibleCandlesRangeProperty, value); }
        }
        public static readonly DependencyProperty VisibleCandlesRangeProperty 
            = DependencyProperty.Register("VisibleCandlesRange", typeof(IntRange), typeof(TimeTicksElement),
                new FrameworkPropertyMetadata(IntRange.Undefined) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public static readonly DependencyProperty CandleWidthAndGapProperty 
            = DependencyProperty.Register("CandleWidthAndGap", typeof(CandleDrawingParameters), typeof(TimeTicksElement),
                  new FrameworkPropertyMetadata(new CandleDrawingParameters(), new PropertyChangedCallback(OnCandleWidthAndGapChanged)) { AffectsRender = true });
        public CandleDrawingParameters CandleWidthAndGap
        {
            get { return (CandleDrawingParameters)GetValue(CandleWidthAndGapProperty); }
            set { SetValue(CandleWidthAndGapProperty, value); }
        }

        private static void OnCandleWidthAndGapChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TimeTicksElement thisElement = (TimeTicksElement)obj;
            thisElement.ReCalc_TimeTicksTimeFrame();
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public ICandlesSource CandlesSource
        {
            get { return (ICandlesSource)GetValue(CandlesSourceProperty); }
            set { SetValue(CandlesSourceProperty, value); }
        }
        public static readonly DependencyProperty CandlesSourceProperty = DependencyProperty.Register("CandlesSource", typeof(ICandlesSource), typeof(TimeTicksElement), 
                                                                            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCandlesSourceChanged)) { AffectsRender = true });

        private static void OnCandlesSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TimeTicksElement thisElement = (TimeTicksElement)obj;
            thisElement.ReCalc_TimeTicksTimeFrame();

            if (e.OldValue != null && e.OldValue is INotifyCollectionChanged)
                (e.OldValue as INotifyCollectionChanged).CollectionChanged -= thisElement.OnCandlesSourceCollectionChanged;

            if (e.NewValue != null && e.NewValue is INotifyCollectionChanged)
                (e.NewValue as INotifyCollectionChanged).CollectionChanged += thisElement.OnCandlesSourceCollectionChanged;
        }

        private void OnCandlesSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                if (e.NewStartingIndex >= VisibleCandlesRange.Start_i && 
                    e.NewStartingIndex < (VisibleCandlesRange.Start_i + VisibleCandlesRange.Count - 1))
                    InvalidateVisual();
            }
            else if (e.Action == NotifyCollectionChangedAction.Add) { /* your code */ }
            else if (e.Action == NotifyCollectionChangedAction.Remove) { /* your code */ }
            else if (e.Action == NotifyCollectionChangedAction.Move) { /* your code */ }
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        // Таймфрейм в минутах, с которым отображаются метки на оси времени. Совпадает с одним из общепринятых таймфреймов - M1, М5, М10, М15, М20, М30 и т.д.
        // Таймфрейм меток часто меняется и никак не связан с таймфреймом свечей.
        int TimeTicksTimeFrame;

        void ReCalc_TimeTicksTimeFrame()
        {
            if (CandlesSource == null || CandlesSource.TimeFrame == 0 || CandleWidthAndGap.Width == 0.0 || TickLabelFontSize == 0.0) return;

            double minutesCoveredByOneTimeTickLabel = threeCharTickLabelWidth / (CandleWidthAndGap.Width + CandleWidthAndGap.Gap) * CandlesSource.TimeFrame.ToMinutes();
            TimeTicksTimeFrame = MyDateAndTime.CeilMinutesToConventionalTimeFrame(minutesCoveredByOneTimeTickLabel);
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (CandlesSource == null || VisibleCandlesRange == IntRange.Undefined || (int)CandlesSource.TimeFrame == 0 || TimeTicksTimeFrame == 0) return;

            double topTimePanelY = RenderSize.Height - TimeAxisHeight;
            double centerTimePanelY = RenderSize.Height - TimeAxisHeight / 2.0;

            if ((int)CandlesSource.TimeFrame < 30.0)
                OnRender_TimeAndDay(drawingContext, axisTickPen);
            else
                OnRender_DayAndMonth(drawingContext, axisTickPen);

            // Горизонтальные линии на всю ширину разделяющая и окаймляющая панели времени и даты:
            //drawingContext.DrawLine(axisTickPen, new Point(0, topTimePanelY), new Point(RenderSize.Width, topTimePanelY));
            drawingContext.DrawLine(axisTickPen, new Point(0, centerTimePanelY), new Point(RenderSize.Width, centerTimePanelY));
            drawingContext.DrawLine(axisTickPen, new Point(0, RenderSize.Height), new Point(RenderSize.Width, RenderSize.Height));
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        void OnRender_TimeAndDay(DrawingContext drawingContext, Pen pen)
        {
            int time_csi = -1;
            bool isHourStart = false;

            int day_csi = -1;
            bool isMonthStart = false, isYearStart = false;
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            double topTimePanelY = RenderSize.Height - TimeAxisHeight;
            double centerTimePanelY = RenderSize.Height - TimeAxisHeight / 2.0;
            double smallMarkLineY = topTimePanelY + TimeAxisHeight / 8.0;

            void DrawTimeTick()
            {
                //double timeTickRightMargin = CandleWidth/2.0 +  (day_csi - VisibleCandlesRange.Start_i) * (CandleWidth + CandleGap);
                string timeTickText = TimeTick.ConvertDateTimeToTimeTickText(isHourStart, CandlesSource[time_csi].t);
                FormattedText timeTickFormattedText = new FormattedText(timeTickText, cultureEnUS, FlowDirection.LeftToRight, currentTypeFace, TickLabelFontSize, AxisTickColor, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                double x = CandleWidthAndGap.Width / 2.0 + (time_csi - VisibleCandlesRange.Start_i) * (CandleWidthAndGap.Width + CandleWidthAndGap.Gap);
                drawingContext.DrawText(timeTickFormattedText, new Point(x + TICK_LEFT_MARGIN, topTimePanelY));
                drawingContext.DrawLine(pen, new Point(x, topTimePanelY), new Point(x, isHourStart ? centerTimePanelY : smallMarkLineY));

                if (IsGridlinesEnabled && GridlinesPen != null && isHourStart)
                    drawingContext.DrawLine(GridlinesPen, new Point(x, 0), new Point(x, topTimePanelY));
            }

            void DrawDayTick()
            {
                string dateTickText = TimeTick.ConvertDateTimeToDateTickText(isYearStart, isMonthStart, CandlesSource[day_csi].t);
                FormattedText dateTickFormattedText = new FormattedText(dateTickText, cultureEnUS, FlowDirection.LeftToRight, currentTypeFace, TickLabelFontSize, AxisTickColor, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                double x = CandleWidthAndGap.Width / 2.0 + (day_csi - VisibleCandlesRange.Start_i) * (CandleWidthAndGap.Width + CandleWidthAndGap.Gap);
                drawingContext.DrawText(dateTickFormattedText, new Point(x + TICK_LEFT_MARGIN, centerTimePanelY));
                drawingContext.DrawLine(pen, new Point(x, centerTimePanelY), new Point(x, RenderSize.Height));
            }
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            double timeLabelWidthInCandles = threeCharTickLabelWidth / (CandleWidthAndGap.Width + CandleWidthAndGap.Gap);
            double dayLabelWidthInCandles = fourCharTickLabelWidth / (CandleWidthAndGap.Width + CandleWidthAndGap.Gap);
            for (int i = VisibleCandlesRange.Start_i + VisibleCandlesRange.Count - 1; i >= VisibleCandlesRange.Start_i; i--)
            {
                ICandle cndl = CandlesSource[i];
                ICandle prev_cndl = i > 0 ? CandlesSource[i - 1] : cndl;

                // Если cndl - это начало нового дня:
                if (i > 0 && cndl.t.Date != prev_cndl.t.Date)
                {
                    // Если cndl - это начало нового месяца:
                    if (cndl.t.Month != prev_cndl.t.Month)
                    {
                        if ((day_csi - i) >= dayLabelWidthInCandles)
                        {
                            DrawDayTick();
                            day_csi = i;
                            isMonthStart = true;
                            isYearStart = cndl.t.Year != prev_cndl.t.Year;
                        }
                        else if (day_csi == -1 || !isMonthStart || (!isYearStart && cndl.t.Year != prev_cndl.t.Year))
                        {
                            day_csi = i;
                            isMonthStart = true;
                            isYearStart = cndl.t.Year != prev_cndl.t.Year;
                        }
                    }
                    // Если cndl - это НЕ начало нового месяца:
                    else
                    {
                        if (day_csi == -1)
                        {
                            day_csi = i;
                            isMonthStart = isYearStart = false;
                        }
                        else if ((day_csi - i) >= dayLabelWidthInCandles)
                        {
                            DrawDayTick();
                            day_csi = i;
                            isMonthStart = isYearStart = false;
                        }
                    }
                }

                // Если cndl - это начало нового часа:
                if ( (MyDateAndTime.IsMinutesMultipleOf(cndl.t, 60) && cndl.t.Second == 0) || !MyDateAndTime.IsInSameHour(cndl.t, prev_cndl.t))
                {
                    if ((time_csi - i) >= timeLabelWidthInCandles)
                    {
                        DrawTimeTick();
                        time_csi = i;
                        isHourStart = true;
                    }
                    else if (time_csi == -1 || !isHourStart)
                    {
                        time_csi = i;
                        isHourStart = true;
                    }
                }
                // Если cndl внутри часа:
                else if (MyDateAndTime.IsMinutesMultipleOf(cndl.t, TimeTicksTimeFrame) && (cndl.t.Second==0 || cndl.t.Minute!=prev_cndl.t.Minute))
                {
                    if (time_csi == -1)
                    {
                        time_csi = i;
                        isHourStart = false;
                    }
                    else if ((time_csi - i) >= timeLabelWidthInCandles)
                    {
                        DrawTimeTick();
                        time_csi = i;
                        isHourStart = false;
                    }
                }
            }

            // И не забудем нарисовать последний таймтик из "кэша", если он там есть:
            if (time_csi != -1) DrawTimeTick();
            if (day_csi != -1) DrawDayTick();
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        void OnRender_DayAndMonth(DrawingContext drawingContext, Pen pen)
        {
            int day_csi = -1;

            int month_csi = -1;
            bool isMonthStart = false, isYearStart = false;
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            double topTimePanelY = RenderSize.Height - TimeAxisHeight;
            double centerTimePanelY = RenderSize.Height - TimeAxisHeight / 2.0;
            double smallMarkLineY = topTimePanelY + TimeAxisHeight / 8.0;
            int tf = (int)CandlesSource.TimeFrame;

            void DrawDayTick()
            {
                FormattedText dayTickFormattedText = new FormattedText(CandlesSource[day_csi].t.Day.ToString(), cultureEnUS, FlowDirection.LeftToRight, currentTypeFace, TickLabelFontSize, AxisTickColor, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                double x = CandleWidthAndGap.Width / 2.0 + (day_csi - VisibleCandlesRange.Start_i) * (CandleWidthAndGap.Width + CandleWidthAndGap.Gap);
                drawingContext.DrawText(dayTickFormattedText, new Point(x + TICK_LEFT_MARGIN, topTimePanelY));
                drawingContext.DrawLine(pen, new Point(x, topTimePanelY), new Point(x, isMonthStart ? centerTimePanelY : smallMarkLineY));

                if (GridlinesPen != null)
                {
                    // Если таймфрейм Daily и более. Младшими считаются линии для дней внутри месяца.
                    if (tf > 1000)
                    {
                        if (!HideMinorGridlines || isMonthStart)
                            drawingContext.DrawLine(GridlinesPen, new Point(x, 0), new Point(x, topTimePanelY));
                    }
                    // Если таймфрейм менее, чем Daily. То нет никаких младших линий.
                    else
                    {
                        drawingContext.DrawLine(GridlinesPen, new Point(x, 0), new Point(x, topTimePanelY));
                    }
                }
            }

            void DrawMonthTick()
            {
                string monthTickText = TimeTick.ConvertDateTimeToMonthTickText(isYearStart, CandlesSource[month_csi].t);
                FormattedText monthTickFormattedText = new FormattedText(monthTickText, cultureEnUS, FlowDirection.LeftToRight, currentTypeFace, TickLabelFontSize, AxisTickColor, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                double x = CandleWidthAndGap.Width / 2.0 + (month_csi - VisibleCandlesRange.Start_i) * (CandleWidthAndGap.Width + CandleWidthAndGap.Gap);
                drawingContext.DrawText(monthTickFormattedText, new Point(x + TICK_LEFT_MARGIN, centerTimePanelY));
                drawingContext.DrawLine(pen, new Point(x, centerTimePanelY), new Point(x, RenderSize.Height));
            }
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            double dayLabelWidthInCandles = twoCharTickLabelWidth / (CandleWidthAndGap.Width + CandleWidthAndGap.Gap);
            double monthLabelWidthInCandles = fourCharTickLabelWidth / (CandleWidthAndGap.Width + CandleWidthAndGap.Gap);
            for (int i = VisibleCandlesRange.Start_i + VisibleCandlesRange.Count - 1; i >= VisibleCandlesRange.Start_i; i--)
            {
                ICandle cndl = CandlesSource[i];
                ICandle prev_cndl = i > 0 ? CandlesSource[i - 1] : CandlesSource[0];
                if (i <= 0 || cndl.t.Date == prev_cndl.t.Date) continue;

                // Если cndl - это начало нового месяца:
                if (cndl.t.Month != prev_cndl.t.Month)
                {
                    // Если cndl - это начало нового года:
                    if (cndl.t.Year != prev_cndl.t.Year)
                    {
                        if ((month_csi - i) >= monthLabelWidthInCandles)
                        {
                            DrawMonthTick();
                            month_csi = i;
                            isYearStart = true;
                        }
                        else if (month_csi == -1 || !isYearStart)
                        {
                            month_csi = i;
                            isYearStart = true;
                        }
                    }
                    // Если cndl - это НЕ начало нового года:
                    else
                    {
                        if (month_csi == -1)
                        {
                            month_csi = i;
                            isYearStart = false;
                        }
                        else if ((month_csi - i) >= monthLabelWidthInCandles)
                        {
                            DrawMonthTick();
                            month_csi = i;
                            isYearStart = false;
                        }
                    }

                    if ((day_csi - i) >= dayLabelWidthInCandles)
                    {
                        DrawDayTick();
                        day_csi = i;
                        isMonthStart = true;
                    }
                    else if (day_csi == -1 || !isMonthStart)
                    {
                        day_csi = i;
                        isMonthStart = true;
                    }

                }
                // Если cndl внутри месяца:
                else
                {
                    if (day_csi == -1)
                    {
                        day_csi = i;
                        isMonthStart = false;
                    }
                    else if ((day_csi - i) >= dayLabelWidthInCandles)
                    {
                        DrawDayTick();
                        day_csi = i;
                        isMonthStart = false;
                    }
                }
            }

            // И не забудем нарисовать последние тики из "кэша", если они там есть:
            if (day_csi != -1) DrawDayTick();
            if (month_csi != -1) DrawMonthTick();
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
    }
}
