using Caliburn.Micro;
using DataVisualization.Core.Events;
using DataVisualization.Models;
using DataVisualization.Services;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System.IO;
using System.Linq;
using System.Windows.Media.Media3D;
using Camera = HelixToolkit.Wpf.SharpDX.Camera;
using Colors = System.Windows.Media.Colors;
using OrthographicCamera = HelixToolkit.Wpf.SharpDX.OrthographicCamera;

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

        private BindableCollection<FastVisualizerSeriesViewModel> _seriesVms;

        public BindableCollection<FastVisualizerSeriesViewModel> SeriesVms
        {
            get => _seriesVms;
            set => Set(ref _seriesVms, value);
        }
        public BindableCollection<FastVisualizerAxisViewModel> AxesVms { get; set; }

        public EffectsManager EffectsManager { get; set; }

        public Vector3D DirectionalLightDirection { get; }
        public System.Windows.Media.Color DirectionalLightColor { get; }
        public System.Windows.Media.Color AmbientLightColor { get; }
        public Stream BackgroundTexture { get; }

        public BindableCollection<FastVisualizerTooltipViewModel> TooltipVm { get; set; }

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
            TooltipVm = new BindableCollection<FastVisualizerTooltipViewModel>();

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
            if (!(message?.Opened is LineChartDataConfiguration))
                return;

            SeriesVms = new BindableCollection<FastVisualizerSeriesViewModel>();

            var data = _dataService.GetData<ChartData>(message.Opened.DataName);

            foreach (var dataSeries in data.Series.Where(s => s.Axis != Axes.X1 && s.Axis != Axes.X2))
            {
                var seriesVm = new FastVisualizerSeriesViewModel(dataSeries);
                SeriesVms.Add(seriesVm);
            }

            var primaryAxisX = data.Series.First(s => s.Axis == Axes.X1);
            var axisXVm = new FastVisualizerAxisViewModel(primaryAxisX, Camera);
            AxesVms.Add(axisXVm);

            var orderedValues = data.Series.Where(s => s.Axis != Axes.X1 && s.Axis != Axes.X2 && !s.IsDateTime).SelectMany(s => s.Values)
                .OrderByDescending(g => g).ToList();

            var primaryAxisY = data.Series.First(s => s.Axis == Axes.Y1);
            var axisYVm = new FastVisualizerAxisViewModel(primaryAxisY, Camera, orderedValues);
            AxesVms.Add(axisYVm);

            var vm = new FastVisualizerTooltipViewModel(SeriesVms.ToList());
            TooltipVm = new BindableCollection<FastVisualizerTooltipViewModel>(new[] { vm });
            NotifyOfPropertyChange(() => TooltipVm);
        }

        public void ShowTooltip(Point3D point)
        {
            TooltipVm.First().ShowTooltip(point);
        }
    }
}
