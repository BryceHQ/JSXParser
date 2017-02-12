using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    public abstract class Node: INode
    {
        public NodeType Type
        {
            get;
        }

        public Location Loc
        {
            get;
        }

        public Node(NodeType type, Location loc)
        {
            Type = type;
            Loc = loc;
        }

        public abstract void Format(PositionedWriter writer, bool semi);
    }
}
