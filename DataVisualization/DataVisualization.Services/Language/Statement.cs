using DataVisualization.Services.Language.Expressions;

namespace DataVisualization.Services.Language
{
    public abstract class Statement
    {
        public abstract object Accept(ExpressionVisitor visitor);
    }

    public class PrintStatement : Statement
    {
        public Expression Expression { get; }

        public PrintStatement(Expression expression)
        {
            Expression = expression;
        }

        public override object Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitPrintStatement(Expression);
        }
    }
    public class ExpressionStatement : Statement
    {
        public Expression Expression { get; }

        public ExpressionStatement(Expression expression)
        {
            Expression = expression;
        }

        public override object Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitExpressionStatement(Expression);
        }
    }
}
