using System;

namespace DataVisualization.Services.Exceptions
{
    public class DataIOException : Exception
    {
        public DataIOException(string message, Exception exception) : base(message, exception)
        {
        }

        public DataIOException(string message) : base(message)
        {
        }
    }
}
