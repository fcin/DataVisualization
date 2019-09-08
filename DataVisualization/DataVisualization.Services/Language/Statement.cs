using System;
using System.Collections.Generic;
using DataVisualization.Services.Language.Expressions;

namespace DataVisualization.Services.Language
{
    public abstract class Statement
    {
        public abstract object Accept(ExpressionVisitor visitor);
    }

    public class PrintStatement : Statement
    {
        public Expression Expression { get; }

        public PrintStatement(Expression expression)
        {
            Expression = expression;
        }

        public override object Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitPrintStatement(Expression);
        }
    }
    public class ExpressionStatement : Statement
    {
        public Expression Expression { get; }

        public ExpressionStatement(Expression expression)
        {
            Expression = expression;
        }

        public override object Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitExpressionStatement(Expression);
        }
    }

    public class VarStatement : Statement
    {
        public Token Name { get; }
        public Expression Initializer { get; }

        public VarStatement(Token name, Expression initializer)
        {
            Name = name;
            Initializer = initializer;
        }

        public override object Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitVarStatement(this);
        }
    }

    public class BlockStatement : Statement
    {
        public IReadOnlyList<Statement> Statements { get; }

        public BlockStatement(IReadOnlyList<Statement> statements)
        {
            Statements = statements;
        }

        public override object Accept(ExpressionVisitor visitor)
        {
            visitor.VisitBlockStatement(this);
            return null;
        }
    }

    public class IfStatement : Statement
    {
        public Expression Condition { get; }
        public Statement ThenStatement { get; }
        public Statement ElseStatement { get; }

        public IfStatement(Expression condition, Statement thenStatement, Statement elseStatement)
        {
            Condition = condition;
            ThenStatement = thenStatement;
            ElseStatement = elseStatement;
        }

        public override object Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitIfStatement(this);
        }
    }

    public class WhileStatement : Statement
    {
        public Expression Condition { get; }
        public Statement Body { get; }

        public WhileStatement(Expression condition, Statement body)
        {
            Condition = condition;
            Body = body;
        }

        public override object Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitWhileStatement(this);
        }
    }

    public class FunctionStatement : Statement
    {
        public Token Name { get; }
        public IReadOnlyList<Token> Parameters { get; }
        public IEnumerable<Statement> Body { get; }

        public FunctionStatement(Token name, IReadOnlyList<Token> parameters, IEnumerable<Statement> body)
        {
            Name = name;
            Parameters = parameters;
            Body = body;
        }

        public override object Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitFunctionStatement(this);
        }
    }

    public class ReturnStatement : Statement
    {
        public Token Keyword { get; }
        public Expression Value { get; }

        public ReturnStatement(Token keyword, Expression value)
        {
            Keyword = keyword;
            Value = value;
        }

        public override object Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitReturnStatement(this);
        }
    }

    public class ClassStatement : Statement
    {
        public Token Name { get; }
        public Expression SuperClass { get; }
        public IEnumerable<FunctionStatement> Methods { get; }

        public ClassStatement(Token name, Expression superClass, IEnumerable<FunctionStatement> methods)
        {
            Name = name;
            SuperClass = superClass;
            Methods = methods;
        }

        public override object Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitClassStatement(this);
        }
    }
}
