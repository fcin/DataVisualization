using System;

namespace DataVisualization.Models
{
    public class AppConsoleLog
    {
        public string Message { get; }
        public DateTime LoggedAt { get; }
        public DateTime LoggedAtLocal => LoggedAt.ToLocalTime();

        public AppConsoleLog(string message)
        {
            Message = message ?? string.Empty;
            LoggedAt = DateTime.UtcNow;
        }
    }
}
