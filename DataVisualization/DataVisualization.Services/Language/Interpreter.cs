using DataVisualization.Services.Exceptions;
using DataVisualization.Services.Language.Expressions;
using DataVisualization.Services.Language.Native;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace DataVisualization.Services.Language
{
    public class Interpreter : ExpressionVisitor
    {

        public List<string> Errors { get; } = new List<string>();

        public Environment Globals { get; }
        private Environment _environment;

        private readonly Dictionary<Expression, int> _locals = new Dictionary<Expression, int>();

        public Interpreter()
        {
            Globals = new Environment();
            _environment = Globals;
            Globals.Define("time", new DvTime());
        }

        public object Interpret(IEnumerable<Statement> statements, CancellationToken token)
        {
            try
            {
                foreach (var statement in statements)
                {
                    token.ThrowIfCancellationRequested();

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
            Trace.WriteLine(result);
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
            return LookupVariable(expression, expression.Name);
        }

        private object LookupVariable(Expression expression, Token name)
        {
            if (_locals.TryGetValue(expression, out var distance))
            {
                return _environment.GetAt(distance, name.Lexeme);
            }

            return Globals.Get(name);
        }

        public override object VisitAssignExpression(AssignExpression assignExpression)
        {
            var value = Evaluate(assignExpression.Value);

            if (_locals.TryGetValue(assignExpression, out var distance))
            {
                _environment.AssignAt(distance, assignExpression.Identifier, value);
            }
            else
            {
                Globals.Assign(assignExpression.Identifier, value);
            }

            return value;
        }

        public override void VisitBlockStatement(BlockStatement blockStatement)
        {
            ExecuteBlock(blockStatement.Statements, _environment);
        }

        public void ExecuteBlock(IEnumerable<Statement> statements, Environment environment)
        {
            var previous = _environment;

            try
            {
                _environment = environment;

                foreach (var statement in statements)
                {
                    Execute(statement);
                }
            }
            finally
            {
                _environment = previous;
            }
        }

        public override object VisitIfStatement(IfStatement ifStatement)
        {
            if (IsTruthy(Evaluate(ifStatement.Condition)))
            {
                Execute(ifStatement.ThenStatement);
            }
            else if (ifStatement.ElseStatement != null)
            {
                Execute(ifStatement.ElseStatement);
            }

            return null;
        }

        public override object VisitLogicalExpression(LogicalExpression logicalExpression)
        {
            var left = Evaluate(logicalExpression.Left);

            if (logicalExpression.Operator.Type == TokenType.Or)
            {
                if (IsTruthy(left))
                    return left;
            }
            else
            {
                if (!IsTruthy(left))
                    return left;
            }

            return Evaluate(logicalExpression.Right);
        }

        public override object VisitWhileStatement(WhileStatement whileStatement)
        {
            while (IsTruthy(Evaluate(whileStatement.Condition)))
            {
                Execute(whileStatement.Body);
            }

            return null;
        }

        public override object VisitCallExpression(CallExpression callExpression)
        {
            var callee = Evaluate(callExpression.Callee);
            var arguments = new List<object>();

            foreach (var argument in callExpression.Arguments)
            {
                arguments.Add(Evaluate(argument));
            }

            if (callee is ICallable function)
            {
                if (arguments.Count != function.Arity())
                {
                    throw new RuntimeException(callExpression.RightParenthesis, $"Expected {function.Arity()} arguments, but got {arguments.Count}");
                }

                return function.Call(this, arguments);
            }

            throw new RuntimeException(callExpression.RightParenthesis, "Specified object is not callable");
        }

        public override object VisitFunctionStatement(FunctionStatement functionStatement)
        {
            var dvFunction = new DvFunction(functionStatement, _environment, false);
            _environment.Define(functionStatement.Name.Lexeme, dvFunction);
            return null;
        }

        public override object VisitReturnStatement(ReturnStatement returnStatement)
        {
            var value = returnStatement.Value != null ? Evaluate(returnStatement.Value) : null;
            throw new ReturnException(value);
        }

        public override object VisitClassStatement(ClassStatement classStatement)
        {
            _environment.Define(classStatement.Name.Lexeme, null);

            var methods = new Dictionary<string, DvFunction>();
            foreach (var method in classStatement.Methods)
            {
                var function = new DvFunction(method, _environment, method.Name.Lexeme.Equals("init"));
                methods.Add(method.Name.Lexeme, function);
            }

            var dvClass = new DvClass(classStatement.Name.Lexeme, methods);

            _environment.Assign(classStatement.Name, dvClass);

            return null;
        }

        public override object VisitGetExpression(GetExpression getExpression)
        {
            var @object = Evaluate(getExpression.Object);
            if (@object is DvInstance instance)
            {
                return instance.Get(getExpression.Name);
            }

            throw new RuntimeException(getExpression.Name, "Only instances can access properties");
        }

        public override object VisitSetExpression(SetExpression setExpression)
        {
            var @object = Evaluate(setExpression.Object);

            if(!(@object is DvInstance instance))
                throw new RuntimeException("Only instances have fields");

            var value = Evaluate(setExpression.Value);
            instance.Set(setExpression.Name, value);

            return value;
        }

        public override object VisitThisExpression(ThisExpression thisExpression)
        {
            return LookupVariable(thisExpression, thisExpression.Keyword);
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
                    
                    Debug.WriteLine($"Left was: {left?.GetType()}");
                    Debug.WriteLine($"Right was: {right?.GetType()}");

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

        public void Resolve(Expression expression, int index)
        {
            if (_locals.ContainsKey(expression))
            {
                _locals[expression] = index;
                return;
            }

            _locals.Add(expression, index);
        }
    }
}
