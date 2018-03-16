using Caliburn.Micro;
using DataVisualization.Core.ViewModels.DataLoading;

namespace DataVisualization.Core.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        private readonly IWindowManager _windowManager;
        public VisualizerViewModel VisualizerVm { get; set; } = new VisualizerViewModel();

        public MainViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
            ActivateItem(VisualizerVm);
        }

        public void OnDataLoad()
        {
            var dataLoaderVm = new DataLoaderViewModel();
            _windowManager.ShowDialog(dataLoaderVm);
        }
    }
}
