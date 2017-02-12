using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface LogicalExpression <: Expression {
        type: "LogicalExpression";
        operator: LogicalOperator;
        left: Expression;
        right: Expression;
    }
    */
    public class LogicalExpression : Expression
    {
        public string Op
        {
            get;
        }
        public IExpression Left
        {
            get;
        }
        public IExpression Right
        {
            get;
        }

        public LogicalExpression(string op, IExpression left, IExpression right, Location sourceLocation) 
            : base(NodeType.LogicalExpression, sourceLocation)
        {
            Op = op;
            Left = left;
            Right = right;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            this.Left.Format(writer, semi);
            writer.Write(" ");
            writer.Write(this.Op);
            writer.Write(" ");
            this.Right.Format(writer, semi);
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", this.Left, this.Op, this.Right);
        }
    }
}
