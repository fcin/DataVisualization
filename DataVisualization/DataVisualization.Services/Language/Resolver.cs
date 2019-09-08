using System;
using System.Collections.Generic;
using DataVisualization.Services.Language.Expressions;
using NLog;

namespace DataVisualization.Services.Language
{
    public class Resolver : ExpressionVisitor
    {
        private readonly List<string> _errors = new List<string>();
        public IEnumerable<string> Errors => _errors;

        private readonly Interpreter _interpreter;
        private readonly Stack<Dictionary<string, bool>> _scopes = new Stack<Dictionary<string, bool>>();
        private FunctionType _currentFunction = FunctionType.None;

        public Resolver(Interpreter interpreter)
        {
            _interpreter = interpreter;
        }

        public override object VisitExpressionStatement(Expression expression)
        {
            Resolve(expression);

            return null;
        }

        public override object VisitPrintStatement(Expression expression)
        {
            Resolve(expression);

            return null;
        }

        public override object VisitVarStatement(VarStatement statement)
        {
            Declare(statement.Name);

            if (statement.Initializer != null)
            {
                Resolve(statement.Initializer);
            }

            Define(statement.Name);

            return null;
        }

        private void Declare(Token name)
        {
            if (_scopes.Count == 0)
                return;

            var scope = _scopes.Peek();

            if (scope.ContainsKey(name.Lexeme))
            {
                _errors.Add($"Variable with name '{name.Lexeme}' already declared in this scope");
                return;
            }

            scope.Add(name.Lexeme, false);
        }

        private void Define(Token name)
        {
            if (_scopes.Count == 0)
                return;

            _scopes.Peek()[name.Lexeme] = true;
        }

        public override object VisitVarExpression(VarExpression expression)
        {
            if (_scopes.Count != 0 && _scopes.Peek().ContainsKey(expression.Name.Lexeme) && !_scopes.Peek()[expression.Name.Lexeme])
            {
                _errors.Add($"Cannot read local variable '{expression.Name.Lexeme}' in its own initializer.");
            }

            ResolveLocal(expression, expression.Name);
            return null;
        }

        private void ResolveLocal(Expression expression, Token name)
        {
            for (int index = _scopes.Count - 1; index >= 0; index--)
            {
                if (_scopes.ToArray()[index].ContainsKey(name.Lexeme))
                {
                    _interpreter.Resolve(expression, _scopes.Count - 1 - index);
                    return;
                }
            }
        }

        public override object VisitAssignExpression(AssignExpression assignExpression)
        {
            Resolve(assignExpression.Value);
            ResolveLocal(assignExpression, assignExpression.Identifier);
            return null;
        }

        public override void VisitBlockStatement(BlockStatement blockStatement)
        {
            BeginScope();
            Resolve(blockStatement.Statements);
            EndScope();
        }

        private void BeginScope()
        {
            _scopes.Push(new Dictionary<string, bool>());
        }

        public void Resolve(IEnumerable<Statement> statements)
        {
            foreach (var statement in statements)
            {
                Resolve(statement);
            }
        }

        private void Resolve(Statement statement)
        {
            statement.Accept(this);
        }

        private void Resolve(Expression expression)
        {
            expression.Accept(this);
        }

        private void EndScope()
        {
            _scopes.Pop();
        }

        public override object VisitIfStatement(IfStatement ifStatement)
        {
            Resolve(ifStatement.Condition);
            Resolve(ifStatement.ThenStatement);

            if (ifStatement.ElseStatement != null)
            {
                Resolve(ifStatement.ElseStatement);
            }

            return null;
        }

        public override object VisitLogicalExpression(LogicalExpression logicalExpression)
        {
            Resolve(logicalExpression.Left);
            Resolve(logicalExpression.Right);

            return null;
        }

        public override object VisitWhileStatement(WhileStatement whileStatement)
        {
            Resolve(whileStatement.Condition);
            Resolve(whileStatement.Condition);

            return null;
        }

        public override object VisitCallExpression(CallExpression callExpression)
        {
            Resolve(callExpression.Callee);

            foreach (var argument in callExpression.Arguments)
            {
                Resolve(argument);
            }

            return null;
        }

        public override object VisitFunctionStatement(FunctionStatement functionStatement)
        {
            Declare(functionStatement.Name);
            Define(functionStatement.Name);

            ResolveFunction(functionStatement, FunctionType.Function);

            return null;
        }

        private void ResolveFunction(FunctionStatement functionStatement, FunctionType functionType)
        {
            var enclosingFunction = _currentFunction;
            _currentFunction = functionType;

            BeginScope();

            foreach (var parameter in functionStatement.Parameters)
            {
                Declare(parameter);
                Define(parameter);
            }

            Resolve(functionStatement.Body);

            EndScope();

            _currentFunction = enclosingFunction;
        }

        public override object VisitReturnStatement(ReturnStatement returnStatement)
        {
            if (_currentFunction == FunctionType.None)
            {
                _errors.Add("Cannot return from top-level code");
                return null;
            }

            if (returnStatement.Value != null)
            {
                Resolve(returnStatement.Value);
            }

            return null;
        }

        public override object VisitClassStatement(ClassStatement classStatement)
        {
            Declare(classStatement.Name);
            Define(classStatement.Name);

            foreach (var method in classStatement.Methods)
            {
                const FunctionType declaration = FunctionType.Method;
                ResolveFunction(method, declaration);
            }

            return null;
        }

        public override object VisitGetExpression(GetExpression getExpression)
        {
            Resolve(getExpression.Object);
            return null;
        }

        public override object VisitSetExpression(SetExpression setExpression)
        {
            Resolve(setExpression.Value);
            Resolve(setExpression.Object);

            return null;
        }

        public override object VisitBinary(BinaryExpression expression)
        {
            Resolve(expression.Left);
            Resolve(expression.Right);

            return null;
        }

        public override object VisitGrouping(GroupingExpression expression)
        {
            Resolve(expression.Expression);

            return null;
        }

        public override object VisitLiteral(LiteralExpression expression)
        {
            return null;
        }

        public override object VisitUnary(UnaryExpression expression)
        {
            Resolve(expression.Right);

            return null;
        }
    }
}
