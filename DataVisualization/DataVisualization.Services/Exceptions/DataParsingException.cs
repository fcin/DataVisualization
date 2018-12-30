using System;

namespace DataVisualization.Services.Exceptions
{
    public class DataParsingException : Exception
    {
        public DataParsingException() : base()
        {

        }

        public DataParsingException(string message, Exception ex) : base(message, ex) 
        {

        }
    }
}
