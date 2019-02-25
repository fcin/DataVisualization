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
            var dataLoaderVm = _dataLoaderVmFactory.Get();
            _windowManager.ShowDialog(dataLoaderVm);
        }

        public void OpenSettings()
        {
            _windowManager.ShowDialog(_globalSettingsVm);
        }
    }
}
