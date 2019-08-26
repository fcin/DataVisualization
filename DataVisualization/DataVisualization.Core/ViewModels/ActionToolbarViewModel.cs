using Caliburn.Micro;
using DataVisualization.Core.Events;

namespace DataVisualization.Core.ViewModels
{
    public class ActionToolbarViewModel : PropertyChangedBase
    {
        private readonly IEventAggregator _eventAggregator;

        public ActionToolbarViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void OnRunCode()
        {
            _eventAggregator.PublishOnUIThread(new RunCodeEventArgs());
        }

        public void OnStopCodeExecution()
        {
            
        }
    }
}
