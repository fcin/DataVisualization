using System;
using System.IO;
using System.Threading.Tasks;
using Caliburn.Micro;
using DataVisualization.Core.Events;
using DataVisualization.Core.Translations;
using DataVisualization.Core.Views;
using DataVisualization.Models;
using DataVisualization.Services;
using MaterialDesignThemes.Wpf;

namespace DataVisualization.Core.ViewModels
{
    public class DvFileLoaderViewModel : LoaderViewModelBase
    {
        private string _filePath;

        private readonly IEventAggregator _eventAggregator;
        private readonly DataConfigurationService _dataConfigurationService;
        private readonly DataService _dataService;
        private readonly LoadingBarManager _loadingBarManager;
        private readonly DataFileReader _dataFileReader;

        public DvFileLoaderViewModel(IEventAggregator eventAggregator, DataConfigurationService dataConfigurationService, 
            DataService dataService, LoadingBarManager loadingBarManager, DataFileReader dataFileReader)
        {
            _eventAggregator = eventAggregator;
            _dataConfigurationService = dataConfigurationService;
            _dataService = dataService;
            _loadingBarManager = loadingBarManager;
            _dataFileReader = dataFileReader;
        }

        public override Task InitializeAsync(string filePath)
        {
            _filePath = filePath;

            return Task.CompletedTask;
        }

        public async Task OnDataLoadAsync()
        {
            var config = new ScriptDataConfiguration
            {
                DataName = Path.GetFileName(_filePath),
                FilePath = _filePath
            };

            try
            {
                _dataConfigurationService.Add(config);

                if (!_dataService.Exists(config.DataName))
                {
                    var loadedData = await _dataFileReader.ReadDataAsync(config);
                    _dataService.AddData(loadedData);
                }

                _eventAggregator.PublishOnUIThread(new NewDataAddedEventArgs());
            }
            catch (Exception ex)
            {
                var message = $"{Translation.InternalError}: {ex.Message}";
                var popup = new PopupBoxView
                {
                    DataContext = new PopupBoxViewModel(PopupBoxType.Ok, message, true)
                };
                await DialogHost.Show(popup, "DataLoaderHost");
            }

        }
    }
}
