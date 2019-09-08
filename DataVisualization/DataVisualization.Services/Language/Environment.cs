﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DataVisualization.Services.Exceptions;

namespace DataVisualization.Services.Language
{
    public class Environment
    {
        public Environment Enclosing { get; }

        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        public Environment()
        {
            Enclosing = null;
        }

        public Environment(Environment enclosing)
        {
            Enclosing = enclosing;
        }

        public void Define(string name, object value)
        {
            if (_values.ContainsKey(name))
            {
                _values[name] = value;
                return;
            }

            _values.Add(name, value);
        }

        public object Get(Token token)
        {
            if (_values.TryGetValue(token.Lexeme, out var value))
                return value;

            if (Enclosing != null)
            {
                return Enclosing.Get(token);
            }

            throw new RuntimeException($"Undefined variable {token.Lexeme}");
        }

        public void Assign(Token name, object value)
        {
            if (_values.ContainsKey(name.Lexeme))
            {
                _values[name.Lexeme] = value;
                return;
            }

            Enclosing?.Assign(name, value);

            if (Enclosing == null)
            {
                throw new RuntimeException($"Undefined variable {name}");
            }
        }

        public object GetAt(int distance, string lexeme)
        {
            var ancestor = Ancestor(distance);
            return ancestor._values[lexeme];
        }

        private Environment Ancestor(int distance)
        {
            var environment = this;
            for (var index = 0; index < distance; index++)
            {
                environment = environment.Enclosing;
            }

            return environment;
        }

        public void AssignAt(int distance, Token name, object value)
        {
            Ancestor(distance)._values.Add(name.Lexeme, value);
        }
    }
}
