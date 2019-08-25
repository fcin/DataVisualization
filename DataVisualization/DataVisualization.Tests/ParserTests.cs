using DataVisualization.Services.Language;
using NUnit.Framework;
using System.Linq;

namespace DataVisualization.Tests
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void ShouldParse()
        {
            const string source = @"1 + 2 * 3 + 4";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer.Scan());

            var result = parser.Parse();

            Assert.AreEqual(0, parser.Errors.Count());
        }

        [Test]
        public void ShouldReturnErrorOnLeftHandOperandMissing()
        {
            const string source = @" + 3";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer.Scan());

            var result = parser.Parse();

            Assert.IsNull(result);
        }
    }
}
