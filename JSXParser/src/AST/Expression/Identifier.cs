using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface Identifier <: Expression, Pattern {
        type: "Identifier";
        name: string;
    }
    */
    public class Identifier : Expression, IPattern
    {
        public string Name
        {
            get;
        }

        public Identifier(string name, Location sourceLocation) : this(name, NodeType.Identifier, sourceLocation)
        {
        }

        public Identifier(string name, NodeType type, Location sourceLocation) : base(type, sourceLocation)
        {
            Name = name;
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
