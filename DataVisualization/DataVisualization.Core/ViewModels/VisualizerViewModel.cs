using Caliburn.Micro;
using DataVisualization.Models;
using DataVisualization.Services;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Geared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace DataVisualization.Core.ViewModels
{
    public class VisualizerViewModel : Screen
    {
        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();

        private Func<double, string> _formatterX;
        public Func<double, string> FormatterX
        {
            get => _formatterX;
            set => SetValue(ref _formatterX, value);
        }

        private long _minX = 0;
        public long MinX
        {
            get => _minX;
            set => SetValue(ref _minX, value);
        }

        private long _maxX = 100;
        public long MaxX
        {
            get => _maxX;
            set => SetValue(ref _maxX, value);
        }

        private readonly DataFileReader _dataFileReader = new DataFileReader();
        private readonly DataService _dataService = new DataService();
        private readonly DataConfigurationService _dataConfigurationService = new DataConfigurationService();

        public void OnRangeChanged(long newMin, long newMax)
        {
            SetPoints(newMin, newMax);
        }

        private List<Series> _data;

        protected override async void OnActivate()
        {
            var config = _dataConfigurationService.Get(conf => conf.DataName.Equals("CsvData"));

            if (config == null)
                return;
            
            if (!_dataService.Exists(config.DataName))
            {
                var loadedData = await _dataFileReader.ReadDataAsync(config);
                _dataService.AddData(loadedData);
            }

            _data = _dataService.GetData(config.DataName).Series.ToList();

            FormatterX = val => new DateTime((long)val).ToString("MM/dd/yyyy");
            
            var rangeStart = new DateTime((long)_data[0].Values[0]);
            var rangeEnd = new DateTime((long)_data[0].Values[_data[0].Values.Count - 1]);
            MinX = rangeStart.Ticks;
            MaxX = rangeEnd.Ticks;

            SetPoints(MinX, MaxX);

            base.OnActivate();
        }

        private void SetPoints(long min, long max)
        {
            var dayConfig = Mappers.Xy<DateModel>()
                .X(dayModel => dayModel.DateTime.Ticks)
                .Y(dayModel => dayModel.Value);

            foreach (var series in SeriesCollection)
                series.Values.Clear();

            for (var index = 1; index < _data.Count; index++)
            {
                var dateRow = _data[0];
                var row = _data[index];

                var (minIndex, maxIndex) = GetMinAndMaxIndex(dateRow.Values, min, max);
                var increment = (maxIndex - minIndex) / 10000;

                if (increment < 1)
                    increment = 1;

                var tempList = new List<DateModel>();
                var values = new GearedValues<DateModel> { Quality = Quality.Low };
                for (var cellIndex = minIndex; cellIndex < maxIndex; cellIndex += increment)
                {
                    var x = new DateTime((long) dateRow.Values[cellIndex]);
                    tempList.Add(new DateModel
                    {
                        DateTime = x,
                        Value = row.Values[cellIndex]
                    });

                }
                values.AddRange(tempList);

                SeriesCollection.Add(new GLineSeries(dayConfig)
                {
                    Values = values,
                    Fill = Brushes.Transparent,
                    PointGeometry = null,
                    LineSmoothness = 0,
                    DataLabels = false,
                    Stroke = new SolidColorBrush(row.SeriesColor)
                });
            }
        }

        private (int minIndex, int maxIndex) GetMinAndMaxIndex(IList<double> dateRowValues, long min, long max)
        {
            var foundMin = 0;
            for (var index = 0; index < dateRowValues.Count; index++)
            {
                var item = dateRowValues[index];
                if (item > min)
                {
                    foundMin = index;
                    break;
                }
            }

            for (int index = foundMin; index < dateRowValues.Count; index++)
            {
                var item = dateRowValues[index];

                if (item > max)
                    return (foundMin, index);
            }

            return (foundMin, dateRowValues.Count);
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
