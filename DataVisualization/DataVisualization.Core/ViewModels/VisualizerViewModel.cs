using System.Threading.Tasks;
using Caliburn.Micro;
using DataVisualization.Services;
using LiveCharts;
using LiveCharts.Definitions.Series;
using LiveCharts.Geared;

namespace DataVisualization.Core.ViewModels
{
    public class VisualizerViewModel : Screen
    {
        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();

        private int _fromY = -10;
        public int FromY
        {
            get => _fromY;
            private set { _fromY = value; NotifyOfPropertyChange(() => FromY); }
        }

        private int _toY = 10;
        public int ToY
        {
            get => _toY;
            private set { _toY = value; NotifyOfPropertyChange(() => ToY); }
        }

        private int _fromX = -10;
        public int FromX
        {
            get => _fromX;
            private set { _fromX = value; NotifyOfPropertyChange(() => FromX); }
        }

        private int _toX = 10;
        public int ToX
        {
            get => _toX;
            private set { _toX = value; NotifyOfPropertyChange(() => ToX); }
        }

        private readonly DataService _dataService = new DataService();

        protected override async void OnActivate()
        {
            var data = await _dataService.GetDataAsync();
            FromY = (int)_dataService.Lowest / 2;
            ToY = (int)_dataService.Highest * 2;
            FromX = -10;
            ToX = _dataService.AverageCount + 10;

            SeriesCollection.AddRange(new ISeriesView[]
            {
                new GLineSeries
                {
                    Values = new GearedValues<double>(data) {Quality = Quality.Low },
                    LineSmoothness = 0
                }
            });

            base.OnActivate();
        }
    }
}
