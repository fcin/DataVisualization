using System.Diagnostics;
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
            Trace.Listeners.Add(memoryListener);

            var lexer = new Lexer(source);
            var parser = new Parser(lexer.Scan());
            var interpreter = new Interpreter();
            interpreter.Interpret(parser.Parse());

            Trace.Flush();

            return memoryListener.Data;
        }
    }
}
