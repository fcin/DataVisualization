using DataVisualization.Services;

namespace DataVisualization.Core.ViewModels.SettingsWindow
{
    public class AdvancedGlobalSettingsViewModel : GlobalSettingsViewModelBase
    {
        private int _pointsCount;
        public int PointsCount
        {
            get => _pointsCount;
            set => Set(ref _pointsCount, value);
        }

        private readonly GlobalSettings _globalSettings;

        public AdvancedGlobalSettingsViewModel(GlobalSettings globalSettings)
        {
            _globalSettings = globalSettings;
            PointsCount = _globalSettings.PointsCount;
        }

        public void Save()
        {
            _globalSettings.PointsCount = PointsCount;
            _globalSettings.Persist();
        }
    }
}
