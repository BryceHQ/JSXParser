using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface ForInStatement <: Statement {
        type: "ForInStatement";
        left: VariableDeclaration |  Pattern;
        right: Expression;
        body: Statement;
    }
    */
    /// <summary>
    /// A for/in statement.
    /// </summary>
    public class ForInStatement : Statement
    {
        public INode Left
        {
            get;
        }
        public IExpression Right
        {
            get;
        }
        public IStatement Body
        {
            get;
        }

        public ForInStatement(INode left, IExpression right, IStatement body, Location sourceLocation) 
            : base(NodeType.ForInStatement, sourceLocation)
        {
            Left = left;
            Right = right;
            Body = body;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            writer.Write("for(");
            this.Left.Format(writer, semi);
            writer.Write("in");
            this.Right.Format(writer, semi);
            this.Body.Format(writer, semi);
        }

        public override string ToString()
        {
            return string.Format(
                "for({0} in {1}){2}", 
                this.Left.ToString(), 
                this.Right.ToString(), 
                this.Body.ToString()
            );
        }
    }
}
