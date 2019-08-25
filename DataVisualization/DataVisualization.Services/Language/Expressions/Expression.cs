using System.Data;
using NLog.Targets;

namespace DataVisualization.Services.Language.Expressions
{
    public abstract class Expression
    {
        public abstract string Accept(ExpressionVisitor visitor);
    }
}
