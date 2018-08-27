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
using Series = DataVisualization.Models.Series;

namespace DataVisualization.Core.ViewModels
{
    public class VisualizerViewModel : Screen
    {
        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();

        private Func<double, string> _formatterX = null;
        public Func<double, string> FormatterX
        {
            get
            {
                if (_formatterX == null)
                {
                    var xLineType = _data?.First(d => d.Axis == Axes.X1).InternalType;
                    if (xLineType == null)
                        return val => val.ToString(CultureInfo.CurrentCulture);
                    switch (xLineType)
                    {
                        case "System.Double":
                            _formatterX = val => val.ToString(CultureInfo.CurrentCulture);
                            break;
                        case "System.DateTime":
                            _formatterX = val => new DateTime((long)val).ToString("MM/dd/yyyy");
                            break;
                        default:
                            throw new ArgumentException("Unsupported type");
                    }
                }

                return _formatterX;
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
        private List<Series> _data;

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
            var config = _dataConfigurationService.GetByName("CsvData");

            if (config == null)
                return;

            if (!_dataService.Exists(config.DataName))
            {
                var loadedData = await _dataFileReader.ReadDataAsync(config);
                _dataService.AddData(loadedData);
            }

            _data = _dataService.GetData(config.DataName).Series.ToList();

            var horizontalAxis = _data.First(d => d.Axis == Axes.X1);

            MinX = (long)horizontalAxis.Values[0];
            MaxX = (long)horizontalAxis.Values[horizontalAxis.Values.Count - 1];

            var hasSecondary = _data.Any(d => d.Axis == Axes.X2 || d.Axis == Axes.Y2);
            if (hasSecondary)
            {
                HasSecondaryAxis = true;
                var secondaryHorizontalAxis = _data.FirstOrDefault(d => d.Axis == Axes.X2) ?? _data.First(d => d.Axis == Axes.X1);
                MinX2 = (long)secondaryHorizontalAxis.Values[0];
                MaxX2 = (long)secondaryHorizontalAxis.Values[secondaryHorizontalAxis.Values.Count - 1];
            }

            RecreateSeries();

            base.OnActivate();
        }

        private void RecreateSeries()
        {
            var allSeriesCount = _data.Count(d => d.Axis == Axes.Y1 || d.Axis == Axes.Y2);

            {
                var horizontalAxisSeries = _data.First(d => d.Axis == Axes.X1);
                var allPrimarySeries = _data.Where(s => s.Axis == Axes.Y1).ToList();
                foreach (var series in allPrimarySeries)
                {
                    var points = _seriesFactory.CreateSeriesPoints(horizontalAxisSeries, series, MinX, MaxX).ToList();

                    AddSeriesToCollection(allSeriesCount, points, series);
                }
            }

            if (HasSecondaryAxis)
            {
                var secondaryXseries = _data.FirstOrDefault(d => d.Axis == Axes.X2) ?? _data.First(d => d.Axis == Axes.X1);
                var allSecondarySeries = _data.Where(s => s.Axis == Axes.Y2).ToList();
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
