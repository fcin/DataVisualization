using System;

namespace DataVisualization.Services.Exceptions
{
    public class ReturnException : Exception
    {
        public object Value { get; set; }

        public ReturnException(object value) : base()
        {
            Value = value;
        }
    }
}
