using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface JSXExpressionContainer <: Node {
        type: "JSXExpressionContainer",
        expression: Expression | JSXEmptyExpression;
    }
    */
    public class JSXExpressionContainer : Node
    {
        public INode Expression
        {
            get;
        }

        public JSXExpressionContainer(INode expression, Location sourceLocation)
            : base(NodeType.JSXExpressionContainer, sourceLocation)
        {
            Expression = expression;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            writer.Write("{");
            this.Expression.Format(writer, semi);
            writer.Write("}");
        }

        public override string ToString()
        {
            return string.Format("{{{0}}}", this.Expression);
        }
    }
}
