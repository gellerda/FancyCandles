# Populating CandleChart with candles
You have already added to your project an empty CandleChart control and now you want to populate it with candles:
1. Add to your project a new class that would realize the [ICandle](https://gellerda.github.io/FancyCandles/api/FancyCandles.ICandle.html) interface:

    ```cs
        public class Candle : FancyCandles.ICandle
        {
            public DateTime t { get; set; }
            public double O { get; set; }
            public double H { get; set; }
            public double L { get; set; }
            public double C { get; set; }
            public long V { get; set; }

            public Candle(DateTime t, double O, double H, double L, double C, long V)
            {
                this.t = t;
                this.O = O;
                this.H = H;
                this.L = L;
                this.C = C;
                this.V = V;
            }
        }
    ```
1. Open MainWindow.xaml.cs file. In the MainWindow constructor, create an instance *ObservableCollection<ICandle>* and fill it with some data. Every item of this ObservableCollection will be visualized as a candle. In this example we will generate some meaningless set of candle data items to fill the aforementioned ObservableCollection:

    ```cs
        ObservableCollection<ICandle> Candles = new ObservableCollection<ICandle>();

        DateTime t0 = new DateTime(2019, 6, 11, 23, 40, 0);
        for (int i = 0; i < 100; i++)
        {
            double p0 = Math.Round(Math.Sin(0.3*i) + 0.1*i, 3);
            double p1 = Math.Round(Math.Sin(0.3*i + 1) + 0.1*i, 3);
            Candles.Add(new Candle(t0.AddMinutes(i * 5), 100 + p0, 101 + p0, 99 + p0, 100 + p1, i));
        }
    ```
1. In the MainWindow constructor, set the DataContext property to aforementioned ObservableCollection:

    ```cs
        DataContext = Candles;
    ```
    As a result your MainWindow constructor in the MainWindow.xaml.cs file may looks like this:

    ```cs
        // don't forget about references to namespaces:
        using System.Collections.ObjectModel;
        using FancyCandles;

        ...

        // Your MainWindow constructor:
        public MainWindow()
        {
            InitializeComponent();
            /// ... some code
            ObservableCollection<ICandle> Candles = new ObservableCollection<ICandle>();

            DateTime t0 = new DateTime(2019, 6, 11, 23, 40, 0);
            for (int i = 0; i < 100; i++)
            {
                double p0 = Math.Round(Math.Sin(0.3*i) + 0.1*i, 3);
                double p1 = Math.Round(Math.Sin(0.3*i + 1) + 0.1*i, 3);
                Candles.Add(new Candle(t0.AddMinutes(i * 5), 100 + p0, 101 + p0, 99 + p0, 100 + p1, i));
            }

            DataContext = Candles;
        }
    ```
1. Open the MainWindow.xaml file. For the CandleChart element set the CandlesSource attribute:

    ```cs
        <fc:CandleChart CandlesSource="{Binding Path=.}"/>
    ```
    Congratulations! Now a set of candles should appear in your CandleChart control.
