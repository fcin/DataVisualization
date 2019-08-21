using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DataVisualization.Core.ViewModels.Visualizers;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;

namespace DataVisualization.Core.Views.Visualizers
{
    /// <summary>
    /// Interaction logic for FastVisualizerView.xaml
    /// </summary>
    public partial class FastVisualizerView : UserControl
    {
        public FastVisualizerView()
        {
            InitializeComponent();
        }

        private void ViewportOnMouseDown(object sender, RoutedEventArgs e)
        {
            var viewport = (Viewport3DX)sender;
            var nearestPoint = viewport.FindNearestPoint(((MouseDown3DEventArgs) e).Position);
            if (nearestPoint != null)
            {
                ((FastVisualizerViewModel) DataContext).ShowTooltip(nearestPoint.Value);
            }
        }
    }
}
