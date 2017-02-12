using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface JSXMemberExpression <: Expression {
        type: "JSXMemberExpression";
        object: JSXMemberExpression | JSXIdentifier,
        property: JSXIdentifier
    }
    */
    public class JSXMemberExpression : Expression
    {
        public IExpression Object
        {
            get;
        }

        public JSXIdentifier Property
        {
            get;
        }

        public JSXMemberExpression(IExpression obj, JSXIdentifier property, Location sourceLocation)
            : base(NodeType.JSXMemberExpression, sourceLocation)
        {
            Object = obj;
            Property = property;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            this.Object.Format(writer, semi);
            writer.Write(".");
            this.Property.Format(writer, semi);
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}", Object, Property);
        }
    }
}
