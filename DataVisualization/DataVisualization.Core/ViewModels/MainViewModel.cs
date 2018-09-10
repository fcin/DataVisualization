using Caliburn.Micro;
using DataVisualization.Core.ViewModels.DataLoading;

namespace DataVisualization.Core.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        private readonly IWindowManager _windowManager;
        public VisualizerViewModel VisualizerVm { get; set; }

        public MainViewModel(IWindowManager windowManager, ISeriesFactory seriesFactory)
        {
            _windowManager = windowManager;

            VisualizerVm = new VisualizerViewModel(seriesFactory, windowManager);
            ActivateItem(VisualizerVm);
        }

        public void OnDataLoad()
        {
            var dataLoaderVm = new DataLoaderViewModel();
            _windowManager.ShowDialog(dataLoaderVm);
        }
    }
}
