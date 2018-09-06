using Caliburn.Micro;
using DataVisualization.Core.Views;
using DataVisualization.Models;
using DataVisualization.Services;
using LiveCharts;
using LiveCharts.Geared;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Series = DataVisualization.Models.Series;

namespace DataVisualization.Core.ViewModels
{
    public class VisualizerViewModel : Screen
    {
        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();

        public Func<double, string> FormatterX
        {
            get
            {
                var xLineType = _data?.Series?.First(d => d.Axis == Axes.X1).InternalType;

                if (xLineType == null || xLineType == ColumnTypeDef.Number)
                    return val => val.ToString(CultureInfo.CurrentCulture);
                else if (xLineType == ColumnTypeDef.Datetime)
                    return val => new DateTime((long)val).ToString("MM/dd/yyyy");
                else
                    throw new ArgumentException("Unsupported type");
            }
        }

        private long? _minX = null;
        public long? MinX
        {
            get => _minX;
            set => SetValue(ref _minX, value);
        }

        private long? _maxX = null;
        public long? MaxX
        {
            get => _maxX;
            set => SetValue(ref _maxX, value);
        }

        private long? _minX2 = null;
        public long? MinX2
        {
            get => _minX2;
            set => SetValue(ref _minX2, value);
        }

        private long? _maxX2 = null;
        public long? MaxX2
        {
            get => _maxX2;
            set => SetValue(ref _maxX2, value);
        }

        public IChartLegend Legend { get; set; }

        private bool _hasSecondaryAxis;
        public bool HasSecondaryAxis
        {
            get => _hasSecondaryAxis;
            set => SetValue(ref _hasSecondaryAxis, value);
        }

        private readonly ISeriesFactory _seriesFactory;
        private readonly DataFileReader _dataFileReader = new DataFileReader();
        private readonly DataService _dataService = new DataService();
        private readonly DataConfigurationService _dataConfigurationService = new DataConfigurationService();
        private DataConfiguration _config;
        private Data _data;

        public VisualizerViewModel(ISeriesFactory seriesFactory)
        {
            _seriesFactory = seriesFactory;
            Legend = new BasicChartLegendView();
        }

        public void OnRangeChanged(long newMin, long newMax)
        {
            RecreateSeries();
        }

        protected override async void OnActivate()
        {
            _config = _dataConfigurationService.GetByName("KeepUpdating");

            if (_config == null)
                return;

            if (!_dataService.Exists(_config.DataName))
            {
                var loadedData = await _dataFileReader.ReadDataAsync(_config);
                _dataService.AddData(loadedData);
            }

            _data = _dataService.GetData(_config.DataName);
            var keepPullingTask = KeepPulling();

            var horizontalAxis = _data.Series.First(d => d.Axis == Axes.X1);

            MinX = (long)horizontalAxis.Values[0];
            MaxX = (long)horizontalAxis.Values[horizontalAxis.Values.Count - 1];

            var hasSecondary = _data.Series.Any(d => d.Axis == Axes.X2 || d.Axis == Axes.Y2);
            if (hasSecondary)
            {
                HasSecondaryAxis = true;
                var secondaryHorizontalAxis = _data.Series.FirstOrDefault(d => d.Axis == Axes.X2) ?? _data.Series.First(d => d.Axis == Axes.X1);
                MinX2 = (long)secondaryHorizontalAxis.Values[0];
                MaxX2 = (long)secondaryHorizontalAxis.Values[secondaryHorizontalAxis.Values.Count - 1];
            }

            RecreateSeries();

            await keepPullingTask;
        }

        private async Task KeepPulling()
        {
            while (true)
            {
                await Task.Delay(3000);
                (List<Series> series, int readLines) = await _dataFileReader.ReadLatest(_config, _data.FileLinesRead);
                if (series.Any() && series.Any(s => s.Values.Any()))
                {
                    _data.FileLinesRead += readLines;
                    foreach (var serie in series)
                    {
                        var index = _data.Series.Select((s, i) => new { item = s, index = i })
                                                .First(s => s.item.Name.Equals(serie.Name)).index;
                        foreach (var row in serie.Values)
                            _data.Series[index].Values.Add(row);
                    }

                    var horizontalAxis = _data.Series.First(d => d.Axis == Axes.X1);
                    MaxX = (long)horizontalAxis.Values[horizontalAxis.Values.Count - 1];

                    RecreateSeries();
                }
            }

        }

        private void RecreateSeries()
        {
            var allSeriesCount = _data.Series.Count(d => d.Axis == Axes.Y1 || d.Axis == Axes.Y2);

            {
                var horizontalAxisSeries = _data.Series.First(d => d.Axis == Axes.X1);
                var allPrimarySeries = _data.Series.Where(s => s.Axis == Axes.Y1).ToList();
                foreach (var series in allPrimarySeries)
                {
                    var points = _seriesFactory.CreateSeriesPoints(horizontalAxisSeries, series, MinX, MaxX).ToList();

                    AddSeriesToCollection(allSeriesCount, points, series);
                }
            }

            if (HasSecondaryAxis)
            {
                var secondaryXseries = _data.Series.FirstOrDefault(d => d.Axis == Axes.X2) ?? _data.Series.First(d => d.Axis == Axes.X1);
                var allSecondarySeries = _data.Series.Where(s => s.Axis == Axes.Y2).ToList();
                foreach (var series in allSecondarySeries)
                {
                    var points = _seriesFactory.CreateSeriesPoints(secondaryXseries, series, MinX2, MaxX2).ToList();

                    AddSeriesToCollection(allSeriesCount, points, series);
                }
            }
        }

        private void AddSeriesToCollection(int allSeriesCount, IEnumerable<DateModel> points, Series series)
        {
            if (SeriesCollection.Count == allSeriesCount)
            {
                var gearedValues = new GearedValues<DateModel> { Quality = Quality.Low };
                gearedValues.AddRange(points);
                SeriesCollection.First(s => s.Title == series.Name).Values = gearedValues;
            }
            else
            {
                var lineSeries = _seriesFactory.CreateLineSeries(points, series);
                SeriesCollection.Add(lineSeries);
            }
        }

        private void SetValue<T>(ref T oldValue, T newValue, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
                return;

            oldValue = newValue;
            NotifyOfPropertyChange(propertyName);
        }
    }
}
