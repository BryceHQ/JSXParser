using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    public abstract class Statement : Node, IStatement
    {
        public Statement(NodeType type, Location sourceLocation)
            :base(type, sourceLocation)
        {
        }
    }
}
