using Caliburn.Micro;
using DataVisualization.Core.ViewModels;
using DataVisualization.Services;

namespace DataVisualization.Core
{
    public class DataLoaderViewModelFactory
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly LoadingBarManager _loadingBarManager;
        private readonly DataConfigurationService _dataConfigurationService;
        private readonly DataService _dataService;
        private readonly DataFileReader _dataFileReader;

        public DataLoaderViewModelFactory(IEventAggregator eventAggregator, LoadingBarManager loadingBarManager, 
            DataConfigurationService dataConfigurationService, DataService dataService, DataFileReader dataFileReader)
        {
            _eventAggregator = eventAggregator;
            _loadingBarManager = loadingBarManager;
            _dataConfigurationService = dataConfigurationService;
            _dataService = dataService;
            _dataFileReader = dataFileReader;
        }

        public DataLoaderViewModel Get()
        {
            return new DataLoaderViewModel(_eventAggregator, _loadingBarManager, _dataConfigurationService, _dataService, _dataFileReader);
        }
    }
}
