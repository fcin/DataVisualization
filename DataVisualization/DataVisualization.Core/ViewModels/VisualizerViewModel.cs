using Caliburn.Micro;
using DataVisualization.Models;
using DataVisualization.Services;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Geared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
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
            set
            {
                _formatterX = value;
                NotifyOfPropertyChange(() => FormatterX);
            }
        }

        private long _minX = 0;
        public long MinX
        {
            get => _minX;
            set
            {
                _minX = value;
                NotifyOfPropertyChange(() => MinX);
            }
        }

        private long _maxX = 100;
        public long MaxX
        {
            get => _maxX;
            set
            {
                _maxX = value;
                NotifyOfPropertyChange(() => MaxX);
            }
        }

        private readonly DataFileReader _dataFileReader = new DataFileReader();
        private readonly DataService _dataService = new DataService();
        private readonly DataConfigurationService _dataConfigurationService = new DataConfigurationService();

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

            var data = _dataService.GetData(config.DataName).Series.ToList();

            var dayConfig = Mappers.Xy<DateModel>()
                .X(dayModel => dayModel.DateTime.Ticks)
                .Y(dayModel => dayModel.Value);

            for (var index = 1; index < data.Count; index++)
            {
                var dateRow = data[0];
                var row = data[index];

                var values = new GearedValues<DateModel>();
                for (var cellIndex = 0; cellIndex < row.Values.Count; cellIndex++)
                {
                    values.Add(new DateModel
                    {
                        DateTime = new DateTime((long)dateRow.Values[cellIndex]),
                        Value = row.Values[cellIndex]
                    });
                    values.Quality = Quality.Low;
                }

                SeriesCollection.Add(new GLineSeries(dayConfig)
                {
                    Values = values,
                    Fill = Brushes.Transparent,
                    PointGeometry = null,
                    LineSmoothness = 0,
                    DataLabels = false
                });
            }

            FormatterX = val => new DateTime((long)val).ToString("MM/dd/yyyy");
            
            var rangeStart = new DateTime((long)data[0].Values[Math.Max(data[0].Values.Count - 10000, 0)]);
            var rangeEnd = new DateTime((long)data[0].Values[data[0].Values.Count - 1]);
            MinX = rangeStart.Ticks;
            MaxX = rangeEnd.Ticks;

            base.OnActivate();
        }
    }
}
