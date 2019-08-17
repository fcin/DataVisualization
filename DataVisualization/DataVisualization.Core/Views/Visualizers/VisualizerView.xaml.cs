using System.Windows.Controls;
using DataVisualization.Core.ViewModels.Visualizers;
using LiveCharts.Events;

namespace DataVisualization.Core.Views.Visualizers
{
    /// <summary>
    /// Interaction logic for VisualizerView.xaml
    /// </summary>
    public partial class VisualizerView : UserControl
    {
        public VisualizerView()
        {
            InitializeComponent();
        }

        private void Axis_OnPreviewRangeChanged(PreviewRangeChangedEventArgs eventargs)
        {
            ((VisualizerViewModel)DataContext)?.OnRangeChanged((long)eventargs.PreviewMinValue, (long)eventargs.PreviewMaxValue);
        }
    }
}
