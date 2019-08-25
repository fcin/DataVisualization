using DataVisualization.Services.Language.Expressions;

namespace DataVisualization.Services.Language
{
    public abstract class ExpressionVisitor
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

        public abstract object VisitExpressionStatement(Expression expression);
        public abstract object VisitPrintStatement(Expression expression);
        public abstract object VisitVarStatement(VarStatement statement);
        public abstract object VisitVarExpression(VarExpression expression);

        public abstract object VisitAssignExpression(AssignExpression assignExpression);

        public abstract void VisitBlockStatement(BlockStatement blockStatement);
    }
}