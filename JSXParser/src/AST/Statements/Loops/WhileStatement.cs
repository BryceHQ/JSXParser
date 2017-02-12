using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface WhileStatement <: Statement {
        type: "WhileStatement";
        test: Expression;
        body: Statement;
    }
    */
    public class WhileStatement : Statement
    {
        public IExpression Test
        {
            get;
        }
        public IStatement Body
        {
            get;
        }

        public WhileStatement(IExpression test, IStatement body, Location sourceLocation) 
            : base(NodeType.WhileStatement, sourceLocation)
        {
            Test = test;
            Body = body;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            writer.Write("while(");
            this.Test.Format(writer, semi);
            this.Body.Format(writer, semi);
        }

        public override string ToString()
        {
            return string.Format(
                "while({0}){1}",
                this.Test.ToString(),
                this.Body.ToString()
            );
        }
    }
}
