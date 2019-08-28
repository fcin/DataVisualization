using System.Diagnostics;
using System.Linq;
using DataVisualization.Services.Language;

namespace DataVisualization.Services
{
    public class ProgramRunner
    {
        /// <summary>
        /// Runs code
        /// </summary>
        /// <param name="source">Code to run</param>
        /// <returns>Stdout</returns>
        public string Run(string source)
        {
            var memoryListener = new MemoryTraceListener();
            if (!Trace.Listeners.OfType<MemoryTraceListener>().Any())
            {
                Trace.Listeners.Add(memoryListener);
            }
            else
            {
                memoryListener = Trace.Listeners.OfType<MemoryTraceListener>().First();
                memoryListener.Clear();
            }

            var lexer = new Lexer(source);
            var scans = lexer.Scan();
            foreach (var scanError in lexer.Errors)
            {
                memoryListener.WriteLine(scanError);
            }

            var parser = new Parser(scans);
            var parsedValues = parser.Parse();
            foreach (var parserError in parser.Errors)
            {
                memoryListener.WriteLine(parserError);
            }

            if (lexer.Errors.Any() || parser.Errors.Any())
            {
                return memoryListener.Data;
            }

            var interpreter = new Interpreter();
            interpreter.Interpret(parsedValues);

            foreach (var runtimeError in interpreter.Errors)
            {
                memoryListener.WriteLine(runtimeError);
            }

            Trace.Flush();

            return memoryListener.Data;
        }
    }
}
