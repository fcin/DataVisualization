using System.Collections.Generic;

namespace DataVisualization.Services.Language
{
    public class DvClass : ICallable
    {
        public string Name { get; }

        private readonly Dictionary<string, DvFunction> _methods;

        public DvClass(string name, Dictionary<string, DvFunction> methods)
        {
            Name = name;
            _methods = methods;
        }

        public int Arity()
        {
            var initializer = FindMethod("init");

            return initializer?.Arity() ?? 0;
        }

        public object Call(Interpreter interpreter, IReadOnlyList<object> arguments)
        {
            var instance = new DvInstance(this);
            
            var initializer = FindMethod("init");

            initializer?.Bind(instance).Call(interpreter, arguments);

            return instance;
        }

        public DvFunction FindMethod(string name)
        {
            _methods.TryGetValue(name, out var value);
            return value;
        }
    }
}
