using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface CallExpression <: Expression {
        type: "CallExpression";
        callee: Expression;
        arguments: [ Expression ];
    }
    */
    public class CallExpression : Expression
    {
        public IExpression Callee
        {
            get;
        }
        public IExpression[] Arguments
        {
            get;
        }

        public CallExpression(IExpression callee, IExpression[] arguments, Location sourceLocation) 
            : base(NodeType.CallExpression, sourceLocation)
        {
            Callee = callee;
            Arguments = arguments;
        }

        public CallExpression(NodeType type, IExpression callee, IExpression[] arguments, Location sourceLocation)
            : base(type, sourceLocation)
        {
            Callee = callee;
            Arguments = arguments;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            if(this.Callee is FunctionExpression)
            {
                writer.Write("(");
                this.Callee.Format(writer, semi);
                writer.Write(")");
            }
            else
            {
                this.Callee.Format(writer, semi);
            }
            writer.Write("(");
            var first = true;
            foreach(var a  in this.Arguments)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.Write(", ");
                }
                a.Format(writer, semi);
            }
            writer.Write(")");
        }

        public override string ToString()
        {
            string callee;
            if(this.Callee is FunctionExpression)
            {
                callee = string.Format("({0})", this.Callee.ToString());
            }
            else
            {
                callee = this.Callee.ToString();
            }
            if (Arguments.Length == 0)
            {
                return string.Format("{0}()", callee);
            }
            if (Arguments.Length == 1)
            {
                return string.Format("{0}({1})", callee, Arguments[0].ToString());
            }

            var sb = new StringBuilder();
            sb.AppendFormat("{0}({1}", callee, Arguments[0].ToString());
            for (var i = 1; i < Arguments.Length; i++)
            {
                sb.AppendFormat(", {0}", Arguments[i].ToString());
            }
            return sb.Append(")").ToString();
        }
    }
}
