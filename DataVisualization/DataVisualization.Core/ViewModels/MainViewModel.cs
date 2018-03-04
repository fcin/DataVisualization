using Caliburn.Micro;
using DataVisualization.Core.ViewModels.DataLoading;
using DataVisualization.Services;

namespace DataVisualization.Core.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        private readonly IWindowManager _windowManager;
        private readonly IDataService _dataService;
        public VisualizerViewModel VisualizerVm { get; set; } = new VisualizerViewModel();

        public MainViewModel(IWindowManager windowManager, IDataService dataService)
        {
            _windowManager = windowManager;
            _dataService = dataService;
            ActivateItem(VisualizerVm);
        }

        public void OnDataLoad()
        {
            var dataLoaderVm = new DataLoaderViewModel(_dataService);
            _windowManager.ShowDialog(dataLoaderVm);
        }
    }
}
