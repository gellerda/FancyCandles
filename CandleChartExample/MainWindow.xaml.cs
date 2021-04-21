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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<ICandle> candles = new ObservableCollection<ICandle>();

        public MainWindow()
        {
            InitializeComponent();

            DateTime t0 = new DateTime(2019, 6, 11, 23, 40, 0);
            for (int i = 0; i < 500; i++)
            {
                double p0 = Math.Round(Math.Sin(0.3 * i) + 0.1 * i, 3);
                double p1 = Math.Round(Math.Sin(0.3 * i + 1) + 0.1 * i, 3);
                candles.Add(new Candle(t0.AddMinutes(i * 5),
                            100 + p0, 101 + p0, 99 + p0, 100 + p1, i));
            }

            DataContext = candles;
        }
    }
}
