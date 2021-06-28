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
using System.ComponentModel;

namespace FancyCandles
{
    internal partial class SelectCandleSourceWindow : Window
    {
        private CandleChart parentCandleChart;
        //----------------------------------------------------------------------------------------------------------------------------------
        public SelectCandleSourceWindow(CandleChart parentCandleChart)
        {
            InitializeComponent();

            Owner = Application.Current.MainWindow;

            SecsView = CollectionViewSource.GetDefaultView(parentCandleChart.CandlesSourceProvider.SecCatalog);
            SecsView.Filter = SecFilter;

            this.parentCandleChart = parentCandleChart;
            DataContext = this;
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        private bool SecFilter(object item)
        {
            ISecurityInfo sec = item as ISecurityInfo;
            return (TickerFilterString.Trim() == "" || sec.Ticker.Contains(TickerFilterString)) &&
                   (NameFilterString.Trim() == "" || sec.SecurityName.Contains(NameFilterString));
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        public string TickerFilterString
        {
            get { return (string)GetValue(TickerFilterStringProperty); }
            set { SetValue(TickerFilterStringProperty, value); }
        }
        public static readonly DependencyProperty TickerFilterStringProperty =
            DependencyProperty.Register("TickerFilterString", typeof(string), typeof(SelectCandleSourceWindow), new PropertyMetadata("", OnTickerFilterStringChanged));

        static void OnTickerFilterStringChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            SelectCandleSourceWindow thisWindow = obj as SelectCandleSourceWindow;
            if (thisWindow == null) return;

            if (e.NewValue != null)
            {
                thisWindow.SecsView.Refresh();
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        public string NameFilterString
        {
            get { return (string)GetValue(NameFilterStringProperty); }
            set { SetValue(NameFilterStringProperty, value); }
        }
        public static readonly DependencyProperty NameFilterStringProperty =
            DependencyProperty.Register("NameFilterString", typeof(string), typeof(SelectCandleSourceWindow), new PropertyMetadata("", OnNameFilterStringChanged));

        static void OnNameFilterStringChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            SelectCandleSourceWindow thisWindow = obj as SelectCandleSourceWindow;
            if (thisWindow == null) return;

            if (e.NewValue != null)
            {
                thisWindow.SecsView.Refresh();
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        public ICollectionView SecsView
        {
            get { return (ICollectionView)GetValue(SecsViewProperty); }
            set { SetValue(SecsViewProperty, value); }
        }
        public static readonly DependencyProperty SecsViewProperty =
            DependencyProperty.Register("SecsView", typeof(ICollectionView), typeof(SelectCandleSourceWindow), new PropertyMetadata(null));
        //----------------------------------------------------------------------------------------------------------------------------------
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ICandlesSourceProvider candlesSourceProvider = parentCandleChart.CandlesSourceProvider;

            string secID = (secList.SelectedItem as ISecurityInfo).SecID;
            TimeFrame timeframe = parentCandleChart.CandlesSource == null ? TimeFrame.M5 : parentCandleChart.CandlesSource.TimeFrame;
            ICandlesSource newCandleSource = candlesSourceProvider.GetCandlesSource(secID, timeframe);
            parentCandleChart.CandlesSource = newCandleSource;

            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------
    }
}
