using System.Diagnostics;
using System.Text;

namespace DataVisualization.Services
{
    public class MemoryTraceListener : TraceListener
    {
        public string Data => _stringBuilder.ToString();

        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public override void Write(string message)
        {
            _stringBuilder.Append(message);
        }

        public override void WriteLine(string message)
        {
            _stringBuilder.AppendLine(message);
        }

        public void Clear()
        {
            _stringBuilder.Clear();
        }
    }
}
