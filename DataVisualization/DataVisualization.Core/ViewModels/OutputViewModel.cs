using System.Collections.Generic;
using Caliburn.Micro;

namespace DataVisualization.Core.ViewModels
{
    public class OutputViewModel : PropertyChangedBase
    {
        public IEnumerable<OutputValue> OutputValues { get; }

        public OutputViewModel()
        {
            OutputValues = new List<OutputValue>();
        }
    }

    public class OutputValue
    {
        public string Message { get; }
        public int Line { get; }

        public OutputValue(string message, int line)
        {
            Message = message;
            Line = line;
        }
    }
}
