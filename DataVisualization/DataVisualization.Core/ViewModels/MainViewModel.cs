﻿using Caliburn.Micro;
using DataVisualization.Core.Events;
using DataVisualization.Core.Views;
using DataVisualization.Services;
using DataVisualization.Services.Exceptions;
using MaterialDesignThemes.Wpf;
using NLog;
using System.Threading;
using System.Windows;
using DataVisualization.Core.ViewModels.Visualizers;

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
        public FastVisualizerLegendViewModel Legend { get; set; }
        public CodeEditorViewModel CodeEditorVm { get; set; }
        public ActionToolbarViewModel ActionToolbarVm { get; set; }
        public OutputViewModel OutputVm { get; set; }

        public MainViewModel(IEventAggregator eventAggregator, VisualizerViewModelBase visualizerVm, 
            DataBrowserViewModel dataBrowserVm, MenuViewModel menuVm, AppConsoleViewModel appConsoleVm, 
            FastVisualizerLegendViewModel legend, CodeEditorViewModel codeEditorVm, ActionToolbarViewModel actionToolbarVm,
            OutputViewModel outputViewModel)
        {
            IsMainWindowEnabled = true;

            eventAggregator.Subscribe(this);

            VisualizerVm = visualizerVm;
            DataBrowserVm = dataBrowserVm;
            MenuVm = menuVm;
            AppConsoleVm = appConsoleVm;
            Legend = legend;
            CodeEditorVm = codeEditorVm;
            ActionToolbarVm = actionToolbarVm;
            OutputVm = outputViewModel;

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
