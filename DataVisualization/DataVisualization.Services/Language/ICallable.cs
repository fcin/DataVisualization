using System.Collections.Generic;

namespace DataVisualization.Services.Language
{
    public interface ICallable
    {
        int Arity();
        object Call(Interpreter interpreter, IReadOnlyList<object> arguments);
    }
}
