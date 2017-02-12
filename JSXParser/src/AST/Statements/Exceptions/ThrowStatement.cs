using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface ThrowStatement <: Statement {
        type: "ThrowStatement";
        argument: Expression;
    }
    */
    public class ThrowStatement : Statement
    {
        public IExpression Argument
        {
            get;
        }

        public ThrowStatement(IExpression argument, Location sourceLocation)
            : base(NodeType.ThrowStatement, sourceLocation)
        {
            Argument = argument;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            writer.Write("throw ");
            this.Argument.Format(writer, semi);
            if (semi)
            {
                writer.Write(";");
            }
        }

        public override string ToString()
        {
            return string.Format("throw {0}", this.Argument.ToString());
        }
    }
}
