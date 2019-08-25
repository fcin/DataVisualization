using System.Data;
using NLog.Targets;

namespace DataVisualization.Services.Language.Expressions
{
    public abstract class Expression
    {
        public abstract object Accept(ExpressionVisitor visitor);
    }
}
