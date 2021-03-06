﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    interface EmptyStatement <: Statement {
        type: "EmptyStatement";
    }
    */
    /// <summary>
    /// An empty statement, i.e., a solitary semicolon.
    /// </summary>
    public class EmptyStatement : Statement
    {
        public EmptyStatement(Location sourceLocation)
            : base(NodeType.EmptyStatement, sourceLocation)
        {
        }

        public override void Format(PositionedWriter writer, bool semi)
        {
            if (semi)
            {
                writer.Write(";");
            }
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}
