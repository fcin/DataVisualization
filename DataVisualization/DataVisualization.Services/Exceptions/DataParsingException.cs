using System;

namespace DataVisualization.Services.Exceptions
{
    public class DataParsingException : Exception
    {
        public DataParsingException()
        {

        }

        public DataParsingException(string message, Exception ex) : base(message, ex) 
        {

        }
    }
}
