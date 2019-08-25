using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DataVisualization.Services.Exceptions;
using DataVisualization.Services.Language.Expressions;

namespace DataVisualization.Services.Language
{
    public class Interpreter : ExpressionVisitor
    {

        public List<string> Errors { get; } = new List<string>();

        private Environment _environment = new Environment();

        public object Interpret(IEnumerable<Statement> statements)
        {
            try
            {
                foreach (var statement in statements)
                {
                    Execute(statement);
                }

                return null;
            }
            catch (RuntimeException ex)
            {
                Debug.WriteLine(ex);
                Errors.Add(ex.Message);
                return null;
            }
        }

        private void Execute(Statement statement)
        {
            statement.Accept(this);
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

        public override object VisitExpressionStatement(Expression expression)
        {
            Evaluate(expression);
            return null;
        }

        public override object VisitPrintStatement(Expression expression)
        {
            var result = Evaluate(expression);
            Debug.WriteLine(result);
            return null;
        }

        public override object VisitVarStatement(VarStatement statement)
        {
            object value = null;

            if (statement.Initializer != null)
                value = Evaluate(statement.Initializer);
            
            _environment.Define(statement.Name.Lexeme, value);
            return value;
        }

        public override object VisitVarExpression(VarExpression expression)
        {
            return _environment.Get(expression.Name);
        }

        public override object VisitAssignExpression(AssignExpression assignExpression)
        {
            var value = Evaluate(assignExpression.Value);

            _environment.Assign(assignExpression.Identifier.Lexeme, value);

            return value;
        }

        public override void VisitBlockStatement(BlockStatement blockStatement)
        {
            var previous = new Environment(_environment);

            try
            {
                foreach (var statement in blockStatement.Statements)
                {
                    Execute(statement);
                }
            }
            finally
            {
                _environment = previous;
            }
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

                    if ((left is double && right is string) || (right is double && left is string))
                    {
                        return left.ToString() + right;
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
