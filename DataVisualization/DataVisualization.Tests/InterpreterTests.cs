using DataVisualization.Services;
using DataVisualization.Services.Language;
using DataVisualization.Services.Language.Expressions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using DataVisualization.Core.Annotations;

namespace DataVisualization.Tests
{
    [TestFixture]
    class InterpreterTests
    {
        private MemoryStream _readStream;
        private MemoryTraceListener _traceListener;
        private readonly string _newLine = System.Environment.NewLine;

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
            
            var result = interpreter.Interpret(new List<Statement> { new ExpressionStatement(expression) }, default(CancellationToken));

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

            var (parser, resolver, interpreter, statements) = Prepare(source);

            interpreter.Interpret(statements, default(CancellationToken));
            
            Assert.AreEqual($"one{_newLine}True{_newLine}3{_newLine}", _traceListener.Data);
        }

        [Test]
        public void ShouldAddAndPrintNumbers()
        {
            var source = @"
            var a = 1;
            var b = 2;
            print a + b;
            ";

            var (parser, resolver, interpreter, statements) = Prepare(source);

            interpreter.Interpret(statements, default(CancellationToken));

            Assert.AreEqual("3" + _newLine, _traceListener.Data);
        }

        [Test]
        public void ShouldPrintNestedLocalVariables()
        {
            const string source = @"
            var a = ""global a"";
            {
                var a = ""outer a"";
                {
                    var a = ""inner a"";
                    print a;
                }
                print a;
            }
            print a;
            ";

            var expectedResult = $"inner a{_newLine}inner a{_newLine}inner a{_newLine}";

            var (parser, resolver, interpreter, statements) = Prepare(source);

            var result = interpreter.Interpret(statements, default(CancellationToken));

            Assert.AreEqual(expectedResult, _traceListener.Data);
        }

        [Test]
        public void ShouldConcatenateStrings()
        {
            const string source = @"
            var first = ""Hello"";
            var last = ""World"";
            print ""Hi, "" + first + "" "" + last + ""!"";
            ";

            var expectedResult = $"Hi, Hello World!{_newLine}";

            var (parser, resolver, interpreter, statements) = Prepare(source);

            var result = interpreter.Interpret(statements, default(CancellationToken));

            Assert.AreEqual(expectedResult, _traceListener.Data);
        }

        [Test]
        public void ShouldConcatenateStringsInsideFunction()
        {
            const string source = @"
                fun sayHi(first, last) {
                  print ""Hi, "" + first + "" "" + last + ""!"";
                }

                sayHi(""Hello"", ""World"");
            ";

            var expectedResult = $"Hi, Hello World!{_newLine}";

            var (parser, resolver, interpreter, statements) = Prepare(source);

            var result = interpreter.Interpret(statements, default(CancellationToken));

            Assert.AreEqual(expectedResult, _traceListener.Data);
        }

        [Test]
        public void ShouldReturn2()
        {
            const string source = @"
                fun shouldReturn2(val) {
                  if(val <= 1)
                    return 1;

                   return 2;
                }

                print shouldReturn2(2);
            ";

            var expectedResult = $"2{_newLine}";

            var (parser, resolver, interpreter, statements) = Prepare(source);

            var result = interpreter.Interpret(statements, default(CancellationToken));

            Assert.AreEqual(expectedResult, _traceListener.Data);
        }

        [Test]
        public void ShouldCalculateValueInsideParameter()
        {
            const string source = @"
                fun function(val) {
                  return val;
                }

                print function(1 + 1);
            ";

            var expectedResult = $"2{_newLine}";

            var (parser, resolver, interpreter, statements) = Prepare(source);

            var result = interpreter.Interpret(statements, default(CancellationToken));

            Assert.AreEqual(expectedResult, _traceListener.Data);
        }

        [Test]
        public void ShouldExecuteRecursiveFunction()
        {
            const string source = @"
                fun function(val) {
                  if(val <= 1)
                    return 1;

                  return function(val - 1);
                }

                print function(5);
            ";

            var expectedResult = $"1{_newLine}";

            var (parser, resolver, interpreter, statements) = Prepare(source);

            var result = interpreter.Interpret(statements, default(CancellationToken));

            Assert.AreEqual(expectedResult, _traceListener.Data);
        }

        [Test]
        public void ShouldCalculateFibonacci()
        {
            const string source = @"
                fun fibonacci(n) {
                  if (n <= 1) return n;
                  return fibonacci(n - 2) + fibonacci(n - 1);
                }

                for (var i = 0; i < 5; i = i + 1) {
                  print fibonacci(i);
                }
            ";

            var expectedResult = "0\r\n1\r\n1\r\n2\r\n3\r\n";

            var (parser, resolver, interpreter, statements) = Prepare(source);

            var result = interpreter.Interpret(statements, default(CancellationToken));

            Assert.AreEqual(expectedResult, _traceListener.Data);
        }

        [Test]
        public void ShouldHandleClosures()
        {
            const string source = @"
                var a = ""global"";
                {
                  fun showA() {
                    print a;
                  }

                  showA();
                  var a = ""block"";
                  showA();
                }
            ";

            var expectedResult = $"global{_newLine}global{_newLine}";

            var (parser, resolver, interpreter, statements) = Prepare(source);

            var result = interpreter.Interpret(statements, default(CancellationToken));

            Assert.AreEqual(expectedResult, _traceListener.Data);
        }

        [Test]
        public void ShouldCreateInstanceOfClass()
        {
            const string source = @"
                class Foo { }
                var a = Foo();
                print a;
            ";

            var expectedResult = $"Foo instance{_newLine}";

            var (parser, resolver, interpreter, statements) = Prepare(source);

            var result = interpreter.Interpret(statements, default(CancellationToken));

            Assert.AreEqual(expectedResult, _traceListener.Data);
        }

        [Test]
        public void ShouldHandleFieldAssignment()
        {
            const string source = @"
                class Foo {}

                var foo = Foo();

                foo.bar = ""bar value"";

                print foo.bar;
            ";

            var expectedResult = $"bar value{_newLine}";

            var (parser, resolver, interpreter, statements) = Prepare(source);

            var result = interpreter.Interpret(statements, default(CancellationToken));

            Assert.AreEqual(expectedResult, _traceListener.Data);
        }

        [Test]
        public void ShouldHandleMethodInvocation()
        {
            const string source = @"
                class Test {
                  execute() {
                    print ""executed"";
                  }
                }

                Test().execute();
            ";

            var expectedResult = $"executed{_newLine}";

            var (parser, resolver, interpreter, statements) = Prepare(source);

            var result = interpreter.Interpret(statements, default(CancellationToken));

            Assert.AreEqual(expectedResult, _traceListener.Data);
        }

        [Test]
        public void ShouldHandleLocalFunction()
        {
            const string source = @"
                class Thing {

                  getCallback() {

                    fun localFunction() {
                      print ""Test"";
                    }
                  }
                }

                var thingObject = Thing();
                var cb = thingObject.getCallback();
                cb();
            ";

            var (parser, resolver, interpreter, statements) = Prepare(source);

            var result = interpreter.Interpret(statements, default(CancellationToken));

            Assert.AreEqual(0, resolver.Errors.Count());
            Assert.AreEqual(0, interpreter.Errors.Count());
            Assert.AreEqual($"test{_newLine}", _traceListener.Data);
        }

        [Test]
        public void ShouldHandleMethodInvocationWithClosure()
        {
            const string source = @"
                class Thing {
                  getCallback() {
                    fun localFunction() {
                      print this;
                    }

                    return localFunction;
                  }
                }

                var callback = Thing().getCallback();
                callback();
            ";

            var expectedResult = $"executed{_newLine}";

            var (parser, resolver, interpreter, statements) = Prepare(source);

            var result = interpreter.Interpret(statements, default(CancellationToken));

            Assert.AreEqual(expectedResult, _traceListener.Data);
        }

        [Test]
        public void ShouldSupportInitializer()
        {
            const string source = @"
                class Thing {
                  
                    init()
                    {
                        print ""Intializer"";
                    }
                }
                Thing();
            ";

            var expectedResult = $"Intializer{_newLine}";

            var (parser, resolver, interpreter, statements) = Prepare(source);

            var result = interpreter.Interpret(statements, default(CancellationToken));

            Assert.AreEqual(expectedResult, _traceListener.Data);
        }

        private (Parser, Resolver, Interpreter, IEnumerable<Statement>) Prepare(string source)
        {
            var lexer = new Lexer(source);
            var parser = new Parser(lexer.Scan());
            var interpreter = new Interpreter();
            var resolver = new Resolver(interpreter);
            var statements = parser.Parse().ToList();
            resolver.Resolve(statements);

            return (parser, resolver, interpreter, statements);
        }
    }
}
