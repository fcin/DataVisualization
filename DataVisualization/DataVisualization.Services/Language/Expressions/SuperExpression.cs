namespace DataVisualization.Services.Language.Expressions
{
    public class SuperExpression : Expression
    {
        public Token Keyword { get; }
        public Token Method { get; }

        public SuperExpression(Token keyword, Token method)
        {
            Keyword = keyword;
            Method = method;
        }

        public override object Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitSuperExpression(this);
        }
    }
}
