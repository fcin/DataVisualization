using System;
using System.Collections.Generic;
using System.Linq;
using DataVisualization.Services.Exceptions;

namespace DataVisualization.Services.Language
{
    public class DvFunction : ICallable
    {
        public FunctionStatement Declaration { get; }
        private readonly Environment _closure;
        private readonly bool _isInitializer;

        public DvFunction(FunctionStatement declaration, Environment environment, bool isInitializer)
        {
            Declaration = declaration;
            _closure = environment;
            _isInitializer = isInitializer;
        }

        public int Arity()
        {
            return Declaration.Parameters.Count();
        }

        public object Call(Interpreter interpreter, IReadOnlyList<object> arguments)
        {
            var environment = new Environment(_closure);

            for (int index = 0; index < arguments.Count; index++)
            {
                environment.Define(Declaration.Parameters[index].Lexeme, arguments[index]);
            }

            try
            {
                interpreter.ExecuteBlock(Declaration.Body, environment);
            }
            catch (ReturnException e)
            {
                return e.Value;
            }

            if (_isInitializer)
            {
                return _closure.GetAt(0, "this");
            }

            return null;
        }

        public DvFunction Bind(DvInstance dvInstance)
        {
            var environment = new Environment(_closure);
            environment.Define("this", dvInstance);

            return new DvFunction(Declaration, environment, _isInitializer);
        }
    }
}
