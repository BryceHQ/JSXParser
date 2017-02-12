using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface VariableDeclarator <: Node {
        type: "VariableDeclarator";
        id: Pattern;
        init: Expression | null;
    }
    */
    /// <summary>
    /// A variable declarator.
    /// </summary>
    public class VariableDeclarator : Node
    {
        public Identifier Id
        {
            get;
        }

        public IExpression Init
        {
            get;
        }

        public VariableDeclarator(Identifier id, IExpression init, Location sourceLocation)
            : base(NodeType.VariableDeclarator, sourceLocation)
        {
            Id = id;
            Init = init;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            this.Id.Format(writer, semi);
            if(this.Init != null)
            {
                writer.Write(" = ");
                this.Init.Format(writer, semi);
            }
        }

        public override string ToString()
        {
            if(this.Init == null)
            {
                return string.Format("{0}", this.Id);
            }
            return string.Format("{0} = {1}", this.Id, this.Init);
        }
    }
}
