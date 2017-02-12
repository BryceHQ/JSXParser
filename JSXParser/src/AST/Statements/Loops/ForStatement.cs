using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface ForStatement <: Statement {
        type: "ForStatement";
        init: VariableDeclaration | Expression | null;
        test: Expression | null;
        update: Expression | null;
        body: Statement;
    }
    */
    /// <summary>
    /// A for statement.
    /// </summary>
    public class ForStatement : Statement
    {
        public INode Init
        {
            get;
        }
        public IExpression Test
        {
            get;
        }
        public IExpression Update
        {
            get;
        }
        public IStatement Body
        {
            get;
        }

        public ForStatement(INode init, IExpression test, IExpression update, IStatement body, Location sourceLocation) 
            : base(NodeType.ForStatement, sourceLocation)
        {
            Init = init;
            Test = test;
            Update = update;
            Body = body;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            writer.Write("for(");
            this.Init.Format(writer, false);
            writer.Write(";");
            this.Test.Format(writer, false);
            writer.Write(";");
            this.Update.Format(writer, false);
            writer.Write(";)");
            this.Body.Format(writer, semi);
        }

        public override string ToString()
        {
            return string.Format(
                "for({0}; {1}; {2}){3}", 
                this.Init.ToString(), 
                this.Test.ToString(), 
                this.Update.ToString(), 
                this.Body.ToString()
            );
        }
    }
}
