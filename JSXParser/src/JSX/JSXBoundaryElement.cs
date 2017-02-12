using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface JSXBoundaryElement <: Node {
        name: JSXIdentifier | JSXMemberExpression | JSXNamespacedName;
    }
    */
    public class JSXBoundaryElement : Node
    {
        public INode Name
        {
            get;
        }

        public JSXBoundaryElement(INode name, NodeType type, Location sourceLocation)
            : base(type, sourceLocation)
        {
            if(name == null)
            {
                throw new ArgumentNullException();
            }
            Name = name;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            this.Name.Format(writer, semi);
        }

        public override string ToString()
        {
            return this.Name.ToString();
        }
    }
}
