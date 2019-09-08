namespace DataVisualization.Services.Language.Expressions
{
    public class SetExpression : Expression
    {
        public Expression Object { get; }
        public Token Name { get; }
        public Expression Value { get; }

        public SetExpression(Expression @object, Token name, Expression value)
        {
            Object = @object;
            Name = name;
            Value = value;
        }

        public override object Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitSetExpression(this);
        }
    }
}
