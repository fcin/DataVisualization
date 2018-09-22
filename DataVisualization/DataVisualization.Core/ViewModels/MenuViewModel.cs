using Caliburn.Micro;
using DataVisualization.Core.ViewModels.SettingsWindow;
using DataVisualization.Services;

namespace DataVisualization.Core.ViewModels
{
    public class MenuViewModel : PropertyChangedBase
    {
        private readonly IWindowManager _windowManager;
        private readonly GlobalSettings _globalSettings;
        private readonly DataLoaderViewModel _dataLoaderVm;
        private readonly GlobalSettingsViewModel _globalSettingsVm;

        public MenuViewModel(IWindowManager windowManager, GlobalSettings globalSettings, DataLoaderViewModel dataLoaderVm, 
            GlobalSettingsViewModel globalSettingsVm)
        {
            _windowManager = windowManager;
            _globalSettings = globalSettings;
            _dataLoaderVm = dataLoaderVm;
            _globalSettingsVm = globalSettingsVm;
        }

        public void NewData()
        {
            _windowManager.ShowDialog(_dataLoaderVm);
        }

        public void OpenSettings()
        {
            _windowManager.ShowDialog(_globalSettingsVm);
        }
    }
}
