using System;

namespace DataVisualization.Services.Exceptions
{
    public class DataPullingException : Exception
    {
        public DataPullingException() : base()
        {

        }

        public DataPullingException(string message, Exception ex) : base(message, ex)
        {

        }
    }
}
