using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    public enum NodeType
    {
        None,
        Program,

        Property,

        Expression,

        Literal,
        Identifier,
        ThisExpression,
        ArrayExpression,
        ObjectExpression,
        FunctionExpression,
        MemberExpression,
        ConditionalExpression,
        CallExpression,
        NewExpression,
        SequenceExpression,

        UnaryExpression,
        UpdateExpression,
        BinaryExpression,
        AssignmentExpression,
        LogicalExpression,

        UnaryOperator,
        BinaryOperator,
        AssignmentOperator,
        LogicalOperator,


        Statement,
        EmptyStatement,
        ExpressionStatement,
        BlockStatement,
        DebuggerStatement,
        WithStatement,
        //Control flow
        ReturnStatement,
        LabeledStatement,
        BreakStatement,
        ContinueStatement,
        IfStatement,
        SwitchStatement,
        SwitchCase,
        //Exceptions
        ThrowStatement,
        TryStatement,
        CatchClause,
        //Loops
        WhileStatement,
        DoWhileStatement,
        ForStatement,
        ForInStatement,

        Declaration,
        FunctionDeclaration,
        VariableDeclaration,
        VariableDeclarator,

        //jsx
        JSXIdentifier,
        JSXMemberExpression,
        JSXNamespacedName,
        JSXEmptyExpression,
        JSXExpressionContainer,
        JSXSpreadChild,
        JSXBoundaryElement,
        JSXOpeningElement,
        JSXClosingElement,
        JSXAttribute,
        JSXElement,

        //extend
        ParenthesizedExpression,
    }
}
