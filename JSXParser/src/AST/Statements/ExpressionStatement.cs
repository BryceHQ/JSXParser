using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface ExpressionStatement <: Statement {
        type: "ExpressionStatement";
        expression: Expression;
    }
    */
    public class ExpressionStatement : Statement
    {
        public IExpression Expression
        {
            get;
        }

        public ExpressionStatement(IExpression expression, Location sourceLocation)
            : base(NodeType.ExpressionStatement, sourceLocation)
        {
            Expression = expression;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            this.Expression.Format(writer, semi);
            if (semi)
            {
                writer.Write(";");
            }
        }

        public override string ToString()
        {
            return this.Expression.ToString();
        }
    }
}
