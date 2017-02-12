using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface MemberExpression <: Expression, Pattern {
        type: "MemberExpression";
        object: Expression;
        property: Expression;
        computed: boolean;
    }
    */
    /// <summary>
    /// A member expression. 
    /// If computed is true, the node corresponds to a computed (a[b]) member expression and property is an Expression. If computed is false, the node corresponds to a static (a.b) member expression and property is an Identifier.
    /// </summary>
    public class MemberExpression : Expression
    {
        public IExpression Object
        {
            get;
        }
        public IExpression Property
        {
            get;
        }
        public bool Computed
        {
            get;
        }
        
        public MemberExpression(IExpression ob, IExpression property, bool computed, Location sourceLocation) 
            : base(NodeType.MemberExpression, sourceLocation)
        {
            Object = ob;
            Property = property;
            Computed = computed;
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            this.Object.Format(writer, semi);
            if (this.Computed)
            {
                writer.Write("[");
                this.Property.Format(writer, semi);
                writer.Write("]");
            }
            else
            {
                writer.Write(".");
                this.Property.Format(writer, semi);
            }
        }

        public override string ToString()
        {
            if (Computed)
            {
                return string.Format("{0}[{1}]", Object, Property);
            }
            return string.Format("{0}.{1}", Object, Property);
        }
    }
}
