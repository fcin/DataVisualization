using Caliburn.Micro;

namespace DataVisualization.Core.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        private readonly IWindowManager _windowManager;
        public VisualizerViewModel VisualizerVm { get; set; }
        public DataBrowserViewModel DataBrowserVm { get; set; }

        public MainViewModel(IWindowManager windowManager, ISeriesFactory seriesFactory, IEventAggregator eventAggregator)
        {
            _windowManager = windowManager;

            VisualizerVm = new VisualizerViewModel(seriesFactory, windowManager, eventAggregator);
            DataBrowserVm = new DataBrowserViewModel(eventAggregator);
            ActivateItem(VisualizerVm);
            ActivateItem(DataBrowserVm);
        }

        public void OnDataLoad()
        {
            var dataLoaderVm = new DataLoaderViewModel();
            _windowManager.ShowDialog(dataLoaderVm);
        }
    }
}
