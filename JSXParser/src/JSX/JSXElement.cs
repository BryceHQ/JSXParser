using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface JSXElement <: Expression {
        type: "JSXElement",
        openingElement: JSXOpeningElement,
        children: [ Literal | JSXExpressionContainer | JSXSpreadChild | JSXElement ],
        closingElement: JSXClosingElement | null
    }
    */
    public class JSXElement : Expression
    {
        public JSXOpeningElement OpeningElement
        {
            get;
        }

        public INode[] Children
        {
            get;
        }

        public JSXClosingElement ClosingElement
        {
            get;
        }

        public JSXElement(JSXOpeningElement opening, INode[] children, JSXClosingElement closing, Location sourceLocation)
            : base(NodeType.JSXElement, sourceLocation)
        {
            this.OpeningElement = opening;
            this.Children = children;
            this.ClosingElement = closing;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            this.OpeningElement.Format(writer, semi);
            if(this.Children.Length > 0)
            {
                writer.IndentForward();
                foreach (var child in this.Children)
                {
                    writer.NewLine();
                    writer.Indent();
                    child.Format(writer, semi);
                }
                writer.IndentBack();
            }
            if(this.ClosingElement != null)
            {
                writer.NewLine();
                writer.Indent();
                this.ClosingElement.Format(writer, semi);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(this.OpeningElement);
            foreach(var child in this.Children)
            {
                sb.Append(child);
            }
            if(this.ClosingElement != null)
            {
                sb.Append(this.ClosingElement);
            }
            return sb.ToString();
        }
    }
}
