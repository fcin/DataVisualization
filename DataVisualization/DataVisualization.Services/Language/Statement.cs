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
}
