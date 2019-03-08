using Caliburn.Micro;
using DataVisualization.Core.Events;
using DataVisualization.Services;
using System.Collections.Generic;
using System.Linq;

namespace DataVisualization.Core.ViewModels.SettingsWindow
{
    public class GeneralGlobalSettingsViewModel : GlobalSettingsViewModelBase
    {
        public IEnumerable<string> AllLanguages => GlobalSettings.AllLanguages.Select(lang => lang.DisplayName);
        private string _selectedLanguage;
        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set => Set(ref _selectedLanguage, value);
        }
        
        private readonly IEventAggregator _eventAggregator;

        public GeneralGlobalSettingsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            SelectedLanguage = GlobalSettings.CurrentLanguage.DisplayName;
        }

        public void Save()
        {
            GlobalSettings.CurrentLanguage = GlobalSettings.AllLanguages.First(lang => lang.DisplayName == SelectedLanguage);
            GlobalSettings.Save();
        }
    }
}
