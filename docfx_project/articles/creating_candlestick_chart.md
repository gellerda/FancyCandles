# Creating empty CandleChart

The [CandleChart class](https://gellerda.github.io/FancyCandles/api/FancyCandles.CandleChart.html), which derives from the [UserControl class](https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.usercontrol?view=netframework-4.8), provides you a simple way to create a candlestick chart control. [CandleChart](https://gellerda.github.io/FancyCandles/api/FancyCandles.CandleChart.html) is declared inside the [FancyCandles namespace](https://gellerda.github.io/FancyCandles/api/FancyCandles.html), which is within the *FancyCandles assembly*.

After the [FancyCandles project](https://github.com/gellerda/FancyCandles) has been added to your solution or the [FancyCandles NuGet package](https://www.nuget.org/packages/FancyCandles/) has been installed in your project, you can go on and create an instance of the [CandleChart control](https://gellerda.github.io/FancyCandles/api/FancyCandles.CandleChart.html) in your project:

1. In the root tag of the **MainWindow.xaml** file of your project, declare the [FancyCandles namespace](https://gellerda.github.io/FancyCandles/api/FancyCandles.html). In this example we map this namespace to the *fc:* prefix:

    ```xml
        <Window x:Class="MyProject.MainWindow"
        ...
        xmlns:fc="clr-namespace:FancyCandles;assembly=FancyCandles"
        ... >
    ```
1. Inside **MainWindow.xaml**, add the [CandleChart](https://gellerda.github.io/FancyCandles/api/FancyCandles.CandleChart.html) control element:

    ```xml
        <fc:CandleChart />
    ```
   As a result your **MainWindow.xaml** may looks like this:

    ```xml
       <Window x:Class="MyProject.MainWindow"
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

Congratulations! You have added the candlestick chart control to your application.<br><br>
![Manage NuGet Packages](../images/screen_empty_chart.png)<br><br>
   But it is empty yet and contains no candles! The next step is to [populate it with candles](populating_candlestick_chart.md).
