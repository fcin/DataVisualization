using Caliburn.Micro;
using DataVisualization.Models;
using DataVisualization.Services;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Geared;
using System;
using System.Linq;

namespace DataVisualization.Core.ViewModels
{
    public class VisualizerViewModel : Screen
    {
        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();
        public Func<double, string> FormatterX { get; set; }

        private readonly DataService _dataService = new DataService();
        private readonly DataConfigurationService _dataConfigurationService = new DataConfigurationService();

        protected override async void OnActivate()
        {
            var config = _dataConfigurationService.Get(conf => conf.DataName.Equals("SmallSample"));
            if (config == null)
                return;
            var data = (await _dataService.GetDataAsync(config)).ToList();

            var dayConfig = Mappers.Xy<DateModel>()
                .X(dayModel => (double)dayModel.DateTime.Ticks / TimeSpan.FromHours(1).Ticks)
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
                    Values = values
                });
            }

            FormatterX = val => new DateTime((long)val * TimeSpan.FromHours(1).Ticks).ToString("t");

            base.OnActivate();
        }
    }
}
