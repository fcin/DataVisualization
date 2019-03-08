using System;

namespace DataVisualization.Services.Exceptions
{
    public class SettingsLoadingException : Exception
    {
        public SettingsLoadingException() { }
        public SettingsLoadingException(string message) : base(message) { }
        public SettingsLoadingException(string message, Exception exception) : base(message, exception) { }
    }
}
