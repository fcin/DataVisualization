namespace DataVisualization.Services.Language.Expressions
{
    public class GroupingExpression : Expression
    {
        public Expression Expression { get;}

        public GroupingExpression(Expression expression)
        {
            Expression = expression;
        }

        public override object Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitGrouping(this);
        }
    }
}