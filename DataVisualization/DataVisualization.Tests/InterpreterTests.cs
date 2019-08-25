using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataVisualization.Services.Language;
using DataVisualization.Services.Language.Expressions;
using NUnit.Framework;

namespace DataVisualization.Tests
{
    [TestFixture]
    class InterpreterTests
    {
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
        }
    }
}
