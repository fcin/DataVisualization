using Caliburn.Micro;

namespace DataVisualization.Core.ViewModels
{
    public class MenuViewModel : PropertyChangedBase
    {
        private readonly IWindowManager _windowManager;

        public MenuViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        public void NewData()
        {
            var dataLoaderVm = new DataLoaderViewModel();
            _windowManager.ShowDialog(dataLoaderVm);
        }
    }
}
