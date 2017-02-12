using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface BreakStatement <: Statement {
        type: "BreakStatement";
        label: Identifier | null;
    }
    */
    public class BreakStatement : Statement
    {
        public Identifier Label
        {
            get;
        }

        public BreakStatement(Identifier label, Location sourceLocation)
            : base(NodeType.BreakStatement, sourceLocation)
        {
            Label = label;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            if (semi)
            {
                writer.Write("break;");
            }
            else
            {
                writer.Write("break");
            }
        }

        public override string ToString()
        {
            return "break";
        }
    }
}
