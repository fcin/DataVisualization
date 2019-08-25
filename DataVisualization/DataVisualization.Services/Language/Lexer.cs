using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel.Channels;
using System.Windows.Media.TextFormatting;

namespace DataVisualization.Services.Language
{
    public class Lexer
    {
        public List<string> Errors { get; }

        private readonly string _source;
        private readonly List<Token> _tokens;
        private readonly Dictionary<string, TokenType> _keywords = new Dictionary<string, TokenType>
        {
            { "and", TokenType.And },
            { "class", TokenType.Class },
            { "else", TokenType.Else },
            { "false", TokenType.False },
            { "for", TokenType.For },
            { "fun", TokenType.Fun },
            { "if", TokenType.If },
            { "null", TokenType.Null },
            { "or", TokenType.Or },
            { "print", TokenType.Print },
            { "return", TokenType.Return },
            { "super", TokenType.Super },
            { "this", TokenType.This },
            { "true", TokenType.True },
            { "var", TokenType.Var },
            { "while", TokenType.While }
        };

        private int _start;
        private int _current;
        private int _line = 1;

        public Lexer(string source)
        {
            _source = source;
            _tokens = new List<Token>();
            Errors = new List<string>();
        }

        public List<Token> Scan()
        {
            while (!IsEof())
            {
                _start = _current;
                ScanToken();
            }

            var eof = new Token(TokenType.Eof, string.Empty, null, _line);
            _tokens.Add(eof);
            return _tokens;
        }

        private void ScanToken()
        {
            var character = Advance();
            switch (character)
            {
                case '(': AddToken(TokenType.LeftParenthesis); break;
                case ')': AddToken(TokenType.RightParenthesis); break;
                case '{': AddToken(TokenType.LeftBrace); break;
                case '}': AddToken(TokenType.RightBrace); break;
                case ',': AddToken(TokenType.Comma); break;
                case '.': AddToken(TokenType.Dot); break;
                case '-': AddToken(TokenType.Minus); break;
                case '+': AddToken(TokenType.Plus); break;
                case ';': AddToken(TokenType.Semicolon); break;
                case '*': AddToken(TokenType.Star); break;
                case '!': AddToken(Match('=') ? TokenType.BangEqual : TokenType.Bang); break;
                case '=': AddToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal); break;
                case '<': AddToken(Match('=') ? TokenType.LessEqual : TokenType.Less); break;
                case '>': AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater); break;
                case '/':
                    if (Peek() == '/')
                    {
                        while (PeekNext() != '\n' && !IsEof())
                            Advance();

                        AddToken(TokenType.MultilineComment);
                    }
                    else if (Peek() == '*')
                    {
                        HandleMultilineComment();
                    }
                    else
                    {
                        AddToken(TokenType.Slash);
                    }

                    break;
                case ' ':
                case '\r':
                case '\t':
                    break;
                case '\n':
                    _line++;
                    break;
                case '"':
                    HandleString();
                    break;
                default:
                    if (IsDigit(character))
                    {
                        HandleNumber();
                        break;
                    }

                    if (IsAlpha(character))
                    {
                        HandleIdentifier();
                        break;
                    }

                    Errors.Add($"{_line}: Unexpected character");
                    break;
            }
        }

        private void HandleMultilineComment()
        {
            Advance();
            Advance();

            while (Peek() != '*' && PeekNext() != '/' || IsEof())
            {

                if (Peek() == '\n')
                    _line++;

                Advance();
            }

            if (IsEof())
            {
                Errors.Add($"{_line}: Not ended comment");
            }

            Advance();
            Advance();

            var comment = _source.Substring(_start, _current - _start);
            AddToken(TokenType.MultilineComment, comment);
        }

        private void HandleIdentifier()
        {
            while (IsAlphaNumeric(Peek()))
                Advance();

            var lexeme = _source.Substring(_start, _current - _start);
            if (_keywords.TryGetValue(lexeme, out var type))
            {
                AddToken(type);
                return;
            }
            
            AddToken(TokenType.Identifier);
        }

        private bool IsAlphaNumeric(char character)
        {
            return IsAlpha(character) || IsDigit(character);
        }

        private bool IsAlpha(char character)
        {
            return (character >= 'a' && character <= 'z') ||
                   (character >= 'A' && character <= 'Z') ||
                    character == '_';
        }

        private void HandleNumber()
        {
            while (IsDigit(Peek()))
                Advance();

            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                Advance();

                while (IsDigit(Peek()))
                    Advance();
            }

            var lexeme = _source.Substring(_start, _current - _start);
            AddToken(TokenType.Number, double.Parse(lexeme));
        }

        private void HandleString()
        {
            while (Peek() != '"' & !IsEof())
            {
                if (Peek() == '\n')
                    _line++;

                Advance();
            }

            if (IsEof())
            {
                Errors.Add($"{_line}: Unterminated string");
            }

            Advance();

            var literal = _source.Substring(_start + 1, _current - _start - 2);
            AddToken(TokenType.String, literal);
        }

        private bool IsDigit(char character) => character >= '0' && character <= '9';

        private char Peek() => IsEof() ? '\0' : _source[_current];

        private char PeekNext()
        {
            if (_current + 1 >= _source.Length )
                return '\0';

            return _source[_current + 1];
        }

        private bool Match(char expected)
        {
            if (IsEof() || _source[_current] != expected)
                return false;

            _current++;
            return true;
        }

        private void AddToken(TokenType type, object literal = null)
        {
            var lexeme = _source.Substring(_start, _current - _start);
            var token = new Token(type, lexeme, literal, _line);
            _tokens.Add(token);
        }

        private char Advance()
        {
            _current++;
            return _source[_current - 1];
        }

        private bool IsEof()
        {
            return _current >= _source.Length;
        }
    }
}
