using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface Function<: Node
    {
        id: Identifier | null;
        params: [Pattern];
        body: BlockStatement;
    }
    */
    interface IFunction : INode
    {
        Identifier Id { get; }
        IPattern[] Params { get; }
        IStatement Body { get; }
    }
}
