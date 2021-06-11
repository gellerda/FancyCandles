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
        private double freq;
        private int timeFrameInMinutes = 5;
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
            Candles = GenerateNewCandlesSource(500);

            DataContext = this;
        }
        //-----------------------------------------------------------------------------------------------------------------
        private CandlesSource GenerateNewCandlesSource(int candlesCount)
        {
            CandlesSource newCandles = new CandlesSource(timeFrameInMinutes);

            for (int i = 0; i < candlesCount; i++)
                newCandles.Add(CalculateCandle(i));

            return newCandles;
        }
        //-----------------------------------------------------------------------------------------------------------------
        private Candle CalculateCandle(int i)
        {
            double slope = 0.1;
            double shiftY = 38650;
            DateTime t0 = new DateTime(2010, 10, 7, 10, 0, 0);
            double p0 = Math.Round(Math.Sin(freq * i) + slope * i, numberOfDecimalDigits);
            double p1 = Math.Round(Math.Sin(freq * i + 1) + slope * i, numberOfDecimalDigits);
            //double p0 = Math.Sin(freq * i) + slope * i;
            //double p1 = Math.Sin(freq * i + 1) + slope * i;
            return new Candle(t0.AddMinutes(i * timeFrameInMinutes), shiftY + p0, shiftY + 1 + p0, shiftY - 1 + p0, shiftY + p1, (i%3 == 0)?0:i);
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
            Candles.Add(CalculateCandle(N));
        }
        //-----------------------------------------------------------------------------------------------------------------
        private void OnReplaceCandlesBySmallerOne(object sender, RoutedEventArgs e)
        {
            timeFrameInMinutes = 5;
            freq = 0.8;
            Candles = GenerateNewCandlesSource(30);
        }
        //-----------------------------------------------------------------------------------------------------------------
        private void OnReplaceCandlesByBiggerOne(object sender, RoutedEventArgs e)
        {
            timeFrameInMinutes = 5;
            freq = 0.8;
            Candles = GenerateNewCandlesSource(800);
        }
        //-----------------------------------------------------------------------------------------------------------------
        private void OnReplaceCandlesByEmptyOne(object sender, RoutedEventArgs e)
        {
            timeFrameInMinutes = 5;
            freq = 0.8;
            Candles = GenerateNewCandlesSource(0);
        }
        //-----------------------------------------------------------------------------------------------------------------
        private void OnReplaceCandlesByDailyOne(object sender, RoutedEventArgs e)
        {
            timeFrameInMinutes = 1440;
            freq = 0.8;
            Candles = GenerateNewCandlesSource(500);
        }
        //-----------------------------------------------------------------------------------------------------------------
        private void OnReplaceCandlesByWeeklyOne(object sender, RoutedEventArgs e)
        {
            timeFrameInMinutes = 1440*7;
            freq = 0.8;
            Candles = GenerateNewCandlesSource(500);
        }
        //-----------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------
    }
}
