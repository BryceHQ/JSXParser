using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface JSXNamespacedName <: Expression {
        type: "JSXNamespacedName";
        namespace: JSXIdentifier,
        name: JSXIdentifier
    }
    */
    public class JSXNamespacedName : Expression
    {
        public JSXIdentifier Namespace
        {
            get;
        }

        public JSXIdentifier Name
        {
            get;
        }

        public JSXNamespacedName(JSXIdentifier parent, JSXIdentifier name, Location sourceLocation)
            : base(NodeType.JSXNamespacedName, sourceLocation)
        {
            this.Namespace = parent;
            this.Name = name;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            this.Namespace.Format(writer, semi);
            writer.Write(":");
            this.Name.Format(writer, semi);
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.Namespace, this.Name);
        }
    }
}
