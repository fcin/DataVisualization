using Caliburn.Micro;
using DataVisualization.Core.ViewModels.SettingsWindow;
using DataVisualization.Services;

namespace DataVisualization.Core.ViewModels
{
    public class MenuViewModel : PropertyChangedBase
    {
        private readonly IWindowManager _windowManager;
        private readonly GlobalSettings _globalSettings;
        private DataLoaderViewModel _dataLoaderVm;

        public MenuViewModel(IWindowManager windowManager, GlobalSettings globalSettings, DataLoaderViewModel dataLoaderVm)
        {
            _windowManager = windowManager;
            _globalSettings = globalSettings;
            _dataLoaderVm = dataLoaderVm;
        }

        public void NewData()
        {
            _windowManager.ShowDialog(_dataLoaderVm);
        }

        public void OpenSettings()
        {
            _windowManager.ShowDialog(new GlobalSettingsViewModel(_globalSettings));
        }
    }
}
