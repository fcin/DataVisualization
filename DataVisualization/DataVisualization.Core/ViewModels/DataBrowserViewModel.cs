using Caliburn.Micro;
using DataVisualization.Core.Events;
using DataVisualization.Models;
using DataVisualization.Services;
using System;
using System.Collections.Generic;
using System.Windows;

namespace DataVisualization.Core.ViewModels
{
    public class DataBrowserViewModel
    {
        public List<DataConfiguration> AllDataConfigurations { get; set; }

        private readonly IEventAggregator _eventAggregator;
        private readonly DataConfigurationService _dataConfigurationService;
        private readonly DataService _dataService;

        public DataBrowserViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _dataConfigurationService = new DataConfigurationService();
            _dataService = new DataService();

            var allConfigurations = _dataConfigurationService.GetAll();
            AllDataConfigurations = new List<DataConfiguration>(allConfigurations);
        }

        internal void OpenConfiguration(DataConfiguration config)
        {
            _eventAggregator.PublishOnUIThread(new DataConfigurationOpenedEventArgs { Opened = config });
        }

        internal void DeleteConfiguration(DataConfiguration selectedItem)
        {
            var result = MessageBox.Show($"Are you sure you want to delete \"{selectedItem.DataName}\"?", "Delete data", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                // Todo: delete config, delete data, make sure Visualizer is closed before deleting.
            }
        }
    }
}
