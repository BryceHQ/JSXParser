using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface AssignmentExpression <: Expression {
        type: "AssignmentExpression";
        operator: AssignmentOperator;
        left: Pattern | Expression;
        right: Expression;
    }
    */
    public class AssignmentExpression : Expression
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

        public AssignmentExpression(string op, IExpression left, IExpression right, Location sourceLocation) 
            : base(NodeType.AssignmentExpression, sourceLocation)
        {
            Op = op;
            Left = left;
            Right = right;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            this.Left.Format(writer, semi);
            writer.Write(" = ");
            this.Right.Format(writer, semi);
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", this.Left, this.Op, this.Right);
        }
    }
}
