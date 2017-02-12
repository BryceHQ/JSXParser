using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
        kind: "init" | "get" | "set";
    */
    public enum PropertyKind
    {
        Init,
        Get,
        Set,
    }
}
