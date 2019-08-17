using System;
using System.Linq;
using Caliburn.Micro;
using DataVisualization.Models;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using Camera = HelixToolkit.Wpf.SharpDX.Camera;

namespace DataVisualization.Core.ViewModels.Visualizers
{
    public class FastVisualizerAxisViewModel : PropertyChangedBase
    {
        private LineGeometry3D _model;

        public LineGeometry3D Model
        {
            get => _model;
            set => Set(ref _model, value);
        }

        public BindableCollection<FastVisualizerLabelViewModel> LabelVms { get; set; }

        private readonly Camera _camera;

        public FastVisualizerAxisViewModel(Series series, Camera camera)
        {
            _camera = camera;
            LabelVms = new BindableCollection<FastVisualizerLabelViewModel>();

            var min = series.Values.First(val => !double.IsNaN(val));
            var max = series.Values.Last(val => !double.IsNaN(val));

            var nearestMin = series.Values.Where(val => !double.IsNaN(val)).Aggregate((x, y) => Math.Abs(x - min) < Math.Abs(y - min) ? x : y);
            var nearestMax = series.Values.Where(val => !double.IsNaN(val)).Aggregate((x, y) => Math.Abs(x - max) < Math.Abs(y - max) ? x : y);
            var minIndex = series.Values.IndexOf(nearestMin);
            var maxIndex = series.Values.IndexOf(nearestMax) + 1; // inclusive
            var diff = maxIndex - minIndex;
            var increment = (int)Math.Max(diff / 100.0d, 1);
            var axisLineBuilder = new LineBuilder();

            var oldMinX = nearestMin;
            var oldMaxX = nearestMax;
            var newMinX = 0d;
            var newMaxX = 100d;
            
            for (int index = 0; index < series.Values.Count; index += increment)
            {
                var seriesValue = series.Values[index];
                var newValueX = (((seriesValue - oldMinX) * (newMaxX - newMinX)) / (oldMaxX - oldMinX)) + newMinX;

                
                var actualValue = new DateTime((long)seriesValue);
                var labelVm = new FastVisualizerLabelViewModel(actualValue.ToString(), (float)newValueX);
                LabelVms.Add(labelVm);
            }

            axisLineBuilder.AddLine(new Vector3(0, 0, 0), new Vector3(100, 0, 0));

            var axisLineGeometry = axisLineBuilder.ToLineGeometry3D();
            Model = axisLineGeometry;
        }
    }
}
