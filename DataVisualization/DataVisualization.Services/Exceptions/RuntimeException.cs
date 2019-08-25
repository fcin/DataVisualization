using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataVisualization.Services.Language;

namespace DataVisualization.Services.Exceptions
{
    public class RuntimeException : Exception
    {
        public RuntimeException(Token token, string message) : base($"{token.Lexeme}: {message}")
        {

        }
    }
}
