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
        

        public AdvancedGlobalSettingsViewModel()
        {
            PointsCount = GlobalSettings.PointsCount;
        }

        public void Save()
        {
            GlobalSettings.PointsCount = PointsCount;
            GlobalSettings.Save();
        }
    }
}
