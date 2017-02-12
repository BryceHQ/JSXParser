using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface JSXAttribute <: Node {
        type: "JSXAttribute",
        name: JSXIdentifier | JSXNamespacedName,
        value: Literal | JSXExpressionContainer | JSXElement | null
    }
    */
    public class JSXAttribute : Expression
    {
        public IExpression Name
        {
            get;
        }

        public INode Value
        {
            get;
        }

        public JSXAttribute(IExpression name, INode value, Location sourceLocation)
            : base(NodeType.JSXElement, sourceLocation)
        {
            Name = name;
            Value = value;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            this.Name.Format(writer, semi);
            writer.Write(" = ");
            this.Value.Format(writer, semi);
        }

        public override string ToString()
        {
            return string.Format("{0} = {1}", this.Name, this.Value);
        }
    }
}
