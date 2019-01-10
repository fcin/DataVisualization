using DataVisualization.Models;

namespace DataVisualization.Core.Events
{
    public class AppConsoleLogEventArgs
    {
        public AppConsoleLog Log { get; set; }

        public AppConsoleLogEventArgs(AppConsoleLog log)
        {
            Log = log;
        }

        public AppConsoleLogEventArgs()
        {

        }
    }
}
