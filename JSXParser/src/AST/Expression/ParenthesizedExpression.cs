using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    自定义
    interface ParenthesizedExpression <: Expression {
        type: "ParenthesizedExpression";
        expr: IExpresson;
    }
    */
    public class ParenthesizedExpression : Expression
    {
        public IExpression Expr
        {
            get;
        }

        public ParenthesizedExpression(IExpression expr, Location sourceLocation)
            : base(NodeType.ParenthesizedExpression, sourceLocation)
        {
            Expr = expr;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            writer.Write("(");
            this.Expr.Format(writer, semi);
            writer.Write(")");
        }

        public override string ToString()
        {
            return string.Format("({0})", this.Expr);
        }
    }
}
