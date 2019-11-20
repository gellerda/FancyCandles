# Creating empty CandleChart
1. Open the MainWindow.xaml file of your project and add the declaration of the FancyCandles namespace. To do this, add the new *xmlns* property inside the *Window* open tag:

    ```xml
        <Window x:Class="MyProjectThatWouldUseFancyCandles.MainWindow"
        ...
        xmlns:fc="clr-namespace:FancyCandles;assembly=FancyCandles"
        ... >
    ```
1. Inside the body of the aforementioned XML document add the *FancyCandles.CandleChart* control element:

    ```xml
        <fc:CandleChart />
    ```
   As a result your MainWindow.xaml may looks like this:

    ```xml
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
   Congratulations! You have added the candlestick chart control (*CandleChart class*) to your application. But it is empty yet and contains no candles! The next step is to [populate it with candles](populating_candlestick_chart.md).
