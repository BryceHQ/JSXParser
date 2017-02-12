using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface TryStatement <: Statement {
        type: "TryStatement";
        block: BlockStatement;
        handler: CatchClause | null;
        finalizer: BlockStatement | null;
    }
    */
    /// <summary>
    /// A try statement. If handler is null then finalizer must be a BlockStatement.
    /// </summary>
    public class TryStatement : Statement
    {
        public BlockStatement Block
        {
            get;
        }

        public CatchClause Handler
        {
            get;
        }

        public BlockStatement Finalizer
        {
            get;
        }

        public TryStatement(BlockStatement block, CatchClause handler, BlockStatement finalizer, Location sourceLocation) 
            : base(NodeType.TryStatement, sourceLocation)
        {
            Block = block;
            Handler = handler;
            Finalizer = finalizer;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            writer.Write("try ");
            this.Block.Format(writer, semi);
            if(this.Handler != null)
            {
                writer.NewLine();
                writer.Indent();
                this.Handler.Format(writer, semi);
                writer.NewLine();
            }
            if(this.Finalizer != null)
            {
                writer.Indent();
                writer.Write("finally");
                this.Finalizer.Format(writer, semi);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("try{0}", this.Block.ToString());
            if (this.Handler != null)
            {
                sb.Append(this.Handler);
            }
            if(this.Finalizer != null)
            {
                sb.AppendFormat("finally{0}", this.Finalizer.ToString());
            }
            return sb.ToString();
        }
    }
}
