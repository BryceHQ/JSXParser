using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface FunctionDeclaration <: Function, Declaration {
        type: "FunctionDeclaration";
        id: Identifier;
    }
    */
    public class FunctionDeclaration : Declaration, IFunction
    {
        public Identifier Id
        {
            get;
        }

        public IPattern[] Params
        {
            get;
        }

        public IStatement Body
        {
            get;
        }

        public FunctionDeclaration(Identifier id, IPattern[] p, IStatement body, Location sourceLocation) 
            : base(NodeType.FunctionExpression, sourceLocation)
        {
            this.Id = id;
            this.Params = p;
            this.Body = body;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            writer.Write("function ");
            if(this.Id != null)
            {
                this.Id.Format(writer, semi);
            }
            writer.Write("(");
            var first = true;
            foreach(var p in this.Params)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.Write(", ");
                }
                p.Format(writer, semi);
            }
            writer.Write(")");
            this.Body.Format(writer, semi);
        }

        public override string ToString()
        {
            var sb = new StringBuilder("function ");
            if(this.Id != null)
            {
                sb.Append(this.Id.ToString());
            }
            var first = true;
            sb.Append("(");
            foreach(var p in this.Params)
            {
                if (first)
                {
                    first = false;
                    sb.Append(p);
                }
                else
                {
                    sb.AppendFormat(", {0}", p);
                }
            }
            sb.AppendFormat("){0}", this.Body.ToString());
            return sb.ToString();
        }
    }
}
