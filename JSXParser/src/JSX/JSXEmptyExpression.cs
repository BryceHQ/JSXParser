using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface JSXEmptyExpression <: Node {
        type: "JSXEmptyExpression"
    }
    */
    public class JSXEmptyExpression : Node
    {
        public JSXEmptyExpression(Location sourceLocation)
            : base(NodeType.JSXEmptyExpression, sourceLocation)
        {
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}
