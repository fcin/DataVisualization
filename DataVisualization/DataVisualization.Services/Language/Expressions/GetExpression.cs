namespace DataVisualization.Services.Language.Expressions
{
    public class GetExpression : Expression
    {
        public Expression Object { get; }
        public Token Name { get; }

        public GetExpression(Expression @object, Token name)
        {
            Object = @object;
            Name = name;
        }

        public override object Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitGetExpression(this);
        }
    }
}
