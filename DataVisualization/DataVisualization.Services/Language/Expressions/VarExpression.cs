using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataVisualization.Services.Language.Expressions
{
    public class VarExpression : Expression
    {
        public Token Name { get; }

        public VarExpression(Token name)
        {
            Name = name;
        }

        public override object Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitVarExpression(this);
        }
    }
}
