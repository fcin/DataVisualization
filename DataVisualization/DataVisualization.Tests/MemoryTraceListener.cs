using System.Diagnostics;

namespace DataVisualization.Tests
{
    public class MemoryTraceListener : TraceListener
    {
        public string Data { get; private set; }

        public override void Write(string message)
        {
            Data += message;
        }

        public override void WriteLine(string message)
        {
            Data += $"{message}\n";
        }
    }
}
