using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface ReturnStatement <: Statement {
        type: "ReturnStatement";
        argument: Expression | null;
    }
    */
    public class ReturnStatement : Statement
    {
        public IExpression Argument
        {
            get;
        }

        public ReturnStatement(IExpression argument, Location sourceLocation)
            : base(NodeType.ReturnStatement, sourceLocation)
        {
            Argument = argument;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            writer.Write("return");
            if(this.Argument != null)
            {
                writer.Write(" ");
                this.Argument.Format(writer, semi);
            }
            if (semi)
            {
                writer.Write(";");
            }
        }

        public override string ToString()
        {
            if (this.Argument == null)
            {
                return "return";
            }
            return string.Format("return {0}", this.Argument.ToString());
        }
    }
}
