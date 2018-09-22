﻿using Caliburn.Micro;
using DataVisualization.Core.Events;
using DataVisualization.Models;
using DataVisualization.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace DataVisualization.Core.ViewModels
{
    public class DataBrowserViewModel : PropertyChangedBase, IHandle<NewDataAddedEventArgs>
    {
        private List<DataConfiguration> _allDataConfigurations;
        public List<DataConfiguration> AllDataConfigurations
        {
            get => _allDataConfigurations;
            set => Set(ref _allDataConfigurations, value);
        }

        private readonly IEventAggregator _eventAggregator;
        private readonly LoadingBarManager _loadingBarManager;
        private readonly DataConfigurationService _dataConfigurationService;
        private readonly DataService _dataService;

        public DataBrowserViewModel(IEventAggregator eventAggregator, LoadingBarManager loadingBarManager, 
            DataConfigurationService dataConfigurationService, DataService dataService)
        {
            _eventAggregator = eventAggregator;
            _loadingBarManager = loadingBarManager;
            _dataConfigurationService = dataConfigurationService;
            _dataService = dataService;

            RefreshDataConfigurations();

            _eventAggregator.Subscribe(this);
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
                var loadingBar = _loadingBarManager.ShowLoadingBar();
                var progress = new Progress<LoadingBarStatus>(progressResult =>
                {
                    loadingBar.Message = progressResult.Message;
                    loadingBar.PercentFinished = progressResult.PercentFinished;
                });

                await Task.Run(() =>
                {
                    _dataConfigurationService.DeleteConfigurationByName(selectedItem.DataName);
                    if (_dataService.Exists(selectedItem.DataName))
                        _dataService.DeleteDataByName(selectedItem.DataName, progress);
                });

                _loadingBarManager.CloseLoadingBar();

                RefreshDataConfigurations();
            }
        }

        public void Handle(NewDataAddedEventArgs message)
        {
            RefreshDataConfigurations();
        }

        private void RefreshDataConfigurations()
        {
            var allConfigurations = _dataConfigurationService.GetAll();
            AllDataConfigurations = new List<DataConfiguration>(allConfigurations);
        }
    }
}
