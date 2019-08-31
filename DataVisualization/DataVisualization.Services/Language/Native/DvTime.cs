using System;
using System.Collections.Generic;

namespace DataVisualization.Services.Language.Native
{
    public class DvTime : ICallable
    {
        public int Arity()
        {
            return 0;
        }

        /// <summary>
        /// Returns current time in milliseconds.
        /// </summary>
        public object Call(Interpreter interpreter, IReadOnlyList<object> arguments)
        {
            return (double)DateTime.UtcNow.Millisecond;
        }
    }
}
