using Caliburn.Micro;
using DataVisualization.Core.Events;

namespace DataVisualization.Core.ViewModels
{
    public sealed class MainViewModel : Conductor<object>, IHandle<LoadingBarOpenedEventArgs>, IHandle<LoadingBarClosedEventArgs>, IHandle<AppConsoleLogEventArgs>
    {
        public VisualizerViewModel VisualizerVm { get; set; }
        public DataBrowserViewModel DataBrowserVm { get; set; }
        public MenuViewModel MenuVm { get; set; }
        public AppConsoleViewModel AppConsoleVm { get; set; }

        private bool _isMainWindowEnabled;
        public bool IsMainWindowEnabled
        {
            get => _isMainWindowEnabled;
            set => Set(ref _isMainWindowEnabled, value);
        }

        public int LogsCount => AppConsoleVm.Logs.Count;

        public MainViewModel(IEventAggregator eventAggregator, VisualizerViewModel visualizerVm, 
            DataBrowserViewModel dataBrowserVm, MenuViewModel menuVm, AppConsoleViewModel appConsoleVm)
        {
            IsMainWindowEnabled = true;

            eventAggregator.Subscribe(this);

            VisualizerVm = visualizerVm;
            DataBrowserVm = dataBrowserVm;
            MenuVm = menuVm;
            AppConsoleVm = appConsoleVm;

            ActivateItem(VisualizerVm);
            ActivateItem(DataBrowserVm);
            ActivateItem(MenuVm);
            ActivateItem(AppConsoleVm);
        }

        public void Handle(LoadingBarOpenedEventArgs message)
        {
            IsMainWindowEnabled = false;
        }

        public void Handle(LoadingBarClosedEventArgs message)
        {
            IsMainWindowEnabled = true;
        }

        public void Handle(AppConsoleLogEventArgs message)
        {
            NotifyOfPropertyChange(() => LogsCount);
        }
    }
}
