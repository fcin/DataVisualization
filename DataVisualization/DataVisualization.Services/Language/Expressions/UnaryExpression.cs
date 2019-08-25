namespace DataVisualization.Services.Language.Expressions
{
    public class UnaryExpression : Expression
    {
        public Token Operator { get; }
        public Expression Right { get; }

        public UnaryExpression(Token @operator, Expression right)
        {
            Operator = @operator;
            Right = right;
        }

        public override object Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitUnary(this);
        }
    }
}