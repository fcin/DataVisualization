using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using DataVisualization.Services.Exceptions;
using DataVisualization.Services.Language.Expressions;

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
            _tokens = tokens;
            _errors = new List<string>();
        }

        public Expression Parse()
        {
            try
            {
                return Expression();
            }
            catch (ParserException)
            {
                return null;
            }
        }

        private Expression Expression()
        {
            return Equality();
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

            return Primary();
        }

        private Expression Primary()
        {
            if(Match(TokenType.False))
                return new LiteralExpression(false);
            if (Match(TokenType.True))
                return new LiteralExpression(true);
            if (Match(TokenType.Null))
                return new LiteralExpression(null);

            if (Match(TokenType.Number, TokenType.String))
            {
                return new LiteralExpression(Previous().Literal);
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
            Advance();

            while (!IsEof())
            {
                if (Previous().Type == TokenType.Semicolon)
                    return;

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
                        return;
                }
            }

            Advance();
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
                _errors.Add($"{token.Line} ' at end' {message}");
            }
            else
            {
                _errors.Add($"{token.Line}  at ' + {token.Lexeme} + ' {message}");
            }

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
