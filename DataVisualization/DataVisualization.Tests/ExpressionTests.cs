using DataVisualization.Services.Language;
using DataVisualization.Services.Language.Expressions;
using NUnit.Framework;

namespace DataVisualization.Tests
{
    [TestFixture]
    public class ExpressionTests
    {
        [Test]
        public void ShouldReturnParenthesizedExpression()
        {
            var left = new LiteralExpression("123");
            var @operator = new Token(TokenType.Plus, "+", null, 1);
            var right = new LiteralExpression("567");

            var expression = new BinaryExpression(left, @operator, right);

            var visitor = new ExpressionVisitor();
            Assert.AreEqual("(+ 123 567)", expression.Accept(visitor));
        }
    }
}
