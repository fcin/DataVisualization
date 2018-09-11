using Caliburn.Micro;
using DataVisualization.Core.Events;
using DataVisualization.Models;
using DataVisualization.Services;
using System.Collections.Generic;

namespace DataVisualization.Core.ViewModels
{
    public class DataBrowserViewModel
    {
        public List<DataConfiguration> AllDataConfigurations { get; set; }

        private readonly IEventAggregator _eventAggregator;
        private readonly DataConfigurationService _dataConfigurationService;

        public DataBrowserViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _dataConfigurationService = new DataConfigurationService();

            var allConfigurations = _dataConfigurationService.GetAll();
            AllDataConfigurations = new List<DataConfiguration>(allConfigurations);
        }

        public void OpenConfiguration(DataConfiguration config)
        {
            _eventAggregator.PublishOnUIThread(new DataConfigurationOpenedEventArgs { Opened = config });
        }
    }
}
