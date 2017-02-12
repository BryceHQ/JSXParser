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
    public class LabeledStatement : Statement
    {
        public Identifier Label
        {
            get;
        }

        public IStatement Body
        {
            get;
        }

        public LabeledStatement(Identifier label, IStatement body, Location sourceLocation)
            : base(NodeType.LabeledStatement, sourceLocation)
        {
            Label = label;
            Body = body;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            this.Label.Format(writer, semi);
            writer.WriteLine(": ");
            writer.IndentForward();
            writer.Indent();
            this.Body.Format(writer, semi);
            writer.IndentBack();
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", this.Label, this.Body);
        }
    }
}
