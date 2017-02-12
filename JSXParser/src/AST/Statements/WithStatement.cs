using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface WithStatement <: Statement {
        type: "WithStatement";
        object: Expression;
        body: Statement;
    }
    */
    public class WithStatement : Statement
    {
        public IExpression Object { get; }

        public IStatement Body { get; }

        public WithStatement(IExpression ob, IStatement body, Location sourceLocation)
            : base(NodeType.WithStatement, sourceLocation)
        {
            this.Object = ob;
            this.Body = body;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            writer.Write("with(");
            this.Object.Format(writer, semi);
            this.Body.Format(writer, semi);
        }

        public override string ToString()
        {
            return string.Format("with({0}){1}", this.Object.ToString(), this.Body.ToString());
        }
    }
}
