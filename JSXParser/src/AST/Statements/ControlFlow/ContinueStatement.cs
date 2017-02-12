using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface LabeledStatement <: Statement {
        type: "LabeledStatement";
        label: Identifier;
        body: Statement;
    }
    */
    public class ContinueStatement : Statement
    {
        public Identifier Label
        {
            get;
        }

        public ContinueStatement(Identifier label, Location sourceLocation)
            : base(NodeType.ContinueStatement, sourceLocation)
        {
            Label = label;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            if (semi)
            {
                writer.Write("continue;");
            }
            else
            {
                writer.Write("continue");
            }
        }

        public override string ToString()
        {
            if (this.Label == null)
            {
                return "continue";
            }
            return string.Format("continue {0}", this.Label.ToString());
        }
    }
}
