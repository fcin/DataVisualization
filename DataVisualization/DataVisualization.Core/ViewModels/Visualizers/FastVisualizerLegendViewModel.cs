using Caliburn.Micro;
using DataVisualization.Core.Events;
using DataVisualization.Models;
using DataVisualization.Services;
using System.Collections.Generic;
using System.Linq;

namespace DataVisualization.Core.ViewModels.Visualizers
{
    public class FastVisualizerLegendViewModel : PropertyChangedBase, IHandle<DataConfigurationOpenedEventArgs>
    {
        private readonly DataService _dataService;
        private readonly IWindowManager _windowManager;

        private IEnumerable<Series> _series;
        private IEventAggregator _eventAggregator;
        private DataConfiguration _dataConfiguration;

        public IEnumerable<Series> Series
        {
            get => _series;
            set => Set(ref _series, value);
        }

        public FastVisualizerLegendViewModel(IEventAggregator eventAggregator, DataService dataService, IWindowManager windowManager)
        {
            _dataService = dataService;
            _windowManager = windowManager;
            _eventAggregator = eventAggregator;
            eventAggregator.Subscribe(this);
        }

        public void Handle(DataConfigurationOpenedEventArgs message)
        {
            if (!(message.Opened is LineChartDataConfiguration))
                return;

            _dataConfiguration = message.Opened;
            var data = _dataService.GetData<ChartData>(message.Opened.DataName);

            Series = data.Series.Where(s => s.Axis != Axes.X1 && s.Axis != Axes.X2);
        }

        public void OpenSeriesProperties(Series series)
        {
            var seriesPropertiesVm = new SeriesPropertiesViewModel(series, _dataService);
            var dialogResult = _windowManager.ShowDialog(seriesPropertiesVm);

            if (dialogResult == true)
            {
                _eventAggregator.PublishOnUIThread(new DataConfigurationOpenedEventArgs { Opened =  _dataConfiguration });
            }
        }
        
    }
}
