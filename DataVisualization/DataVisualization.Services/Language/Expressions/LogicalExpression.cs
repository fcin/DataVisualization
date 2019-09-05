﻿namespace DataVisualization.Services.Language.Expressions
{
    public class LogicalExpression : Expression
    {
        public Expression Left { get; }
        public Token Operator { get; }
        public Expression Right { get; }

        public LogicalExpression(Expression left, Token @operator, Expression right)
        {
            Left = left;
            Operator = @operator;
            Right = right;
        }

        public override object Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitLogicalExpression(this);
        }
    }
}