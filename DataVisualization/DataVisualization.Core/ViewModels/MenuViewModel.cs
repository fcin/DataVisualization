using Caliburn.Micro;

namespace DataVisualization.Core.ViewModels
{
    public class MenuViewModel : PropertyChangedBase
    {
        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly LoadingBarManager _loadingBarManager;

        public MenuViewModel(IWindowManager windowManager, IEventAggregator eventAggregator, LoadingBarManager loadingBarManager)
        {
            _windowManager = windowManager;
            _eventAggregator = eventAggregator;
            _loadingBarManager = loadingBarManager;
        }

        public void NewData()
        {
            var dataLoaderVm = new DataLoaderViewModel(_eventAggregator, _loadingBarManager);
            _windowManager.ShowDialog(dataLoaderVm);
        }

        public void OpenSettings()
        {

        }
    }
}
