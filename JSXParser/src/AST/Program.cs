using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface Program <: Node {
        type: "Program";
        body: [ Statement ];
    }
    */
    public class Program : Node
    {
        public IStatement[] Body
        {
            get;
        }

        public Program(IStatement[] body, Location sourceLocation)
            : base(NodeType.Program, sourceLocation)
        {
            Body = body;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            foreach (var statement in Body)
            {
                statement.Format(writer, semi);
                writer.NewLine();
            }
        }

        public string Format(bool semi)
        {
            var writer = new PositionedWriter();
            foreach (var statement in Body)
            {
                statement.Format(writer, semi);
                writer.NewLine();
            }
            return writer.ToString();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var statement in Body)
            {
                sb.Append(statement.ToString());
            }
            return sb.ToString();
        }
    }
}
