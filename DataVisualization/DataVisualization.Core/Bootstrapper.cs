using Caliburn.Micro;
using DataVisualization.Core.ViewModels;
using DataVisualization.Core.ViewModels.SettingsWindow;
using DataVisualization.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace DataVisualization.Core
{
    public class Bootstrapper : BootstrapperBase
    {
        private readonly SimpleContainer _container = new SimpleContainer();

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            var globalSettings = new GlobalSettings();
            Thread.CurrentThread.CurrentCulture = globalSettings.CurrentLanguage;
            Thread.CurrentThread.CurrentUICulture = globalSettings.CurrentLanguage;

            DisplayRootViewFor<MainViewModel>();
        }

        protected override void Configure()
        {
            _container.Singleton<MainViewModel, MainViewModel>();
            _container.Singleton<IWindowManager, WindowManager>();
            _container.Singleton<IEventAggregator, EventAggregator>();
            _container.Singleton<ISeriesFactory, SeriesFactory>();
            _container.Singleton<LoadingBarManager, LoadingBarManager>();
            _container.Singleton<GlobalSettings, GlobalSettings>();
            _container.PerRequest<DataService, DataService>();
            _container.PerRequest<DataConfigurationService, DataConfigurationService>();
            _container.PerRequest<DataLoaderViewModelFactory, DataLoaderViewModelFactory>();
            _container.PerRequest<VisualizerViewModel, VisualizerViewModel>();
            _container.PerRequest<DataBrowserViewModel, DataBrowserViewModel>();
            _container.PerRequest<MenuViewModel, MenuViewModel>();
            _container.PerRequest<GlobalSettingsViewModel, GlobalSettingsViewModel>();
            _container.PerRequest<DataFileReader, DataFileReader>();

            base.Configure();
        }

        protected override object GetInstance(Type service, string key)
        {
            var instance = _container.GetInstance(service, key);
            if (instance != null)
                return instance;

            throw new InvalidOperationException("Could not locate any instances.");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}
