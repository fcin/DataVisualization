using Caliburn.Micro;
using DataVisualization.Core.ViewModels.SettingsWindow;

namespace DataVisualization.Core.ViewModels
{
    public class MenuViewModel : PropertyChangedBase
    {
        private readonly IWindowManager _windowManager;
        private readonly DataLoaderViewModelFactory _dataLoaderVmFactory;
        private readonly GlobalSettingsViewModel _globalSettingsVm;

        public MenuViewModel(IWindowManager windowManager, DataLoaderViewModelFactory dataLoaderVmFactory, 
            GlobalSettingsViewModel globalSettingsVm)
        {
            _windowManager = windowManager;
            _dataLoaderVmFactory = dataLoaderVmFactory;
            _globalSettingsVm = globalSettingsVm;
        }

        public void NewData()
        {
            var wizardVm = _dataLoaderVmFactory.Get();
            _windowManager.ShowDialog(wizardVm);
        }

        public void OpenSettings()
        {
            _windowManager.ShowDialog(_globalSettingsVm);
        }
    }
}
