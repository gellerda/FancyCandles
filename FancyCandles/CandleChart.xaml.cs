/* 
    Copyright 2019 Dennis Geller.

    This file is part of FancyCandles.

    FancyCandles is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    FancyCandles is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with FancyCandles.  If not, see<https://www.gnu.org/licenses/>. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.ComponentModel;
using System.Runtime.CompilerServices; // [CallerMemberName]
using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using Newtonsoft.Json.Linq;
using Microsoft.Win32;
using FancyCandles.Indicators;

namespace FancyCandles
{
#pragma warning  disable CS1591
    internal struct CandleDrawingParameters
    {
        public double Width;
        public double Gap;
        public CandleDrawingParameters(double width, double gapBetweenCandles)
        {
            Width = width;
            Gap = gapBetweenCandles;
        }
    }
#pragma warning restore CS1591

    /// <summary>Represents the extreme values of Price and Volume for a set of candlesticks.</summary>
    public struct CandleExtremums
    {
        /// <summary>The Price minimum.</summary>
        /// <value>The Price minimum.</value>
        public double PriceLow;

        /// <summary>The Price maximum.</summary>
        /// <value>The Price maximum.</value>
        public double PriceHigh;

        /// <summary>The Volume minimum.</summary>
        /// <value>The Volume minimum.</value>
        public double VolumeLow;

        /// <summary>The Volume maximum.</summary>
        /// <value>The Volume maximum.</value>
        public double VolumeHigh;

        /// <summary>Initializes a new instance of the CandleExtremums structure that has the specified PriceLow, PriceHigh, VolumeLow, and VolumeHigh.</summary>
        /// <param name="priceLow">The PriceLow of the CandleExtremums.</param>
        /// <param name="priceHigh">The PriceHigh of the CandleExtremums.</param>
        /// <param name="volumeLow">The VolumeLow of the CandleExtremums.</param>
        /// <param name="volumeHigh">The VolumeHigh of the CandleExtremums.</param>
        public CandleExtremums(double priceLow, double priceHigh, double volumeLow, double volumeHigh)
        {
            PriceLow = priceLow;
            PriceHigh = priceHigh;
            VolumeLow = volumeLow;
            VolumeHigh = volumeHigh;
        }
#pragma warning  disable CS1591
        public override bool Equals(object obj) { return false; }
#pragma warning restore CS1591
    }

    /// <summary>Candlestick chart control derived from UserControl.</summary>
    [JsonObject(MemberSerialization.OptIn)]
    public partial class CandleChart : UserControl, INotifyPropertyChanged
    {
        //----------------------------------------------------------------------------------------------------------------------------------
#pragma warning disable CS1591
        public static readonly double ToolTipFontSize = 9.0;
#pragma warning restore CS1591

        //----------------------------------------------------------------------------------------------------------------------------------
        private void OnUserControlLoaded(object sender, RoutedEventArgs e)
        {
            ReCalc_MaxNumberOfCharsInPrice_and_MaxNumberOfFractionalDigitsInPrice();
            CandleGap = InitialCandleGap;
            CandleWidth = InitialCandleWidth;
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Default constructor.</summary>
        public CandleChart()
        {
            InitialCandleWidth = DefaultInitialCandleWidth;
            InitialCandleGap = DefaultInitialCandleGap;

            InitializeComponent();

            priceChartContextMenu.DataContext = this;
            volumeHistogramContextMenu.DataContext = this;

            VisibleCandlesRange = IntRange.Undefined;
            VisibleCandlesExtremums = new CandleExtremums(0.0, 0.0, 0L, 0L);
            Loaded += new RoutedEventHandler(OnUserControlLoaded);
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        private void OpenCandleChartPropertiesWindow(object sender, RoutedEventArgs e)
        {
            string overlayIndicatorArrayJson = SerializeToJson(OverlayIndicators);
            RecordAllUndoableProperties();

            CandleChartPropertiesWindow popup = new CandleChartPropertiesWindow(this);
            if (popup.ShowDialog() == true)
            {
                ClearUndoablePropertyRecords();
            }
            else
            {
                ObservableCollection<OverlayIndicator> new_overlayIndicators = DeserializeOverlayIndicatorsFromJson(overlayIndicatorArrayJson);
                OverlayIndicators = new_overlayIndicators;
                UndoRecordedUndoableProperties();
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        private FancyPrimitives.RelayCommand openSelectCandleSourceWindowCommand;
        /// <summary>Gets the Command that opens the dialog window for selecting a new financial instrument.</summary>
        ///<value>The Command that opens the dialog window for selecting a new financial instrument.</value>
        public FancyPrimitives.RelayCommand OpenSelectCandleSourceWindowCommand
        {
            get
            {
                return openSelectCandleSourceWindowCommand ??
                  (openSelectCandleSourceWindowCommand = new FancyPrimitives.RelayCommand(nothing =>
                  {
                      SelectCandleSourceWindow popup = new SelectCandleSourceWindow(this);
                      if (popup.ShowDialog() == true)
                      {
                      }
                      else
                      {
                      }
                  },
                  nothing =>
                  {
                      return CandlesSourceProvider != null && CandlesSourceProvider.SecCatalog.Count > 0;
                  }));
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the specific culture that will be used to draw price, volume, date and time values.</summary>
        ///<value>The specific culture that will be used to draw price, volume, date and time values. The default is <see href="https://docs.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.currentculture?view=netframework-4.7.2">CultureInfo.CurrentCulture</see>.</value>
        public CultureInfo Culture
        {
            get { return (CultureInfo)GetValue(CultureProperty); }
            set { SetValue(CultureProperty, value); }
        }
        /// <summary>Identifies the <see cref="Culture"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty CultureProperty =
            DependencyProperty.Register("Culture", typeof(CultureInfo), typeof(CandleChart), new PropertyMetadata(CultureInfo.CurrentCulture));
        //----------------------------------------------------------------------------------------------------------------------------------
        private double currentPrice;
        /// <summary>Gets the current price value - the closing price of the last candle.</summary>
        ///<value>The current price value - the closing price of the last candle.</value>
        ///<seealso cref = "IsCurrentPriceLabelVisible">IsCurrentPriceLabelVisible</seealso>
        public double CurrentPrice
        {
            get { return currentPrice; }
            private set
            {
                if (value == currentPrice) return;
                currentPrice = value;
                OnPropertyChanged();
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets a value indicating whether the current price is shown on the price scale.</summary>
        ///<value>A value indicating whether the current price is shown on the price scale. The default value is <c>true</c>.</value>
        ///<remarks>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="IsCurrentPriceLabelVisibleProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        ///<seealso cref = "CurrentPrice">CurrentPrice</seealso>
        [UndoableProperty]
        [JsonProperty]
        public bool IsCurrentPriceLabelVisible
        {
            get { return (bool)GetValue(IsCurrentPriceLabelVisibleProperty); }
            set { SetValue(IsCurrentPriceLabelVisibleProperty, value); }
        }
        /// <summary>Identifies the <see cref="IsCurrentPriceLabelVisible"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty IsCurrentPriceLabelVisibleProperty =
            DependencyProperty.Register("IsCurrentPriceLabelVisible", typeof(bool), typeof(CandleChart), new PropertyMetadata(true));
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the color of the wait indicator.</summary>
        ///<value>The color of the wait indicator. The default is determined by the <see cref="DefaultWaitIndicatorForeground"/> value.</value>
        ///<remarks>
        ///The wait indicator is located in the center of the price chart area. It becomes visible when a candle data is loading.
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="WaitIndicatorForegroundProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        [UndoableProperty]
        [JsonProperty]
        public Brush WaitIndicatorForeground
        {
            get { return (Brush)GetValue(WaitIndicatorForegroundProperty); }
            set { SetValue(WaitIndicatorForegroundProperty, value); }
        }
        ///<summary>Identifies the <see cref="WaitIndicatorForeground"/> dependency property.</summary>
        ///<value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty WaitIndicatorForegroundProperty =
            DependencyProperty.Register("WaitIndicatorForeground", typeof(Brush), typeof(CandleChart), new PropertyMetadata(DefaultWaitIndicatorForeground));

        ///<summary>Gets the default value for the WaitIndicatorForeground property.</summary>
        ///<value>The default value for the <see cref="WaitIndicatorForeground"/> property: <c>Brushes.DarkGray</c>.</value>
        public static Brush DefaultWaitIndicatorForeground { get { return (Brush)Brushes.DarkGray.GetCurrentValueAsFrozen(); } }
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the background of the price chart and volume diagram areas.</summary>
        ///<value>The background of the price chart and volume diagram areas. The default is determined by the <see cref="DefaultChartAreaBackground"/> values.</value>
        ///<remarks>
        ///This background is not applied to the horizontal and vertical axis areas, which contain tick marks and labels.
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="ChartAreaBackgroundProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        [UndoableProperty]
        [JsonProperty]
        public Brush ChartAreaBackground
        {
            get { return (Brush)GetValue(ChartAreaBackgroundProperty); }
            set { SetValue(ChartAreaBackgroundProperty, value); }
        }
        /// <summary>Identifies the <see cref="ChartAreaBackground"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty ChartAreaBackgroundProperty =
            DependencyProperty.Register("ChartAreaBackground", typeof(Brush), typeof(CandleChart), new PropertyMetadata(DefaultChartAreaBackground));

        ///<summary>Gets the default value for the ChartAreaBackground property.</summary>
        ///<value>The default value for the <see cref="ChartAreaBackground"/> property: <c>#FFFFFDE9</c>.</value>
        public static Brush DefaultChartAreaBackground { get { return (Brush)Brushes.Cornsilk.GetCurrentValueAsFrozen(); } }
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the border color of the price chart and volume diagram areas.</summary>
        ///<value>The border color of the price chart and volume diagram areas. The default is determined by the <see cref="DefaultChartAreaBorderColor"/> values.</value>
        ///<remarks>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="ChartAreaBorderColorProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        [UndoableProperty]
        [JsonProperty]
        public Brush ChartAreaBorderColor
        {
            get { return (Brush)GetValue(ChartAreaBorderColorProperty); }
            set { SetValue(ChartAreaBorderColorProperty, value); }
        }
        /// <summary>Identifies the <see cref="ChartAreaBorderColor"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty ChartAreaBorderColorProperty =
            DependencyProperty.Register("ChartAreaBorderColor", typeof(Brush), typeof(CandleChart), new PropertyMetadata(DefaultChartAreaBorderColor));

        ///<summary>Gets the default value for the ChartAreaBorderColor property.</summary>
        ///<value>The default value for the <see cref="ChartAreaBorderColor"/> property: <c>Black</c>.</value>
        public static Brush DefaultChartAreaBorderColor { get { return (Brush)Brushes.Black.GetCurrentValueAsFrozen(); } }
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the fill brush for the rectangle, that covers this chart control if it has been disabled.</summary>
        ///<value>The fill brush for the rectangle, that covers this chart control if it has been disabled. The default is determined by the <see cref="DefaultDisabledFill"/>values.</value>
        ///<remarks>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="DisabledFillProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        [UndoableProperty]
        [JsonProperty]
        public Brush DisabledFill
        {
            get { return (Brush)GetValue(DisabledFillProperty); }
            set { SetValue(DisabledFillProperty, value); }
        }
        /// <summary>Identifies the <see cref="DisabledFill"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty DisabledFillProperty =
            DependencyProperty.Register("DisabledFill", typeof(Brush), typeof(CandleChart), new PropertyMetadata(DefaultDisabledFill));

        ///<summary>Gets the default value for the DisabledFill property.</summary>
        ///<value>The default value for the <see cref="DisabledFill"/> property: <c>#CCAAAAAA</c>.</value>
        public static Brush DefaultDisabledFill { get { return (Brush)(new SolidColorBrush(Color.FromArgb(204, 170, 170, 170))).GetCurrentValueAsFrozen(); } } // #CCAAAAAA
        //----------------------------------------------------------------------------------------------------------------------------------
        #region SERIALIZE AND DESEREALIZE FROM JSON ******************************************************************************************************************

        private class CandleChartContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

                if (type == typeof(Pen))
                    properties = properties.Where(p => (p.PropertyName == "Thickness" || p.PropertyName == "Brush")).ToList();

                return properties;
            }
        }

        private string SerializeToJson(object objToSerialize)
        {
            return JsonConvert.SerializeObject(objToSerialize, Formatting.Indented,
                                               new JsonSerializerSettings { ContractResolver = new CandleChartContractResolver() });
        }

        private ObservableCollection<OverlayIndicator> DeserializeOverlayIndicatorsFromJson(string overlayIndicatorsJsonString)
        {
            JArray indicators = JArray.Parse(overlayIndicatorsJsonString);
            return DeserializeOverlayIndicatorsFromJson(indicators);
        }

        private ObservableCollection<OverlayIndicator> DeserializeOverlayIndicatorsFromJson(JArray overlayIndicatorsJArray)
        {
            MethodInfo getMethodInfo = typeof(JToken).GetMethod(nameof(JToken.ToObject), Type.EmptyTypes);
            ObservableCollection<OverlayIndicator> new_overlayIndicators = new ObservableCollection<OverlayIndicator>();
            for (int i = 0; i < overlayIndicatorsJArray.Count; i++)
            {
                Type overlayIndicatorType = FindOverlayIndicatorType(overlayIndicatorsJArray[i]["TypeName"].ToString());
                MethodInfo getMethodTInfo = getMethodInfo.MakeGenericMethod(overlayIndicatorType);
                OverlayIndicator overlayIndicator = (OverlayIndicator)getMethodTInfo.Invoke(overlayIndicatorsJArray[i], null);
                new_overlayIndicators.Add(overlayIndicator);
            }

            return new_overlayIndicators;
        }

        private void RestoreFromJson(string candleChartJson)
        {
            JObject candleChartJToken = JObject.Parse(candleChartJson);
            JArray overlayIndicatorsJArray = (JArray)candleChartJToken["OverlayIndicators"];
            OverlayIndicators = DeserializeOverlayIndicatorsFromJson(overlayIndicatorsJArray);
            candleChartJToken.Remove("OverlayIndicators");

            MethodInfo getMethodInfo = typeof(JToken).GetMethod(nameof(JToken.ToObject), Type.EmptyTypes);
            foreach (JProperty property in candleChartJToken.Properties())
            {
                PropertyInfo propertyInfo = typeof(CandleChart).GetProperty(property.Name);
                Type propertyType = propertyInfo.PropertyType;
                MethodInfo getMethodTInfo = getMethodInfo.MakeGenericMethod(propertyType);
                object propertyValue = getMethodTInfo.Invoke(property.Value, null);
                FancyPrimitives.MyUtility.SetProperty(this, property.Name, propertyValue, out _);
            }
        }

        private Type FindOverlayIndicatorType(string overlayIndicatorTypeName)
        {
            Type overlayIndicatorType = Type.GetType(overlayIndicatorTypeName);
            if (overlayIndicatorType != null)
                return overlayIndicatorType;

            overlayIndicatorType = Assembly.GetEntryAssembly().GetType(overlayIndicatorTypeName);
            if (overlayIndicatorType != null)
                return overlayIndicatorType;

            string fullAddInIndicatorsFolder = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, AddInIndicatorsFolder);
            var allowedExtensions = new[] { ".dll", ".exe" };
            List<string> addInAssemblyFileNames;
            try
            {
                addInAssemblyFileNames = Directory.GetFiles(fullAddInIndicatorsFolder).Where(file => allowedExtensions.Any(file.ToLower().EndsWith)).ToList();
            }
            catch
            {
                addInAssemblyFileNames = new List<string>();
            }

            foreach (string addInAssemblyFileName in addInAssemblyFileNames)
            {
                string asmPath = System.IO.Path.Combine(fullAddInIndicatorsFolder, addInAssemblyFileName);
                Assembly asm = Assembly.LoadFile(asmPath);
                overlayIndicatorType = asm.GetType(overlayIndicatorTypeName);
                if (overlayIndicatorType != null)
                    return overlayIndicatorType;
            }

            return null;
        }

        #endregion **********************************************************************************************************************************************
        //----------------------------------------------------------------------------------------------------------------------------------
        #region LOAD AND SAVE SETTINGS *******************************************************************************************************************************

        private void OpenSaveSettingsAsDialog(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Chart Settings (*.chs)|*.chs";
            //openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (saveFileDialog.ShowDialog() == true)
                SaveSettingsAs(saveFileDialog.FileName);
        }

        private void OpenLoadSettingsDialog(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Chart Settings (*.chs)|*.chs";
            //openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == true)
                LoadSettings(openFileDialog.FileName);
        }

        ///<summary>Loads and sets chart settings from the specified file.</summary>
        ///<param name="pathToSettingsFile">The Path to the file containing chart settings.</param>
        ///<remarks></remarks>
        ///<seealso cref = "SaveSettingsAs">SaveSettingsAs()</seealso>
        public void LoadSettings(string pathToSettingsFile)
        {
            string jsonCandleChart = File.ReadAllText(pathToSettingsFile);
            RestoreFromJson(jsonCandleChart);
        }

        ///<summary>Saves current chart settings to the specified file.</summary>
        ///<param name="pathToSettingsFile">The Path to the file to save settings to.</param>
        ///<remarks></remarks>
        ///<seealso cref = "LoadSettings">LoadSettings()</seealso>
        public void SaveSettingsAs(string pathToSettingsFile)
        {
            string jsonCandleChart = SerializeToJson(this);
            File.WriteAllText(pathToSettingsFile, jsonCandleChart);
        }

        #endregion **********************************************************************************************************************************************
        //----------------------------------------------------------------------------------------------------------------------------------
        #region OVERLAY INDICATORS *******************************************************************************************************************************
        /// <summary>Gets or sets the collection of technical overlay indicators attached to the price chart.</summary>
        ///<value>The collection of technical overlay indicators attached to the price chart. The default is empty collection.</value>
        ///<remarks>
        ///This collection contains technical overlay indicators shown on the price chart area. Overlay indicators have the same unit of measure as the price has.
        ///Therefore such indicator charts can be drawn on the same panel as the price chart.
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="OverlayIndicatorsProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        ///<seealso href="https://gellerda.github.io/FancyCandles/articles/creating_overlay_indicator.html">Creating your own overlay technical indicator</seealso>
        ///<seealso cref="AddInIndicatorsFolder"/>
        [JsonProperty]
        public ObservableCollection<OverlayIndicator> OverlayIndicators
        {
            get { return (ObservableCollection<OverlayIndicator>)GetValue(OverlayIndicatorsProperty); }
            set { SetValue(OverlayIndicatorsProperty, value); }
        }
        /// <summary>Identifies the <see cref="OverlayIndicators"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty OverlayIndicatorsProperty =
            DependencyProperty.Register("OverlayIndicators", typeof(ObservableCollection<OverlayIndicator>), typeof(CandleChart),
                new UIPropertyMetadata(new ObservableCollection<OverlayIndicator>(), OnOverlayIndicatorsChanged));

        private static void OnOverlayIndicatorsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            CandleChart thisCandleChart = obj as CandleChart;
            if (thisCandleChart == null) return;

            thisCandleChart.SetCandlesSourceForAll_OverlayIndicators();
        }

        private void SetCandlesSourceForAll_OverlayIndicators()
        {
            if (OverlayIndicators == null) return;

            for (int i = 0; i < OverlayIndicators.Count; i++)
                OverlayIndicators[i].CandlesSource = CandlesSource;
        }

        private FancyPrimitives.RelayCommand removeOverlayIndicatorCommand;
        /// <summary>Gets the Command for removing an indicator object from the OverlayIndicators collection.</summary>
        ///<value>The RelayCommand object representing the command for removing an indicator object from the OverlayIndicators collection..</value>
        ///<remarks>
        ///The Command parameter must contain the index of the collection element to be removed from the OverlayIndicators collection.
        ///</remarks>
        public FancyPrimitives.RelayCommand RemoveOverlayIndicatorCommand
        {
            get
            {
                return removeOverlayIndicatorCommand ??
                  (removeOverlayIndicatorCommand = new FancyPrimitives.RelayCommand(overlayIndicator_i =>
                  {
                      int i = (int)overlayIndicator_i;
                      if (i >= 0 && i < OverlayIndicators.Count)
                          OverlayIndicators.RemoveAt(i);
                  }));
            }
        }

        private FancyPrimitives.RelayCommand moveOverlayIndicatorLeftCommand;
        /// <summary>Gets the Command for moving backward one step an indicator in the OverlayIndicators collection.</summary>
        ///<value>The Command for moving backward one step an indicator in the OverlayIndicators collection.</value>
        ///<remarks>
        ///MoveOverlayIndicatorLeftCommand command moves the specified indicator one step in direction to the beginning of the OverlayIndicators collection. 
        ///The Command parameter must contain the index of the collection element to be moved.
        ///</remarks>
        ///<seealso cref = "MoveOverlayIndicatorRightCommand">MoveOverlayIndicatorRightCommand</seealso>
        public FancyPrimitives.RelayCommand MoveOverlayIndicatorLeftCommand
        {
            get
            {
                return moveOverlayIndicatorLeftCommand ??
                  (moveOverlayIndicatorLeftCommand = new FancyPrimitives.RelayCommand(overlayIndicator_i =>
                  {
                      int old_i = (int)overlayIndicator_i;
                      if (old_i < 1) return;
                      OverlayIndicators.Move(old_i, old_i - 1);
                  }));
            }
        }

        private FancyPrimitives.RelayCommand moveOverlayIndicatorRightCommand;
        /// <summary>Gets the Command for moving forward one step an indicator in the OverlayIndicators collection.</summary>
        ///<value>The Command for moving forward one step an indicator in the OverlayIndicators collection.</value>
        ///<remarks>
        ///MoveOverlayIndicatorRightCommand command moves the specified indicator one step in direction to the end of the OverlayIndicators collection. 
        ///The Command parameter must contain the index of the collection element to be moved.
        ///</remarks>
        ///<seealso cref = "MoveOverlayIndicatorLeftCommand">MoveOverlayIndicatorLeftCommand</seealso>
        public FancyPrimitives.RelayCommand MoveOverlayIndicatorRightCommand
        {
            get
            {
                return moveOverlayIndicatorRightCommand ??
                  (moveOverlayIndicatorRightCommand = new FancyPrimitives.RelayCommand(overlayIndicator_i =>
                  {
                      int old_i = (int)overlayIndicator_i;
                      if (old_i == (OverlayIndicators.Count - 1)) return;
                      OverlayIndicators.Move(old_i, old_i + 1);
                  }));
            }
        }

        private FancyPrimitives.RelayCommand addOverlayIndicatorCommand;
        /// <summary>Gets the Command for adding new indicator object to the OverlayIndicators collection.</summary>
        ///<value>The Command for adding new indicator object to the OverlayIndicators collection.</value>
        ///<remarks>
        ///AddOverlayIndicatorCommand creates a new indicator object of specified in the Command parameter type and adds it to the OverlayIndicator collection.
        ///The Command parameter must contain the <see href="https://docs.microsoft.com/ru-ru/dotnet/api/system.type?view=netframework-4.7.2">Type</see> of overlay indicator to be added.
        ///It must be the type of class derived from <see cref="OverlayIndicator"/>.
        ///The new indicator is created with default parameters. You have to tune up its parameters after adding it to OverlayIndicators.
        ///</remarks>
        public FancyPrimitives.RelayCommand AddOverlayIndicatorCommand
        {
            get
            {
                return addOverlayIndicatorCommand ??
                  (addOverlayIndicatorCommand = new FancyPrimitives.RelayCommand(parameter_overlayIndicatorType =>
                  {
                      Type overlayIndicatorType = (Type)parameter_overlayIndicatorType;
                      OverlayIndicator overlayIndicator = (OverlayIndicator)Activator.CreateInstance(overlayIndicatorType);
                      OverlayIndicators.Add(overlayIndicator);
                      OverlayIndicator addedOverlayIndicator = OverlayIndicators[OverlayIndicators.Count - 1];
                      addedOverlayIndicator.CandlesSource = CandlesSource;
                  }));
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        ///<summary>Gets or sets the folder with an assemblies containing user's add-in technical indicators.</summary>
        ///<value>The path to the folder containing user's add-in technical indicators. The default value is empty string.</value>
        ///<remarks>
        ///<para>Adding your own technical indicator classes, derived from <see cref="OverlayIndicator"/>, to Startup project of your solution 
        ///is not the only way to add new indicators to your application.</para>
        ///<para>You or even users of your application can add a new add-in indicator by creating it in a separate solution. You have to do the following:</para>
        ///<list type="bullet">
        ///<item><term>Add a new indicator class derived from <see cref="OverlayIndicator"/> in a new project inside a new solution and build an assembly.</term></item>
        ///<item><term>Locate the assembly file containing the new indicator class in some folder, usually below your main application root directory.</term></item>
        ///<item><term>Specify the aforementioned folder path in the <see cref="AddInIndicatorsFolder"/> of your application.</term></item>
        ///</list>
        ///The path can be full or relative to the <see href="https://docs.microsoft.com/en-us/dotnet/api/system.appdomain.basedirectory?view=netframework-4.7.2">BaseDirectory</see> of your application.
        ///<para>For example, it could look like this:
        ///<code>&lt;fc:CandleChart AddInIndicatorsAssemblyPath="AddInIndicators"/&gt;</code>
        ///In the example above, folder "AddInIndicators" must be located inside the base folder of your application. There can be multiple assembly files in this folder. 
        ///All of them will be found by the <see cref="CandleChart"/> element.</para>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="AddInIndicatorsFolderProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        ///<seealso href="https://gellerda.github.io/FancyCandles/articles/creating_overlay_indicator.html">Creating your own overlay technical indicator</seealso>
        ///<seealso cref="OverlayIndicators"/>
        public string AddInIndicatorsFolder
        {
            get { return (string)GetValue(AddInIndicatorsFolderProperty); }
            set { SetValue(AddInIndicatorsFolderProperty, value); }
        }

        /// <summary>Identifies the <see cref="AddInIndicatorsFolder"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty AddInIndicatorsFolderProperty =
            DependencyProperty.Register("AddInIndicatorsFolder", typeof(string), typeof(CandleChart), new PropertyMetadata(""));
        //----------------------------------------------------------------------------------------------------------------------------------

        #endregion **********************************************************************************************************************************************
        //----------------------------------------------------------------------------------------------------------------------------------
        #region UNDO FUNCTIONALITY *******************************************************************************************************************************
        private Dictionary<string, Object> undoablePropertyRecords = new Dictionary<string, object>(); // <PropertyName, PropertyValue>
        //----------------------------------------------------------------------------------------------------------------------------------
        private void RecordAllUndoableProperties()
        {
            ClearUndoablePropertyRecords();

            PropertyInfo[] props = GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                IEnumerable<UndoablePropertyAttribute> attributes = prop.GetCustomAttributes(typeof(UndoablePropertyAttribute), true).Cast<UndoablePropertyAttribute>();
                if (attributes.Count() > 0)
                    undoablePropertyRecords.Add(prop.Name, prop.GetValue(this));
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        private void UndoRecordedUndoableProperties()
        {
            foreach (KeyValuePair<string, Object> keyValue in undoablePropertyRecords)
                FancyPrimitives.MyUtility.SetProperty(this, keyValue.Key, keyValue.Value, out _);

            ClearUndoablePropertyRecords();
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        private void ClearUndoablePropertyRecords()
        {
            undoablePropertyRecords.Clear();
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        #endregion **********************************************************************************************************************************************
        //----------------------------------------------------------------------------------------------------------------------------------
        #region LEGEND PROPERTIES *******************************************************************************************************************************

        private readonly static string loadingLegendText = "Loading...";

        private string CreateLegendText(ISecurityInfo secInfo)
        {
            return $"{secInfo.Ticker}, {CandlesSource.TimeFrame}";
        } 

        ///<summary>Gets or sets the text of the legend.</summary>
        ///<value>The text of the legend. The default is determined by the <see cref="DefaultLegendText"/> value.</value>
        ///<remarks>
        ///The legend could contain any text, describing this chart. Usually it contains a ticker symbol (a name of the security) and a timeframe, for example: <em>"AAPL"</em>, <em>"GOOGL, M5"</em>, <em>"BTC/USD, D"</em> etc.
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="LegendTextProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        ///<seealso cref = "DefaultLegendText">DefaultLegendText</seealso>
        [UndoableProperty]
        [JsonProperty]
        public string LegendText
        {
            get { return (string)GetValue(LegendTextProperty); }
            set { SetValue(LegendTextProperty, value); }
        }
        /// <summary>Identifies the <see cref="LegendText"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty LegendTextProperty =
            DependencyProperty.Register("LegendText", typeof(string), typeof(CandleChart), new PropertyMetadata(DefaultLegendText));

        ///<summary>Gets the default value for the LegendText property.</summary>
        ///<value>The default value for the LegendText property: <c>"DefaultLegend"</c>.</value>
        ///<seealso cref = "LegendText">LegendText</seealso>
        public static string DefaultLegendText { get { return "DefaultLegend"; } }
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the font family of the legend.</summary>
        ///<value>The font family of the legend. The default value is equal to the default value of the <see cref="TextBlock.FontFamilyProperty">TextBlock.FontFamilyProperty</see>.</value>
        ///<remarks>
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="LegendFontFamilyProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        [UndoableProperty]
        [JsonProperty]
        public FontFamily LegendFontFamily
        {
            get { return (FontFamily)GetValue(LegendFontFamilyProperty); }
            set { SetValue(LegendFontFamilyProperty, value); }
        }
        /// <summary>Identifies the <see cref="LegendFontFamily"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty LegendFontFamilyProperty =
            DependencyProperty.Register("LegendFontFamily", typeof(FontFamily), typeof(CandleChart), new PropertyMetadata(TextBlock.FontFamilyProperty.DefaultMetadata.DefaultValue));
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the font size of the legend. The legend locates inside the price chart area.</summary>
        ///<value>The font size of the legend. The default is determined by the <see cref="DefaultLegendFontSize"/> value.</value>
        ///<remarks>
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="LegendFontSizeProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        ///<seealso cref = "DefaultLegendFontSize">DefaultLegendFontSize</seealso>
        [UndoableProperty]
        [JsonProperty]
        public double LegendFontSize
        {
            get { return (double)GetValue(LegendFontSizeProperty); }
            set { SetValue(LegendFontSizeProperty, value); }
        }
        /// <summary>Identifies the <see cref="LegendFontSize">LegendFontSize</see> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty LegendFontSizeProperty =
            DependencyProperty.Register("LegendFontSize", typeof(double), typeof(CandleChart), new PropertyMetadata(DefaultLegendFontSize));

        ///<summary>Gets the default value for the LegendFontSize property.</summary>
        ///<value>The default value for the LegendFontSize property: <c>30.0</c>.</value>
        ///<seealso cref = "LegendFontSize">LegendFontSize</seealso>
        public static double DefaultLegendFontSize { get { return 30.0; } }
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the font style of the legend.</summary>
        ///<value>The font style of the legend. The default is determined by the <see cref="DefaultLegendFontStyle"/> value.</value>
        ///<remarks>
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="LegendFontStyleProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        ///<seealso cref = "DefaultLegendFontStyle">DefaultLegendFontStyle</seealso>
        [UndoableProperty]
        [JsonProperty]
        public FontStyle LegendFontStyle
        {
            get { return (FontStyle)GetValue(LegendFontStyleProperty); }
            set { SetValue(LegendFontStyleProperty, value); }
        }
        /// <summary>Identifies the <see cref="LegendFontStyle"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty LegendFontStyleProperty =
            DependencyProperty.Register("LegendFontStyle", typeof(FontStyle), typeof(CandleChart), new PropertyMetadata(DefaultLegendFontStyle));

        ///<summary>Gets the default value for the LegendFontStyle property.</summary>
        ///<value>The default value for the LegendFontStyle property: <c>FontStyles.Normal</c>.</value>
        ///<seealso cref = "LegendFontStyle">LegendFontStyle</seealso>
        public static FontStyle DefaultLegendFontStyle { get { return FontStyles.Normal; } }
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the font weight of the legend. The legend locates inside the price chart area.</summary>
        /// <value>The font weight of the legend. The default is determined by the <see cref="DefaultLegendFontWeight"/> value.</value>
        ///<remarks>
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="LegendFontWeightProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        /// <seealso cref = "DefaultLegendFontWeight">DefaultLegendFontWeight</seealso>
        [UndoableProperty]
        [JsonProperty]
        public FontWeight LegendFontWeight
        {
            get { return (FontWeight)GetValue(LegendFontWeightProperty); }
            set { SetValue(LegendFontWeightProperty, value); }
        }
        /// <summary>Identifies the <see cref="LegendFontWeight"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty LegendFontWeightProperty =
            DependencyProperty.Register("LegendFontWeight", typeof(FontWeight), typeof(CandleChart), new PropertyMetadata(DefaultLegendFontWeight));

        ///<summary>Gets the default value for the LegendFontWeight property.</summary>
        ///<value>The default value for the LegendFontWeight property: <c>FontWeights.Bold</c>.</value>
        ///<seealso cref = "LegendFontWeight">LegendFontWeight</seealso>
        public static FontWeight DefaultLegendFontWeight { get { return FontWeights.Bold; } }
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the foreground of the legend. The legend locates inside the price chart area.</summary>
        /// <value>The foreground of the legend. The default is determined by the <see cref="DefaultLegendForeground"/> value.</value>
        ///<remarks>
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="LegendForegroundProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        /// <seealso cref = "DefaultLegendForeground">DefaultLegendForeground</seealso>
        [UndoableProperty]
        [JsonProperty]
        public Brush LegendForeground
        {
            get { return (Brush)GetValue(LegendForegroundProperty); }
            set { SetValue(LegendForegroundProperty, value); }
        }
        /// <summary>Identifies the <see cref="LegendForeground"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty LegendForegroundProperty =
            DependencyProperty.Register("LegendForeground", typeof(Brush), typeof(CandleChart), new PropertyMetadata(DefaultLegendForeground));

        ///<summary>Gets the default value for the LegendForeground property.</summary>
        ///<value>The default value for the LegendForeground property: <c>#3C000000</c>.</value>
        ///<seealso cref = "LegendForeground">LegendForeground</seealso>
        public static Brush DefaultLegendForeground { get { return (Brush)(new SolidColorBrush(Color.FromArgb(60, 0, 0, 0))).GetCurrentValueAsFrozen(); } } // #3C000000
                                                                                                                                                            //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the horizontal alignment for the legend inside the price chart area.</summary>
        /// <value>The horizontal alignment of the legend. The default is determined by the <see cref="DefaultLegendHorizontalAlignment"/> value.</value>
        ///<remarks>
        ///The legend locates inside the price chart area and could be horizontally and vertically aligned.
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="LegendHorizontalAlignmentProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        /// <seealso cref = "DefaultLegendHorizontalAlignment">DefaultLegendHorizontalAlignment</seealso>
        /// <seealso cref = "LegendVerticalAlignment">LegendVerticalAlignment</seealso>
        [UndoableProperty]
        [JsonProperty]
        public HorizontalAlignment LegendHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(LegendHorizontalAlignmentProperty); }
            set { SetValue(LegendHorizontalAlignmentProperty, value); }
        }
        /// <summary>Identifies the <see cref="LegendHorizontalAlignment"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty LegendHorizontalAlignmentProperty =
            DependencyProperty.Register("LegendHorizontalAlignment", typeof(HorizontalAlignment), typeof(CandleChart), new PropertyMetadata(DefaultLegendHorizontalAlignment));

        ///<summary>Gets the default value for the LegendHorizontalAlignment property.</summary>
        ///<value>The default value for the LegendHorizontalAlignment property: <c>HorizontalAlignment.Left</c>.</value>
        ///<seealso cref = "LegendHorizontalAlignment">LegendHorizontalAlignment</seealso>
        public static HorizontalAlignment DefaultLegendHorizontalAlignment { get { return HorizontalAlignment.Left; } }
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the vertical alignment for the legend inside the price chart area.</summary>
        /// <value>The vertical alignment of the legend. The default is determined by the <see cref="DefaultLegendVerticalAlignment"/> value.</value>
        ///<remarks>
        ///The legend locates inside the price chart area and could be horizontally and vertically aligned.
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="LegendVerticalAlignmentProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        /// <seealso cref = "DefaultLegendVerticalAlignment">DefaultLegendVerticalAlignment</seealso>
        /// <seealso cref = "LegendHorizontalAlignment">LegendHorizontalAlignment</seealso>
        [UndoableProperty]
        [JsonProperty]
        public VerticalAlignment LegendVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(LegendVerticalAlignmentProperty); }
            set { SetValue(LegendVerticalAlignmentProperty, value); }
        }
        /// <summary>Identifies the <see cref="LegendVerticalAlignment"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty LegendVerticalAlignmentProperty =
            DependencyProperty.Register("LegendVerticalAlignment", typeof(VerticalAlignment), typeof(CandleChart), new PropertyMetadata(DefaultLegendVerticalAlignment));

        ///<summary>Gets the default value for the LegendVerticalAlignment property.</summary>
        ///<value>The default value for the LegendVerticalAlignment property: <c>VerticalAlignment.Bottom</c>.</value>
        ///<seealso cref = "LegendVerticalAlignment">LegendVerticalAlignment</seealso>
        public static VerticalAlignment DefaultLegendVerticalAlignment { get { return VerticalAlignment.Bottom; } }
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the margins for the legend. The legend locates inside the price chart area.</summary>
        /// <value>The legend margin. The default is determined by the <see cref="DefaultLegendMargin"/> value.</value>
        ///<remarks>
        ///The legend locates inside the price chart area and could be horizontally and vertically aligned. It could contain any text, describing this chart. Usually it contains a ticker symbol (a name of the security) and a timeframe, for example: <em>"AAPL"</em>, <em>"GOOGL, M5"</em>, <em>"BTC/USD, D"</em> etc.
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="LegendMarginProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        /// <seealso cref = "DefaultLegendMargin">DefaultLegendMargin</seealso>
        [UndoableProperty]
        [JsonProperty]
        public Thickness LegendMargin
        {
            get { return (Thickness)GetValue(LegendMarginProperty); }
            set { SetValue(LegendMarginProperty, value); }
        }
        /// <summary>Identifies the <see cref="LegendMargin"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty LegendMarginProperty =
            DependencyProperty.Register("LegendMargin", typeof(Thickness), typeof(CandleChart), new PropertyMetadata(DefaultLegendMargin));

        ///<summary>Gets the default value for the LegendMargin property.</summary>
        ///<value>The default value for the LegendMargin property: <c>(10, 0, 10, 0)</c>.</value>
        ///<seealso cref = "LegendMargin">LegendMargin</seealso>
        public static Thickness DefaultLegendMargin { get { return new Thickness(10, 0, 10, 0); } }

        #endregion **********************************************************************************************************************************************
        //----------------------------------------------------------------------------------------------------------------------------------
        #region PRICE CHART PROPERTIES **************************************************************************************************************************

        /// <summary>Gets or sets the top margin for the price chart.</summary>
        ///<value>The top margin of the price chart, in device-independent units. The default is determined by the <see cref="DefaultPriceChartTopMargin"/> value.</value>
        ///<remarks> 
        ///You can set up top and bottom margins for the price chart inside its area by setting the <see cref="PriceChartTopMargin"/> and <see cref="PriceChartBottomMargin"/> properties respectively.
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="PriceChartTopMarginProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        ///<seealso cref = "DefaultPriceChartTopMargin">DefaultPriceChartTopMargin</seealso>
        ///<seealso cref = "PriceChartBottomMargin">PriceChartBottomMargin</seealso>
        [UndoableProperty]
        [JsonProperty]
        public double PriceChartTopMargin
        {
            get { return (double)GetValue(PriceChartTopMarginProperty); }
            set { SetValue(PriceChartTopMarginProperty, value); }
        }
        /// <summary>Identifies the <see cref="PriceChartTopMargin"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty PriceChartTopMarginProperty =
            DependencyProperty.Register("PriceChartTopMargin", typeof(double), typeof(CandleChart), new PropertyMetadata(DefaultPriceChartTopMargin));

        ///<summary>Gets the default value for the PriceChartTopMargin property.</summary>
        ///<value>The default value for the PriceChartTopMargin property: <c>15.0</c>.</value>
        ///<seealso cref = "PriceChartTopMargin">PriceChartTopMargin</seealso>
        public static double DefaultPriceChartTopMargin { get { return 15.0; } }
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the bottom margin for the price chart.</summary>
        ///<value>The bottom margin of the price chart, in device-independent units. The default is determined by the <see cref="DefaultPriceChartBottomMargin"/> value.</value>
        ///<remarks> 
        ///You can set up top and bottom margins for the price chart inside its area by setting the <see cref="PriceChartTopMargin"/> and <see cref="PriceChartBottomMargin"/> properties respectively.
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="PriceChartBottomMarginProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        ///<seealso cref = "DefaultPriceChartBottomMargin">DefaultPriceChartBottomMargin</seealso>
        ///<seealso cref = "PriceChartTopMargin">PriceChartTopMargin</seealso>
        [UndoableProperty]
        [JsonProperty]
        public double PriceChartBottomMargin
        {
            get { return (double)GetValue(PriceChartBottomMarginProperty); }
            set { SetValue(PriceChartBottomMarginProperty, value); }
        }
        /// <summary>Identifies the <see cref="PriceChartBottomMargin"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty PriceChartBottomMarginProperty =
            DependencyProperty.Register("PriceChartBottomMargin", typeof(double), typeof(CandleChart), new PropertyMetadata(DefaultPriceChartBottomMargin));

        ///<summary>Gets the default value for the PriceChartBottomMargin property.</summary>
        ///<value>The default value for the PriceChartBottomMargin property: <c>15.0</c>.</value>
        ///<seealso cref = "PriceChartBottomMargin">PriceChartBottomMargin</seealso>
        public static double DefaultPriceChartBottomMargin { get { return 15.0; } }
        //----------------------------------------------------------------------------------------------------------------------------------
        ///<summary>Gets or sets the Brush that specifies how the body of the bullish candle is painted.</summary>
        ///<value>The brush to paint the bodies of the bullish candles. The default value is determined by the <see cref="DefaultBullishCandleFill"/> property.</value>
        ///<remarks> 
        /// Usually candles are separated into two types - Bullish and Bearish. The Bullish candle has its Close price higher than its Open price. All other candles are Bearish. 
        /// To visualize such a separation usually the Bearish and Bullish candles are painted in different Fill and Stroke (outline) colors. 
        /// To set the Brush for the candle outline (the tails and the body outline) use the <see cref="BullishCandleStroke"/> and <see cref="BearishCandleStroke"/> properties for the bullish and bearish candles respectively.
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="BullishCandleFillProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        ///<seealso cref = "BearishCandleFill">BearishCandleFill</seealso>
        ///<seealso cref = "BearishCandleStroke">BearishCandleStroke</seealso>
        ///<seealso cref = "BullishCandleStroke">BullishCandleStroke</seealso>
        [UndoableProperty]
        [JsonProperty]
        public Brush BullishCandleFill
        {
            get { return (Brush)GetValue(BullishCandleFillProperty); }
            set { SetValue(BullishCandleFillProperty, value); }
        }
        /// <summary>Identifies the <see cref="BullishCandleFill"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty BullishCandleFillProperty =
            DependencyProperty.Register("BullishCandleFill", typeof(Brush), typeof(CandleChart), new PropertyMetadata(DefaultBullishCandleFill));

        ///<summary>Gets the default value for the BullishCandleFill property.</summary>
        ///<value>The default value for the BullishCandleFill property: <c>Brushes.Green</c>.</value>
        ///<seealso cref = "BullishCandleFill">BullishCandleFill</seealso>
        public static Brush DefaultBullishCandleFill { get { return (Brush)(new SolidColorBrush(Colors.Green)).GetCurrentValueAsFrozen(); } }
        //----------------------------------------------------------------------------------------------------------------------------------
        ///<summary>Gets or sets the Brush that specifies how the body of the bearish candle is painted.</summary>
        ///<value>The brush to paint the bodies of the bearish candles. The default value is determined by the <see cref="DefaultBearishCandleFill"/> property.</value>
        ///<remarks> 
        /// Usually candles are separated into two types - Bullish and Bearish. The Bullish candle has its Close price higher than its Open price. All other candles are Bearish. 
        /// To visualize such a separation usually the Bearish and Bullish candles are painted in different Fill and Stroke (outline) colors. 
        /// To set the Brush for the candle outline (the tails and the body outline) use the <see cref="BullishCandleStroke"/> and <see cref="BearishCandleStroke"/> properties for the bullish and bearish candles respectively.
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="BearishCandleFillProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        ///<seealso cref = "BullishCandleFill">BearishCandleFill</seealso>
        ///<seealso cref = "BullishCandleStroke">BearishCandleStroke</seealso>
        ///<seealso cref = "BearishCandleStroke">BullishCandleStroke</seealso>
        [UndoableProperty]
        [JsonProperty]
        public Brush BearishCandleFill
        {
            get { return (Brush)GetValue(BearishCandleFillProperty); }
            set { SetValue(BearishCandleFillProperty, value); }
        }
        /// <summary>Identifies the <see cref="BearishCandleFill"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty BearishCandleFillProperty =
            DependencyProperty.Register("BearishCandleFill", typeof(Brush), typeof(CandleChart), new PropertyMetadata(DefaultBearishCandleFill));

        ///<summary>Gets the default value for the BearishCandleFill property.</summary>
        ///<value>The default value for the BearishCandleFill property: <c>Brushes.Red</c>.</value>
        ///<seealso cref = "BearishCandleFill">BearishCandleFill</seealso>
        public static Brush DefaultBearishCandleFill { get { return (Brush)(new SolidColorBrush(Colors.Red)).GetCurrentValueAsFrozen(); } }
        //----------------------------------------------------------------------------------------------------------------------------------
        ///<summary>Gets or sets the Brush that specifies how the outline of the bullish candle is painted.</summary>
        ///<value>The Brush to paint the tails and the body outline of the bullish candles. The default value is determined by the <see cref="DefaultBullishCandleStroke"/> property.</value>
        ///<remarks> 
        /// Usually candles are separated into two types - Bullish and Bearish. The Bullish candle has its Close price higher than its Open price. All other candles are Bearish. 
        /// To visualize such a separation usually the Bearish and Bullish candles are painted in different Fill and Stroke (outline) colors. 
        /// To set the Brush for the candle body fill use the <see cref="BullishCandleFill"/> and <see cref="BearishCandleFill"/> properties for the bullish and bearish candles respectively.
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="BullishCandleStrokeProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        ///<seealso cref = "BearishCandleStroke">BearishCandleStroke</seealso>
        ///<seealso cref = "BearishCandleFill">BearishCandleFill</seealso>
        ///<seealso cref = "BullishCandleFill">BullishCandleFill</seealso>
        [UndoableProperty]
        [JsonProperty]
        public Brush BullishCandleStroke
        {
            get { return (Brush)GetValue(BullishCandleStrokeProperty); }
            set { SetValue(BullishCandleStrokeProperty, value); }
        }
        /// <summary>Identifies the <see cref="BullishCandleStroke"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty BullishCandleStrokeProperty =
            DependencyProperty.Register("BullishCandleStroke", typeof(Brush), typeof(CandleChart), new PropertyMetadata(DefaultBullishCandleStroke));

        ///<summary>Gets the default value for the BullishCandleStroke property.</summary>
        ///<value>The default value for the BullishCandleStroke property: <c>Brushes.Green</c>.</value>
        ///<seealso cref = "BullishCandleStroke">BullishCandleStroke</seealso>
        public static Brush DefaultBullishCandleStroke { get { return (Brush)(new SolidColorBrush(Colors.Green)).GetCurrentValueAsFrozen(); } }
        //----------------------------------------------------------------------------------------------------------------------------------
        ///<summary>Gets or sets the Brush that specifies how the outline of the bearish candle is painted.</summary>
        ///<value>The Brush to paint the tails and the body outline of the bearish candles. The default value is determined by the <see cref="DefaultBearishCandleStroke"/> property.</value>
        ///<remarks> 
        /// Usually candles are separated into two types - Bullish and Bearish. The Bullish candle has its Close price higher than its Open price. All other candles are Bearish. 
        /// To visualize such a separation usually the Bearish and Bullish candles are painted in different Fill and Stroke (outline) colors. 
        /// To set the Brush for the candle body fill use the <see cref="BullishCandleFill"/> and <see cref="BearishCandleFill"/> properties for the bullish and bearish candles respectively.
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="BearishCandleStrokeProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        ///<seealso cref = "BullishCandleStroke">BullishCandleStroke</seealso>
        ///<seealso cref = "BullishCandleFill">BullishCandleFill</seealso>
        ///<seealso cref = "BearishCandleFill">BearishCandleFill</seealso>
        [UndoableProperty]
        [JsonProperty]
        public Brush BearishCandleStroke
        {
            get { return (Brush)GetValue(BearishCandleStrokeProperty); }
            set { SetValue(BearishCandleStrokeProperty, value); }
        }
        /// <summary>Identifies the <see cref="BearishCandleStroke"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty BearishCandleStrokeProperty =
            DependencyProperty.Register("BearishCandleStroke", typeof(Brush), typeof(CandleChart), new PropertyMetadata(DefaultBearishCandleStroke));

        ///<summary>Gets the default value for the BearishCandleStroke property.</summary>
        ///<value>The default value for the BearishCandleStroke property: <c>Brushes.Red</c>.</value>
        ///<seealso cref = "BearishCandleStroke">BearishCandleStroke</seealso>
        public static Brush DefaultBearishCandleStroke { get { return (Brush)(new SolidColorBrush(Colors.Red)).GetCurrentValueAsFrozen(); } }
        //----------------------------------------------------------------------------------------------------------------------------------
        ///<summary>Gets or sets the initial candle width.</summary>
        ///<value>The initial width of the candle, in device-independent units (1/96th inch per unit). 
        ///The default is determined by the <see cref="DefaultInitialCandleWidth"/> value.</value>
        ///<remarks>Initially the width of a candle <see cref="CandleWidth"/> is equal to this property value, but then the <see cref="CandleWidth"/> property value changes due to user's manipulations.</remarks>
        ///<seealso cref = "DefaultInitialCandleWidth">DefaultInitialCandleWidth</seealso>
        ///<seealso cref = "CandleWidth">CandleWidth</seealso>
        public double InitialCandleWidth { get; set; }

        ///<summary>Gets the default value for the InitialCandleWidth property.</summary>
        ///<value>The default value for the <see cref="InitialCandleWidth"/> property, in device-independent units: <c>3.0</c>.</value>
        ///<seealso cref = "InitialCandleWidth">InitialCandleWidth</seealso>
        ///<seealso cref = "CandleWidth">CandleWidth</seealso>
        public readonly double DefaultInitialCandleWidth = 5.0;


        private double candleWidth;
        /// <summary>Gets the width of the candle.</summary>
        ///<value>The candle width, in device-independent units (1/96th inch per unit).</value>
        ///<remarks>Initially after loading this property value is equal to the <see cref="InitialCandleWidth"/>, but then it changes due to user's manipulations.</remarks>
        ///<seealso cref = "InitialCandleWidth">InitialCandleWidth</seealso>
        ///<seealso cref = "DefaultInitialCandleWidth">DefaultInitialCandleWidth</seealso>
        ///<seealso cref = "CandleGap">CandleGap</seealso>
        public double CandleWidth
        {
            get { return candleWidth; }
            private set
            {
                if (candleWidth==value) return;
                candleWidth = value;
                OnPropertyChanged();
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        ///<summary>Gets or sets the initial gap between adjacent candles.</summary>
        ///<value>The initial gap between adjacent candles, in device-independent units (1/96th inch per unit). 
        ///The default is determined by the <see cref="DefaultInitialCandleGap"/> value.</value>
        ///<remarks>Initially the gap between candles <see cref="CandleGap"/> is equal to this property value, but then the <see cref="CandleGap"/> property value changes due to user's manipulations.</remarks>
        ///<seealso cref = "DefaultInitialCandleGap">DefaultInitialCandleGap</seealso>
        ///<seealso cref = "CandleGap">CandleGap</seealso>
        public double InitialCandleGap { get; set; }

        ///<summary>Gets the default value for the InitialCandleGap property.</summary>
        ///<value>The default value for the <see cref="InitialCandleGap"/> property, in device-independent units: <c>1.0</c>.</value>
        ///<seealso cref = "InitialCandleGap">InitialCandleGap</seealso>
        ///<seealso cref = "CandleGap">CandleGap</seealso>
        public readonly double DefaultInitialCandleGap = 1.0;

        double candleGap;
        /// <summary>Gets the horizontal gap between adjacent candles.</summary>
        ///<value>The horizontal gap between adjacent candles, in device-independent units (1/96th inch per unit).</value>
        ///<remarks>Initially after loading this property value is equal to the <see cref="InitialCandleGap"/>, but then it changes due to user's manipulations.</remarks>
        ///<seealso cref = "DefaultInitialCandleGap">DefaultInitialCandleGap</seealso>
        ///<seealso cref = "InitialCandleGap">InitialCandleGap</seealso>
        ///<seealso cref = "CandleWidth">CandleWidth</seealso>
        public double CandleGap
        {
            get { return candleGap; }
            private set
            {
                if (candleGap==value) return;
                candleGap = value;
                OnPropertyChanged();
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        bool ReCalc_CandleWidthAndGap(int new_VisibleCandlesCount)
        {
            if (new_VisibleCandlesCount <= 0) return false;

            double new_ActualWidth = priceChartContainer.ActualWidth;
            if (new_ActualWidth == 0)
            {
                CandleGap = 0.0;
                CandleWidth = 0.0;
                return false;
            }

            double new_candleWidth = CandleWidth;
            while (new_VisibleCandlesCount * (new_candleWidth + 1.0) + 1.0 > new_ActualWidth)
            {
                if (Math.Round(new_candleWidth) < 3.0)
                    return false;
                else
                    new_candleWidth -= 2.0;
            }

            while (new_VisibleCandlesCount * (new_candleWidth + 3.0) + 1.0 < new_ActualWidth)
                new_candleWidth += 2.0;

            CandleGap = (new_ActualWidth - new_VisibleCandlesCount * new_candleWidth - 1.0) / new_VisibleCandlesCount;
            CandleWidth = new_candleWidth;
            return true;
        }
        #endregion **********************************************************************************************************************************************
        //----------------------------------------------------------------------------------------------------------------------------------
        #region VOLUME HISTOGRAM PROPERTIES *********************************************************************************************************************
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the visibility for the volume histogram panel.</summary>
        ///<value>The boolean value that means whether the volume histogram panel is visible or not. The default is determined by the <see cref="DefaultIsVolumePanelVisible"/> value.</value>
        ///<remarks> 
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="IsVolumePanelVisibleProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        [UndoableProperty]
        [JsonProperty]
        public bool IsVolumePanelVisible
        {
            get { return (bool)GetValue(IsVolumePanelVisibleProperty); }
            set { SetValue(IsVolumePanelVisibleProperty, value); }
        }
        /// <summary>Identifies the <see cref="IsVolumePanelVisible"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty IsVolumePanelVisibleProperty =
            DependencyProperty.Register("IsVolumePanelVisible", typeof(bool), typeof(CandleChart), new PropertyMetadata(DefaultIsVolumePanelVisible));

        ///<summary>Gets the default value for the IsVolumePanelVisible property.</summary>
        ///<value>The default value for the <see cref="IsVolumePanelVisible"/> property: <c>True</c>.</value>
        public static bool DefaultIsVolumePanelVisible { get { return true; } }
        //----------------------------------------------------------------------------------------------------------------------------------
        ///<summary>Gets or sets the volume bar width to the candle width ratio that eventually defines the width of the volume bar.</summary>
        ///<value>The ratio of the volume bar width to the candle width. The default is determined by the <see cref="DefaultVolumeBarWidthToCandleWidthRatio"/> value.</value>
        ///<remarks> 
        ///We define the width of the volume bar as a variable that is dependent on the candle width as follows:
        ///<p style="margin: 0 0 0 20"><em>Volume bar width</em> = <see cref="VolumeBarWidthToCandleWidthRatio"/> * <see cref="CandleWidth"/></p>
        ///The value of this property must be in the range [0, 1]. If the value of this property is zero then the volume bar width will be 1.0 in device-independent units, irrespective of the candle width.
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="VolumeBarWidthToCandleWidthRatioProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        [UndoableProperty]
        [JsonProperty]
        public double VolumeBarWidthToCandleWidthRatio
        {
            get { return (double)GetValue(VolumeBarWidthToCandleWidthRatioProperty); }
            set { SetValue(VolumeBarWidthToCandleWidthRatioProperty, value); }
        }
        /// <summary>Identifies the <see cref="VolumeBarWidthToCandleWidthRatio"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty VolumeBarWidthToCandleWidthRatioProperty =
            DependencyProperty.Register("VolumeBarWidthToCandleWidthRatio", typeof(double), typeof(CandleChart), new PropertyMetadata(DefaultVolumeBarWidthToCandleWidthRatio, null, CoerceVolumeBarWidthToCandleWidthRatio));

        private static object CoerceVolumeBarWidthToCandleWidthRatio(DependencyObject objWithOldDP, object newDPValue)
        {
            //CandleChart thisCandleChart = (CandleChart)objWithOldDP; // Содержит старое значение для изменяемого свойства.
            double newValue = (double)newDPValue;
            return Math.Min(1.0, Math.Max(0.0, newValue));
        }

        ///<summary>Gets the default value for the VolumeBarWidthToCandleWidthRatio property.</summary>
        ///<value>The default value for the <see cref="VolumeBarWidthToCandleWidthRatio"/> property: <c>0.3</c>.</value>
        ///<seealso cref = "VolumeBarWidthToCandleWidthRatio">VolumeBarWidthToCandleWidthRatio</seealso>
        public static double DefaultVolumeBarWidthToCandleWidthRatio { get { return 0.3; } }
        //----------------------------------------------------------------------------------------------------------------------------------
        ///<summary>Gets or sets the top margin for the volume histogram.</summary>
        ///<value>The top margin of the volume histogram, in device-independent units. The default is determined by the <see cref="DefaultVolumeHistogramTopMargin"/> value.</value>
        ///<remarks> 
        ///You can set up top and bottom margins for the volume histogram inside its area by setting the <see cref="VolumeHistogramTopMargin"/> and <see cref="VolumeHistogramBottomMargin"/> properties respectively.
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="VolumeHistogramTopMarginProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        [UndoableProperty]
        [JsonProperty]
        public double VolumeHistogramTopMargin
        {
            get { return (double)GetValue(VolumeHistogramTopMarginProperty); }
            set { SetValue(VolumeHistogramTopMarginProperty, value); }
        }
        /// <summary>Identifies the <see cref="VolumeHistogramTopMargin"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty VolumeHistogramTopMarginProperty =
            DependencyProperty.Register("VolumeHistogramTopMargin", typeof(double), typeof(CandleChart), new PropertyMetadata(DefaultVolumeHistogramTopMargin));

        ///<summary>Gets the default value for VolumeHistogramTopMargin property.</summary>
        ///<value>The default value for the <see cref="VolumeHistogramTopMargin"/> property, in device-independent units: <c>10.0</c>.</value>
        public static double DefaultVolumeHistogramTopMargin { get { return 10.0; } }
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the bottom margin for the volume histogram.</summary>
        ///<value>The bottom margin of the volume histogram, in device-independent units. The default is determined by the <see cref="DefaultVolumeHistogramBottomMargin"/> value.</value>
        ///<remarks> 
        ///You can set up top and bottom margins for the volume histogram inside its area by setting the <see cref="VolumeHistogramTopMargin"/> and <see cref="VolumeHistogramBottomMargin"/> properties respectively.
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="VolumeHistogramBottomMarginProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        [UndoableProperty]
        [JsonProperty]
        public double VolumeHistogramBottomMargin
        {
            get { return (double)GetValue(VolumeHistogramBottomMarginProperty); }
            set { SetValue(VolumeHistogramBottomMarginProperty, value); }
        }
        /// <summary>Identifies the <see cref="VolumeHistogramBottomMargin"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty VolumeHistogramBottomMarginProperty =
            DependencyProperty.Register("VolumeHistogramBottomMargin", typeof(double), typeof(CandleChart), new PropertyMetadata(DefaultVolumeHistogramBottomMargin));

        ///<summary>Gets the default value for VolumeHistogramBottomMargin property.</summary>
        ///<value>The default value for the <see cref="VolumeHistogramBottomMargin"/> property, in device-independent units: <c>5.0</c>.</value>
        public static double DefaultVolumeHistogramBottomMargin { get { return 5.0; } }
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the color of the bullish volume bar.</summary>
        ///<value>The brush to fill all bullish volume bars. The default is determined by the <see cref="DefaultBullishVolumeBarFill"/> value.</value>
        ///<remarks> 
        /// We separate all volume bars to "bullish" or "bearish" according to whether the correspondent candle is bullish or bearish. A candle is bullish if its Close higher than its Open. A candle is Bearish if its Close lower than its Open. To visualize such a separation all bars are painted into two different colors - 
        /// <see cref="BullishVolumeBarFill"/> and <see cref="BearishVolumeBarFill"/> for bullish and bearish bars respectively. Likewise you can set the <see cref="BullishCandleFill"/> and <see cref="BearishCandleFill"/> properties to change the appearance of bullish and bearish price candles.
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="BullishVolumeBarFillProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        [UndoableProperty]
        [JsonProperty]
        public Brush BullishVolumeBarFill
        {
            get { return (Brush)GetValue(BullishVolumeBarFillProperty); }
            set { SetValue(BullishVolumeBarFillProperty, value); }
        }
        /// <summary>Identifies the <see cref="BullishVolumeBarFill"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty BullishVolumeBarFillProperty =
            DependencyProperty.Register("BullishVolumeBarFill", typeof(Brush), typeof(CandleChart), new PropertyMetadata(DefaultBullishVolumeBarFill));

        ///<summary>Gets the default value for the BullishVolumeBarFill property.</summary>
        ///<value>The default value for the BullishVolumeBarFill property: <c>Brushes.Green</c>.</value>
        ///<seealso cref = "BullishVolumeBarFill">BullishVolumeBarFill</seealso>
        public static Brush DefaultBullishVolumeBarFill { get { return (Brush)(new SolidColorBrush(Colors.Green)).GetCurrentValueAsFrozen(); } }
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the color of the bearish volume bar.</summary>
        ///<value>The brush to fill all bearish volume bars. The default is determined by the <see cref="DefaultBearishVolumeBarFill"/> value.</value>
        ///<remarks> 
        /// We separate all volume bars to "bullish" or "bearish" according to whether the correspondent candle is bullish or bearish. The Bullish candle has its Close higher than its Open. The Bearish candle has its Close lower than its Open. To visualize such a separation all bars are painted into two different colors - 
        /// <see cref="BullishVolumeBarFill"/> and <see cref="BearishVolumeBarFill"/> for bullish and bearish bars respectively. Likewise you can set the <see cref="BullishCandleFill"/> and <see cref="BearishCandleFill"/> properties to change the appearance of bullish and bearish price candles.
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="BearishVolumeBarFillProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        [UndoableProperty]
        [JsonProperty]
        public Brush BearishVolumeBarFill
        {
            get { return (Brush)GetValue(BearishVolumeBarFillProperty); }
            set { SetValue(BearishVolumeBarFillProperty, value); }
        }
        /// <summary>Identifies the <see cref="BearishVolumeBarFill"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty BearishVolumeBarFillProperty =
            DependencyProperty.Register("BearishVolumeBarFill", typeof(Brush), typeof(CandleChart), new PropertyMetadata(DefaultBearishVolumeBarFill));

        ///<summary>Gets the default value for the BearishVolumeBarFill property.</summary>
        ///<value>The default value for the BearishVolumeBarFill property: <c>Brushes.Red</c>.</value>
        ///<seealso cref = "BearishVolumeBarFill">BearishVolumeBarFill</seealso>
        public static Brush DefaultBearishVolumeBarFill { get { return (Brush)(new SolidColorBrush(Colors.Red)).GetCurrentValueAsFrozen(); } }

        #endregion **********************************************************************************************************************************************
        //----------------------------------------------------------------------------------------------------------------------------------
        #region COMMON PROPERTIES FOR THE PRICE AXIS AND THE TIME AXIS*******************************************************************************************
        ///<summary>Gets or sets the color of ticks and its labels for all the axises.</summary>
        ///<value>The color of ticks and its labels for all the axises. The default is determined by the <see cref="DefaultAxisTickColor"/> value.</value>
        ///<remarks> 
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="AxisTickColorProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        [UndoableProperty]
        [JsonProperty]
        public Brush AxisTickColor
        {
            get { return (Brush)GetValue(AxisTickColorProperty); }
            set { SetValue(AxisTickColorProperty, value); }
        }
        /// <summary>Identifies the <see cref="AxisTickColor">AxisTickColor</see> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty AxisTickColorProperty =
            DependencyProperty.RegisterAttached("AxisTickColor", typeof(Brush), typeof(CandleChart), new FrameworkPropertyMetadata(DefaultAxisTickColor, FrameworkPropertyMetadataOptions.Inherits));

        ///<summary>Gets the default value for the <see cref="AxisTickColor">AxisTickColor</see> property.</summary>
        ///<value>The default value for the <see cref="AxisTickColor"/> property: <c>Brushes.Black</c>.</value>
        public static Brush DefaultAxisTickColor { get { return (Brush)(new SolidColorBrush(Colors.Black)).GetCurrentValueAsFrozen(); } }
        //----------------------------------------------------------------------------------------------------------------------------------
        ///<summary>Gets or sets the tick label font family for all the axises.</summary>
        ///<value>The tick label font family for all the axises. The default is <see href="https://docs.microsoft.com/en-us/dotnet/api/system.windows.systemfonts.messagefontfamily?view=netframework-4.7.2">SystemFonts.MessageFontFamily</see>.</value>
        ///<remarks> 
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="AxisTickLabelFontFamilyProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        [UndoableProperty]
        [JsonProperty]
        public FontFamily AxisTickLabelFontFamily
        {
            get { return (FontFamily)GetValue(AxisTickLabelFontFamilyProperty); }
            set { SetValue(AxisTickLabelFontFamilyProperty, value); }
        }
        /// <summary>Identifies the <see cref="AxisTickLabelFontFamily">AxisTickLabelFontFamily</see> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty AxisTickLabelFontFamilyProperty =
            DependencyProperty.Register("AxisTickLabelFontFamily", typeof(FontFamily), typeof(CandleChart), new PropertyMetadata(SystemFonts.MessageFontFamily));

        #endregion **********************************************************************************************************************************************
        //----------------------------------------------------------------------------------------------------------------------------------
        #region PROPERTIES OF THE PRICE AXIS (AND OF THE VOLUME AXIS, WHICH DOESN'T HAVE ITS OWN PROPERTIES) ****************************************************
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the foreground for the current price label on the price axis.</summary>
        ///<value>The foreground for the current price label on the price axis. The default is determined by the <see cref="DefaultCurrentPriceLabelForeground"/>values.</value>
        ///<remarks>
        ///The price (or volume) value label locates on the price (or volume) axis area.
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="CurrentPriceLabelForegroundProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        [UndoableProperty]
        [JsonProperty]
        public Brush CurrentPriceLabelForeground
        {
            get { return (Brush)GetValue(CurrentPriceLabelForegroundProperty); }
            set { SetValue(CurrentPriceLabelForegroundProperty, value); }
        }
        /// <summary>Identifies the <see cref="CurrentPriceLabelForeground"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty CurrentPriceLabelForegroundProperty =
            DependencyProperty.Register("CurrentPriceLabelForeground", typeof(Brush), typeof(CandleChart), new PropertyMetadata(DefaultCurrentPriceLabelForeground));

        ///<summary>Gets the default value for the CurrentPriceLabelForeground property.</summary>
        ///<value>The default value for the <see cref="CurrentPriceLabelForeground"/> property: <c>Brushes.Red</c>.</value>
        public static Brush DefaultCurrentPriceLabelForeground { get { return (Brush)(new SolidColorBrush(Colors.Red)).GetCurrentValueAsFrozen(); } }
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the background for the current price label on the price axis.</summary>
        ///<value>The background for the current price label on the price axis. The default is determined by the <see cref="DefaultCurrentPriceLabelBackground"/>values.</value>
        ///<remarks>
        ///The price (or volume) value label locates on the price (or volume) axis area.
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="CurrentPriceLabelBackgroundProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        [UndoableProperty]
        [JsonProperty]
        public Brush CurrentPriceLabelBackground
        {
            get { return (Brush)GetValue(CurrentPriceLabelBackgroundProperty); }
            set { SetValue(CurrentPriceLabelBackgroundProperty, value); }
        }
        /// <summary>Identifies the <see cref="CurrentPriceLabelBackground"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty CurrentPriceLabelBackgroundProperty =
            DependencyProperty.Register("CurrentPriceLabelBackground", typeof(Brush), typeof(CandleChart), new PropertyMetadata(DefaultCurrentPriceLabelBackground));

        ///<summary>Gets the default value for the CurrentPriceLabelBackground property.</summary>
        ///<value>The default <see cref="Brush"/> value for the <see cref="CurrentPriceLabelBackground"/> property: <c>#FFE8EDFF</c>.</value>
        public static Brush DefaultCurrentPriceLabelBackground { get { return (Brush)(new SolidColorBrush(Color.FromArgb(255, 232, 237, 255))).GetCurrentValueAsFrozen(); } } // #FFE8EDFF
        //----------------------------------------------------------------------------------------------------------------------------------
        ///<summary>Gets or sets the font size of the tick labels for the price and volume axises.</summary>
        ///<value>The font size of the tick labels for the price and volume axises. The default is determined by the <see cref="DefaultPriceAxisTickLabelFontSize"/> value.</value>
        ///<remarks>
        /// The volume axis doesn't have its own appearance properties. Therefore, the volume axis appearance depends on price axis properties. 
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="PriceAxisTickLabelFontSizeProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        [UndoableProperty]
        [JsonProperty]
        public double PriceAxisTickLabelFontSize
        {
            get { return (double)GetValue(PriceAxisTickLabelFontSizeProperty); }
            set { SetValue(PriceAxisTickLabelFontSizeProperty, value); }
        }
        /// <summary>Identifies the <see cref="PriceAxisTickLabelFontSize">PriceAxisTickLabelFontSize</see> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty PriceAxisTickLabelFontSizeProperty =
            DependencyProperty.RegisterAttached("PriceAxisTickLabelFontSize", typeof(double), typeof(CandleChart), new FrameworkPropertyMetadata(DefaultPriceAxisTickLabelFontSize, FrameworkPropertyMetadataOptions.Inherits, OnPriceAxisTickLabelFontSizeChanged));
        static void OnPriceAxisTickLabelFontSizeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            CandleChart thisCandleChart = obj as CandleChart;
            thisCandleChart?.OnPropertyChanged("PriceAxisWidth");
            thisCandleChart?.OnPropertyChanged("PriceAxisTickLabelHeight");
        }

        ///<summary>Gets the default value for the <see cref="PriceAxisTickLabelFontSize">PriceAxisTickLabelFontSize</see> property.</summary>
        ///<value>The default value for the <see cref="PriceAxisTickLabelFontSize"/> property: <c>11.0</c>.</value>
        public static double DefaultPriceAxisTickLabelFontSize { get { return 11.0; } }
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets the width of the price and volume axis area.</summary>
        ///<value>The width of the price and volume axis area, which contains the ticks and its labels.</value>
        ///<remarks>
        /// The volume axis area has the same width as the price axis area.
        ///</remarks>
        public double PriceAxisWidth
        {
            get
            {
                FormattedText txt = new FormattedText(new string('9', MaxNumberOfCharsInPrice), Culture, FlowDirection.LeftToRight, new Typeface(AxisTickLabelFontFamily.ToString()), PriceAxisTickLabelFontSize, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                return txt.Width + PriceTicksElement.TICK_LINE_WIDTH + 2 * PriceTicksElement.TICK_HORIZ_MARGIN;
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets the height of the price or volume tick label.</summary>
        ///<value>The height of the price or volume tick label. This value is equals to the height of the text of the label.</value>
        ///<remarks>
        /// The volume tick label has the same height as the price tick label.
        ///</remarks>
        public double PriceAxisTickLabelHeight
        {
            get
            {
                return (new FormattedText("1,23", Culture, FlowDirection.LeftToRight, new Typeface(AxisTickLabelFontFamily.ToString()), PriceAxisTickLabelFontSize, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip)).Height;
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        ///<summary>Gets or sets the minimal gap between the adjacent price or volume tick labels.</summary>
        ///<value>The minimal gap between adjacent labels for the price and volume axis. It must be non-negative value. The default is determined by the <see cref="DefaultGapBetweenPriceTickLabels"/> value.</value>
        ///<remarks>
        ///<para>This property regulates the density of the tick labels inside the price or volume axis area. The higher the <see cref="GapBetweenPriceTickLabels"/>, the less close adjacent labels are located.</para>
        ///<para>The volume axis doesn't have its own appearance properties. Therefore, the volume axis appearance depends on price axis properties.</para>
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="GapBetweenPriceTickLabelsProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        public double GapBetweenPriceTickLabels
        {
            get { return (double)GetValue(GapBetweenPriceTickLabelsProperty); }
            set { SetValue(GapBetweenPriceTickLabelsProperty, value); }
        }
        /// <summary>Identifies the <see cref="GapBetweenPriceTickLabels">PriceAxisTickLabelFontSize</see> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty GapBetweenPriceTickLabelsProperty =
            DependencyProperty.RegisterAttached("GapBetweenPriceTickLabels", typeof(double), typeof(CandleChart), new FrameworkPropertyMetadata(DefaultGapBetweenPriceTickLabels, FrameworkPropertyMetadataOptions.Inherits));

        ///<summary>Gets the default value for the <see cref="GapBetweenPriceTickLabels">GapBetweenPriceTickLabels</see> property.</summary>
        ///<value>The default value for the <see cref="GapBetweenPriceTickLabels"/> property: <c>0.0</c>.</value>
        public static double DefaultGapBetweenPriceTickLabels { get { return 0.0; } }
        //----------------------------------------------------------------------------------------------------------------------------------
        private int maxNumberOfCharsInPrice = 0;
        private readonly double correctPriceValueProbability = 0.93;

        /// <summary>Gets the maximal number of chars in a price for the current candle collection.</summary>
        ///<value>The maximal number of chars in a price for the current candle collection.</value>
        ///<remarks>
        ///This value is recalculated every time the candle collection is changed.
        ///</remarks>
        public int MaxNumberOfCharsInPrice
        {
            get { return maxNumberOfCharsInPrice; }
            private set
            {
                if (value == maxNumberOfCharsInPrice) return;
                maxNumberOfCharsInPrice = value;
                OnPropertyChanged();
                OnPropertyChanged("PriceAxisWidth");
            }
        }

        private int maxNumberOfFractionalDigitsInPrice = 0;
        /// <summary>Gets the maximum number of fractional digits in a price and volume for the current candle collection.</summary>
        ///<value>The maximum number of fractional digits in a price and volume for the current candle collection.</value>
        ///<remarks>
        ///This value is recalculated every time the candle collection is changed.
        ///</remarks>
        public int MaxNumberOfFractionalDigitsInPrice
        {
            get { return maxNumberOfFractionalDigitsInPrice; }
            private set
            {
                if (value == maxNumberOfFractionalDigitsInPrice) return;
                maxNumberOfFractionalDigitsInPrice = value;
                OnPropertyChanged();
            }
        }

        // Просматривает CandlesSource и пересчитывает maxNumberOfCharsInPrice
        private void ReCalc_MaxNumberOfCharsInPrice_and_MaxNumberOfFractionalDigitsInPrice()
        {
            if (CandlesSource == null) return;

            if (CandlesSource.Count == 0)
                MaxNumberOfFractionalDigitsInPrice = 0;
            else
                MaxNumberOfFractionalDigitsInPrice = numberOfFractionalDigitsSample.MaxValueAmongTopFrequent(correctPriceValueProbability);

            string priceNumberFormat = $"N{MaxNumberOfFractionalDigitsInPrice}";

            if (CandlesSource.Count == 0)
                MaxNumberOfCharsInPrice = MyNumberFormatting.MaxVolumeStringLength;
            else
            {
                string decimalSeparator = Culture.NumberFormat.NumberDecimalSeparator;
                char[] decimalSeparatorArray = decimalSeparator.ToCharArray();

                int charsInPrice = CandlesSource.Select(c => MyNumberFormatting.PriceToString(c.H, priceNumberFormat, Culture, decimalSeparator, decimalSeparatorArray).Length).Max();

                int charsInVolume = 0;
                if (IsVolumePanelVisible)
                    charsInVolume = MyNumberFormatting.MaxVolumeStringLength;

                MaxNumberOfCharsInPrice = Math.Max(charsInPrice, charsInVolume);
            }
        }

        private void Update_MaxNumberOfCharsInPrice_and_MaxNumberOfFractionalDigitsInPrice(ICandle newCandle)
        {
            string decimalSeparator = Culture.NumberFormat.NumberDecimalSeparator;
            char[] decimalSeparatorArray = decimalSeparator.ToCharArray();

            MaxNumberOfFractionalDigitsInPrice = numberOfFractionalDigitsSample.MaxValueAmongTopFrequent(correctPriceValueProbability);
            string priceNumberFormat = $"N{MaxNumberOfFractionalDigitsInPrice}";

            int L1 = MyNumberFormatting.PriceToString(newCandle.H, priceNumberFormat, Culture, decimalSeparator, decimalSeparatorArray).Length;

            int L2 = 0;
            if (IsVolumePanelVisible)
                L2 = MyNumberFormatting.MaxVolumeStringLength;

            int L = Math.Max(L1, L2);

            if (L > MaxNumberOfCharsInPrice)
                MaxNumberOfCharsInPrice = L;
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        #endregion **********************************************************************************************************************************************
        //----------------------------------------------------------------------------------------------------------------------------------
        #region PROPERTIES OF THE TIME AXIS *********************************************************************************************************************
        //----------------------------------------------------------------------------------------------------------------------------------
        ///<summary>Gets or sets the font size of the tick labels for the time axis.</summary>
        ///<value>The font size of the tick labels for the time (and date) axis. The default is determined by the <see cref="DefaultTimeAxisTickLabelFontSize"/> value.</value>
        ///<remarks>
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="TimeAxisTickLabelFontSizeProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        [UndoableProperty]
        [JsonProperty]
        public double TimeAxisTickLabelFontSize
        {
            get { return (double)GetValue(TimeAxisTickLabelFontSizeProperty); }
            set { SetValue(TimeAxisTickLabelFontSizeProperty, value); }
        }
        /// <summary>Identifies the <see cref="TimeAxisTickLabelFontSize">TimeAxisTickLabelFontSize</see> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty TimeAxisTickLabelFontSizeProperty =
            DependencyProperty.RegisterAttached("TimeAxisTickLabelFontSize", typeof(double), typeof(CandleChart), new FrameworkPropertyMetadata(DefaultTimeAxisTickLabelFontSize, FrameworkPropertyMetadataOptions.Inherits, OnTimeTickFontSizeChanged));
        static void OnTimeTickFontSizeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            CandleChart thisCandleChart = obj as CandleChart;
            thisCandleChart?.OnPropertyChanged("TimeAxisHeight");
        }

        ///<summary>Gets the default value for the <see cref="TimeAxisTickLabelFontSize">TimeAxisTickLabelFontSize</see> property.</summary>
        ///<value>The default value for the <see cref="TimeAxisTickLabelFontSize"/> property: <c>10.0</c>.</value>
        public static double DefaultTimeAxisTickLabelFontSize { get { return 10.0; } }
        //----------------------------------------------------------------------------------------------------------------------------------
        ///<summary>Gets the height of the time axis area.</summary>
        ///<value>The height of the time axis area, which contains the time and date ticks with its labels.</value>
        public double TimeAxisHeight
        {
            get
            {
                double timeTextHeight = (new FormattedText("1Ajl", Culture, FlowDirection.LeftToRight, new Typeface(AxisTickLabelFontFamily.ToString()), TimeAxisTickLabelFontSize, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip)).Height;
                return 2 * timeTextHeight + 4.0;
            }
        }

        #endregion **********************************************************************************************************************************************
        //----------------------------------------------------------------------------------------------------------------------------------
        #region GRIDLINES PROPERTIES ****************************************************************************************************************************
        //----------------------------------------------------------------------------------------------------------------------------------
        ///<summary>Gets or sets the pen for the horizontal gridlines.</summary>
        ///<value>The pen for the horizontal gridlines. The default is determined by the <see cref="DefaultHorizontalGridlinesBrush"/> and <see cref="DefaultHorizontalGridlinesThickness"/> values.</value>
        ///<remarks>
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="HorizontalGridlinesPenProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        [UndoableProperty]
        [JsonProperty]
        public Pen HorizontalGridlinesPen
        {
            get { return (Pen)GetValue(HorizontalGridlinesPenProperty); }
            set { SetValue(HorizontalGridlinesPenProperty, value); }
        }
        /// <summary>Identifies the <see cref="HorizontalGridlinesPen"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty HorizontalGridlinesPenProperty =
            DependencyProperty.Register("HorizontalGridlinesPen", typeof(Pen), typeof(CandleChart), new PropertyMetadata(DefaultHorizontalGridlinesPen));

        ///<summary>Gets the default value for the Brush constituent of the HorizontalGridlinesPen property.</summary>
        ///<value>The default value for the <see cref="Brush"/> constituent of the <see cref="HorizontalGridlinesPen"/> property: <c>#1E000000</c>.</value>
        ///<seealso cref = "DefaultHorizontalGridlinesThickness">DefaultHorizontalGridlinesThickness</seealso>
        public static Brush DefaultHorizontalGridlinesBrush { get { return (Brush)(new SolidColorBrush(Color.FromArgb(30, 0, 0, 0))).GetCurrentValueAsFrozen(); } } // #1E000000

        ///<summary>Gets the default value for Thickness constituent of the HorizontalGridlinesPen property.</summary>
        ///<value>The default value for the Thickness constituent of the <see cref="HorizontalGridlinesPen"/> property: <c>1.0</c>.</value>
        ///<seealso cref = "DefaultHorizontalGridlinesBrush">DefaultHorizontalGridlinesBrush</seealso>
        public static double DefaultHorizontalGridlinesThickness { get { return 1.0; } }

        private static Pen DefaultHorizontalGridlinesPen { get { return (Pen)(new Pen(DefaultHorizontalGridlinesBrush, DefaultHorizontalGridlinesThickness)).GetCurrentValueAsFrozen(); } }
        //----------------------------------------------------------------------------------------------------------------------------------
        ///<summary>Gets or sets the pen for the vertical gridlines.</summary>
        ///<value>The pen for the vertical gridlines. The default is determined by the <see cref="DefaultVerticalGridlinesBrush"/> and <see cref="DefaultVerticalGridlinesThickness"/> values.</value>
        ///<remarks>
        ///<h3>Dependency Property Information</h3>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="VerticalGridlinesPenProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        [UndoableProperty]
        [JsonProperty]
        public Pen VerticalGridlinesPen
        {
            get { return (Pen)GetValue(VerticalGridlinesPenProperty); }
            set { SetValue(VerticalGridlinesPenProperty, value); }
        }
        /// <summary>Identifies the <see cref="VerticalGridlinesPen"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty VerticalGridlinesPenProperty =
            DependencyProperty.Register("VerticalGridlinesPen", typeof(Pen), typeof(CandleChart),
                new PropertyMetadata(DefaultVerticalGridlinesPen));

        ///<summary>Gets the default value for the Brush constituent of the VerticalGridlinesPen property.</summary>
        ///<value>The default value for the <see cref="Brush"/> constituent of the <see cref="VerticalGridlinesPen"/> property: <c>#1E000000</c>.</value>
        ///<seealso cref = "DefaultVerticalGridlinesThickness">DefaultVerticalGridlinesThickness</seealso>
        public static Brush DefaultVerticalGridlinesBrush { get { return (Brush)(new SolidColorBrush(Color.FromArgb(50, 105, 42, 0))).GetCurrentValueAsFrozen(); } } // #32692A00

        ///<summary>Gets the default value for Thickness constituent of the VerticalGridlinesPen property.</summary>
        ///<value>The default value for the Thickness constituent of the <see cref="VerticalGridlinesPen"/> property: <c>1.0</c>.</value>
        ///<seealso cref = "DefaultVerticalGridlinesBrush">DefaultVerticalGridlinesBrush</seealso>
        public static double DefaultVerticalGridlinesThickness { get { return 1.0; } }

        private static Pen DefaultVerticalGridlinesPen { get { return (Pen)(new Pen(DefaultVerticalGridlinesBrush, DefaultVerticalGridlinesThickness)).GetCurrentValueAsFrozen(); } }
        //----------------------------------------------------------------------------------------------------------------------------------
        ///<summary>Gets or sets the visibility of the horizontal gridlines.</summary>
        ///<value>The visibility of the horizontal gridlines: Visible if <c>True</c>; Hidden if <c>False</c>. The default is determined by the <see cref="DefaultIsHorizontalGridlinesEnabled"/> values.</value>
        ///<remarks>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="IsHorizontalGridlinesEnabledProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        ///<seealso cref = "IsVerticalGridlinesEnabled">IsHorizontalGridlinesEnabled</seealso>
        [UndoableProperty]
        [JsonProperty]
        public bool IsHorizontalGridlinesEnabled
        {
            get { return (bool)GetValue(IsHorizontalGridlinesEnabledProperty); }
            set { SetValue(IsHorizontalGridlinesEnabledProperty, value); }
        }
        /// <summary>Identifies the <see cref="IsHorizontalGridlinesEnabled"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty IsHorizontalGridlinesEnabledProperty =
            DependencyProperty.Register("IsHorizontalGridlinesEnabled", typeof(bool), typeof(CandleChart), new PropertyMetadata(DefaultIsHorizontalGridlinesEnabled));

        ///<summary>Gets the default value for the IsHorizontalGridlinesEnabled property.</summary>
        ///<value>The default value for the <see cref="IsHorizontalGridlinesEnabled"/> property: <c>True</c>.</value>
        public static bool DefaultIsHorizontalGridlinesEnabled { get { return true; } }
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the visibility of the vertical gridlines.</summary>
        ///<value>The visibility of the vertical gridlines: Visible if <c>True</c>; Hidden if <c>False</c>. The default is determined by the <see cref="DefaultIsVerticalGridlinesEnabled"/> values.</value>
        ///<remarks>
        ///This property applies to all vertical gridlines, which are showed for all ticks of the time axis. But sometimes you don't need to show all of this gridlines and want to visualize lines only for the most round time and date values. 
        ///In that case you need to set both the <see cref="IsVerticalGridlinesEnabled"/> and the <see cref="HideMinorVerticalGridlines"/> properties to <c>True</c>.
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="IsVerticalGridlinesEnabledProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        ///<seealso cref = "IsHorizontalGridlinesEnabled">IsHorizontalGridlinesEnabled</seealso>
        [UndoableProperty]
        [JsonProperty]
        public bool IsVerticalGridlinesEnabled
        {
            get { return (bool)GetValue(IsVerticalGridlinesEnabledProperty); }
            set { SetValue(IsVerticalGridlinesEnabledProperty, value); }
        }
        /// <summary>Identifies the <see cref="IsVerticalGridlinesEnabled"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty IsVerticalGridlinesEnabledProperty =
            DependencyProperty.Register("IsVerticalGridlinesEnabled", typeof(bool), typeof(CandleChart), new PropertyMetadata(DefaultIsVerticalGridlinesEnabled));

        ///<summary>Gets the default value for the IsVerticalGridlinesEnabled property.</summary>
        ///<value>The default value for the <see cref="IsVerticalGridlinesEnabled"/> property: <c>True</c>.</value>
        public static bool DefaultIsVerticalGridlinesEnabled { get { return true; } }
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the visibility of the minor vertical gridlines.</summary>
        ///<value>The visibility of the vertical gridlines for minor (not "round") time ticks: Visible if <c>False</c>; Hidden if <c>True</c>. The default is determined by the <see cref="DefaultHideMinorVerticalGridlines"/>values.</value>
        ///<remarks>
        ///Sometimes you need to show gridlines only for the most round time or date values, and hide other minor gridlines.
        ///In that case you need to set both the <see cref="IsVerticalGridlinesEnabled"/> and the <see cref="HideMinorVerticalGridlines"/> properties to <c>True</c>.
        ///Whether the particular timetick value is Minor or not depends on the current timeframe. The common rule is: round time or date values are Major, others are Minor.
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="HideMinorVerticalGridlinesProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        ///<seealso cref = "IsVerticalGridlinesEnabled">IsHorizontalGridlinesEnabled</seealso>
        [UndoableProperty]
        [JsonProperty]
        public bool HideMinorVerticalGridlines
        {
            get { return (bool)GetValue(HideMinorVerticalGridlinesProperty); }
            set { SetValue(HideMinorVerticalGridlinesProperty, value); }
        }
        /// <summary>Identifies the <see cref="HideMinorVerticalGridlines"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty HideMinorVerticalGridlinesProperty =
            DependencyProperty.Register("HideMinorVerticalGridlines", typeof(bool), typeof(CandleChart), new PropertyMetadata(DefaultHideMinorVerticalGridlines));

        ///<summary>Gets the default value for the HideMinorVerticalGridlines property.</summary>
        ///<value>The default value for the <see cref="HideMinorVerticalGridlines"/> property: <c>False</c>.</value>
        public static bool DefaultHideMinorVerticalGridlines { get { return false; } }

        #endregion **********************************************************************************************************************************************
        //----------------------------------------------------------------------------------------------------------------------------------
        #region PROPERTIES OF THE CROSS *************************************************************************************************************************
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the brush for the cross lines.</summary>
        ///<value>The brush for the cross lines. The default is determined by the <see cref="DefaultCrossLinesBrush"/>values.</value>
        ///<remarks>
        ///The Cross lines always have a thickness of 1.0.
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="CrossLinesBrushProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        [UndoableProperty]
        [JsonProperty]
        public Brush CrossLinesBrush
        {
            get { return (Brush)GetValue(CrossLinesBrushProperty); }
            set { SetValue(CrossLinesBrushProperty, value); }
        }
        /// <summary>Identifies the <see cref="CrossLinesBrush"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty CrossLinesBrushProperty =
            DependencyProperty.Register("CrossLinesBrush", typeof(Brush), typeof(CandleChart), new PropertyMetadata(DefaultCrossLinesBrush) );

        ///<summary>Gets the default value for the CrossLinesBrush property.</summary>
        ///<value>The default value for the <see cref="CrossLinesBrush"/> property: <c>#1E000A97</c>.</value>
        public static Brush DefaultCrossLinesBrush { get { return (Brush)(new SolidColorBrush(Color.FromArgb(30, 0, 10, 151))).GetCurrentValueAsFrozen(); } } // #1E000A97
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the visibility of the cross lines.</summary>
        ///<value>The visibility of the crosslines: Visible if <c>True</c>; Hidden if <c>False</c>. The default is determined by the <see cref="DefaultIsCrossLinesVisible"/> values.</value>
        ///<remarks>
        ///The cross lines locates inside the price chart (or volume histogram) area and pass through the current mouse position. 
        ///You can separately set up the visibility for the cross lines and for the correspondent price (or volume) label by setting the 
        ///<see cref="IsCrossLinesVisible"/> and <see cref="IsCrossPriceLabelVisible"/> properties respectively.
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="IsCrossLinesVisibleProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        [UndoableProperty]
        [JsonProperty]
        public bool IsCrossLinesVisible
        {
            get { return (bool)GetValue(IsCrossLinesVisibleProperty); }
            set { SetValue(IsCrossLinesVisibleProperty, value); }
        }
        /// <summary>Identifies the <see cref="IsCrossLinesVisible"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty IsCrossLinesVisibleProperty =
            DependencyProperty.Register("IsCrossLinesVisible", typeof(bool), typeof(CandleChart), new PropertyMetadata(DefaultIsCrossLinesVisible));

        ///<summary>Gets the default value for the IsCrossLinesVisible property.</summary>
        ///<value>The default value for the <see cref="IsCrossLinesVisible"/> property: <c>true</c>.</value>
        public static bool DefaultIsCrossLinesVisible { get { return true; } }
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the visibility of the cross price (or volume) label.</summary>
        ///<value>The visibility of the cross price (or volume) label: Visible if <c>True</c>; Hidden if <c>False</c>. The default is determined by the <see cref="DefaultIsCrossPriceLabelVisible"/> values.</value>
        ///<remarks>
        ///The cross price (or volume) label locates inside the price (or volume) axis area. 
        ///You can separately set up the visibility for the cross lines and for the correspondent price (or volume) label by setting the 
        ///<see cref="IsCrossLinesVisible"/> and <see cref="IsCrossPriceLabelVisible"/> properties respectively.
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="DefaultIsCrossPriceLabelVisible"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        [UndoableProperty]
        [JsonProperty]
        public bool IsCrossPriceLabelVisible
        {
            get { return (bool)GetValue(IsCrossPriceLabelVisibleProperty); }
            set { SetValue(IsCrossPriceLabelVisibleProperty, value); }
        }
        /// <summary>Identifies the <see cref="IsCrossPriceLabelVisible"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty IsCrossPriceLabelVisibleProperty =
            DependencyProperty.Register("IsCrossPriceLabelVisible", typeof(bool), typeof(CandleChart), new PropertyMetadata(DefaultIsCrossPriceLabelVisible));

        ///<summary>Gets the default value for the IsCrossLinesVisible property.</summary>
        ///<value>The default value for the <see cref="IsCrossLinesVisible"/> property: <c>true</c>.</value>
        public static bool DefaultIsCrossPriceLabelVisible { get { return true; } }
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the foreground for the price (or volume) label of the cross.</summary>
        ///<value>The foreground for the price or volume label of the cross. The default is determined by the <see cref="DefaultCrossPriceLabelForeground"/>values.</value>
        ///<remarks>
        ///The price (or volume) value label locates on the price (or volume) axis area.
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="CrossPriceLabelForegroundProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        [UndoableProperty]
        [JsonProperty]
        public Brush CrossPriceLabelForeground
        {
            get { return (Brush)GetValue(CrossPriceLabelForegroundProperty); }
            set { SetValue(CrossPriceLabelForegroundProperty, value); }
        }
        /// <summary>Identifies the <see cref="CrossPriceLabelForeground"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty CrossPriceLabelForegroundProperty =
            DependencyProperty.Register("CrossPriceLabelForeground", typeof(Brush), typeof(CandleChart), new PropertyMetadata(DefaultCrossPriceLabelForeground));

        ///<summary>Gets the default value for the CrossPriceLabelForeground property.</summary>
        ///<value>The default value for the <see cref="CrossPriceLabelForeground"/> property: <c>Brushes.Red</c>.</value>
        public static Brush DefaultCrossPriceLabelForeground { get { return (Brush)(Brushes.Black).GetCurrentValueAsFrozen(); } }
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the background for the price or volume label of the cross.</summary>
        ///<value>The background for the price or volume label of the cross. The default is determined by the <see cref="DefaultCrossPriceLabelBackground"/>values.</value>
        ///<remarks>
        ///The price (or volume) value label locates on the price (or volume) axis area.
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="CrossPriceLabelBackgroundProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        [UndoableProperty]
        [JsonProperty]
        public Brush CrossPriceLabelBackground
        {
            get { return (Brush)GetValue(CrossPriceLabelBackgroundProperty); }
            set { SetValue(CrossPriceLabelBackgroundProperty, value); }
        }
        /// <summary>Identifies the <see cref="CrossPriceLabelBackground"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty CrossPriceLabelBackgroundProperty =
            DependencyProperty.Register("CrossPriceLabelBackground", typeof(Brush), typeof(CandleChart), new PropertyMetadata(DefaultCrossPriceLabelBackground));

        ///<summary>Gets the default value for the CrossPriceLabelBackground property.</summary>
        ///<value>The default <see cref="Brush"/> value for the <see cref="CrossPriceLabelBackground"/> property: <c>#FFE8EDFF</c>.</value>
        public static Brush DefaultCrossPriceLabelBackground { get { return (Brush)(Brushes.Gainsboro).GetCurrentValueAsFrozen(); } } // #FFE8EDFF

        #endregion **********************************************************************************************************************************************
        //----------------------------------------------------------------------------------------------------------------------------------
        #region SCROLLBAR PROPERTIES ****************************************************************************************************************************
        //----------------------------------------------------------------------------------------------------------------------------------
        ///<summary>Gets or sets the background for the scrollbar.</summary>
        ///<value>The brush for the scrollbar background. The default is determined by the <see cref="DefaultScrollBarBackground"/>values.</value>
        ///<remarks>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="ScrollBarBackgroundProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        [UndoableProperty]
        [JsonProperty]
        public Brush ScrollBarBackground
        {
            get { return (Brush)GetValue(ScrollBarBackgroundProperty); }
            set { SetValue(ScrollBarBackgroundProperty, value); }
        }
        /// <summary>Identifies the <see cref="ScrollBarBackground"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty ScrollBarBackgroundProperty =
            DependencyProperty.Register("ScrollBarBackground", typeof(Brush), typeof(CandleChart), new PropertyMetadata(DefaultScrollBarBackground));

        ///<summary>Gets the default value for the ScrollBarBackground property.</summary>
        ///<value>The default value for the <see cref="ScrollBarBackground"/> property: <c>#FFF0F0F0</c>.</value>
        public static Brush DefaultScrollBarBackground { get { return (Brush)(new SolidColorBrush(Color.FromArgb(255, 240, 240, 240))).GetCurrentValueAsFrozen(); } } // #FFF0F0F0
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the height of the scrollbar.</summary>
        ///<value>The height of the scrollbar background. The default is determined by the <see cref="DefaultScrollBarHeight"/>values.</value>
        ///<remarks>
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="ScrollBarHeightProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        [UndoableProperty]
        [JsonProperty]
        public double ScrollBarHeight
        {
            get { return (double)GetValue(ScrollBarHeightProperty); }
            set { SetValue(ScrollBarHeightProperty, value); }
        }
        /// <summary>Identifies the <see cref="ScrollBarHeight"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty ScrollBarHeightProperty =
            DependencyProperty.Register("ScrollBarHeight", typeof(double), typeof(CandleChart), new PropertyMetadata(DefaultScrollBarHeight));

        ///<summary>Gets the default value for the ScrollBarHeight property.</summary>
        ///<value>The default value for the <see cref="ScrollBarHeight"/> property: <c>15.0</c>.</value>
        public static double DefaultScrollBarHeight { get { return 15.0; } }

        #endregion **********************************************************************************************************************************************
        //----------------------------------------------------------------------------------------------------------------------------------
        private void ChangeCurrentTimeFrame(TimeFrame newTimeFrame)
        {
            string secID = (CandlesSource as ICandlesSourceFromProvider).SecID;
            ICandlesSource newCandleSource = CandlesSourceProvider.GetCandlesSource(secID, newTimeFrame);
            CandlesSource = newCandleSource;

            ISecurityInfo secInfo = CandlesSourceProvider.GetSecFromCatalog(secID);
            SetCurrentValue(LegendTextProperty, $"{secInfo.Ticker}, {newTimeFrame}");
        }

        private void ChangeCurrentTimeFrame(object sender, RoutedEventArgs e)
        {
            if (!(CandlesSource is ICandlesSourceFromProvider) || CandlesSourceProvider == null) return;

            TimeFrame newTimeFrame = (TimeFrame)((MenuItem)e.OriginalSource).Header;
            ChangeCurrentTimeFrame(newTimeFrame);
        }

        private FancyPrimitives.RelayCommand changeCurrentTimeframeCommand;
        /// <summary>Gets the Command for changing the current time frame.</summary>
        ///<value>The command for changing the current time frame.</value>
        ///<remarks>
        ///This command can be executed only if the current <see cref="CandlesSource"/> value is of type <see cref="ICandlesSourceFromProvider"/> 
        ///and <see cref="ICandlesSourceProvider.SecCatalog"/> of <see cref="CandlesSourceProvider"/> contains the current financial instrument.
        ///It gets the new <see cref="ICandlesSource"/> value from <see cref="CandlesSourceProvider"/> of the same financial instrument but of the new time frame 
        ///and assigns it to the <see cref="CandlesSource"/> property.
        ///The command receives the new <see cref="TimeFrame"/> value as a command parameter;
        ///</remarks>
        public FancyPrimitives.RelayCommand ChangeCurrentTimeframeCommand
        {
            get
            {
                return changeCurrentTimeframeCommand ??
                  (changeCurrentTimeframeCommand = new FancyPrimitives.RelayCommand(newTimeFrame =>
                  {
                      ChangeCurrentTimeFrame((TimeFrame)newTimeFrame);
                  },
                  nothing =>
                  {
                      return (CandlesSource is ICandlesSourceFromProvider) && (CandlesSourceProvider != null && ((ICandlesSourceProvider)CandlesSourceProvider).SecCatalog.Count > 0);
                  } ));
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        ///<summary>Gets or sets the provider of candle collections, that can be used as a value for the <see cref="CandlesSource"/> property.</summary>
        ///<value>The provider of candle collections, which can be used as a value for the <see cref="CandlesSource"/> property.</value>
        ///<remarks>
        ///<para>
        ///Using the <see cref="CandlesSourceProvider"/> property is optional. You can set the <see cref="CandlesSource"/> property to populate your <see cref="CandleChart"/> with candle data and it's absolutely ok. 
        ///But if you want to provide the ability to select a security and time frame from the <see cref="CandleChart"/> context menu you have to set the <see cref="CandlesSourceProvider"/> property.
        ///<see cref="ICandlesSourceProvider"/> provides a list of available securities and time frames and, of course, a candle data to use with <see cref="CandleChart"/>.
        ///Setting this property makes the <c>Select New Security</c> and <c>Time Frame</c> items of the <see cref="CandleChart"/> context menu enabled. 
        ///When a user of your application has selected a new security or time frame, a new value will be assigned to the <see cref="CandlesSource"/> property. 
        ///</para>
        ///<para>
        ///Take a note, that the <see cref="CandlesSource"/> property support <see cref="IResourceWithUserCounter"/> types. 
        ///Every time you sets the <see cref="CandlesSource"/> property value, it calls the <see cref="IResourceWithUserCounter.IncreaseUserCount"/> method of the new value and the 
        ///the <see cref="IResourceWithUserCounter.DecreaseUserCount"/> method of the old value if they implement the <see cref="IResourceWithUserCounter"/> interface. 
        ///It can be helpful in some scenarios where you want to optimize the number of candle sources in your application and delete those that are not in use.
        ///</para>
        ///</remarks>
        public ICandlesSourceProvider CandlesSourceProvider
        {
            get { return (ICandlesSourceProvider)GetValue(CandlesSourceProviderProperty); }
            set { SetValue(CandlesSourceProviderProperty, value); }
        }
        /// <summary>Identifies the <see cref="CandlesSourceProvider"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty CandlesSourceProviderProperty =
            DependencyProperty.Register("CandlesSourceProvider", typeof(ICandlesSourceProvider), typeof(CandleChart), new PropertyMetadata(null));
        //----------------------------------------------------------------------------------------------------------------------------------
        private DiscreteRandomVariableSample numberOfFractionalDigitsSample;
        private readonly int numberOfObservationsToStopRecalculatingNumberOfFractionalDigits = 500;

        ///<summary>Gets or sets the data source for the candles of this chart.</summary>
        ///<value>The data source for the candles of this chart. The default value is null.</value>
        ///<remarks>
        ///<para>
        ///Note that the timeframe is an immutable characteristic of a candle collection. 
        ///Therefore <see cref="ICandlesSource.TimeFrame"/> is the readonly property of the <see cref="ICandlesSource"/> interface.
        ///The only way to change the timeframe of your <see cref="CandleChart"/> is to change the value of the <see cref="CandlesSource"/> property 
        ///to a whole new candle collection with a new timeframe.
        ///</para>
        ///<para>
        ///Take a note, that the <see cref="CandlesSource"/> property support <see cref="IResourceWithUserCounter"/> types. 
        ///Every time you sets the <see cref="CandlesSource"/> property value, it calls the <see cref="IResourceWithUserCounter.IncreaseUserCount"/> method of the new value and the 
        ///the <see cref="IResourceWithUserCounter.DecreaseUserCount"/> method of the old value if they implements the <see cref="IResourceWithUserCounter"/> interface. 
        ///It can be helpful in some scenarios where you want to optimize the number of candle sources in your application and delete those that are not in use.
        ///</para>
        //////<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="CandlesSourceProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        ///<seealso href="https://gellerda.github.io/FancyCandles/articles/populating_candlestick_chart.html">Populating CandleChart with candles</seealso>
        public ICandlesSource CandlesSource
        {
            get { return (ICandlesSource)GetValue(CandlesSourceProperty); }
            set { SetValue(CandlesSourceProperty, value); }
        }
        /// <summary>Identifies the <see cref="CandlesSource"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty CandlesSourceProperty =
            DependencyProperty.Register("CandlesSource", typeof(ICandlesSource), typeof(CandleChart), new UIPropertyMetadata(null, OnCandlesSourceChanged, CoerceCandlesSource));

        DateTime lastCenterCandleDateTime;
        private static object CoerceCandlesSource(DependencyObject objWithOldDP, object newDPValue)
        {
            CandleChart thisCandleChart = (CandleChart)objWithOldDP;

            IntRange vcRange = thisCandleChart.VisibleCandlesRange;
            if (IntRange.IsUndefined(vcRange))
                thisCandleChart.lastCenterCandleDateTime = DateTime.MinValue;
            else
            {
                if (thisCandleChart.CandlesSource != null && (vcRange.Start_i + vcRange.Count) < thisCandleChart.CandlesSource.Count())
                {
                    int centralCandle_i = (2 * vcRange.Start_i + vcRange.Count) / 2;
                    thisCandleChart.lastCenterCandleDateTime = thisCandleChart.CandlesSource[centralCandle_i].t;
                }
                else
                    thisCandleChart.lastCenterCandleDateTime = DateTime.MaxValue;
            }

            return newDPValue;
        }

        static void OnCandlesSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            CandleChart thisCandleChart = obj as CandleChart;
            if (thisCandleChart == null) return;

            if (e.OldValue != null)
            {
                if (e.OldValue is INotifyCollectionChanged)
                    (e.OldValue as INotifyCollectionChanged).CollectionChanged -= thisCandleChart.OnCandlesSourceCollectionChanged;

                (e.OldValue as IResourceWithUserCounter)?.DecreaseUserCount();
            }

            if (e.NewValue != null)
            {
                if (e.NewValue is INotifyCollectionChanged)
                    (e.NewValue as INotifyCollectionChanged).CollectionChanged += thisCandleChart.OnCandlesSourceCollectionChanged;

                (e.NewValue as IResourceWithUserCounter)?.IncreaseUserCount();

                thisCandleChart.numberOfFractionalDigitsSample = new DiscreteRandomVariableSample(15);
                for (int i = 0; i < thisCandleChart.CandlesSource.Count; i++)
                {
                    ICandle cndl = thisCandleChart.CandlesSource[i];
                    if (cndl.O != 0.0 && cndl.H != 0.0 && cndl.L != 0.0 && cndl.C != 0.0 )
                    {
                        thisCandleChart.numberOfFractionalDigitsSample.AddNewObservation(MyNumberFormatting.NumberOfFractionalDigits(cndl.O));
                        thisCandleChart.numberOfFractionalDigitsSample.AddNewObservation(MyNumberFormatting.NumberOfFractionalDigits(cndl.H));
                        thisCandleChart.numberOfFractionalDigitsSample.AddNewObservation(MyNumberFormatting.NumberOfFractionalDigits(cndl.L));
                        thisCandleChart.numberOfFractionalDigitsSample.AddNewObservation(MyNumberFormatting.NumberOfFractionalDigits(cndl.C));
                    }
                }

                if (e.NewValue is ICandlesSourceFromProvider && thisCandleChart.CandlesSourceProvider != null)
                {
                    try
                    {
                        ISecurityInfo secInfo = thisCandleChart.CandlesSourceProvider.GetSecFromCatalog((thisCandleChart.CandlesSource as ICandlesSourceFromProvider).SecID);
                        thisCandleChart.SetCurrentValue(LegendTextProperty, thisCandleChart.CreateLegendText(secInfo));
                    }
                    catch 
                    {
                        thisCandleChart.SetCurrentValue(LegendTextProperty, loadingLegendText);
                    }
                }
            }

            thisCandleChart.SetCandlesSourceForAll_OverlayIndicators();

            if (thisCandleChart.CandlesSource == null || thisCandleChart.CandlesSource.Count == 0)
                thisCandleChart.CurrentPrice = 0;
            else
                thisCandleChart.CurrentPrice = thisCandleChart.CandlesSource[thisCandleChart.CandlesSource.Count - 1].C;

            if (thisCandleChart.IsLoaded)
            {
                thisCandleChart.ReCalc_MaxNumberOfCharsInPrice_and_MaxNumberOfFractionalDigitsInPrice();

                DateTime old_lastCenterCandleDateTime = thisCandleChart.lastCenterCandleDateTime;
                thisCandleChart.ReCalc_VisibleCandlesRange();

                if (old_lastCenterCandleDateTime != DateTime.MinValue)
                    thisCandleChart.SetVisibleCandlesRangeCenter(old_lastCenterCandleDateTime);
                //else
                  //  thisCandleChart.ReCalc_VisibleCandlesRange();

                //thisCandleChart.ReCalc_FinishedCandlesExtremums();
                thisCandleChart.ReCalc_VisibleCandlesExtremums();
            }
        }

        private void OnCandlesSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //different kind of changes that may have occurred in collection
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (CandlesSource.Count == 1 && LegendText == loadingLegendText)
                {
                    ISecurityInfo secInfo = CandlesSourceProvider.GetSecFromCatalog((CandlesSource as ICandlesSourceFromProvider).SecID);
                    SetCurrentValue(LegendTextProperty, CreateLegendText(secInfo));
                }

                if (numberOfFractionalDigitsSample.NumberOfObservations < numberOfObservationsToStopRecalculatingNumberOfFractionalDigits)
                {
                    ICandle newCandle = CandlesSource[e.NewStartingIndex];
                    if (newCandle.O != 0.0 && newCandle.H != 0.0 && newCandle.L != 0.0 && newCandle.C != 0.0)
                    {
                        numberOfFractionalDigitsSample.AddNewObservation(MyNumberFormatting.NumberOfFractionalDigits(newCandle.O));
                        numberOfFractionalDigitsSample.AddNewObservation(MyNumberFormatting.NumberOfFractionalDigits(newCandle.H));
                        numberOfFractionalDigitsSample.AddNewObservation(MyNumberFormatting.NumberOfFractionalDigits(newCandle.L));
                        numberOfFractionalDigitsSample.AddNewObservation(MyNumberFormatting.NumberOfFractionalDigits(newCandle.C));
                    }
                    Update_MaxNumberOfCharsInPrice_and_MaxNumberOfFractionalDigitsInPrice(newCandle);
                }

                int maxVisibleCandlesCount = (int)(priceChartContainer.ActualWidth / (CandleWidth + CandleGap));

                if (e.NewStartingIndex == (CandlesSource.Count - 1))
                    CurrentPrice = CandlesSource[CandlesSource.Count - 1].C;

                if (CandlesSource.Count<= maxVisibleCandlesCount)
                    VisibleCandlesRange = new IntRange(0, CandlesSource.Count);
                else if (e.NewStartingIndex == (CandlesSource.Count - 1))
                {
                    //if (CandlesSource.Count > 1)
                    //    ReCalc_FinishedCandlesExtremums_AfterNewFinishedCandleAdded(CandlesSource[CandlesSource.Count - 2]);

                    if ((VisibleCandlesRange.Start_i + VisibleCandlesRange.Count) == e.NewStartingIndex)
                        VisibleCandlesRange = new IntRange(VisibleCandlesRange.Start_i + 1, VisibleCandlesRange.Count);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                if (numberOfFractionalDigitsSample.NumberOfObservations < numberOfObservationsToStopRecalculatingNumberOfFractionalDigits)
                {
                    ICandle newCandle = CandlesSource[e.NewStartingIndex];
                    numberOfFractionalDigitsSample.AddNewObservation(MyNumberFormatting.NumberOfFractionalDigits(newCandle.O));
                    numberOfFractionalDigitsSample.AddNewObservation(MyNumberFormatting.NumberOfFractionalDigits(newCandle.H));
                    numberOfFractionalDigitsSample.AddNewObservation(MyNumberFormatting.NumberOfFractionalDigits(newCandle.L));
                    numberOfFractionalDigitsSample.AddNewObservation(MyNumberFormatting.NumberOfFractionalDigits(newCandle.C));
                    Update_MaxNumberOfCharsInPrice_and_MaxNumberOfFractionalDigitsInPrice(newCandle);
                }

                if (e.NewStartingIndex == (CandlesSource.Count-1))
                    CurrentPrice = CandlesSource[CandlesSource.Count - 1].C;

                int vc_i = e.NewStartingIndex - VisibleCandlesRange.Start_i; // VisibleCandles index.
                if (vc_i >= 0 && vc_i < VisibleCandlesRange.Count)
                    ReCalc_VisibleCandlesExtremums_AfterOneCandleChanged(e.NewStartingIndex);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove) { /* your code */ }
            else if (e.Action == NotifyCollectionChangedAction.Move) { /* your code */ }
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        CandleExtremums visibleCandlesExtremums;
        ///<summary>Gets the Low and High of the visible candles in vector format (Low,High).</summary>
        ///<value>The Low and High of the visible candles in vector format (Low,High).</value>
        ///<remarks>
        ///<para>The visible candles are those that fall inside the visible candles range, which is determined by the <see cref="VisibleCandlesRange"/> property.</para>
        ///The Low of a set of candles is a minimum Low value of this candles. The High of a set of candles is a maximum High value of this candles.
        ///</remarks>
        public CandleExtremums VisibleCandlesExtremums
        {
            get { return visibleCandlesExtremums; }
            private set
            {
                visibleCandlesExtremums = value;
                OnPropertyChanged();
            }
        }

        private void ReCalc_VisibleCandlesExtremums()
        {
            if (IntRange.IsUndefined(VisibleCandlesRange)) 
                return;

            int end_i = VisibleCandlesRange.Start_i + VisibleCandlesRange.Count - 1;
            double maxH = double.MinValue, maxV = double.MinValue, minL = double.MaxValue, minV = double.MaxValue;
            for (int i = VisibleCandlesRange.Start_i; i <= end_i; i++)
            {
                ICandle cndl = CandlesSource[i];
                if (cndl.H == 0.0 || cndl.L == 0) continue;
                if (cndl.H > maxH) maxH = cndl.H;
                if (cndl.L < minL) minL = cndl.L;
                if (cndl.V < minV) minV = cndl.V;
                if (cndl.V > maxV) maxV = cndl.V;
            }

            VisibleCandlesExtremums = new CandleExtremums(minL, maxH, minV, maxV);
        }

        private void ReCalc_VisibleCandlesExtremums_AfterOneCandleChanged(int changedCandle_i)
        {
            ICandle cndl = CandlesSource[changedCandle_i];

            if (cndl.H == 0.0 || cndl.L == 0) return;

            double newPriceL = Math.Min(cndl.L, VisibleCandlesExtremums.PriceLow);
            double newPriceH = Math.Max(cndl.H, VisibleCandlesExtremums.PriceHigh);
            double newVolL = Math.Min(cndl.V, VisibleCandlesExtremums.VolumeLow);
            double newVolH = Math.Max(cndl.V, VisibleCandlesExtremums.VolumeHigh);
            VisibleCandlesExtremums = new CandleExtremums(newPriceL, newPriceH, newVolL, newVolH);
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets the range of indexes of candles, currently visible in this chart window.</summary>
        ///<value>The range of indexes of candles, currently visible in this chart window. The default value is <see cref="IntRange.Undefined"/>.</value>
        ///<remarks>
        ///This property defines the part of collection of candles <see cref="CandlesSource"/> that currently visible in the chart window.
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="VisibleCandlesRangeProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        public IntRange VisibleCandlesRange
        {
            get { return (IntRange)GetValue(VisibleCandlesRangeProperty); }
            set { SetValue(VisibleCandlesRangeProperty, value); }
        }
        /// <summary>Identifies the <see cref="VisibleCandlesRange"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty VisibleCandlesRangeProperty =
            DependencyProperty.Register("VisibleCandlesRange", typeof(IntRange), typeof(CandleChart), new PropertyMetadata(IntRange.Undefined, OnVisibleCanlesRangeChanged, CoerceVisibleCandlesRange));

        static void OnVisibleCanlesRangeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            CandleChart thisCandleChart = (CandleChart)obj;
            if (thisCandleChart.IsLoaded)
                thisCandleChart.ReCalc_VisibleCandlesExtremums();
        }

        private static object CoerceVisibleCandlesRange(DependencyObject objWithOldDP, object baseValue)
        {
            CandleChart thisCandleChart = (CandleChart)objWithOldDP; // Содержит старое значение для изменяемого свойства.
            IntRange newValue = (IntRange)baseValue;

            if (IntRange.IsUndefined(newValue))
                return newValue;
            // Это хак для привязки к скроллеру, когда передается только компонента IntRange.Start_i, а компонента IntRange.Count берется из старого значения свойства:
            else if (IntRange.IsContainsOnlyStart_i(newValue))
                return new IntRange(newValue.Start_i, thisCandleChart.VisibleCandlesRange.Count);
            // А это обычная ситуация:
            else
            {
                int newVisibleCandlesStart_i = Math.Max(0, newValue.Start_i);
                int newVisibleCandlesEnd_i = Math.Min(thisCandleChart.CandlesSource.Count - 1, newValue.Start_i + Math.Max(1, newValue.Count) - 1);
                int maxVisibleCandlesCount = thisCandleChart.MaxVisibleCandlesCount;
                int newVisibleCandlesCount = newVisibleCandlesEnd_i - newVisibleCandlesStart_i + 1;
                if (newVisibleCandlesCount > maxVisibleCandlesCount)
                {
                    newVisibleCandlesStart_i = newVisibleCandlesEnd_i - maxVisibleCandlesCount + 1;
                    newVisibleCandlesCount = maxVisibleCandlesCount;
                }

                return new IntRange(newVisibleCandlesStart_i, newVisibleCandlesCount);
            }
        }

        // Пересчитывает VisibleCandlesRange.Count таким образом, чтобы по возможности сохранить индекс последней видимой свечи 
        // и соответствовать текущим значениям CandleWidth и CandleGap.
        private void ReCalc_VisibleCandlesRange()
        {
            if (priceChartContainer.ActualWidth == 0 || CandlesSource == null)
            {
                VisibleCandlesRange = IntRange.Undefined;
                return;
            }

            int newCount = (int)(priceChartContainer.ActualWidth / (CandleWidth + CandleGap));

            if (newCount > CandlesSource.Count)
            {
                VisibleCandlesRange = new IntRange(0, CandlesSource.Count);
                return;
            }

            int new_start_i = IntRange.IsUndefined(VisibleCandlesRange) ? (CandlesSource.Count - newCount) : VisibleCandlesRange.Start_i + VisibleCandlesRange.Count - newCount;
            if (new_start_i < 0) new_start_i = 0;
            if (new_start_i + newCount > CandlesSource.Count)
                new_start_i = CandlesSource.Count - newCount;

            VisibleCandlesRange = new IntRange(new_start_i, newCount);
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        int MaxVisibleCandlesCount
        { get { return (int)(priceChartContainer.ActualWidth / 2); } }
        //----------------------------------------------------------------------------------------------------------------------------------
        ///<summary>Shifts the range of visible candles to the position where the <c>t</c> property of the central visible candle is equal (or closest) to specified value.</summary>
        ///<param name="visibleCandlesRangeCenter">Central visible candle should have its <c>t</c> property equal to this parameter (or close to it as much as possible).</param>
        public void SetVisibleCandlesRangeCenter(DateTime visibleCandlesRangeCenter)
        {
            int maxVisibleCandlesCount = (int)(priceChartContainer.ActualWidth / (CandleWidth + CandleGap));
            if (CandlesSource.Count < maxVisibleCandlesCount)
            {
                VisibleCandlesRange = new IntRange(0, CandlesSource.Count);
                return;
            }

            if (IntRange.IsUndefined(VisibleCandlesRange))
                return;

            ICandle cndl = CandlesSource[VisibleCandlesRange.Count / 2];
            if (visibleCandlesRangeCenter < cndl.t) 
            {
                VisibleCandlesRange = new IntRange(0, VisibleCandlesRange.Count);
                return;
            }

            cndl = CandlesSource[CandlesSource.Count - 1 - VisibleCandlesRange.Count / 2];
            if (visibleCandlesRangeCenter > cndl.t) 
            {
                VisibleCandlesRange = new IntRange(CandlesSource.Count - VisibleCandlesRange.Count, VisibleCandlesRange.Count);
                return;
            }

            VisibleCandlesRange = IntRange.CreateContainingOnlyStart_i(FindCandleByDatetime(CandlesSource, visibleCandlesRangeCenter) - VisibleCandlesRange.Count / 2);
        }

        ///<summary>Sets the range of visible candles, that starts and ends at specified moments in time.</summary>
        ///<param name="lowerBound">The datetime value at which the range of visible candles must start.</param>
        ///<param name="upperBound">The datetime value at which the range of visible candles must end.</param>
        ///<remarks>
        ///This function finds in the <see cref="CandlesSource"/> collection of candles two of them that has its <c>t</c> property equal or closest to <c>datetime0</c> and <c>datetime1</c>. 
        ///Then it sets the <see cref="VisibleCandlesRange"/> to the <see cref="IntRange"/> value that starts at the index of the first aforementioned candle, and ends at the index of the second one.
        ///</remarks>
        public void SetVisibleCandlesRangeBounds(DateTime lowerBound, DateTime upperBound)
        {
            if (CandlesSource == null || CandlesSource.Count == 0) return;

            if (lowerBound > upperBound)
            {
                DateTime t_ = lowerBound;
                lowerBound = upperBound;
                upperBound = t_;
            }

            int i0, i1;
            int N = CandlesSource.Count;
            if (CandlesSource[0].t > upperBound)
            {
                VisibleCandlesRange = new IntRange(0, 1);
                return;
            }

            if (CandlesSource[N - 1].t < lowerBound)
            {
                VisibleCandlesRange = new IntRange(N - 1, 1);
                return;
            }

            if (CandlesSource[0].t > lowerBound)
                i0 = 0;
            else
                i0 = FindCandleByDatetime(CandlesSource, lowerBound);

            if (CandlesSource[N - 1].t < upperBound)
                i1 = N - 1;
            else
                i1 = FindCandleByDatetime(CandlesSource, upperBound);

            int newVisibleCandlesCount = i1 - i0 + 1;
            ReCalc_CandleWidthAndGap(newVisibleCandlesCount);
            VisibleCandlesRange = new IntRange(i0, newVisibleCandlesCount);
        }

        private static int FindCandleByDatetime(IList<ICandle> candles, DateTime t)
        {
            int i0 = 0, i1 = candles.Count - 1;
            DateTime t_ = candles[i0].t;
            if (t <= t_) return i0;
            //else if (t < t_) return -1;

            t_ = candles[i1].t;
            if (t >= t_) return i1;
            //else if (t > t_) return -1;

            while (true)
            {
                if ((i0 + 1) == i1) return i1;

                int i = (i0 + i1) / 2;
                t_ = candles[i].t;
                if (t == t_) return i;
                else if (t > t_)
                    i0 = i;
                else
                    i1 = i;
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the modifier key that in conjunction with mouse wheel rolling will cause a change of the visible candles range width.</summary>
        ///<value>The the modifier key that in conjunction with mouse wheel rolling will cause a change of the visible candles range width. The default value is <see cref="ModifierKeys.None"/>.</value>
        ///<remarks>
        ///Depending on the keyboard modifier keys the mouse wheel can serve for two functions: scrolling through the candle collection and changing the width of visible candles range. 
        ///You can set up modifier keys for the aforementioned functions by setting the <see cref="MouseWheelModifierKeyForScrollingThroughCandles"/> and 
        ///<see cref="MouseWheelModifierKeyForCandleWidthChanging"/> properties respectively.
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="VisibleCandlesRangeProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        public ModifierKeys MouseWheelModifierKeyForCandleWidthChanging
        {
            get { return (ModifierKeys)GetValue(MouseWheelModifierKeyForCandleWidthChangingProperty); }
            set { SetValue(MouseWheelModifierKeyForCandleWidthChangingProperty, value); }
        }
        /// <summary>Identifies the <see cref="MouseWheelModifierKeyForCandleWidthChanging"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty MouseWheelModifierKeyForCandleWidthChangingProperty =
            DependencyProperty.Register("MouseWheelModifierKeyForCandleWidthChanging", typeof(ModifierKeys), typeof(CandleChart), new PropertyMetadata(ModifierKeys.None));
        //--------
        /// <summary>Gets or sets a modifier key that in conjunction with mouse wheel rolling will cause a scrolling through the candles.</summary>
        ///<value>The the modifier key that in conjunction with mouse wheel rolling will cause a scrolling through the candles. The default value is <see cref="ModifierKeys.Control"/>.</value>
        ///<remarks>
        ///Depending on the keyboard modifier keys the mouse wheel can serve for two functions: scrolling through the candle collection and changing the width of visible candles range. 
        ///You can set up modifier keys for the aforementioned functions by setting the <see cref="MouseWheelModifierKeyForScrollingThroughCandles"/> and 
        ///<see cref="MouseWheelModifierKeyForCandleWidthChanging"/> properties respectively.
        ///<table border="1" frame="hsides" rules="rows" style="margin: 0 0 10 20"> 
        ///<tr><td>Identifier field</td><td><see cref="VisibleCandlesRangeProperty"/></td></tr> 
        ///<tr><td>Metadata properties set to <c>True</c></td><td>-</td></tr> </table>
        ///</remarks>
        public ModifierKeys MouseWheelModifierKeyForScrollingThroughCandles
        {
            get { return (ModifierKeys)GetValue(MouseWheelModifierKeyForScrollingThroughCandlesProperty); }
            set { SetValue(MouseWheelModifierKeyForScrollingThroughCandlesProperty, value); }
        }
        /// <summary>Identifies the <see cref="MouseWheelModifierKeyForScrollingThroughCandles"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty MouseWheelModifierKeyForScrollingThroughCandlesProperty =
            DependencyProperty.Register("MouseWheelModifierKeyForScrollingThroughCandles", typeof(ModifierKeys), typeof(CandleChart), new PropertyMetadata(ModifierKeys.Control));
        //--------
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Пересчитывает VisibleCandlesRange.Start_i, CandleWidth и CandleGap таким образом, 
            // чтобы установить заданное значение для VisibleCandlesRange.Count и по возможности сохраняет индекс последней видимой свечи. 
            void SetVisibleCandlesRangeCount(int newCount)
            {
                if (newCount > CandlesSource.Count) newCount = CandlesSource.Count;
                if (newCount == VisibleCandlesRange.Count) return;
                if (!ReCalc_CandleWidthAndGap(newCount)) return; // Если график уже нельзя больше сжимать.

                int new_start_i = VisibleCandlesRange.Start_i + VisibleCandlesRange.Count - newCount;
                if (new_start_i < 0) new_start_i = 0;
                VisibleCandlesRange = new IntRange(new_start_i, newCount);
            }
            //------

            if (Keyboard.Modifiers == MouseWheelModifierKeyForCandleWidthChanging)
            {
                if (e.Delta > 0)
                    SetVisibleCandlesRangeCount(VisibleCandlesRange.Count - 3);
                else if (e.Delta < 0)
                    SetVisibleCandlesRangeCount(VisibleCandlesRange.Count + 3);
            }
            else if (Keyboard.Modifiers == MouseWheelModifierKeyForScrollingThroughCandles)
            {
                if (e.Delta > 0)
                {
                    if ((VisibleCandlesRange.Start_i + VisibleCandlesRange.Count) < CandlesSource.Count)
                        VisibleCandlesRange = IntRange.CreateContainingOnlyStart_i(VisibleCandlesRange.Start_i + 1);
                }
                else if (e.Delta < 0)
                {
                    if (VisibleCandlesRange.Start_i > 0) 
                        VisibleCandlesRange = IntRange.CreateContainingOnlyStart_i(VisibleCandlesRange.Start_i - 1);
                }
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        private void OnMouseMoveInsidePriceChartContainer(object sender, MouseEventArgs e)
        {
            CurrentMousePosition = Mouse.GetPosition(priceChartContainer);
        }

        private void OnMouseMoveInsideVolumeHistogramContainer(object sender, MouseEventArgs e)
        {
            CurrentMousePosition = Mouse.GetPosition(volumeHistogramContainer);
        }

        Point currentMousePosition;
        /// <summary>This is a property for internal use only. You should not use it.</summary>
        public Point CurrentMousePosition
        {
            get { return currentMousePosition; }
            private set
            {
                if (currentMousePosition == value) return;
                currentMousePosition = value;
                OnPropertyChanged();
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        private void OnPanelCandlesContainerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!IsLoaded || e.NewSize.Width == 0 || CandlesSource?.Count() == 0)
                return;

            if (e.NewSize.Width != e.PreviousSize.Width)
                ReCalc_VisibleCandlesRange();
        }
        //---------------- INotifyPropertyChanged ----------------------------------------------------------
        /// <summary>INotifyPropertyChanged interface realization.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>INotifyPropertyChanged interface realization.</summary>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------
    }
}