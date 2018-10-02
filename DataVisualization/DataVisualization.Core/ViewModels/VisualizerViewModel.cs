using Caliburn.Micro;
using DataVisualization.Core.Events;
using DataVisualization.Core.Views;
using DataVisualization.Models;
using DataVisualization.Services;
using DataVisualization.Services.Extensions;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Series = DataVisualization.Models.Series;

namespace DataVisualization.Core.ViewModels
{
    public class VisualizerViewModel : Screen, IHandle<DataConfigurationOpenedEventArgs>, IHandle<BeforeDataConfigurationDeletedEventArgs>
    {
        private bool _isDisplayed;
        public bool IsDisplayed
        {
            get => _isDisplayed;
            set => SetValue(ref _isDisplayed, value);
        }

        private SeriesCollection _seriesCollection = new SeriesCollection();
        public SeriesCollection SeriesCollection
        {
            get => _seriesCollection;
            set => Set(ref _seriesCollection, value);
        }

        private readonly Formatter _formatter = new Formatter();
        public Func<double, string> FormatterX
        {
            get
            {
                var xLineType = _data.Series.First(d => d.Axis == Axes.X1).InternalType;
                var horizontalAxis = _data.Series.First(d => d.Axis == Axes.X1);
                var xValues = SeriesCollection.First().Values.OfType<DateModel>().Select(val => val.HorizontalAxis);
                return _formatter.GetFormat(xLineType, xValues);
            }
        }

        private double? _minX = null;
        public double? MinX
        {
            get => _minX;
            set => SetValue(ref _minX, value);
        }

        private double? _maxX = null;
        public double? MaxX
        {
            get => _maxX;
            set => SetValue(ref _maxX, value);
        }

        private double? _minX2 = null;
        public double? MinX2
        {
            get => _minX2;
            set => SetValue(ref _minX2, value);
        }

        private double? _maxX2 = null;
        public double? MaxX2
        {
            get => _maxX2;
            set => SetValue(ref _maxX2, value);
        }

        private IChartLegend _legend;
        public IChartLegend Legend
        {
            get => _legend;
            set => SetValue(ref _legend, value);
        }

        private bool _hasSecondaryAxis;
        public bool HasSecondaryAxis
        {
            get => _hasSecondaryAxis;
            set => SetValue(ref _hasSecondaryAxis, value);
        }

        private readonly ISeriesFactory _seriesFactory;
        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly DataFileReader _dataFileReader;
        private readonly DataService _dataService;
        private readonly DataConfigurationService _dataConfigurationService;
        private readonly GlobalSettings _globalSettings;
        private DataConfiguration _config;
        private Data _data;
        private CancellationTokenSource _cts;

        public VisualizerViewModel(ISeriesFactory seriesFactory, IWindowManager windowManager, IEventAggregator eventAggregator,
            DataService dataService, DataConfigurationService dataConfigurationService, GlobalSettings globalSettings, DataFileReader dataFileReader)
        {
            _seriesFactory = seriesFactory;
            _windowManager = windowManager;
            _eventAggregator = eventAggregator;
            _dataService = dataService;
            _dataConfigurationService = dataConfigurationService;
            _globalSettings = globalSettings;
            _cts = new CancellationTokenSource();
            _dataFileReader = dataFileReader;

            _eventAggregator.Subscribe(this);
        }

        public async void Handle(DataConfigurationOpenedEventArgs message)
        {
            if (_config != null && _config.DataName == message.Opened.DataName)
                return;

            if (_config != null)
            {
                SeriesCollection.Clear();
                _cts.Cancel();
                _cts = new CancellationTokenSource();
            }

            _config = _dataConfigurationService.GetByName(message.Opened.DataName);

            if (_config == null)
                return;

            IsDisplayed = true;

            _data = _dataService.GetData(_config.DataName);

            Legend = new BasicChartLegendView(_windowManager, _dataService, _data, currentSeries =>
            {
                _data = _dataService.GetData(_config.DataName);
                SeriesCollection.Clear();
                RecreateSeries();
            });

            var keepPullingTask = KeepPulling(_cts.Token);

            var horizontalAxis = _data.Series.First(d => d.Axis == Axes.X1);

            MinX = horizontalAxis.Values.First(val => val != double.NaN);
            MaxX = horizontalAxis.Values.Last(val => val != double.NaN);

            var hasSecondary = _data.Series.Any(d => d.Axis == Axes.X2 || d.Axis == Axes.Y2);
            if (hasSecondary)
            {
                HasSecondaryAxis = true;
                var secondaryHorizontalAxis = _data.Series.FirstOrDefault(d => d.Axis == Axes.X2) ?? _data.Series.First(d => d.Axis == Axes.X1);
                MinX2 = secondaryHorizontalAxis.Values.First(val => val != double.NaN);
                MaxX2 = secondaryHorizontalAxis.Values.Last(val => val != double.NaN);
            }

            RecreateSeries();

            await keepPullingTask;
        }

        public void Handle(BeforeDataConfigurationDeletedEventArgs message)
        {
            SeriesCollection.Clear();
            _cts.Cancel();
            _cts = new CancellationTokenSource();
            IsDisplayed = false;
            TryClose();
        }

        public void OnRangeChanged(long newMin, long newMax)
        {
            RecreateSeries();
        }

        private async Task KeepPulling(CancellationToken ct)
        {
            if (_config.RefreshRate == TimeSpan.Zero)
                return;

            while (!ct.IsCancellationRequested)
            {
                await Task.Delay(_config.RefreshRate);

                (List<Series> series, int readLines) = await _dataFileReader.ReadLatestAsync(_config, _data.FileLinesRead);

                if (readLines < 0)
                {
                    var result = MessageBox.Show("Number of rows decreased since last run. Do you want to reload it?", "Data changed", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        var loadedData = await _dataFileReader.ReadDataAsync(_config);
                        _data.Series = loadedData.Series;
                        _data.FileLinesRead = loadedData.FileLinesRead;
                        _dataService.UpdateData(_data);

                        var horizontalAxis = _data.Series.First(d => d.Axis == Axes.X1);

                        MinX = horizontalAxis.Values.First(val => val != double.NaN);
                        MaxX = horizontalAxis.Values.Last(val => val != double.NaN);

                        RecreateSeries();
                        continue;
                    }
                }

                if (series.Any() && series.Any(s => s.Values.Any()))
                {
                    foreach (var serie in series)
                    {
                        var index = _data.Series.Select((s, i) => new { item = s, index = i })
                                                .First(s => s.item.Name.Equals(serie.Name)).index;

                        foreach (var chunk in serie.Chunks)
                        {
                            _data.Series[index].AddToChunks(chunk.Chunk, _globalSettings.PointsCount);
                        }
                    }

                    _data.FileLinesRead += readLines;
                    _dataService.UpdateData(_data);

                    var horizontalAxis = _data.Series.First(d => d.Axis == Axes.X1);

                    MaxX = horizontalAxis.Values.Last(val => val != double.NaN);

                    RecreateSeries();
                }
            }
        }

        private void RecreateSeries()
        {
            var horizontalAxisSeries = _data.Series.First(d => d.Axis == Axes.X1);
            var allPrimarySeries = _data.Series.Where(s => s.Axis == Axes.Y1).ToList();

            var secondaryXseries = _data.Series.FirstOrDefault(d => d.Axis == Axes.X2) ?? _data.Series.First(d => d.Axis == Axes.X1);
            var allSecondarySeries = _data.Series.Where(s => s.Axis == Axes.Y2).ToList();

            var allPrimarySeriesViews = _seriesFactory.CreateSeriesViews(horizontalAxisSeries, allPrimarySeries, MinX, MaxX);
            var allSecondarySeriesViews = _seriesFactory.CreateSeriesViews(secondaryXseries, allSecondarySeries, MinX, MaxX);

            SeriesCollection.Clear();
            SeriesCollection.AddRange(allPrimarySeriesViews.Concat(allSecondarySeriesViews));

            NotifyOfPropertyChange(() => FormatterX);
        }

        private void SetValue<T>(ref T oldValue, T newValue, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
                return;

            oldValue = newValue;
            NotifyOfPropertyChange(propertyName);
        }

        public void OnZoomIn()
        {
            if (MinX == null || MaxX == null)
                return;

            var diff = (MaxX.Value - MinX.Value) * 0.25;

            if (diff <= 0)
                return;

            MinX += diff;
            MaxX -= diff;
            RecreateSeries();
        }

        public void OnZoomOut()
        {
            if (MinX == null || MaxX == null)
                return;

            var diff = (MaxX.Value - MinX.Value) * 0.25;

            MinX -= diff;
            MaxX += diff;
            RecreateSeries();
        }

        public void OnMoveLeft()
        {
            if (MinX == null || MaxX == null)
                return;

            var diff = (MaxX.Value - MinX.Value) * 0.1;

            MinX -= diff;
            MaxX -= diff;
            RecreateSeries();
        }

        public void OnMoveRight()
        {
            if (MinX == null || MaxX == null)
                return;

            var diff = (MaxX.Value - MinX.Value) * 0.1;

            MinX += diff;
            MaxX += diff;
            RecreateSeries();
        }

        public void OnCenterScreen()
        {
            var xAxisValues = _data.Series.First(s => s.Axis == Axes.X1).Values;
            MinX = xAxisValues.First(val => val != double.NaN);
            MaxX = xAxisValues.Last(val => val != double.NaN);
            RecreateSeries();
        }

        public void OnGoToNewest()
        {
            if (MinX == null || MaxX == null)
                return;

            var xAxisValues = _data.Series.First(s => s.Axis == Axes.X1).Values;
            MinX = xAxisValues.Skip(xAxisValues.Count - _globalSettings.PointsCount).First(val => val != double.NaN);
            MaxX = xAxisValues.Last(val => val != double.NaN);
            RecreateSeries();
        }
    }
}
