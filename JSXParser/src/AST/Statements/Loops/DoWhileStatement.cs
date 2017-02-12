using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface DoWhileStatement <: Statement {
        type: "DoWhileStatement";
        body: Statement;
        test: Expression;
    }
    */
    /// <summary>
    /// A do/while statement.
    /// </summary>
    public class DoWhileStatement : Statement
    {
        public IExpression Test
        {
            get;
        }
        public IStatement Body
        {
            get;
        }

        public DoWhileStatement(IExpression test, IStatement body, Location sourceLocation) 
            : base(NodeType.DoWhileStatement, sourceLocation)
        {
            Test = test;
            Body = body;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            writer.Write("do");
            this.Body.Format(writer, semi);
            writer.NewLine();
            writer.Indent();
            writer.Write("while(");
            this.Test.Format(writer, semi);
            writer.Write(")");
        }

        public override string ToString()
        {
            return string.Format(
                "do{0} while ({1});",
                this.Body.ToString(),
                this.Test.ToString()
            );
        }
    }
}
