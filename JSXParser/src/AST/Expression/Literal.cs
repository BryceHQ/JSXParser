using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface Literal <: Expression {
        type: "Literal";
        value: string | boolean | null | number | RegExp;
    }
    */
    public class Literal : Expression
    {
        public string Value
        {
            get;
        }

        public string Raw
        {
            get;
        }

        public Literal(string value, string raw, Location sourceLocation)
            : base(NodeType.Literal, sourceLocation)
        {
            Value = value;
            Raw = raw;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            writer.Write(this.Raw);
        }

        public override string ToString()
        {
            return this.Raw;
        }
    }
}
