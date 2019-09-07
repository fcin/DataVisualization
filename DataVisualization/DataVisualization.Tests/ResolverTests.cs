using System.Linq;
using DataVisualization.Services.Language;
using NUnit.Framework;

namespace DataVisualization.Tests
{
    [TestFixture]
    public class ResolverTests
    {

        [Test]
        public void ShouldReturnErrorOnMultipleDeclarationsOfTheSameVariableInTheSameScope()
        {
            var source = @"
            fun test()
            {
                var a = 5;
                var a = 5;
            }
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer.Scan());
            var interpreter = new Interpreter();
            var resolver = new Resolver(interpreter);
            var statements = parser.Parse();

            resolver.Resolve(statements);
            var errors = resolver.Errors.ToList();

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual($"Variable with name 'a' already declared in this scope", errors[0]);
        }
    }
}
