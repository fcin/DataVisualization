using Caliburn.Micro;
using DataVisualization.Core.Events;

namespace DataVisualization.Core.ViewModels
{
    public class MainViewModel : Conductor<object>, IHandle<LoadingBarOpenedEventArgs>, IHandle<LoadingBarClosedEventArgs>
    {
        private readonly IWindowManager _windowManager;
        public VisualizerViewModel VisualizerVm { get; set; }
        public DataBrowserViewModel DataBrowserVm { get; set; }
        public MenuViewModel MenuVm { get; set; }
        private bool _isMainWindowEnabled;
        public bool IsMainWindowEnabled
        {
            get => _isMainWindowEnabled;
            set => Set(ref _isMainWindowEnabled, value);
        }

        public MainViewModel(IWindowManager windowManager, ISeriesFactory seriesFactory,
            IEventAggregator eventAggregator, LoadingBarManager loadingBarManager)
        {
            IsMainWindowEnabled = true;
            _windowManager = windowManager;

            eventAggregator.Subscribe(this);

            VisualizerVm = new VisualizerViewModel(seriesFactory, windowManager, eventAggregator);
            DataBrowserVm = new DataBrowserViewModel(eventAggregator, loadingBarManager);
            MenuVm = new MenuViewModel(windowManager, eventAggregator, loadingBarManager);

            ActivateItem(VisualizerVm);
            ActivateItem(DataBrowserVm);
            ActivateItem(MenuVm);
        }

        public void Handle(LoadingBarOpenedEventArgs message)
        {
            IsMainWindowEnabled = false;
        }

        public void Handle(LoadingBarClosedEventArgs message)
        {
            IsMainWindowEnabled = true;
        }
    }
}
