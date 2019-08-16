using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using Camera = HelixToolkit.Wpf.SharpDX.Camera;
using OrthographicCamera = HelixToolkit.Wpf.SharpDX.OrthographicCamera;
using Colors = System.Windows.Media.Colors;

namespace DataVisualization.Core.ViewModels
{
    public class FastVisualizerViewModel : VisualizerViewModelBase
    {
        public Camera Camera { get; set; }
        public LineGeometry3D Model { get; set; }
        public PhongMaterial Material { get; set; }
        public Transform3D Transform { get; set; }
        public EffectsManager EffectsManager { get; set; }

        public Vector3D DirectionalLightDirection { get; private set; }
        public System.Windows.Media.Color DirectionalLightColor { get; private set; }
        public System.Windows.Media.Color AmbientLightColor { get; private set; }
        public Stream BackgroundTexture { get; }

        public FastVisualizerViewModel()
        {
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

            var lineBuilder = new LineBuilder();
            var colors = new List<Color4>();
            for (int i = 0; i < 10000; i++)
            {
                lineBuilder.AddLine(new Vector3(i - 1, i - 1, 0), new Vector3(i, i, 0));
                colors.Add(Color.Black);
                colors.Add(Color.Black);
            }
            var lineGeometry = lineBuilder.ToLineGeometry3D();
            lineGeometry.Colors = new Color4Collection(colors);

            Model = lineGeometry;

            Material = PhongMaterials.Red;
            Transform = new TranslateTransform3D(0, 0, 0);

            BackgroundTexture =
                BitmapExtensions.CreateLinearGradientBitmapStream(EffectsManager, 128, 128, Direct2DImageFormat.Bmp,
                    new Vector2(0, 0), new Vector2(0, 128), new[]
                    {
                        new SharpDX.Direct2D1.GradientStop(){ Color = Colors.White.ToColor4(), Position = 0f },
                        new SharpDX.Direct2D1.GradientStop(){ Color = Colors.White.ToColor4(), Position = 1f }
                    });
        }
    }
}
