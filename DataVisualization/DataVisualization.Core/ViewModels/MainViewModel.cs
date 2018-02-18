using Caliburn.Micro;

namespace DataVisualization.Core.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        public VisualizerViewModel VisualizerVm { get; set; } = new VisualizerViewModel();

        public MainViewModel()
        {
            ActivateItem(VisualizerVm);
        }
    }
}
