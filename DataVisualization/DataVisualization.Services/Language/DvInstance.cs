using System.Collections.Generic;
using DataVisualization.Services.Exceptions;

namespace DataVisualization.Services.Language
{
    public class DvInstance
    {
        private readonly DvClass _dvClass;
        private readonly Dictionary<string, object> _fields = new Dictionary<string, object>();

        public DvInstance(DvClass dvClass)
        {
            _dvClass = dvClass;
        }

        public override string ToString()
        {
            return $"{_dvClass.Name} instance";
        }

        public object Get(Token name)
        {
            if (_fields.ContainsKey(name.Lexeme))
            {
                return _fields[name.Lexeme];
            }

            var method = _dvClass.FindMethod(name.Lexeme);
            if (method != null)
                return method;

            throw new RuntimeException(name, $"Undefined property name {name}");
        }

        public void Set(Token name, object value)
        {
            if (_fields.ContainsKey(name.Lexeme))
            {
                _fields[name.Lexeme] = value;
                return;
            }

            _fields.Add(name.Lexeme, value);
        }
    }
}
