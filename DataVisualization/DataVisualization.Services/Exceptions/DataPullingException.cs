using System;

namespace DataVisualization.Services.Exceptions
{
    public class DataPullingException : Exception
    {
        public DataPullingException()
        {

        }

        public DataPullingException(string message, Exception ex) : base(message, ex)
        {

        }
    }
}
