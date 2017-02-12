using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    public abstract class Expression : Node, IExpression
    {
        public Expression(NodeType type, Location sourceLocation)
            : base(type, sourceLocation)
        {
        }
    }
}
