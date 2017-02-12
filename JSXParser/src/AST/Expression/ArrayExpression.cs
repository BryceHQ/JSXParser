using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface ArrayExpression <: Expression {
        type: "ArrayExpression";
        elements: [ Expression | null ];
    }
    */
    public class ArrayExpression : Expression
    {
        public IExpression[] Elements
        {
            get;
        }

        public ArrayExpression(IExpression[] elements, Location sourceLocation) 
            : base(NodeType.ArrayExpression, sourceLocation)
        {
            Elements = elements;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            writer.Write("[");
            var first = true;
            var indented = false;
            foreach (var elem in this.Elements)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.Write(", ");
                }
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
                elem.Format(writer, semi);
            }
            if (indented)
            {
                writer.IndentBack();
            }
            writer.Write("]");
        }

        public override string ToString()
        {
            var sb = new StringBuilder("[");
            var first = true;
            foreach (var elem in this.Elements)
            {
                if (first)
                {
                    sb.Append(elem);
                    first = false;
                }
                else
                {
                    sb.AppendFormat(", {0}", elem);
                }
            }
            return sb.Append("]").ToString();
        }
    }
}
