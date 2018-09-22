using Caliburn.Micro;
using DataVisualization.Core.ViewModels.SettingsWindow;
using DataVisualization.Services;

namespace DataVisualization.Core.ViewModels
{
    public class MenuViewModel : PropertyChangedBase
    {
        private readonly IWindowManager _windowManager;
        private readonly GlobalSettings _globalSettings;
        private readonly DataLoaderViewModelFactory _dataLoaderVmFactory;
        private readonly GlobalSettingsViewModel _globalSettingsVm;

        public MenuViewModel(IWindowManager windowManager, GlobalSettings globalSettings, 
            DataLoaderViewModelFactory dataLoaderVmFactory, GlobalSettingsViewModel globalSettingsVm)
        {
            _windowManager = windowManager;
            _globalSettings = globalSettings;
            _dataLoaderVmFactory = dataLoaderVmFactory;
            _globalSettingsVm = globalSettingsVm;
        }

        public void NewData()
        {
            var dataLoaderVm = _dataLoaderVmFactory.Get();
            _windowManager.ShowDialog(dataLoaderVm);
        }

        public void OpenSettings()
        {
            _windowManager.ShowDialog(_globalSettingsVm);
        }
    }
}
