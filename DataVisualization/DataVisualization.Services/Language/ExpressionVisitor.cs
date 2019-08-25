using DataVisualization.Services.Language.Expressions;

namespace DataVisualization.Services.Language
{
    public class ExpressionVisitor
    {
        public virtual object VisitBinary(BinaryExpression expression)
        {
            return $"({expression.Operator.Lexeme} {expression.Left} {expression.Right})";
        }

        public virtual object VisitLiteral(LiteralExpression expression)
        {
            return $"({expression.Literal})";
        }

        public virtual object VisitGrouping(GroupingExpression expression)
        {
            return $"({expression.Expression})";
        }

        public virtual object VisitUnary(UnaryExpression expression)
        {
            return $"({expression.Operator} {expression.Right})";
        }
    }
}