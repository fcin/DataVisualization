using Caliburn.Micro;
using DataVisualization.Core.Events;
using DataVisualization.Core.Views;
using DataVisualization.Services;
using DataVisualization.Services.Exceptions;
using MaterialDesignThemes.Wpf;
using NLog;
using System.Threading;
using System.Windows;

namespace DataVisualization.Core.ViewModels
{
    public sealed class MainViewModel : Conductor<object>, IHandle<LoadingBarOpenedEventArgs>, IHandle<LoadingBarClosedEventArgs>, IHandle<AppConsoleLogEventArgs>
    {
        public VisualizerViewModelBase VisualizerVm { get; set; }
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

            VisualizerVm = new FastVisualizerViewModel();
            DataBrowserVm = dataBrowserVm;
            MenuVm = menuVm;
            AppConsoleVm = appConsoleVm;

            ActivateItem(VisualizerVm);
            ActivateItem(DataBrowserVm);
            ActivateItem(MenuVm);
            ActivateItem(AppConsoleVm);
        }

        public async void OnDialogHostLoaded()
        {
            if (!GlobalSettings.LoadedSuccessfully)
            {
                var ex = GlobalSettings.LoadedWithException;
                var message = string.Join(": ", ex.Message, ex.InnerException?.Message);
                var popup = new PopupBoxView
                {
                    DataContext = new PopupBoxViewModel(PopupBoxType.Ok, message, true)
                };

                NLog.LogManager.GetCurrentClassLogger().Fatal(ex);
                await DialogHost.Show(popup, "RootHost");
                Application.Current.Shutdown();
                return;
            }
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
