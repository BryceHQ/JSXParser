using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    public class SyntaxException : Exception
    {
        public Position Pos
        {
            get;
            set;
        }

        public SyntaxException(Position pos, string message) 
            : base(string.Format("{0} 错误出现在第 {1} 行，第 {2} 列处.", message, pos.Line, pos.Column))
        {
            Pos = pos;
        }

    }
}
