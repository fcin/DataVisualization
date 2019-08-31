using System.Collections.Generic;

namespace DataVisualization.Services.Language.Expressions
{
    public class CallExpression : Expression
    {
        public Expression Callee { get; }
        public Token RightParenthesis { get; }
        public IReadOnlyList<Expression> Arguments { get; }

        public CallExpression(Expression callee, Token rightParenthesis, IReadOnlyList<Expression> arguments)
        {
            Callee = callee;
            RightParenthesis = rightParenthesis;
            Arguments = arguments;
        }

        public override object Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitCallExpression(this);
        }
    }
}
