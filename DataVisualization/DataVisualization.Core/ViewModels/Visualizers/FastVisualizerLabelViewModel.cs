using System.Windows.Media;
using Caliburn.Micro;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;

namespace DataVisualization.Core.ViewModels.Visualizers
{
    public class FastVisualizerLabelViewModel : PropertyChangedBase
    {
        private BillboardSingleText3D _model;

        public BillboardSingleText3D Model
        {
            get => _model;
            set => Set(ref _model, value);
        }

        public FastVisualizerLabelViewModel(string text, Vector3 position)
        {
            Model = new BillboardSingleText3D
            {
                TextInfo = new TextInfo(text, position),
                FontColor = Colors.Black.ToColor4(),
                FontSize = 12,
                BackgroundColor = Colors.White.ToColor4(),
                FontStyle = System.Windows.FontStyles.Italic,
                Padding = new System.Windows.Thickness(2)
            };
        }

        public FastVisualizerLabelViewModel()
        {

        }
    }
}
