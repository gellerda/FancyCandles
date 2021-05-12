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
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace FancyCandles
{
    /// <summary>
    /// Interaction logic for SelectCandleSourceWindow.xaml
    /// </summary>
    public partial class SelectCandleSourceWindow : Window
    {
        private CandleChart parentCandleChart;

        public SelectCandleSourceWindow(CandleChart parentCandleChart)
        {
            InitializeComponent();
            this.parentCandleChart = parentCandleChart;
            secList.DataContext = parentCandleChart;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ICandleProvider candleProvider = parentCandleChart.CandleProvider;
            ReadOnlyObservableCollection<ICandle> newCandleSource = candleProvider.GetCandleSource(secList.SelectedIndex, parentCandleChart.TimeFrame);
            parentCandleChart.CandlesSource = newCandleSource;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
