using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataVisualization.Services.Language;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace DataVisualization.Tests
{
    [TestFixture]
    public class LexerTests
    {
        [Test]
        public void ShouldHandleComments()
        {
            var comment = "// Hello World";

            var lexer = new Lexer(comment);

            var result = lexer.Scan();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(comment, result[0].Lexeme);
        }

        [Test]
        public void ShouldHandleMultilineComments()
        {
            var comment =
            @"/* 
                Hello World
            */";

            var lexer = new Lexer(comment);

            var result = lexer.Scan();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(comment, result[0].Lexeme);
        }
    }
}
