using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
        interface Property <: Node {
            type: "Property";
            key: Literal | Identifier;
            value: Expression;
            kind: "init" | "get" | "set";
        }
    */
    public class Property : Node
    {
        public IExpression Key
        {
            get;
            internal set;
        }
        public IExpression Value
        {
            get;
            internal set;
        }
        public PropertyKind Kind
        {
            get;
            internal set;
        }

        public Property(IExpression key, IExpression value, PropertyKind kind, Location sourceLocation) 
            : base(NodeType.Property, sourceLocation)
        {
            Key = key;
            Value = value;
            Kind = kind;
        }
        internal Property(Location sourceLocation)
            : base(NodeType.Property, sourceLocation)
        {
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            this.Key.Format(writer, semi);
            writer.Write(": ");
            this.Value.Format(writer, semi);
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", this.Key, this.Value);
        }
    }
}
