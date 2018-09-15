using Caliburn.Micro;
using DataVisualization.Core.Events;
using DataVisualization.Core.ViewModels;
using System;

namespace DataVisualization.Core
{
    public class LoadingBarManager
    {
        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;
        private LoadingBarViewModel _loadingBarVm;

        public LoadingBarManager(IWindowManager windowManager, IEventAggregator eventAggregator)
        {
            _windowManager = windowManager;
            _eventAggregator = eventAggregator;
        }

        public ILoadingBarWindow ShowLoadingBar()
        {
            if (_loadingBarVm != null)
                throw new InvalidOperationException("Loading bar already opened");

            _eventAggregator.PublishOnUIThread(new LoadingBarOpenedEventArgs());
            _loadingBarVm = new LoadingBarViewModel();
            _windowManager.ShowWindow(_loadingBarVm);
            return _loadingBarVm;
        }

        public void CloseLoadingBar()
        {
            if (_loadingBarVm == null)
                throw new InvalidOperationException("Loading bar is not opened");

            _loadingBarVm.Close();
            _eventAggregator.PublishOnUIThread(new LoadingBarClosedEventArgs());
            _loadingBarVm = null;
        }
    }
}
