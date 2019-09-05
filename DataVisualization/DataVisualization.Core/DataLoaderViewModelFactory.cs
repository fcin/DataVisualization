using Caliburn.Micro;
using DataVisualization.Core.ViewModels;
using DataVisualization.Services;

namespace DataVisualization.Core
{
    public class DataLoaderViewModelFactory
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly LoadingBarManager _loadingBarManager;
        private readonly IWindowManager _windowManager;
        private readonly DataConfigurationService _dataConfigurationService;
        private readonly DataService _dataService;
        private readonly DataFileReader _dataFileReader;
        private readonly DvFileLoaderViewModel _dvFileLoaderVm;

        public DataLoaderViewModelFactory(IEventAggregator eventAggregator, LoadingBarManager loadingBarManager, IWindowManager windowManager, 
            DataConfigurationService dataConfigurationService, DataService dataService, DataFileReader dataFileReader, 
            DvFileLoaderViewModel dvFileLoaderVm)
        {
            _eventAggregator = eventAggregator;
            _loadingBarManager = loadingBarManager;
            _windowManager = windowManager;
            _dataConfigurationService = dataConfigurationService;
            _dataService = dataService;
            _dataFileReader = dataFileReader;
            _dvFileLoaderVm = dvFileLoaderVm;
        }

        public WizardViewModel Get()
        {
            var dataLoaderVm = new DataLoaderViewModel(_eventAggregator, _loadingBarManager, _windowManager, _dataConfigurationService, 
                _dataService, _dataFileReader);
            return new WizardViewModel(dataLoaderVm, _dvFileLoaderVm);
        }
    }
}
