using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface UnaryExpression <: Expression {
        type: "UnaryExpression";
        operator: UnaryOperator;
        prefix: boolean;
        argument: Expression;
    }
    */
    public class UnaryExpression : Expression
    {
        public string Op
        {
            get;
        }
        public bool Prefix
        {
            get;
        }
        public IExpression Argument
        {
            get;
        }

        public UnaryExpression(string op, bool prefix, IExpression argument, Location sourceLocation) 
            : base(NodeType.UnaryExpression, sourceLocation)
        {
            Op = op;
            Prefix = prefix;
            Argument = argument;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            if (this.Prefix)
            {
                writer.Write(this.Op);
                this.Argument.Format(writer, semi);
            }
            else
            {
                this.Argument.Format(writer, semi);
                writer.Write(" ");
                writer.Write(this.Op);
            }
        }

        public override string ToString()
        {
            if (this.Prefix)
            {
                return string.Format("{0}{1}", this.Op, this.Argument);
            }
            else
            {
                return string.Format("{0}{1}", this.Argument, this.Op);
            }
        }
    }
}
