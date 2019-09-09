using System.Collections.Generic;

namespace DataVisualization.Services.Language
{
    public class DvClass : ICallable
    {
        public string Name { get; }

        private readonly Dictionary<string, DvFunction> _methods;
        private readonly DvClass _superclass;

        public DvClass(string name, DvClass superclass, Dictionary<string, DvFunction> methods)
        {
            Name = name;
            _superclass = superclass;
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
            if (_methods.TryGetValue(name, out var value))
            {
                return value;
            }

            return _superclass?.FindMethod(name);
        }
    }
}
