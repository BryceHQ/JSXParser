using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface ConditionalExpression <: Expression {
        type: "ConditionalExpression";
        test: Expression;
        alternate: Expression;
        consequent: Expression;
    }
    */
    public class ConditionalExpression : Expression
    {
        public IExpression Test
        {
            get;
        }
        public IExpression Consequent
        {
            get;
        }
        public IExpression Alternate
        {
            get;
        }

        public ConditionalExpression(IExpression test, IExpression consequent, IExpression alternate, Location sourceLocation) 
            : base(NodeType.ConditionalExpression, sourceLocation)
        {
            Test = test;
            Consequent = consequent;
            Alternate = alternate;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            this.Test.Format(writer, semi);
            writer.Write(" ? ");
            this.Consequent.Format(writer, semi);
            writer.Write(" : ");
            this.Alternate.Format(writer, semi);
        }

        public override string ToString()
        {
            return string.Format("{0} ? {1} : {2}", Test, Consequent, Alternate);
        }
    }
}
