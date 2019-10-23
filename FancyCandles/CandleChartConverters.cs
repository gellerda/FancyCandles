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
using System.Windows.Data;
using System.Globalization;

namespace FancyCandles
{
    //*******************************************************************************************************************************************************************
    class CrossPriceMarginConverter : IMultiValueConverter
    {
        // values[0] - Point CurrentMousePosition
        // values[1] - double PriceTickTextHeight
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2 || (values[0]).GetType() != typeof(Point) || (values[1]).GetType() != typeof(double))
                return new Thickness(0, 0, 0, 0);

            Point currentMousePosition = (Point)values[0];
            double priceTickTextHeight = (double)values[1];
            return new Thickness(0, currentMousePosition.Y - priceTickTextHeight / 2.0, 0, 0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }

    }
    //*******************************************************************************************************************************************************************
    class CrossPriceValueConverter : IMultiValueConverter
    {
        // values[0] - Point CurrentMousePosition
        // values[1] - double ChartAreaHeight
        // values[2] - Vector=(totalLow, totalHigh) 
        // values[3] - double PriceChartTopMargin
        // values[4] - double PriceChartBottomMargin
        // values[5] - int MaxNumberOfDigitsAfterPointInPrice
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ( values == null || values.Length < 6 || (values[0]).GetType() != typeof(Point) || (values[1]).GetType() != typeof(double) || (values[2]).GetType() != typeof(Vector)
                 || (values[3]).GetType() != typeof(double) || (values[4]).GetType() != typeof(double) || (values[5]).GetType() != typeof(int))
                return true;

            Point currentMousePosition = (Point)values[0];
            double ChartAreaHeight = (double)values[1];
            double totalL = ((Vector)values[2]).X;
            double totalH = ((Vector)values[2]).Y;
            double chartTopMargin = (double)values[3];
            double chartBottomMargin = (double)values[4];
            int maxNumberOfDigitsAfterPointInPrice = (int)values[5];
            return Math.Round((totalH - (currentMousePosition.Y - chartTopMargin) / (ChartAreaHeight - chartTopMargin - chartBottomMargin) * (totalH - totalL)), maxNumberOfDigitsAfterPointInPrice).ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }

    }
    //*******************************************************************************************************************************************************************
    class CrossVolumeConverter : IMultiValueConverter
    {
        // values[0] - Point CurrentMousePosition
        // values[1] - double VolumeHistogramHeight
        // values[2] - long CandlesMaxVolume
        // values[3] - double VolumeHistogramTopMargin
        // values[4] - double VolumeHistogramBottomMargin
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 5 || (values[0]).GetType() != typeof(Point) || (values[1]).GetType() != typeof(double) || (values[2]).GetType() != typeof(long)
                 || (values[3]).GetType() != typeof(double) || (values[4]).GetType() != typeof(double) )
                return (long)0;

            Point currentMousePosition = (Point)values[0];
            double volumeHistogramHeight = (double)values[1];
            long candlesMaxVolume = (long)values[2];
            double volumeHistogramTopMargin = (double)values[3];
            double volumeHistogramBottomMargin = (double)values[4];
            return ((long)((candlesMaxVolume - (currentMousePosition.Y - volumeHistogramTopMargin) / (volumeHistogramHeight - volumeHistogramTopMargin - volumeHistogramBottomMargin) * candlesMaxVolume))).ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }

    }
    //*******************************************************************************************************************************************************************
    class VerticalCrossLineVisibilityConverter : IMultiValueConverter
    {
        // values[0] - bool IsCrossLinesVisible
        // values[1] - bool IsMouseOverPriceChart
        // values[2] - bool IsMouseOverVolumeHistogram
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 3 || (values[0]).GetType() != typeof(bool) || (values[1]).GetType() != typeof(bool) || (values[2]).GetType() != typeof(bool))
                return true;

            bool isCrossLinesVisible = (bool)values[0];
            bool isMouseOverPriceChart = (bool)values[1];
            bool isMouseOverVolumeHistogram = (bool)values[2];
            return (isCrossLinesVisible && (isMouseOverPriceChart || isMouseOverVolumeHistogram)) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }

    }
    //*******************************************************************************************************************************************************************
    class SquareBoolToVisibilityConverter : IMultiValueConverter
    {
        // values[0] - bool bool1
        // values[1] - bool bool2
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2 || (values[0]).GetType() != typeof(bool) || (values[1]).GetType() != typeof(bool))
                return true;

            bool bool0 = (bool)values[0];
            bool bool1 = (bool)values[1];
            return (bool0 && bool1) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }

    }
    //*******************************************************************************************************************************************************************
    class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object bool_value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)bool_value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    //*******************************************************************************************************************************************************************
    // Opposite to BoolToVisibilityConverter
    class NotBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object bool_value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)bool_value) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    //*******************************************************************************************************************************************************************
    class IntRange_Start_i_Converter : IValueConverter
    {
        // value - IntRange visibleCandlesRange
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IntRange visibleCandlesRange = (IntRange)value;
            return visibleCandlesRange.Start_i;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int start_i = System.Convert.ToInt32((double)value);
            IntRange visibleCandlesRange = IntRange.CreateContainingOnlyStart_i(start_i);
            return visibleCandlesRange;
        }
    }
    //*******************************************************************************************************************************************************************
    class IntRange_Count_Converter : IValueConverter
    {
        // value - IntRange visibleCandlesRange
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IntRange visibleCandlesRange = (IntRange)value;
            return visibleCandlesRange.Count;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int start_i = System.Convert.ToInt32((double)value);
            IntRange visibleCandlesRange = IntRange.CreateContainingOnlyStart_i(start_i);
            return visibleCandlesRange;
        }
    }
    //*******************************************************************************************************************************************************************
    // Возвращает максимально допустимое значение для свойства FirstCandle_csi.
    class FirstCandleMaxIndexConverter : IMultiValueConverter
    {
        // values[0] - ObservableCollection<Candle> candles
        // values[1] - IntRange VisibleCandlesRange
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2 || (values[1]).GetType() != typeof(IntRange))
                return 0.0;

            IList<ICandle> candles = (IList<ICandle>)values[0];
            if (candles == null)
                return (double)int.MaxValue;
            else
            {
                int candlesCount = ((IntRange)values[1]).Count;
                if (candlesCount == 0)
                    return (double)int.MaxValue;
                return (double)(candles.Count - candlesCount);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }
    //*******************************************************************************************************************************************************************
    // Возвращает Margin для candlesItemsControl.
    class TopBottomMarginConverter : IMultiValueConverter
    {
        // values[0] - double topMargin
        // values[1] - double bottomMargin
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2 || (values[0]).GetType()!=typeof(double) || (values[1]).GetType() != typeof(double))
                return new Thickness(0,0,0,0);

            double topMargin = (double)values[0];
            double bottomMargin = (double)values[1];
            return new Thickness(0, topMargin, 0, bottomMargin);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }
    //*******************************************************************************************************************************************************************
    /*class BoolToVolumeGridRowHeightConverter : IValueConverter
    {
        public object Convert(object bool_value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)bool_value) ? new GridLength(1, GridUnitType.Star) : new GridLength(0.0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class BoolToPriceGridRowHeightConverter : IValueConverter
    {
        public object Convert(object bool_value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)bool_value) ? new GridLength(4, GridUnitType.Star) : new GridLength(1,GridUnitType.Auto);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }*/
    //*******************************************************************************************************************************************************************
}
