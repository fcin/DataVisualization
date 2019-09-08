using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        public async Task<string> RunAsync(string source, CancellationToken token)
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
            var parsedValues = parser.Parse().ToList();
            foreach (var parserError in parser.Errors)
            {
                memoryListener.WriteLine(parserError);
            }


            if (lexer.Errors.Any() || parser.Errors.Any())
            {
                return memoryListener.Data;
            }

            var interpreter = new Interpreter();

            var resolver = new Resolver(interpreter);
            resolver.Resolve(parsedValues);

            foreach (var resolverError in resolver.Errors)
            {
                memoryListener.WriteLine(resolverError);
            }

            if (resolver.Errors.Any())
            {
                return memoryListener.Data;
            }

            try
            {
                await Task.Run(() =>
                {
                    interpreter.Interpret(parsedValues, token);
                }, token);
            }
            catch (TaskCanceledException)
            {
                return memoryListener.Data;
            }

            foreach (var runtimeError in interpreter.Errors)
            {
                memoryListener.WriteLine(runtimeError);
            }

            Trace.Flush();

            return memoryListener.Data;
        }
    }
}
