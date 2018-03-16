using Caliburn.Micro;
using DataVisualization.Models;
using DataVisualization.Services;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Geared;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private long _minX;
        public long MinX
        {
            get => _minX;
            set
            {
                _minX = value;
                NotifyOfPropertyChange(() => MinX);
            }
        }

        private long _maxX;
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

            List<List<object>> data;
            if (!_dataService.Exists(config.DataName))
            {
                data = (await _dataFileReader.ReadDataAsync(config)).ToList();
                _dataService.AddData(new Data { Name = config.DataName, Values = data });
            }
            else
            {
                data = _dataService.GetData(config.DataName).Values.Select(s => s.ToList()).ToList();
            }

            var dayConfig = Mappers.Xy<DateModel>()
                .X(dayModel => (double)dayModel.DateTime.Ticks / TimeSpan.FromMinutes(1).Ticks)
                .Y(dayModel => dayModel.Value);

            for (var index = 1; index < data.Count; index++)
            {
                var dateRow = data[0];
                var row = data[index];

                var values = new GearedValues<DateModel>();
                for (var cellIndex = 0; cellIndex < row.Count; cellIndex++)
                {
                    values.Add(new DateModel
                    {
                        DateTime = (DateTime)dateRow[cellIndex],
                        Value = (double)row[cellIndex]
                    });
                    values.Quality = Quality.Low;
                }

                SeriesCollection.Add(new GLineSeries(dayConfig)
                {
                    Values = values,
                    Fill = Brushes.Transparent,
                    PointGeometry = null
                });
            }

            FormatterX = val => new DateTime((long)val * TimeSpan.FromMinutes(1).Ticks).ToString("MM/dd/yyyy");
            MinX = ((DateTime)data[0][0]).Ticks / TimeSpan.FromMinutes(1).Ticks;
            var maxLength = data[0].Count;
            var max = ((DateTime)data[0][maxLength - 1]).Ticks / TimeSpan.FromMinutes(1).Ticks;
            MaxX = max - ((max - MinX) / 100) * 95;

            base.OnActivate();
        }
    }
}
