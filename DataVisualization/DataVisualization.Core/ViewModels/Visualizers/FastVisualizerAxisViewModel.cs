using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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

        public FastVisualizerAxisViewModel(Series series, Camera camera, List<double> orderedValues = null)
        {
            _camera = camera;
            LabelVms = new BindableCollection<FastVisualizerLabelViewModel>();

            if (series.Axis == Axes.X1)
            {
                var min = series.Values.First(val => !double.IsNaN(val));
                var max = series.Values.Last(val => !double.IsNaN(val));

                var nearestMin = series.Values.Where(val => !double.IsNaN(val)).Aggregate((x, y) => Math.Abs(x - min) < Math.Abs(y - min) ? x : y);
                var nearestMax = series.Values.Where(val => !double.IsNaN(val)).Aggregate((x, y) => Math.Abs(x - max) < Math.Abs(y - max) ? x : y);
                var minIndex = series.Values.IndexOf(nearestMin);
                var maxIndex = series.Values.IndexOf(nearestMax) + 1; // inclusive
                var diff = maxIndex - minIndex;
                var increment = (int)Math.Max(diff / 100.0d, 1);

                var oldMinX = nearestMin;
                var oldMaxX = nearestMax;
                var newMinX = 0d;
                var newMaxX = 100d;
            
                for (int index = 0; index < series.Values.Count; index += increment)
                {
                    var seriesValue = series.Values[index];
                    var newValueX = (((seriesValue - oldMinX) * (newMaxX - newMinX)) / (oldMaxX - oldMinX)) + newMinX;

                
                    var actualValue = new DateTime((long)seriesValue);
                    var labelVm = new FastVisualizerLabelViewModel(actualValue.ToString(CultureInfo.CurrentCulture), new Vector3((float)newValueX, 0, 0));
                    LabelVms.Add(labelVm);
                }

                var axisLineBuilder = new LineBuilder();
                axisLineBuilder.AddLine(new Vector3(0, 0, 0), new Vector3(100, 0, 0));

                var axisLineGeometry = axisLineBuilder.ToLineGeometry3D();
                Model = axisLineGeometry;
            }
            else if(series.Axis == Axes.Y1)
            {
                if(orderedValues == null)
                    throw new ArgumentNullException(nameof(orderedValues));

                var axisLineBuilder = new LineBuilder();
                axisLineBuilder.AddLine(new Vector3(0, 1, 0), new Vector3(0, 0, 0));

                var axisLineGeometry = axisLineBuilder.ToLineGeometry3D();
                Model = axisLineGeometry;

                const int labelsCount = 10;
                var interval = orderedValues.Count / labelsCount;
                
                for (int index = 1; index < labelsCount; index++)
                {
                    var firstIndex = interval * index;
                    var labelAverage = orderedValues[firstIndex];
                    var labelVm = new FastVisualizerLabelViewModel(labelAverage.ToString("F2"), new Vector3(-0.025f, 1 - index / 10f, 0));
                    LabelVms.Add(labelVm);
                }
            }
        }
    }
}
