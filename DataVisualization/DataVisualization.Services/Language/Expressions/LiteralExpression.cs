namespace DataVisualization.Services.Language.Expressions
{
    public class LiteralExpression : Expression
    {
        public object Literal { get; }

        public LiteralExpression(object literal)
        {
            Literal = literal;
        }

        public override string Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitLiteral(this);
        }

        public override string ToString()
        {
            return Literal == null ? "null" : Literal.ToString();
        }
    }
}