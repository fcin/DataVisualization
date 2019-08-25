using System.Data;
using NLog.Targets;

namespace DataVisualization.Services.Language.Expressions
{
    public abstract class Expression
    {
        public abstract string Accept(ExpressionVisitor visitor);
    }

    public class BinaryExpression : Expression
    {
        public Expression Left { get; }
        public Token Operator { get; }
        public Expression Right { get; }

        public BinaryExpression(Expression left, Token @operator, Expression right)
        {
            Left = left;
            Operator = @operator;
            Right = right;
        }

        public override string Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitBinary(this);
        }
    }

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

    public class GroupingExpression : Expression
    {
        public Expression Expression { get;}

        public GroupingExpression(Expression expression)
        {
            Expression = expression;
        }

        public override string Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitGrouping(this);
        }
    }

    public class UnaryExpression : Expression
    {
        public Token Operator { get; }
        public Expression Right { get; }

        public UnaryExpression(Token @operator, Expression right)
        {
            Operator = @operator;
            Right = right;
        }

        public override string Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitUnary(this);
        }
    }

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
