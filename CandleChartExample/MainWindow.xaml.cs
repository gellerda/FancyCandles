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
        ObservableCollection<ICandle> candles = new ObservableCollection<ICandle>();
        //-----------------------------------------------------------------------------------------------------------------
        public MainWindow()
        {
            InitializeComponent();

            for (int i = 0; i < 500; i++)
                candles.Add(CalculateCandle(i));

            DataContext = candles;
        }
        //-----------------------------------------------------------------------------------------------------------------
        private Candle CalculateCandle(int i)
        {
            DateTime t0 = new DateTime(2019, 6, 11, 23, 40, 0);
            double p0 = Math.Round(Math.Sin(0.3 * i) + 0.1 * i, 3);
            double p1 = Math.Round(Math.Sin(0.3 * i + 1) + 0.1 * i, 3);
            return new Candle(t0.AddMinutes(i * 5), 100 + p0, 101 + p0, 99 + p0, 100 + p1, i);
        }
        //-----------------------------------------------------------------------------------------------------------------
        private void OnChangeLastCandle(object sender, RoutedEventArgs e)
        {
            int N = candles.Count;
            Candle lastCandle = (Candle)candles[N - 1];

            // lastCandle.C += 1; - You can't do this way! This will cause no changes in the chart.

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
            candles.Add(CalculateCandle(N));
        }
        //-----------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------
    }
}
