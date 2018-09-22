using Caliburn.Micro;
using DataVisualization.Core.Events;
using DataVisualization.Services;
using System.Collections.Generic;
using System.Linq;

namespace DataVisualization.Core.ViewModels.SettingsWindow
{
    public class GeneralGlobalSettingsViewModel : GlobalSettingsViewModelBase
    {
        public IEnumerable<string> AllLanguages => _globalSettings.AllLanguages.Select(lang => lang.DisplayName);
        private string _selectedLanguage;
        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set => Set(ref _selectedLanguage, value);
        }

        private readonly GlobalSettings _globalSettings;
        private readonly IEventAggregator _eventAggregator;

        public GeneralGlobalSettingsViewModel(GlobalSettings globalSettings, IEventAggregator eventAggregator)
        {
            _globalSettings = globalSettings;
            _eventAggregator = eventAggregator;
            SelectedLanguage = _globalSettings.CurrentLanguage.DisplayName;
        }

        public void Save()
        {
            _globalSettings.CurrentLanguage = _globalSettings.AllLanguages.First(lang => lang.DisplayName == SelectedLanguage);
            _globalSettings.Persist();

            _eventAggregator.PublishOnUIThread(new GlobalSettingsChangedEventArgs { NewGlobalSettings = _globalSettings });
        }
    }
}
