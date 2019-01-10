using Caliburn.Micro;
using DataVisualization.Core.Events;
using DataVisualization.Models;

namespace DataVisualization.Core.ViewModels
{
    public class AppConsoleViewModel : PropertyChangedBase, IHandle<AppConsoleLogEventArgs>
    {
        private const int MaxQueueSize = 10_000;
        public BindableCollection<AppConsoleLog> Logs { get; set; }

        public AppConsoleViewModel(IEventAggregator eventAggregator)
        {
            Logs = new BindableCollection<AppConsoleLog>();
            eventAggregator.Subscribe(this);
        }

        public void Handle(AppConsoleLogEventArgs message)
        {
            if (Logs.Count >= MaxQueueSize)
                Logs.RemoveAt(0);

            Logs.Add(message.Log);
        }
    }
}
