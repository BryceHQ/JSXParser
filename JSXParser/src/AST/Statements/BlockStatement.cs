using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface BlockStatement <: Statement {
        type: "BlockStatement";
        body: [ Statement ];
    }
    */
    public class BlockStatement : Statement
    {
        public IStatement[] Body
        {
            get;
        }

        public BlockStatement(IStatement[] body, Location sourceLocation)
            : base(NodeType.BlockStatement, sourceLocation)
        {
            Body = body;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            writer.WriteLine("{");
            writer.IndentForward();
            foreach(var statement in this.Body)
            {
                writer.Indent();
                statement.Format(writer, semi);
                writer.NewLine();
            }
            writer.IndentBack();
            writer.Indent();
            writer.Write("}");
        }

        public override string ToString()
        {
            var sb = new StringBuilder("{\n");
            foreach(var statement in Body)
            {
                sb.AppendFormat("{0}", statement.ToString());
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}
