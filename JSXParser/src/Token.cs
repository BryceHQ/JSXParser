using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    public class Token
    {
        public string Value
        {
            get;
        }
        public TokenTypeInfo Info
        {
            get;
        }
        public int Start
        {
            get;
        }
        public int End
        {
            get;
        }

        public Location SourceLocation
        {
            get;
        }
        public TokenType Type
        {
            get
            {
                return this.Info.Type;
            }
        }


        public Token(string value, TokenTypeInfo info, int start, int end, Location loc)
        {
            Value = value;
            Info = info;
            Start = start;
            End = end;
            SourceLocation = loc;
        }
    }
}
