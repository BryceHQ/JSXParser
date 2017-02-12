using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface JSXOpeningElement <: JSXBoundaryElement {
        type: "JSXOpeningElement",
        attributes: [ JSXAttribute | JSXSpreadAttribute ],
        selfClosing: boolean;
    }
    */
    public class JSXOpeningElement : JSXBoundaryElement
    {
        public JSXAttribute[] Attributes
        {
            get;
        }
        public bool SelfClosing
        {
            get;
        }

        public JSXOpeningElement(JSXAttribute[] attributes, bool selfClosing, INode name, Location sourceLocation)
            : base(name, NodeType.JSXOpeningElement, sourceLocation)
        {
            Attributes = attributes;
            SelfClosing = selfClosing;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            writer.Write("<");
            this.Name.Format(writer, semi);
            var indented = false;
            foreach (var attr in this.Attributes)
            {
                writer.Write(" ");
                if (writer.ReachLineEnd())
                {
                    if (!indented)
                    {
                        indented = true;
                        writer.IndentForward();
                    }
                    writer.NewLine();
                    writer.Indent();
                }
                attr.Format(writer, semi);
            }
            if (indented)
            {
                writer.IndentBack();
            }
            if (this.SelfClosing)
            {
                writer.Write("/>");
            }
            else
            {
                writer.Write(">");
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder("<");
            sb.Append(this.Name.ToString());
            foreach(var attr in this.Attributes)
            {
                sb.Append(" ");
                sb.Append(attr.ToString());
            }
            if (this.SelfClosing)
            {
                sb.Append("/>");
            }
            else
            {
                sb.Append(">");
            }
            return sb.ToString();
        }
    }
}
