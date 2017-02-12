using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface SequenceExpression <: Expression {
        type: "SequenceExpression";
        expressions: [ Expression ];
    }
    */
    public class SequenceExpression : Expression
    {
        public IExpression[] Expressions
        {
            get;
        }

        public SequenceExpression(IExpression[] expressions, Location sourceLocation) 
            : base(NodeType.SequenceExpression, sourceLocation)
        {
            Expressions = expressions;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            var first = true;
            foreach(var expr in this.Expressions)
            {
                if (first)
                {
                    first = false;
                    expr.Format(writer, semi);
                }
                else
                {
                    writer.Write(", ");
                    expr.Format(writer, semi);
                }
            }
        }

        public override string ToString()
        {
            if (Expressions.Length == 0)
            {
                return "";
            }
            if (Expressions.Length == 1)
            {
                return Expressions[0].ToString();
            }

            var sb = new StringBuilder();
            sb.AppendFormat("{0}", Expressions[0].ToString());
            for (var i = 1; i < Expressions.Length; i++)
            {
                sb.AppendFormat(", {0}", Expressions[i].ToString());
            }
            return sb.ToString();
        }
    }
}
