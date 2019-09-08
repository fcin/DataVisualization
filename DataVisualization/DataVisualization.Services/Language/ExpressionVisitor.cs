using DataVisualization.Services.Language.Expressions;

namespace DataVisualization.Services.Language
{
    public abstract class ExpressionVisitor
    {
        public abstract object VisitBinary(BinaryExpression expression);
        public abstract object VisitLiteral(LiteralExpression expression);
        public abstract object VisitGrouping(GroupingExpression expression);
        public abstract object VisitUnary(UnaryExpression expression);
        public abstract object VisitExpressionStatement(Expression expression);
        public abstract object VisitPrintStatement(Expression expression);
        public abstract object VisitVarStatement(VarStatement statement);
        public abstract object VisitVarExpression(VarExpression expression);
        public abstract object VisitAssignExpression(AssignExpression assignExpression);
        public abstract void VisitBlockStatement(BlockStatement blockStatement);
        public abstract object VisitIfStatement(IfStatement ifStatement);
        public abstract object VisitLogicalExpression(LogicalExpression logicalExpression);
        public abstract object VisitWhileStatement(WhileStatement whileStatement);
        public abstract object VisitCallExpression(CallExpression callExpression);
        public abstract object VisitFunctionStatement(FunctionStatement functionStatement);
        public abstract object VisitReturnStatement(ReturnStatement returnStatement);
        public abstract object VisitClassStatement(ClassStatement classStatement);
        public abstract object VisitGetExpression(GetExpression getExpression);
        public abstract object VisitSetExpression(SetExpression setExpression);
        public abstract object VisitThisExpression(ThisExpression thisExpression);
    }
}