﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    public class Position
    {
        public int Line
        {
            get;
            set;
        }

        public int Column
        {
            get;
            set;
        }

        public Position(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public Position Clone()
        {
            return new Position(this.Line, this.Column);
        }
    }
}
