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
using DataVisualization.Core.ViewModels.Visualizers;
using DataVisualization.Models;

namespace DataVisualization.Core.Views.Visualizers
{
    /// <summary>
    /// Interaction logic for FastVisualizerLegendView.xaml
    /// </summary>
    public partial class FastVisualizerLegendView : UserControl
    {
        public FastVisualizerLegendView()
        {
            InitializeComponent();
        }

        private void OnSeriesDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2)
                return;

            var uiElement = (FrameworkElement)sender;
            var series = (Series) uiElement.DataContext;

            ((FastVisualizerLegendViewModel) DataContext).OpenSeriesProperties(series);
        }
    }
}
