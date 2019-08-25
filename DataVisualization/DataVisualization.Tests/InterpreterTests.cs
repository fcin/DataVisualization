using DataVisualization.Services.Language;
using DataVisualization.Services.Language.Expressions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DataVisualization.Tests
{
    [TestFixture]
    class InterpreterTests
    {
        private MemoryStream _readStream;
        private MemoryTraceListener _traceListener;

        [SetUp]
        public void Setup()
        {
            _readStream = new MemoryStream();
            _traceListener = new MemoryTraceListener();
            Debug.Listeners.Add(_traceListener);
        }

        [Test]
        public void ShouldInterpretAddition()
        {
            var left = new LiteralExpression(1.0d);
            var @operator = new Token(TokenType.Plus, "+", null, 1);
            var right = new LiteralExpression(2.0d);

            var expression = new BinaryExpression(left, @operator, right);
            var interpreter = new Interpreter();
            
            var result = interpreter.Interpret(new List<Statement> { new ExpressionStatement(expression) });

            Assert.IsNull(result);
        }

        [Test]
        public void ShouldPrintStatements()
        {
            var source = @"
            print ""one"";
            print true;
            print 2 + 1;
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer.Scan());
            var interpreter = new Interpreter();
            
            interpreter.Interpret(parser.Parse());
            
            Assert.AreEqual("one\nTrue\n3\n", _traceListener.Data);
        }

        [Test]
        public void ShouldAddAndPrintNumbers()
        {
            var source = @"
            var a = 1;
            var b = 2;
            print a + b;
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer.Scan());
            var interpreter = new Interpreter();

            interpreter.Interpret(parser.Parse());

            Assert.AreEqual("3\n", _traceListener.Data);
        }
    }
}
