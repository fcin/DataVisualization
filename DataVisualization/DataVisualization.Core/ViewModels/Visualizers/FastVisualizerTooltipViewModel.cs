using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Caliburn.Micro;
using DataVisualization.Models;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using TextInfo = HelixToolkit.Wpf.SharpDX.TextInfo;

namespace DataVisualization.Core.ViewModels.Visualizers
{
    public class FastVisualizerTooltipViewModel : PropertyChangedBase
    {

        private LineGeometry3D _model;

        public LineGeometry3D Model
        {
            get => _model;
            set => Set(ref _model, value);
        }

        public BindableCollection<BillboardSingleText3D> TooltipLabels { get; set; }

        //private readonly Dictionary<double, List<NearestPoint>> _nearestPoints;
        private readonly List<FastVisualizerSeriesViewModel> _seriesVms;


        public FastVisualizerTooltipViewModel(List<FastVisualizerSeriesViewModel> seriesVms)
        {
            Model = new LineGeometry3D();
            TooltipLabels = new BindableCollection<BillboardSingleText3D>();
            _seriesVms = seriesVms;
            //_nearestPoints = series.SelectMany(s => s.Values.Select(v => new NearestPoint(s.Name, v)))
            //    .Distinct()
            //    .GroupBy(g => g.Value, g => g)
            //    .ToDictionary(g => g.Key, g => g.ToList());
        }

        public void ShowTooltip(Point3D point)
        {
            TooltipLabels.Clear();

            var lineBuilder = new LineBuilder();
            lineBuilder.AddLine(new Vector3((float)point.X, 1, 0), new Vector3((float)point.X, 0, 0));
            var lineGeometry = lineBuilder.ToLineGeometry3D();
            Model = lineGeometry;
            NotifyOfPropertyChange(() => Model);

            var points = _seriesVms
                .SelectMany(s => s.Model.Lines
                    .Select((l, index) => new NearestPoint(s.Series.Name, l.P0, s.Series.Values[index])))
                .GroupBy(g => g.Value.X, g => g).ToDictionary(g => g.Key, g => g);

            if (points.TryGetValue((int) point.X, out var nearestPoints))
            {
                var index = 0;
                foreach (var nearestPoint in nearestPoints)
                {
                    var name = $"{nearestPoint.SeriesName}: {nearestPoint.OriginalY:F2}";
                    var billboard = new BillboardSingleText3D
                    {
                        TextInfo = new TextInfo(name, new Vector3((float)point.X, 1 - (0.05f * index), 1)),
                        FontColor = Colors.Black.ToColor4(),
                        FontSize = 12,
                        BackgroundColor = Colors.White.ToColor4(),
                        FontStyle = System.Windows.FontStyles.Italic,
                        Padding = new System.Windows.Thickness(2)
                    };
                    TooltipLabels.Add(billboard);
                    index++;
                }
            }

            NotifyOfPropertyChange(() => TooltipLabels);
        }
    }

    public class NearestPoint
    {

        public string SeriesName { get; set; }
        public Vector3 Value { get; set; }
        public double OriginalY { get; set; }

        public NearestPoint(string seriesName, Vector3 value, double originalY)
        {
            SeriesName = seriesName;
            Value = value;
            OriginalY = originalY;
        }
        
    }
}
