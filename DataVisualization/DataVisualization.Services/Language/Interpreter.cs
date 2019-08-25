using System;
using System.Diagnostics;
using System.Collections.Generic;
using DataVisualization.Services.Exceptions;
using DataVisualization.Services.Language.Expressions;

namespace DataVisualization.Services.Language
{
    public class Interpreter : ExpressionVisitor
    {

        public List<string> Errors { get; } = new List<string>();

        public object Interpret(Expression expression)
        {
            try
            {
                return Evaluate(expression);
            }
            catch (RuntimeException ex)
            {
                Debug.WriteLine(ex);
                Errors.Add(ex.Message);
                return null;
            }
        }

        public override object VisitLiteral(LiteralExpression expression)
        {
            return expression.Literal;
        }

        public override object VisitGrouping(GroupingExpression expression)
        {
            return Evaluate(expression.Expression);
        }

        public override object VisitUnary(UnaryExpression expression)
        {
            var right = Evaluate(expression.Right);

            switch (expression.Operator.Type)
            {
                case TokenType.Minus:
                    return -(double)right;
                case TokenType.Bang:
                    return !IsTruthy(right);
            }

            return null;
        }

        public override object VisitBinary(BinaryExpression expression)
        {
            var left = Evaluate(expression.Left);
            var right = Evaluate(expression.Right);

            switch (expression.Operator.Type)
            {
                case TokenType.Minus:
                    HandleNumberOperand(left, expression.Operator, right);
                    return (double)left - (double)right;
                case TokenType.Greater:
                    HandleNumberOperand(left, expression.Operator, right);
                    return (double)left > (double)right;
                case TokenType.GreaterEqual:
                    HandleNumberOperand(left, expression.Operator, right);
                    return (double)left >= (double)right;
                case TokenType.Less:
                    HandleNumberOperand(left, expression.Operator, right);
                    return (double)left < (double)right;
                case TokenType.LessEqual:
                    HandleNumberOperand(left, expression.Operator, right);
                    return (double)left <= (double)right;
                case TokenType.Slash:
                    HandleNumberOperand(left, expression.Operator, right);
                    return (double)left / (double)right;
                case TokenType.Star:
                    HandleNumberOperand(left, expression.Operator, right);
                    return (double)left * (double)right;
                case TokenType.BangEqual:
                    return !IsEqual(left, right);
                case TokenType.Equal:
                    return IsEqual(left, right);
                case TokenType.Plus:
                    if (left is double leftDouble && right is double rightDouble)
                    {
                        return leftDouble + rightDouble;
                    }

                    if (left is string leftString && right is string rightString)
                    {
                        return leftString + rightString;
                    }

                    throw new RuntimeException(expression.Operator, "Operands must be two numbers or two strings.");
            }

            return null;
        }

        private void HandleNumberOperand(object left, Token @operator, object right)
        {
            if(!(left is double) || !(right is double))
                throw new RuntimeException(@operator, "Operand must be a number");
        }

        private bool IsEqual(object left, object right)
        {
            if (left == null && right == null)
                return true;
            if (left == null)
                return false;

            return left.Equals(right);
        }

        private bool IsTruthy(object right)
        {
            if (right == null)
                return false;
            if (right is bool rightBool)
                return rightBool;

            return true;
        }

        private object Evaluate(Expression expression)
        {
            return expression.Accept(this);
        }
    }
}
