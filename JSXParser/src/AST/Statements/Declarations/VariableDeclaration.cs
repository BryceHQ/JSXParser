using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface VariableDeclaration <: Declaration {
        type: "VariableDeclaration";
        declarations: [ VariableDeclarator ];
        kind: "var";
    }
    */
    public class VariableDeclaration : Declaration
    {
        public VariableDeclarator[] Declarations
        {
            get;
        }

        public string Kind
        {
            get;
        }

        public VariableDeclaration(VariableDeclarator[] declarations, Location sourceLocation) 
            : base(NodeType.VariableDeclaration, sourceLocation)
        {
            Declarations = declarations;
            Kind = "var";
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            writer.Write(this.Kind);
            writer.Write(" ");
            var first = true;
            var indented = false;
            foreach (var d in this.Declarations)
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
                d.Format(writer, semi);
            }
            if (indented)
            {
                writer.IndentBack();
            }
            if (semi)
            {
                writer.Write(";");
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0} ", this.Kind);
            var first = true;
            foreach (var d in this.Declarations)
            {
                if (first)
                {
                    sb.Append(d);
                    first = false;
                }
                else
                {
                    sb.AppendFormat(", {0}", d);
                }
            }
            return sb.Append(";").ToString();
        }
    }
}
