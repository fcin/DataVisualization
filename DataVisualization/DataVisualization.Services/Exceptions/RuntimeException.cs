using DataVisualization.Services.Language;
using System;

namespace DataVisualization.Services.Exceptions
{
    public class RuntimeException : Exception
    {
        public RuntimeException(Token token, string message) : base($"{token.Lexeme}: {message}")
        {

        }

        public RuntimeException(string message) : base(message)
        {

        }
    }
}
