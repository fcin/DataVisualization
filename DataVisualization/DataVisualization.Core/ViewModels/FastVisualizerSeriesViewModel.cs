using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Caliburn.Micro;
using HelixToolkit.Wpf.SharpDX;

namespace DataVisualization.Core.ViewModels
{
    public class FastVisualizerSeriesViewModel : PropertyChangedBase
    {
        private LineGeometry3D _model;

        public LineGeometry3D Model
        {
            get => _model;
            set => Set(ref _model, value);
        }
        public Color Color { get; set; }
        public Transform3D Transform { get; set; }

        public FastVisualizerSeriesViewModel()
        {
            
        }

        public FastVisualizerSeriesViewModel(LineGeometry3D model, Color color, Transform3D transform)
        {
            Model = model;
            Color = color;
            Transform = transform;
        }
    }
}
