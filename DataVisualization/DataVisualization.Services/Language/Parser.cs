using DataVisualization.Services.Exceptions;
using DataVisualization.Services.Language.Expressions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DataVisualization.Services.Language
{
    public class Parser
    {
        public IEnumerable<string> Errors => _errors;
        private readonly List<string> _errors;

        private readonly List<Token> _tokens;
        private int _current;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens.Where(t => t.Type != TokenType.Comment && t.Type != TokenType.MultilineComment).ToList();
            _errors = new List<string>();
        }

        public IEnumerable<Statement> Parse()
        {
            var statements = new List<Statement>();

            while (!IsEof())
            {
                statements.Add(HandleDeclaration());
            }

            return statements;
        }

        private Statement HandleDeclaration()
        {
            try
            {
                if (Match(TokenType.Class))
                {
                    return ClassDeclaration();
                }

                if (Match(TokenType.Fun))
                {
                    return FunctionDeclaration();
                }

                if (Match(TokenType.Var))
                {
                    return VarDeclaration();
                }

                return HandleStatement();
            }
            catch (ParserException)
            {
                Synchronize();
                return null;
            }
        }

        private Statement ClassDeclaration()
        {
            var name = Consume(TokenType.Identifier, "Expected class name");
            Consume(TokenType.LeftBrace, "Expected '{' after class declaration");
            
            var methods = new List<FunctionStatement>();

            while (!Check(TokenType.RightBrace) && !IsEof())
            {
                var method = (FunctionStatement)FunctionDeclaration();
                methods.Add(method);
            }

            Consume(TokenType.RightBrace, "Expected '}' at the end of class declaration");

            return new ClassStatement(name, null, methods);
        }

        private Statement FunctionDeclaration()
        {
            var name = Consume(TokenType.Identifier, "Expected function name");
            Consume(TokenType.LeftParenthesis, "Expected '(' after function name.");
            var parameters = new List<Token>();
            if (!Check(TokenType.RightParenthesis))
            {
                do
                {
                    if (parameters.Count >= MaxArgumentsCount)
                    {
                        Error(Peek(), "Cannot have more than 255 parameters.");
                    }

                    parameters.Add(Consume(TokenType.Identifier, "Expect parameter name."));
                } while (Match(TokenType.Comma));
            }
            Consume(TokenType.RightParenthesis, "Expected ')' after parameters.");

            Consume(TokenType.LeftBrace, "Expected '{' after function declaration");
            var body = (BlockStatement)HandleBlock();
            return new FunctionStatement(name, parameters, body.Statements);
        }

        private Statement VarDeclaration()
        {
            var name = Consume(TokenType.Identifier, "Identifier expected");

            Expression initializer = null;
            if (Match(TokenType.Equal))
            {
                initializer = Expression();
            }

            Consume(TokenType.Semicolon, "Expected ';' after variable declaration");
            return new VarStatement(name, initializer);
        }

        private Statement HandleStatement()
        {
            if (Match(TokenType.Print))
            {
                return HandlePrintStatement();
            }

            if (Match(TokenType.Return))
            {
                var keyword = Previous();
                Expression value = null;
                if (!Check(TokenType.Semicolon))
                {
                    value = Expression();
                }

                Consume(TokenType.Semicolon, "Expected ';' at the end of return statement");

                return new ReturnStatement(keyword, value);
            }

            if (Match(TokenType.While))
            {
                Consume(TokenType.LeftParenthesis, "Expected '(' after while");

                var condition = Expression();

                Consume(TokenType.RightParenthesis, "Expected ')' after while");

                var body = HandleStatement();

                return new WhileStatement(condition, body);
            }

            if (Match(TokenType.For))
            {
                Consume(TokenType.LeftParenthesis, "Expected '(' after for statement");
                Statement initializer = null;
                if (Match(TokenType.Semicolon))
                {
                    initializer = null;
                }
                else if (Match(TokenType.Var))
                {
                    initializer = VarDeclaration();
                }
                else
                {
                    initializer = HandleDeclaration();
                }

                Expression condition = null;

                if (!Check(TokenType.Semicolon))
                {
                    condition = Expression();
                }

                Consume(TokenType.Semicolon, "Expected ';' after condition");

                Expression incrementer = null;
                if (!Check(TokenType.RightParenthesis))
                {
                    incrementer = Expression();
                }

                Consume(TokenType.RightParenthesis, "Expected ')' after for loop incrementer");

                var body = HandleStatement();

                if (incrementer != null)
                {
                    body = new BlockStatement(new List<Statement> { body, new ExpressionStatement(incrementer) });
                }

                if (condition == null)
                {
                    condition = new LiteralExpression(true);
                }

                body = new WhileStatement(condition, body);

                if (initializer != null)
                {
                    body = new BlockStatement(new List<Statement> { initializer, body });
                }

                return body;
            }

            if (Match(TokenType.LeftBrace))
            {
                return HandleBlock();
            }

            if (Match(TokenType.If))
            {
                Consume(TokenType.LeftParenthesis, "Expected '(' after 'if'.");
                var condition = Expression();
                Consume(TokenType.RightParenthesis, "Expected ')' after if condition.");
                var thenStatement = HandleStatement();
                Statement elseStatement = null;
                if (Match(TokenType.Else))
                {
                    elseStatement = HandleStatement();
                }

                return new IfStatement(condition, thenStatement, elseStatement);
            }

            return HandleExpressionStatement();
        }

        private Statement HandleBlock()
        {
            var statements = new List<Statement>();

            while (!Check(TokenType.RightBrace) && !IsEof())
            {
                statements.Add(HandleDeclaration());
            }

            Consume(TokenType.RightBrace, "Expected '}' after block.");

            return new BlockStatement(statements);
        }

        private Statement HandleExpressionStatement()
        {
            var expression = Expression();
            Consume(TokenType.Semicolon, "Expected ';' after value");
            return new ExpressionStatement(expression);
        }

        private Statement HandlePrintStatement()
        {
            var expression = Expression();
            Consume(TokenType.Semicolon, "Expected ';' after value");
            return new PrintStatement(expression);
        }

        private Expression Expression()
        {
            return Assignment();
        }

        private Expression Assignment()
        {
            var expression = Or();

            if (Match(TokenType.Equal))
            {
                var equals = Previous();
                var value = Assignment();

                if (expression is VarExpression varExpression)
                {
                    var name = varExpression.Name;
                    return new AssignExpression(name, value);
                }

                if (expression is GetExpression getExpression)
                {
                    return new SetExpression(getExpression.Object, getExpression.Name, value);
                }

                throw Error(equals, "Invalid assignment");
            }

            return expression;
        }

        private Expression Or()
        {
            var expression = And();

            while (Match(TokenType.Or))
            {
                var @operator = Peek();
                var right = And();
                expression = new LogicalExpression(expression, @operator, right);
            }

            return expression;
        }

        private Expression And()
        {
            var expression = Equality();

            while (Match(TokenType.And))
            {
                var @operator = Peek();
                var right = Equality();
                expression = new LogicalExpression(expression, @operator, right);
            }

            return expression;
        }

        private Expression Equality()
        {
            var expression = Comparison();

            while (Match(TokenType.BangEqual, TokenType.EqualEqual))
            {
                var @operator = Previous();
                var right = Comparison();
                expression = new BinaryExpression(expression, @operator, right);
            }

            return expression;
        }

        private Expression Comparison()
        {
            var addition = Addition();

            while (Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
            {
                var @operator = Previous();
                var right = Addition();
                addition = new BinaryExpression(addition, @operator, right);
            }

            return addition;
        }

        private Expression Addition()
        {
            var expression = Multiplication();

            while (Match(TokenType.Plus, TokenType.Minus))
            {
                var @operator = Previous();
                var right = Multiplication();
                expression = new BinaryExpression(expression, @operator, right);
            }

            return expression;
        }

        private Expression Multiplication()
        {
            var expression = Unary();

            while (Match(TokenType.Star, TokenType.Slash))
            {
                var @operator = Previous();
                var right = Unary();
                expression = new BinaryExpression(expression, @operator, right);
            }

            return expression;
        }

        private Expression Unary()
        {

            if (Match(TokenType.Bang, TokenType.Minus))
            {
                var right = Unary();
                var @operator = Previous();
                return new UnaryExpression(@operator, right);
            }

            return Call();
        }

        private Expression Call()
        {
            var expression = Primary();

            while (true)
            {
                if (Match(TokenType.LeftParenthesis))
                {
                    expression = FinishCall(expression);
                }
                else if (Match(TokenType.Dot))
                {
                    var name = Consume(TokenType.Identifier, "Expected property name after '.' identifier");
                    expression = new GetExpression(expression, name);
                }
                else
                {
                    break;
                }
            }

            return expression;
        }

        private const int MaxArgumentsCount = 255;

        private Expression FinishCall(Expression callee)
        {
            var arguments = new List<Expression>();

            if (!Check(TokenType.RightParenthesis))
            {
                do
                {
                    if (arguments.Count > MaxArgumentsCount)
                    {
                        Error(Peek(), $"Maximum number of arguments is {MaxArgumentsCount}");
                    }
                    arguments.Add(Expression());
                } while (Match(TokenType.Comma));
            }

            var token = Consume(TokenType.RightParenthesis, "Expected ')' after function call");

            return new CallExpression(callee, token, arguments);
        }

        private Expression Primary()
        {
            if (Match(TokenType.False))
                return new LiteralExpression(false);
            if (Match(TokenType.True))
                return new LiteralExpression(true);
            if (Match(TokenType.Null))
                return new LiteralExpression(null);

            if (Match(TokenType.Number, TokenType.String))
            {
                return new LiteralExpression(Previous().Literal);
            }

            if (Match(TokenType.Identifier))
            {
                return new VarExpression(Previous());
            }

            if (Match(TokenType.LeftParenthesis))
            {
                var expression = Expression();
                Consume(TokenType.RightParenthesis, "Expected ')' after expression");
                return new GroupingExpression(expression);
            }

            throw Error(Peek(), "Expected expression");
        }

        private void Synchronize()
        {
            while (!IsEof())
            {
                if (_current == 0)
                    Advance();

                if (Previous().Type == TokenType.Semicolon)
                {
                    Advance();
                    return;
                }

                switch (Peek().Type)
                {
                    case TokenType.Class:
                    case TokenType.Fun:
                    case TokenType.Var:
                    case TokenType.For:
                    case TokenType.If:
                    case TokenType.While:
                    case TokenType.Print:
                    case TokenType.Return:
                        Advance();
                        return;
                }

                Advance();
            }
        }

        private Token Consume(TokenType token, string error)
        {
            if (Check(token))
            {
                return Advance();
            }

            throw Error(Peek(), error);
        }

        private ParserException Error(Token token, string message)
        {
            if (token.Type == TokenType.Eof)
            {
                _errors.Add($"Line: {token.Line}: ' at end', {message}");
            }
            else
            {
                _errors.Add($"Line: {token.Line}: '{token.Lexeme}', {message}");
            }

            Trace.WriteLine($"{token.Lexeme}: {message}");

            return new ParserException(token, message);
        }

        private bool Match(params TokenType[] types)
        {
            foreach (var tokenType in types)
            {
                if (Check(tokenType))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        private bool Check(TokenType type)
        {
            if (IsEof())
                return false;

            return type == Peek().Type;
        }

        private Token Advance()
        {
            if (!IsEof())
                _current++;

            return Previous();
        }

        private Token Previous() => _tokens[_current - 1];

        private bool IsEof() => Peek().Type == TokenType.Eof;

        private Token Peek() => _tokens[_current];
    }
}
