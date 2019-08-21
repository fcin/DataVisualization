using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Caliburn.Micro;
using DataVisualization.Models;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using Color = System.Windows.Media.Color;

namespace DataVisualization.Core.ViewModels.Visualizers
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
        public Series Series { get; }

        public FastVisualizerSeriesViewModel(Series series)
        {
            Series = series;

            var vertices = series.Values;

            var oldMinY = vertices.Where(v => !double.IsNaN(v)).Min();
            var oldMaxY = vertices.Where(v => !double.IsNaN(v)).Max();
            var newMinY = 0d;
            var newMaxY = 1d;

            var oldMaxX = (float)vertices.Count;
            var oldMinX = 0d;
            var newMaxX = 100d;
            var newMinX = 0d;

            var lineBuilder = new LineBuilder();

            for (var index = 1; index < vertices.Count; index++)
            {
                var lastVertex = vertices[index - 1];
                var vertex = vertices[index];

                var newOldValue = (((lastVertex - oldMinY) * (newMaxY - newMinY)) / (oldMaxY - oldMinY)) + newMinY;
                var newValue = (((vertex - oldMinY) * (newMaxY - newMinY)) / (oldMaxY - oldMinY)) + newMinY;

                var newOldValueX = (((index - 1 - oldMinX) * (newMaxX - newMinX)) / (oldMaxX - oldMinX)) + newMinX;
                var newValueX = (((index - oldMinX) * (newMaxX - newMinX)) / (oldMaxX - oldMinX)) + newMinX;

                lineBuilder.AddLine(new Vector3((float)newOldValueX, (float)newOldValue, 0), new Vector3((float)newValueX, (float)newValue, 0));
            }

            var lineGeometry = lineBuilder.ToLineGeometry3D();
            Model = lineGeometry;

            var selectedColor = (Color)(ColorConverter.ConvertFromString(series.ColorHex) ?? Colors.Black);
            Color = selectedColor;
            Transform = new TranslateTransform3D(0, 0, 0);
        }
    }
}
