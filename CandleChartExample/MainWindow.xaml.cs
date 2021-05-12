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
        private ObservableCollection<ICandle> candles;
        //-----------------------------------------------------------------------------------------------------------------
        public ReadOnlyObservableCollection<ICandle> ReadOnlyCandles
        {
            get { return (ReadOnlyObservableCollection<ICandle>)GetValue(ReadOnlyCandlesProperty); }
            set { SetValue(ReadOnlyCandlesProperty, value); }
        }
        public static readonly DependencyProperty ReadOnlyCandlesProperty =
            DependencyProperty.Register("ReadOnlyCandles", typeof(ReadOnlyObservableCollection<ICandle>), typeof(MainWindow), new PropertyMetadata(null));
        //-----------------------------------------------------------------------------------------------------------------
        public MainWindow()
        {
            InitializeComponent();

            freq = 0.3;
            candles = GenerateCandleCollection(500, freq);
            ReadOnlyCandles = new ReadOnlyObservableCollection<ICandle>(candles);

            DataContext = this;
        }
        //-----------------------------------------------------------------------------------------------------------------
        private ObservableCollection<ICandle> GenerateCandleCollection(int candlesCount, double freq)
        {
            ObservableCollection<ICandle> newCandles = new ObservableCollection<ICandle>();

            for (int i = 0; i < candlesCount; i++)
                newCandles.Add(CalculateCandle(i, freq));

            return newCandles;
        }
        //-----------------------------------------------------------------------------------------------------------------
        private Candle CalculateCandle(int i, double freq)
        {
            DateTime t0 = new DateTime(2019, 6, 11, 23, 40, 0);
            double p0 = Math.Round(Math.Sin(freq * i) + 0.1 * i, 3);
            double p1 = Math.Round(Math.Sin(freq * i + 1) + 0.1 * i, 3);
            return new Candle(t0.AddMinutes(i * 5), 100 + p0, 101 + p0, 99 + p0, 100 + p1, (i%3 == 0)?0:i);
        }
        //-----------------------------------------------------------------------------------------------------------------
        private void OnChangeLastCandle(object sender, RoutedEventArgs e)
        {
            int N = candles.Count;
            if (N == 0) return;

            Candle lastCandle = (Candle)candles[N - 1];

            // lastCandle.C += 1; - You can't do it this way! This will cause no changes in the chart.

            double newC = lastCandle.C + 1;
            double newH = Math.Max(newC, lastCandle.H);
            double newL = Math.Min(newC, lastCandle.L);
            Candle newCandle = new Candle(lastCandle.t, lastCandle.O, newH, newL, newC, lastCandle.V); // You must create a new Candle instance!

            candles[N - 1] = newCandle; // This is correct!
        }
        //-----------------------------------------------------------------------------------------------------------------
        private void OnAddNewCandle(object sender, RoutedEventArgs e)
        {
            int N = candles.Count;
            candles.Add(CalculateCandle(N,freq));
        }
        //-----------------------------------------------------------------------------------------------------------------
        private void OnReplaceCandlesBySmallerOne(object sender, RoutedEventArgs e)
        {
            freq = 0.8;
            candles = GenerateCandleCollection(30, freq);
            ReadOnlyCandles = new ReadOnlyObservableCollection<ICandle>(candles);
        }
        //-----------------------------------------------------------------------------------------------------------------
        private void OnReplaceCandlesByBiggerOne(object sender, RoutedEventArgs e)
        {
            freq = 0.8;
            candles = GenerateCandleCollection(800, freq);
            ReadOnlyCandles = new ReadOnlyObservableCollection<ICandle>(candles);
        }
        //-----------------------------------------------------------------------------------------------------------------
        private void OnReplaceCandlesByEmptyOne(object sender, RoutedEventArgs e)
        {
            freq = 0.8;
            candles = GenerateCandleCollection(0, freq);
            ReadOnlyCandles = new ReadOnlyObservableCollection<ICandle>(candles);
        }
        //-----------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------
    }
}
