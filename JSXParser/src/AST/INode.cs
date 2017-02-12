using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /// <summary>
    /// 
    /// </summary>
    public interface INode
    {
        NodeType Type
        {
            get;
        }

        Location Loc
        {
            get;
        }

        void Format(PositionedWriter writer, bool semi);
    }
}
