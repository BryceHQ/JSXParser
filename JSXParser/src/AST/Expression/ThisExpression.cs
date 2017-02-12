using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface ThisExpression <: Expression {
        type: "ThisExpression";
    }
    */
    public class ThisExpression : Expression
    {

        public ThisExpression(Location sourceLocation) 
            : base(NodeType.ThisExpression, sourceLocation)
        {
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            writer.Write("this");
        }

        public override string ToString()
        {
            return "this";
        }
    }
}
