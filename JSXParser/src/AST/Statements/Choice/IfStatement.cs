using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface IfStatement <: Statement {
        type: "IfStatement";
        test: Expression;
        consequent: Statement;
        alternate: Statement | null;
    }
    */
    public class IfStatement : Statement
    {
        public IExpression Test
        {
            get;
        }
        public IStatement Consequent
        {
            get;
        }
        public IStatement Alternate
        {
            get;
        }

        public IfStatement(IExpression test, IStatement consequent, IStatement alternate, Location sourceLocation) 
            : base(NodeType.IfStatement, sourceLocation)
        {
            Test = test;
            Consequent = consequent;
            Alternate = alternate;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            writer.Write("if(");
            this.Test.Format(writer, semi);
            writer.Write(")");
            this.Consequent.Format(writer, semi);
            if(this.Alternate != null)
            {
                writer.NewLine();
                writer.Indent();
                writer.Write("else ");
                this.Alternate.Format(writer, semi);
            }
        }

        public override string ToString()
        {
            if(Alternate == null)
            {
                return string.Format(
                    "if({0}){1}", 
                    Test.ToString(), 
                    Consequent.ToString()
                );
            }
            return string.Format(
                "if({0}){1} else {2}", 
                Test.ToString(), 
                Consequent.ToString(), 
                Alternate.ToString()
            );
        }
    }
}
