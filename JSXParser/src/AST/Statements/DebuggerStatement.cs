using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface EmptyStatement <: Statement {
        type: "EmptyStatement";
    }
    */
    /// <summary>
    /// An empty statement, i.e., a solitary semicolon.
    /// </summary>
    public class DebuggerStatement : Statement
    {
        public DebuggerStatement(Location sourceLocation)
            : base(NodeType.DebuggerStatement, sourceLocation)
        {
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            if (semi)
            {
                writer.Write("debugger;");
            }
            else
            {
                writer.Write("debugger");
            }
        }

        public override string ToString()
        {
            return "debugger";
        }
    }
}
