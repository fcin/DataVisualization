namespace DataVisualization.Services.Language.Expressions
{
    public class AssignExpression : Expression
    {
        public Token Identifier { get; }
        public Expression Value { get;}

        public AssignExpression(Token identifier, Expression value)
        {
            Identifier = identifier;
            Value = value;
        }

        public override object Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitAssignExpression(this);
        }
    }
}
