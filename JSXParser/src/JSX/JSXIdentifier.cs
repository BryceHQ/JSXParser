using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface JSXIdentifier <: Identifier {
        type: "JSXIdentifier";
    }
    */
    public class JSXIdentifier : Identifier
    {
        public JSXIdentifier(string name, Location sourceLocation)
            : base(name, NodeType.JSXIdentifier, sourceLocation)
        {
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            writer.Write(this.Name);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
