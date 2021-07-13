using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using FancyCandles;

namespace CandleChartExample
{
    public partial class MainWindow : Window
    {
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // This snippet of code fixes a problem of incorrect popup, tips and context menu location.
        // Read more about it: https://stackoverflow.com/questions/18113597/wpf-handedness-with-popups
        private static readonly System.Reflection.FieldInfo _menuDropAlignmentField;

        static MainWindow()
        {
            _menuDropAlignmentField = typeof(SystemParameters).GetField("_menuDropAlignment", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            System.Diagnostics.Debug.Assert(_menuDropAlignmentField != null);

            EnsureStandardPopupAlignment();
            SystemParameters.StaticPropertyChanged += SystemParameters_StaticPropertyChanged;
        }

        private static void SystemParameters_StaticPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            EnsureStandardPopupAlignment();
        }

        private static void EnsureStandardPopupAlignment()
        {
            if (SystemParameters.MenuDropAlignment && _menuDropAlignmentField != null)
            {
                _menuDropAlignmentField.SetValue(null, false);
            }
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        private double freq;
        private TimeFrame timeFrame = TimeFrame.M5;
        private int numberOfDecimalDigits = 2;
        //-----------------------------------------------------------------------------------------------------------------
        public CandlesSource Candles
        {
            get { return (CandlesSource)GetValue(CandlesProperty); }
            set { SetValue(CandlesProperty, value); }
        }
        public static readonly DependencyProperty CandlesProperty =
            DependencyProperty.Register("Candles", typeof(CandlesSource), typeof(MainWindow), new PropertyMetadata(null));
        //-----------------------------------------------------------------------------------------------------------------
        public MainWindow()
        {
            InitializeComponent();

            freq = 0.3;
            Candles = GenerateNewCandlesSource(500, 38650);

            DataContext = this;
        }
        //-----------------------------------------------------------------------------------------------------------------
        private CandlesSource GenerateNewCandlesSource(int candlesCount, double shiftY)
        {
            CandlesSource newCandles = new CandlesSource(timeFrame);

            for (int i = 0; i < candlesCount; i++)
                newCandles.Add(CalculateCandle(i, shiftY));

            return newCandles;
        }
        //-----------------------------------------------------------------------------------------------------------------
        private Candle CalculateCandle(int i, double shiftY)
        {
            double slope = 0.1;
            DateTime t0 = new DateTime(2010, 10, 7, 10, 0, 0);
            double p0 = Math.Sin(freq * i) + slope * i;
            double p1 = Math.Sin(freq * i + 1) + slope * i;
            DateTime t = (timeFrame>=0) ? t0.AddMinutes((int)timeFrame * i) : t0.AddSeconds(-(int)timeFrame * i);
            return new Candle(t, Math.Round(shiftY + p0, numberOfDecimalDigits), 
                                 Math.Round(shiftY + 1 + p0, numberOfDecimalDigits), 
                                 Math.Round(shiftY - 1 + p0, numberOfDecimalDigits), 
                                 Math.Round(shiftY + p1, numberOfDecimalDigits), 
                                 (i%3 == 0)?0:i);
        }
        //-----------------------------------------------------------------------------------------------------------------
        private void OnChangeLastCandle(object sender, RoutedEventArgs e)
        {
            int N = Candles.Count;
            if (N == 0) return;

            Candle lastCandle = (Candle)Candles[N - 1];

            // lastCandle.C += 1; - You can't do it this way! This will cause no changes in the chart.

            double newC = lastCandle.C + 1;
            double newH = Math.Max(newC, lastCandle.H);
            double newL = Math.Min(newC, lastCandle.L);
            Candle newCandle = new Candle(lastCandle.t, lastCandle.O, newH, newL, newC, lastCandle.V); // You must create a new Candle instance!

            Candles[N - 1] = newCandle; // This is correct!
        }
        //-----------------------------------------------------------------------------------------------------------------
        private void OnAddNewCandle(object sender, RoutedEventArgs e)
        {
            int N = Candles.Count;
            Candles.Add(CalculateCandle(N, 38650));
        }
        //-----------------------------------------------------------------------------------------------------------------
        private void OnReplaceCandlesBySmallerOne(object sender, RoutedEventArgs e)
        {
            timeFrame = TimeFrame.M5;
            freq = 0.8;
            Candles = GenerateNewCandlesSource(30, 38650);
        }
        //-----------------------------------------------------------------------------------------------------------------
        private void OnReplaceCandlesByBiggerOne(object sender, RoutedEventArgs e)
        {
            timeFrame = TimeFrame.M5;
            freq = 0.8;
            Candles = GenerateNewCandlesSource(800, 38650);
        }
        //-----------------------------------------------------------------------------------------------------------------
        private void OnReplaceCandlesByEmptyOne(object sender, RoutedEventArgs e)
        {
            timeFrame = TimeFrame.M5;
            freq = 0.8;
            Candles = GenerateNewCandlesSource(0, 38650);
        }
        //-----------------------------------------------------------------------------------------------------------------
        private void OnReplaceCandlesByDailyOne(object sender, RoutedEventArgs e)
        {
            timeFrame = TimeFrame.Daily;
            freq = 0.8;
            Candles = GenerateNewCandlesSource(500, 38650);
        }
        //-----------------------------------------------------------------------------------------------------------------
        private void OnReplaceCandlesByWeeklyOne(object sender, RoutedEventArgs e)
        {
            timeFrame = TimeFrame.Weekly;
            freq = 0.8;
            Candles = GenerateNewCandlesSource(500, 38650);
        }
        //-----------------------------------------------------------------------------------------------------------------
        private void OnReplaceCandlesBy10SecondOne(object sender, RoutedEventArgs e)
        {
            timeFrame = TimeFrame.S10;
            freq = 0.8;
            Candles = GenerateNewCandlesSource(500, 38650);
        }

        private void OnReplaceCandlesByOneWithNegativeCandles(object sender, RoutedEventArgs e)
        {
            timeFrame = TimeFrame.M5;
            freq = 0.8;
            Candles = GenerateNewCandlesSource(500, -40);
        }
        //-----------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------
    }
}
