using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface CatchClause <: Node {
        type: "CatchClause";
        param: Pattern;
        body: BlockStatement;
    }
    */
    public class CatchClause : Node
    {
        public Identifier Param
        {
            get;
        }

        public BlockStatement Body
        {
            get;
        }

        public CatchClause(Identifier param, BlockStatement body, Location sourceLocation)
            : base(NodeType.CatchClause, sourceLocation)
        {
            Param = param;
            Body = body;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            writer.Write("catch(");
            this.Param.Format(writer, semi);
            writer.Write(")");
            this.Body.Format(writer, semi);
        }

        public override string ToString()
        {
            return string.Format(
                "catch({0}){1}", 
                this.Param.ToString(),
                this.Body.ToString()
            );
        }
    }
}
