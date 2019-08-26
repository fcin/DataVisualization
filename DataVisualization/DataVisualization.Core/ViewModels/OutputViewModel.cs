using System.Collections.Generic;
using Caliburn.Micro;
using DataVisualization.Core.Events;

namespace DataVisualization.Core.ViewModels
{
    public class OutputViewModel : PropertyChangedBase, IHandle<OutputLogEventArgs>
    {
        private string _outputValues;

        public string OutputValues
        {
            get => _outputValues;
            set => Set(ref _outputValues, value);
        }

        public OutputViewModel(IEventAggregator eventAggregator)
        {
            OutputValues = string.Empty;
            eventAggregator.Subscribe(this);
        }

        public void Handle(OutputLogEventArgs message)
        {
            OutputValues = message.Output;
        }
    }
}
