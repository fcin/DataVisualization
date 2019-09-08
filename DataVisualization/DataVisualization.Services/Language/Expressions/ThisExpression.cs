namespace DataVisualization.Services.Language.Expressions
{
    public class ThisExpression : Expression
    {
        public Token Keyword { get; }

        public ThisExpression(Token keyword)
        {
            Keyword = keyword;
        }

        public override object Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitThisExpression(this);
        }
    }
}
