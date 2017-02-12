using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface Declaration <: Statement { }
    */
    public abstract class Declaration : Statement
    {
        public Declaration(NodeType type, Location sourceLocation) 
            : base(type, sourceLocation)
        {
        }
    }
}
