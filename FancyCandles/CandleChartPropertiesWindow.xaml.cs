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
using System.Diagnostics;
using System.Windows.Markup;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
using System.Globalization;
using FancyCandles.Indicators;
using System.Reflection;

namespace FancyCandles
{
    /// <summary>
    /// Логика взаимодействия для PriceChartPropertiesWindow.xaml
    /// </summary>
    internal partial class CandleChartPropertiesWindow : Window
    {
        private CandleChart parentCandleChart;
        //----------------------------------------------------------------------------------------------------------------------------------
        public CandleChartPropertiesWindow(CandleChart parentCandleChart)
        {
            this.parentCandleChart = parentCandleChart;

            Thickness oldLegendMargin = parentCandleChart.LegendMargin;
            if (oldLegendMargin.Left != oldLegendMargin.Right)
            {
                if (parentCandleChart.LegendHorizontalAlignment == HorizontalAlignment.Left)
                    parentCandleChart.SetCurrentValue(CandleChart.LegendMarginProperty, new Thickness(oldLegendMargin.Left, oldLegendMargin.Top, oldLegendMargin.Left, oldLegendMargin.Bottom));
                else if (parentCandleChart.LegendHorizontalAlignment == HorizontalAlignment.Right)
                    parentCandleChart.SetCurrentValue(CandleChart.LegendMarginProperty, new Thickness(oldLegendMargin.Right, oldLegendMargin.Top, oldLegendMargin.Right, oldLegendMargin.Bottom));
            }

            if (parentCandleChart.LegendHorizontalAlignment == HorizontalAlignment.Center)
                parentCandleChart.SetCurrentValue(CandleChart.LegendMarginProperty, new Thickness(0, oldLegendMargin.Top, 0, oldLegendMargin.Bottom));

            DataContext = parentCandleChart;
            OverlayIndicatorTypes = GetOverlayIndicatorTypes(parentCandleChart.AddInIndicatorsFolder);

            InitializeComponent();
            Owner = Application.Current.MainWindow;

#if DEBUG
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
#endif
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        public IList<System.Type> OverlayIndicatorTypes { get; private set; }

        private class IndicatorTypeEqualityComparer : IEqualityComparer<System.Type>
        {
            public bool Equals(System.Type t1, System.Type t2)
            {
                return t1.Name == t2.Name;
            }

            public int GetHashCode(System.Type t)
            {
                return t.Name.GetHashCode();
            }
        }

        private static IList<System.Type> GetOverlayIndicatorTypes(string addInIndicatorsFolder)
        {
            IEnumerable<System.Type> fancyCandlesAssemblyIndicatorTypes = typeof(OverlayIndicator).Assembly.GetTypes()
                                        .Where(t => t.IsClass && t.IsSubclassOf(typeof(OverlayIndicator)) && !t.IsAbstract);

            IEnumerable<System.Type> entryAssemblyIndicatorTypes = Assembly.GetEntryAssembly().GetTypes()
                                        .Where(t => t.IsClass && t.IsSubclassOf(typeof(OverlayIndicator)) && !t.IsAbstract);

            IndicatorTypeEqualityComparer indicatorComparer = new IndicatorTypeEqualityComparer();
            IEnumerable<System.Type> allIndicatorTypes = fancyCandlesAssemblyIndicatorTypes.Union(entryAssemblyIndicatorTypes, indicatorComparer);

            string fullAddInIndicatorsFolder = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, addInIndicatorsFolder);
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
                IEnumerable<System.Type> asmIndicatorTypes = asm.GetTypes().Where(t => t.IsClass && t.IsSubclassOf(typeof(OverlayIndicator)) && !t.IsAbstract);
                allIndicatorTypes = allIndicatorTypes.Union(asmIndicatorTypes, indicatorComparer);
            }

            return allIndicatorTypes.ToArray();
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            DataContext = null;
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            DataContext = null;
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        private void ListOverlayIndicators_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listElement = (ListBox)sender;
            Collection<OverlayIndicator> overlayIndicators = listElement.ItemsSource as Collection<OverlayIndicator>;

            if (overlayIndicators == null) return;

            if (e.RemovedItems.Count > 0)
            {
                overlayIndicatorEditor.Children.Clear();
                overlayIndicatorEditor.DataContext = null;
            }

            if (e.AddedItems.Count > 0)
            {
                OverlayIndicator selectedOverlayIndicator = (OverlayIndicator)e.AddedItems[0];

                string indicatorXaml = selectedOverlayIndicator.PropertiesEditorXAML;
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] b = encoding.GetBytes(indicatorXaml);
                ParserContext context = new ParserContext();
                context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
                context.XmlnsDictionary.Add("i", "clr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity");
                context.XmlnsDictionary.Add("local", "clr-namespace:FancyCandles;assembly=FancyCandles");
                context.XmlnsDictionary.Add("fp", "clr-namespace:FancyPrimitives;assembly=FancyPrimitives");
                UIElement indicatorEditorElement = (UIElement)XamlReader.Load(new MemoryStream(b), context);

                overlayIndicatorEditor.Children.Clear();
                overlayIndicatorEditor.DataContext = selectedOverlayIndicator;
                overlayIndicatorEditor.Children.Add(indicatorEditorElement);
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        private void ListOverlayIndicators_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!e.HeightChanged) return;
            {
                ListBox listElement = (ListBox)sender;
                Collection<OverlayIndicator> overlayIndicators = listElement.ItemsSource as Collection<OverlayIndicator>;

                if (overlayIndicators == null) return;

                if (e.NewSize.Height > e.PreviousSize.Height)
                    listElement.SelectedIndex = overlayIndicators.Count - 1;
                else
                    listElement.SelectedIndex = 0;
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------
    }
}
