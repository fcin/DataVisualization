using System;
using DataVisualization.Services.Language;

namespace DataVisualization.Services.Exceptions
{
    public class ParserException : Exception
    {
        public ParserException(string message, Exception ex) : base(message, ex)
        {

        }

        public ParserException(string message) : base(message)
        {

        }

        public ParserException(Token token, string message) : this($"{token.Literal}: {message}")
        {

        }
    }
}
