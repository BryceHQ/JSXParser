using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface JSXClosingElement <: JSXBoundaryElement {
        type: "JSXClosingElement"
    }
    */
    public class JSXClosingElement : JSXBoundaryElement
    {
        public JSXClosingElement(INode name, Location sourceLocation)
            : base(name, NodeType.JSXClosingElement, sourceLocation)
        {
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            writer.Write("</");
            this.Name.Format(writer, semi);
            writer.Write(">");
        }

        public override string ToString()
        {
            return string.Format("</{0}>", this.Name);
        }
    }
}
