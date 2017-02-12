using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface NewExpression <: CallExpression {
        type: "NewExpression";
    }
    */
    public class NewExpression : CallExpression
    {
        public NewExpression(IExpression callee, IExpression[] arguments, Location sourceLocation) 
            : base(NodeType.NewExpression, callee, arguments, sourceLocation)
        {
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            writer.Write("new ");
            base.Format(writer, semi);
        }

        public override string ToString()
        {
            return string.Format("new {0}", base.ToString());
        }
    }
}
