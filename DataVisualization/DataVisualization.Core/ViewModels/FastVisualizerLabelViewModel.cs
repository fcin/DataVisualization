using System.Windows.Media;
using System.Windows.Media.Media3D;
using Caliburn.Micro;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;

namespace DataVisualization.Core.ViewModels
{
    public class FastVisualizerLabelViewModel : PropertyChangedBase
    {
        private BillboardSingleText3D _model;

        public BillboardSingleText3D Model
        {
            get => _model;
            set => Set(ref _model, value);
        }

        public FastVisualizerLabelViewModel(string text, float offsetX)
        {
            Model = new BillboardSingleText3D
            {
                TextInfo = new TextInfo(text, new Vector3(offsetX, 0, 0)),
                FontColor = Colors.Blue.ToColor4(),
                FontSize = 12,
                BackgroundColor = Colors.Plum.ToColor4(),
                FontStyle = System.Windows.FontStyles.Italic,
                Padding = new System.Windows.Thickness(2)
            };
        }

        public FastVisualizerLabelViewModel()
        {

        }
    }
}
