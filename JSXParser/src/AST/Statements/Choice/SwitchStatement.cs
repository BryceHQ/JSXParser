using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface SwitchStatement <: Statement {
        type: "SwitchStatement";
        discriminant: Expression;
        cases: [ SwitchCase ];
    }
    */
    public class SwitchStatement : Statement
    {
        public IExpression Discriminant
        {
            get;
        }

        public SwitchCase[] Cases
        {
            get;
        }

        public SwitchStatement(IExpression discriminant, SwitchCase[] cases, Location sourceLocation)
            : base(NodeType.SwitchStatement, sourceLocation)
        {
            Discriminant = discriminant;
            Cases = cases;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            writer.Write("switch(");
            this.Discriminant.Format(writer, semi);
            writer.Write("){");
            writer.NewLine();
            writer.IndentForward();
            foreach (var c in this.Cases)
            {
                writer.Indent();
                c.Format(writer, semi);
                writer.NewLine();
            }
            writer.IndentBack();
            writer.Indent();
            writer.Write("}");
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("switch({0}){{\n", this.Discriminant.ToString());
            foreach (var c in this.Cases)
            {
                sb.Append(c.ToString());
            }
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
