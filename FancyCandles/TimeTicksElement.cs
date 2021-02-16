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

namespace FancyCandles
{
    class TimeTicksElement : FrameworkElement
    {
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
            if (Time_TickWidth==0.0)
                OnTimeTickFontSizeChanged(this, new DependencyPropertyChangedEventArgs());
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        static TimeTicksElement()
        {
            Pen defaultPen = new Pen(CandleChart.DefaultVerticalGridlinesBrush, CandleChart.DefaultVerticalGridlinesThickness); // { DashStyle = new DashStyle(new double[] { 2, 3 }, 0) };
            defaultPen.Freeze();
            GridlinesPenProperty = DependencyProperty.Register("GridlinesPen", typeof(Pen), typeof(TimeTicksElement),
                new FrameworkPropertyMetadata(defaultPen, null, CoerceGridlinesPen) { AffectsRender = true });
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
        public int TimeFrame
        {
            get { return (int)GetValue(TimeFrameProperty); }
            set { SetValue(TimeFrameProperty, value); }
        }
        public static readonly DependencyProperty TimeFrameProperty
            = DependencyProperty.Register("TimeFrame", typeof(int), typeof(TimeTicksElement),
                new FrameworkPropertyMetadata(0) { AffectsRender = true });
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
        public double TimeTickFontSize
        {
            get { return (double)GetValue(TimeTickFontSizeProperty); }
            set { SetValue(TimeTickFontSizeProperty, value); }
        }
        public static readonly DependencyProperty TimeTickFontSizeProperty 
            = CandleChart.TimeTickFontSizeProperty.AddOwner(typeof(TimeTicksElement),
                new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnTimeTickFontSizeChanged)) { AffectsRender = true, AffectsMeasure = true });

        private static void OnTimeTickFontSizeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TimeTicksElement thisElement = (TimeTicksElement)obj;
            thisElement.Time_TickWidth = thisElement.Calc_Time_TickWidth(thisElement.TimeTickFontSize);
            thisElement.DayOrMonthOrYear_TickWidth = thisElement.Calc_DayOrMonthOrYear_TickWidth(thisElement.TimeTickFontSize);
            thisElement.Day_TickWidth = thisElement.Calc_Day_TickWidth(thisElement.TimeTickFontSize);
            thisElement.ReCalc_TimeTicksTimeFrame();
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
        static double TicksWidthWithGapFactor = 2.0; // = (Tick width + Gap width between ticks) / Tick width

        double Time_TickWidth = 0.0;
        double Calc_Time_TickWidth(double fontSize)
        {
            return (new FormattedText("23H", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip)).Width;
        }

        double DayOrMonthOrYear_TickWidth = 0;
        double Calc_DayOrMonthOrYear_TickWidth(double fontSize)
        {
            return (new FormattedText("2020", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip)).Width;
        }

        double Day_TickWidth = 0;
        double Calc_Day_TickWidth(double fontSize)
        {
            return TicksWidthWithGapFactor * (new FormattedText("30", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip)).Width;
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
        public IList<ICandle> CandlesSource
        {
            get { return (IList<ICandle>)GetValue(CandlesSourceProperty); }
            set { SetValue(CandlesSourceProperty, value); }
        }
        public static readonly DependencyProperty CandlesSourceProperty = DependencyProperty.Register("CandlesSource", typeof(IList<ICandle>), typeof(TimeTicksElement), 
                                                                            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCandlesSourceChanged)) { AffectsRender = true });

        private static void OnCandlesSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TimeTicksElement thisElement = (TimeTicksElement)obj;
            thisElement.ReCalc_TimeTicksTimeFrame();
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        // Таймфрейм в минутах, с которым отображаются метки на оси времени. Совпадает с одним из общепринятых таймфреймов - М5, М10, М15, М20, М30 и т.д.
        // Таймфрейм меток часто меняется и никак не связан с таймфреймом свечей.
        int TimeTicksTimeFrame;

        bool ReCalc_TimeTicksTimeFrame()
        {
            if (TimeFrame == 0 || CandleWidthAndGap.Width == 0.0 || TimeTickFontSize == 0.0) return false;

            double minutesCoveredByOneTimeTickLabel = Math.Ceiling(Time_TickWidth / (CandleWidthAndGap.Width + CandleWidthAndGap.Gap)) * TimeFrame;
            int old_TimeTicksTimeFrame = TimeTicksTimeFrame;
            TimeTicksTimeFrame = MyDateAndTime.CeilMinutesToConventionalTimeFrame(minutesCoveredByOneTimeTickLabel);
            return TimeTicksTimeFrame != old_TimeTicksTimeFrame;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (CandlesSource == null || VisibleCandlesRange == IntRange.Undefined || TimeFrame == 0 || TimeTicksTimeFrame == 0) return;

            double halfTimePanelHeight = TimeAxisHeight / 2.0;
            double topTimePanelY = RenderSize.Height - TimeAxisHeight;
            double centerTimePanelY = RenderSize.Height - TimeAxisHeight / 2.0;

            if (TimeFrame < 60)
                OnRender_TimeAndDay(drawingContext, axisTickPen);
            else
                OnRender_DayAndMonth(drawingContext, axisTickPen);

            // Горизонтальные линии на всю ширину разделяющая и окаймляющая панели времени и даты:
            drawingContext.DrawLine(axisTickPen, new Point(0, topTimePanelY), new Point(RenderSize.Width, topTimePanelY));
            drawingContext.DrawLine(axisTickPen, new Point(0, centerTimePanelY), new Point(RenderSize.Width, centerTimePanelY));
            drawingContext.DrawLine(axisTickPen, new Point(0, RenderSize.Height), new Point(RenderSize.Width, RenderSize.Height));
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        void OnRender_TimeAndDay(DrawingContext drawingContext, Pen pen)
        {
            int time_csi = -1;
            bool isHourStart = false;

            int date_csi = -1;
            bool isMonthStart = false, isYearStart = false;
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            double topTimePanelY = RenderSize.Height - TimeAxisHeight;
            double centerTimePanelY = RenderSize.Height - TimeAxisHeight / 2.0;
            double smallMarkLineY = topTimePanelY + TimeAxisHeight / 8.0;

            void DrawTimeTick()
            {
                //double timeTickRightMargin = CandleWidth/2.0 +  (day_csi - VisibleCandlesRange.Start_i) * (CandleWidth + CandleGap);
                string timeTickText = TimeTick.ConvertDateTimeToTimeTickText(isHourStart, CandlesSource[time_csi].t);
                FormattedText timeTickFormattedText = new FormattedText(timeTickText, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), TimeTickFontSize, AxisTickColor, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                double x = CandleWidthAndGap.Width / 2.0 + (time_csi - VisibleCandlesRange.Start_i) * (CandleWidthAndGap.Width + CandleWidthAndGap.Gap);
                drawingContext.DrawText(timeTickFormattedText, new Point(x + 2, topTimePanelY));
                drawingContext.DrawLine(pen, new Point(x, topTimePanelY), new Point(x, isHourStart ? centerTimePanelY : smallMarkLineY));

                if (IsGridlinesEnabled && GridlinesPen != null && isHourStart)
                    drawingContext.DrawLine(GridlinesPen, new Point(x, 0), new Point(x, topTimePanelY));
            }

            void DrawDateTick()
            {
                string dateTickText = TimeTick.ConvertDateTimeToDateTickText(isYearStart, isMonthStart, CandlesSource[date_csi].t);
                FormattedText dateTickFormattedText = new FormattedText(dateTickText, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), TimeTickFontSize, AxisTickColor, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                double x = CandleWidthAndGap.Width / 2.0 + (date_csi - VisibleCandlesRange.Start_i) * (CandleWidthAndGap.Width + CandleWidthAndGap.Gap);
                drawingContext.DrawText(dateTickFormattedText, new Point(x + 2, centerTimePanelY));
                drawingContext.DrawLine(pen, new Point(x, centerTimePanelY), new Point(x, RenderSize.Height));
            }
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            int time_LabelWidthInCandles = Convert.ToInt32(Math.Ceiling(Time_TickWidth / (CandleWidthAndGap.Width + CandleWidthAndGap.Gap)));
            int date_LabelWidthInCandles = Convert.ToInt32(Math.Ceiling(DayOrMonthOrYear_TickWidth / (CandleWidthAndGap.Width + CandleWidthAndGap.Gap)));
            for (int i = VisibleCandlesRange.Start_i + VisibleCandlesRange.Count - 1; i >= VisibleCandlesRange.Start_i; i--)
            {
                ICandle cndl = CandlesSource[i];
                ICandle prev_cndl = i > 0 ? CandlesSource[i - 1] : CandlesSource[0];

                // Если cndl - это начало нового дня:
                if (i > 0 && cndl.t.Date != prev_cndl.t.Date)
                {
                    // Если cndl - это начало нового месяца:
                    if (cndl.t.Month != prev_cndl.t.Month)
                    {
                        if ((date_csi - i) >= date_LabelWidthInCandles)
                        {
                            DrawDateTick();
                            date_csi = i;
                            isMonthStart = true;
                            isYearStart = cndl.t.Year != prev_cndl.t.Year;
                        }
                        else if (date_csi == -1 || !isMonthStart || (!isYearStart && cndl.t.Year != prev_cndl.t.Year))
                        {
                            date_csi = i;
                            isMonthStart = true;
                            isYearStart = cndl.t.Year != prev_cndl.t.Year;
                        }
                    }
                    // Если cndl - это НЕ начало нового месяца:
                    else
                    {
                        if (date_csi == -1)
                        {
                            date_csi = i;
                            isMonthStart = isYearStart = false;
                        }
                        else if ((date_csi - i) >= date_LabelWidthInCandles)
                        {
                            DrawDateTick();
                            date_csi = i;
                            isMonthStart = isYearStart = false;
                        }
                    }
                }

                // Если cndl - это начало нового часа:
                if (MyDateAndTime.IsTimeMultipleOf(cndl.t, 60) || (i > 0 && !MyDateAndTime.IsInSameHour(cndl.t, prev_cndl.t)))
                {
                    if ((time_csi - i) >= time_LabelWidthInCandles)
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
                else if (MyDateAndTime.IsTimeMultipleOf(cndl.t, TimeTicksTimeFrame))
                {
                    if (time_csi == -1)
                    {
                        time_csi = i;
                        isHourStart = false;
                    }
                    else if ((time_csi - i) >= time_LabelWidthInCandles)
                    {
                        DrawTimeTick();
                        time_csi = i;
                        isHourStart = false;
                    }
                }
            }

            // И не забудем нарисовать последний таймтик из "кэша", если он там есть:
            if (time_csi != -1) DrawTimeTick();
            if (date_csi != -1) DrawDateTick();
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        void OnRender_DayAndMonth(DrawingContext drawingContext, Pen pen)
        {
            int day_csi = -1;

            int month_csi = -1;
            bool isMonthStart = false, isYearStart = false;
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            double markLineHeight = RenderSize.Height / 8.0;
            double halfRenderSizeHeight = RenderSize.Height / 2.0;
            double topTimePanelY = RenderSize.Height - TimeAxisHeight;
            double centerTimePanelY = RenderSize.Height - TimeAxisHeight / 2.0;
            double smallMarkLineY = topTimePanelY + TimeAxisHeight / 8.0;

            void DrawDayTick()
            {
                FormattedText dayTickFormattedText = new FormattedText(CandlesSource[day_csi].t.Day.ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), TimeTickFontSize, AxisTickColor, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                double x = CandleWidthAndGap.Width / 2.0 + (day_csi - VisibleCandlesRange.Start_i) * (CandleWidthAndGap.Width + CandleWidthAndGap.Gap);
                drawingContext.DrawText(dayTickFormattedText, new Point(x + 2, topTimePanelY));
                drawingContext.DrawLine(pen, new Point(x, topTimePanelY), new Point(x, isMonthStart ? centerTimePanelY : smallMarkLineY));

                if (GridlinesPen != null)
                {
                    // Если таймфрейм Daily и более. Младшими считаются линии для дней внутри месяца.
                    if (TimeFrame > 1000)
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
                FormattedText monthTickFormattedText = new FormattedText(monthTickText, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), TimeTickFontSize, AxisTickColor, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                double x = CandleWidthAndGap.Width / 2.0 + (month_csi - VisibleCandlesRange.Start_i) * (CandleWidthAndGap.Width + CandleWidthAndGap.Gap);
                drawingContext.DrawText(monthTickFormattedText, new Point(x + 2, centerTimePanelY));
                drawingContext.DrawLine(pen, new Point(x, centerTimePanelY), new Point(x, RenderSize.Height));
            }
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            int day_LabelWidthInCandles = Convert.ToInt32(Math.Ceiling(Day_TickWidth / (CandleWidthAndGap.Width + CandleWidthAndGap.Gap)));
            int month_LabelWidthInCandles = Convert.ToInt32(Math.Ceiling(DayOrMonthOrYear_TickWidth / (CandleWidthAndGap.Width + CandleWidthAndGap.Gap)));
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
                        if ((month_csi - i) >= month_LabelWidthInCandles)
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
                        else if ((month_csi - i) >= month_LabelWidthInCandles)
                        {
                            DrawMonthTick();
                            month_csi = i;
                            isYearStart = false;
                        }
                    }

                    if ((day_csi - i) >= day_LabelWidthInCandles)
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
                    else if ((day_csi - i) >= day_LabelWidthInCandles)
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
        /*protected override Size MeasureOverride(Size availableSize)
        {
            Size desiredSize = base.MeasureOverride(availableSize);
            double textHeight = (new FormattedText("1Ajl", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), TimeTickFontSize, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip)).Height;
            desiredSize.Height = 2*textHeight + 4.0;
            return desiredSize;
        }*/
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
    }
}
