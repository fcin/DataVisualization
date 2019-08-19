using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Caliburn.Micro;
using DataVisualization.Core.Events;
using DataVisualization.Models;
using DataVisualization.Services;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using Camera = HelixToolkit.Wpf.SharpDX.Camera;
using Color = SharpDX.Color;
using OrthographicCamera = HelixToolkit.Wpf.SharpDX.OrthographicCamera;
using Colors = System.Windows.Media.Colors;

namespace DataVisualization.Core.ViewModels.Visualizers
{
    public class FastVisualizerViewModel : VisualizerViewModelBase, IHandle<DataConfigurationOpenedEventArgs>
    {
        private readonly DataService _dataService;

        private Camera _camera;

        public Camera Camera
        {
            get => _camera;
            set => Set(ref _camera, value);
        }

        public BindableCollection<FastVisualizerSeriesViewModel> SeriesVms { get; set; }
        public BindableCollection<FastVisualizerAxisViewModel> AxesVms { get; set; }

        public EffectsManager EffectsManager { get; set; }

        public Vector3D DirectionalLightDirection { get; }
        public System.Windows.Media.Color DirectionalLightColor { get; }
        public System.Windows.Media.Color AmbientLightColor { get; }
        public Stream BackgroundTexture { get; }

        public FastVisualizerViewModel(IEventAggregator eventAggregator, DataService dataService)
        {
            eventAggregator.Subscribe(this);
            _dataService = dataService;

            Camera = new OrthographicCamera()
            {
                Position = new Point3D(0, 0, 0),
                FarPlaneDistance = 5000000,
                Width = 1
            };
            EffectsManager = new DefaultEffectsManager();
            SeriesVms = new BindableCollection<FastVisualizerSeriesViewModel>();
            AxesVms = new BindableCollection<FastVisualizerAxisViewModel>();

            AmbientLightColor = Colors.DimGray;
            DirectionalLightColor = Colors.White;
            DirectionalLightDirection = new Vector3D(-2, -5, -2);

            BackgroundTexture =
                BitmapExtensions.CreateLinearGradientBitmapStream(EffectsManager, 128, 128, Direct2DImageFormat.Bmp,
                    new Vector2(0, 0), new Vector2(0, 128), new[]
                    {
                        new SharpDX.Direct2D1.GradientStop(){ Color = Colors.White.ToColor4(), Position = 0f },
                        new SharpDX.Direct2D1.GradientStop(){ Color = Colors.White.ToColor4(), Position = 1f }
                    });
        }

        public void Handle(DataConfigurationOpenedEventArgs message)
        {
            SeriesVms.Clear();
            var data = _dataService.GetData(message.Opened.DataName);
            
            foreach (var dataSeries in data.Series.Where(s => s.Axis != Axes.X1 && s.Axis != Axes.X2))
            {
                var vertices = dataSeries.Values;
                
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
                
                var selectedColor = (System.Windows.Media.Color) (ColorConverter.ConvertFromString(dataSeries.ColorHex) ?? Colors.Black);
                var seriesVm = new FastVisualizerSeriesViewModel(lineGeometry, selectedColor, new TranslateTransform3D(0, 0, 0));
                SeriesVms.Add(seriesVm);
            }

            var primaryAxisX = data.Series.First(s => s.Axis == Axes.X1);
            var axisXVm = new FastVisualizerAxisViewModel(primaryAxisX, Camera);
            AxesVms.Add(axisXVm);

            var heighestValue = data.Series.Where(s => s.Axis == Axes.Y1).SelectMany(s => s.Values)
                .Where(v => !double.IsNaN(v)).Max();
            var lowestValue = data.Series.Where(s => s.Axis == Axes.Y1).SelectMany(s => s.Values)
                .Where(v => !double.IsNaN(v)).Min();

            var orderedValues = data.Series.Where(s => s.Axis != Axes.X1 && s.Axis != Axes.X2 && !s.IsDateTime).SelectMany(s => s.Values)
                .OrderByDescending(g => g).ToList();

            var primaryAxisY = data.Series.First(s => s.Axis == Axes.Y1);
            var axisYVm = new FastVisualizerAxisViewModel(primaryAxisY, Camera, orderedValues);
            AxesVms.Add(axisYVm);
        }
    }
}
