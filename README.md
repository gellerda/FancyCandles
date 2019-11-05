FancyCandles WPF library lets you add professional candlestick charts to your applications. A wide variety of settings are available for fine tuning. Traditionally candlesticks are used for visualizing a price history of any financial instruments - stocks, currency pairs or futures. But you can utilize candlestick charts for plotting any time series that contain OHLC (t) components: {t - time, O - Open, H - High, L - Low, C - Close}.

FancyCandles is an open source library under the GPLv3 license. That lets you use FancyCandles candlestick charts in your internal trading/analytical software applications or another open source projects absolutely free. A licensing under permissive licenses is available for a commercial usage in a proprietary software projects.

# Documentation
You can read the [documentation online](https://gellerda.github.io/FancyCandles/) or [download a local version](https://gellerda.github.io/FancyCandles/download/download_doc.html).

# Quick start

## Creating empty CandleChart
1. Clone or Download FancyCandles repository to your computer.
1. Add FancyCandles project to the solution of your WPF application that would use the candlestick chart control:
    * Right-click on solution of your application in Solution Explorer and select *Add/Existing project...*.
    * Select path to the *...FancyCandles-master/FancyCandles/FancyCandles.csproj* project file and click *Open* button.
1. Add to your project that would use the candlestick chart control reference to the FancyCandles project:
    * Right-click on *References* under your project in Solution Explorer and select *Add reference...*.
    * In the *Projects* tab find the *FancyCandles* project row and ckeck it.
    * Click *OK*.
1. Open the MainWindow.xaml file of your project and add the declaration of the FancyCandles namespace. To do this, add the new *xmlns* property inside the *Window* open tag:

    ```xaml
        <Window x:Class="MyProjectThatWouldUseFancyCandles.MainWindow"
        ...
        xmlns:fc="clr-namespace:FancyCandles;assembly=FancyCandles"
        ... >
    ```
1. Inside the body of the aforementioned XML document add the *FancyCandles.CandleChart* control element:

    ```xaml
        <fc:CandleChart />
    ```
   As a result your MainWindow.xaml may looks like this:

    ```xaml
       <Window x:Class="MyProjectThatWouldUseFancyCandles.MainWindow"
               xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
               xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
               xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
               xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
               xmlns:local="clr-namespace:ggg"
               xmlns:fc="clr-namespace:FancyCandles;assembly=FancyCandles"
               mc:Ignorable="d"
               Title="MainWindow" Height="450" Width="800">
           <Grid>
               <fc:CandleChart CandlesSource="{Binding Path=.}"/>
           </Grid>
       </Window>       
    ```
   Congratulations! You have added the candlestick chart control (*CandleChart class*) to your application. But it is empty yet and contains no candles! The next step is to populate it with data.

## Populating CandleChart with candles
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

# License
[GNU GPLv3 license](https://github.com/gellerda/FancyCandles/blob/master/LICENSE).
