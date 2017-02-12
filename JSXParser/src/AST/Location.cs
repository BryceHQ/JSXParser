using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    public class Location
    {
        public Position Start
        {
            get;
            set;
        }

        public Position End
        {
            get;
            set;
        }

        public Location()
        {
        }

        public Location(Position start, Position end)
        {
            this.Start = start;
            this.End = end;
        }
    }
}
