using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface ObjectExpression <: Expression {
        type: "ObjectExpression";
        properties: [ Property ];
    }
    */
    public class ObjectExpression : Expression
    {
        public Property[] Properties
        {
            get;
        }

        public ObjectExpression(Property[] properties, Location sourceLocation) 
            : base(NodeType.ObjectExpression, sourceLocation)
        {
            Properties = properties;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            writer.Write("{");
            if (this.Properties.Length > 0)
            {
                writer.IndentForward();
                var first = true;
                foreach (var p in this.Properties)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        writer.Write(", ");
                    }
                    writer.NewLine();
                    writer.Indent();
                    p.Format(writer, semi);
                }
                writer.NewLine();
                writer.IndentBack();
                writer.Indent();
            }
            writer.Write("}");
        }

        public override string ToString()
        {
            if(Properties.Length == 0)
            {
                return "{}";
            }
            if(Properties.Length == 1)
            {
                return string.Format("{{{0}}}", Properties[0].ToString());
            }

            var sb = new StringBuilder("{ ");
            sb.AppendFormat("{0}", Properties[0].ToString());
            for (var i = 1; i < Properties.Length; i++)
            {
                sb.AppendFormat(", {0}", Properties[i].ToString());
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}
