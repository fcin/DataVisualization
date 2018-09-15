using Caliburn.Micro;
using DataVisualization.Core.Events;
using DataVisualization.Models;
using DataVisualization.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace DataVisualization.Core.ViewModels
{
    public class DataBrowserViewModel : PropertyChangedBase
    {
        private List<DataConfiguration> _allDataConfigurations;
        public List<DataConfiguration> AllDataConfigurations {
            get => _allDataConfigurations;
            set => Set(ref _allDataConfigurations, value);
        }

        private readonly IEventAggregator _eventAggregator;
        private readonly ProgressBarManager _progressBarManager;
        private readonly DataConfigurationService _dataConfigurationService;
        private readonly DataService _dataService;

        public DataBrowserViewModel(IEventAggregator eventAggregator, ProgressBarManager progressBarManager)
        {
            _eventAggregator = eventAggregator;
            _progressBarManager = progressBarManager;
            _dataConfigurationService = new DataConfigurationService();
            _dataService = new DataService();

            var allConfigurations = _dataConfigurationService.GetAll();
            AllDataConfigurations = new List<DataConfiguration>(allConfigurations);
        }

        internal void OpenConfiguration(DataConfiguration config)
        {
            _eventAggregator.PublishOnUIThread(new DataConfigurationOpenedEventArgs { Opened = config });
        }

        internal async Task DeleteConfigurationAsync(DataConfiguration selectedItem)
        {
            var result = MessageBox.Show($"Are you sure you want to delete \"{selectedItem.DataName}\"?", "Delete data", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                _eventAggregator.PublishOnUIThread(new BeforeDataConfigurationDeletedEventArgs { ConfigToDelete = selectedItem });
                var loadingBar = _progressBarManager.ShowProgressBar();
                var progress = new Progress<LoadingBarStatus>(progressResult => {
                    loadingBar.Message = progressResult.Message;
                    loadingBar.PercentFinished = progressResult.PercentFinished;
                });

                await Task.Run(() => {
                    _dataConfigurationService.DeleteConfigurationByName(selectedItem.DataName);
                    if(_dataService.Exists(selectedItem.DataName))
                        _dataService.DeleteDataByName(selectedItem.DataName, progress);
                });

                _progressBarManager.CloseProgressBar();

                var allConfigurations = _dataConfigurationService.GetAll();
                AllDataConfigurations = new List<DataConfiguration>(allConfigurations);
            }
        }
    }
}
