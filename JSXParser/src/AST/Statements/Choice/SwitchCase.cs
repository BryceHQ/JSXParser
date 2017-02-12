using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface SwitchCase <: Node {
        type: "SwitchCase";
        test: Expression | null;
        consequent: [ Statement ];
    }
    */
    /// <summary>
    /// A case (if test is an Expression) or default (if test === null) clause in the body of a switch statement.
    /// </summary>
    public class SwitchCase : Node
    {
        public IExpression Test
        {
            get;
        }
        public IStatement[] Consequent
        {
            get;
        }

        public SwitchCase(IExpression test, IStatement[] consequent, Location sourceLocation)
            : base(NodeType.SwitchCase, sourceLocation)
        {
            Test = test;
            Consequent = consequent;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            if(this.Test == null)
            {
                writer.Write("default:");
            }
            else
            {
                writer.Write("case ");
                this.Test.Format(writer, semi);
                writer.Write(":");
            }
            writer.IndentForward();
            foreach(var c in this.Consequent)
            {
                writer.NewLine();
                writer.Indent();
                c.Format(writer, semi);
            }
            writer.IndentBack();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if(this.Test == null)
            {
                sb.Append("default:\n");
            }
            else
            {
                sb.AppendFormat("case {0}:\n", this.Test.ToString());
            }
            foreach (var c in this.Consequent)
            {
                sb.Append(c.ToString());
            }
            return sb.ToString();
        }
    }
}
