namespace DataVisualization.Services.Language.Expressions
{
    public class ExpressionVisitor
    {
        public string VisitBinary(BinaryExpression expression)
        {
            return $"({expression.Operator.Lexeme} {expression.Left} {expression.Right})";
        }

        public string VisitLiteral(LiteralExpression expression)
        {
            return $"({expression.Literal})";
        }

        public string VisitGrouping(GroupingExpression expression)
        {
            return $"({expression.Expression})";
        }

        public string VisitUnary(UnaryExpression expression)
        {
            return $"({expression.Operator} {expression.Right})";
        }
    }
}