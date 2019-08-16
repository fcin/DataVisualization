using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Media3D;
using Caliburn.Micro;
using DataVisualization.Core.Events;
using DataVisualization.Services;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using Camera = HelixToolkit.Wpf.SharpDX.Camera;
using OrthographicCamera = HelixToolkit.Wpf.SharpDX.OrthographicCamera;
using Colors = System.Windows.Media.Colors;

namespace DataVisualization.Core.ViewModels
{
    public class FastVisualizerViewModel : VisualizerViewModelBase, IHandle<DataConfigurationOpenedEventArgs>
    {
        private Camera _camera;

        public Camera Camera
        {
            get => _camera;
            set => Set(ref _camera, value);
        }

        private LineGeometry3D _model;
        private DataService _dataService;

        public LineGeometry3D Model
        {
            get => _model;
            set => Set(ref _model, value);
        }
        public PhongMaterial Material { get; set; }
        public Transform3D Transform { get; set; }
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
            var data = _dataService.GetData(message.Opened.DataName);


            var lineBuilder = new LineBuilder();
            var colors = new List<Color4>();
            foreach (var dataSeries in data.Series)
            {
                var vertices = dataSeries.Values;
                
                var oldMinY = vertices.Where(v => !double.IsNaN(v)).Min();
                var oldMaxY = vertices.Where(v => !double.IsNaN(v)).Max();
                var newMinY = -1d;
                var newMaxY = 1d;

                var oldMaxX = (float)vertices.Count;
                var oldMinX = 0d;
                var newMaxX = 100d;
                var newMinX = 0d;

                for (var index = 1; index < vertices.Count; index++)
                {
                    var lastVertex = vertices[index - 1];
                    var vertex = vertices[index];

                    var newOldValue = (((lastVertex - oldMinY) * (newMaxY - newMinY)) / (oldMaxY - oldMinY)) + newMinY;
                    var newValue = (((vertex - oldMinY) * (newMaxY - newMinY)) / (oldMaxY - oldMinY)) + newMinY;

                    var newOldValueX = (((index - 1 - oldMinX) * (newMaxX - newMinX)) / (oldMaxX - oldMinX)) + newMinX;
                    var newValueX = (((index - oldMinX) * (newMaxX - newMinX)) / (oldMaxX - oldMinX)) + newMinX;

                    lineBuilder.AddLine(new Vector3((float)newOldValueX, (float)newOldValue, 0), new Vector3((float)newValueX, (float)newValue, 0));
                    colors.Add(Color.Black);
                    colors.Add(Color.Black);
                }
            }

            var lineGeometry = lineBuilder.ToLineGeometry3D();
            lineGeometry.Colors = new Color4Collection(colors);

            Model = lineGeometry;

            Material = PhongMaterials.Red;
            Transform = new TranslateTransform3D(0, 0, 0);
        }
    }
}
