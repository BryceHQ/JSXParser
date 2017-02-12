using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    enum UnaryOperator {
        "-" | "+" | "!" | "~" | "typeof" | "void" | "delete" | "++" | "--"
    }
    */
    public enum UnaryOperator
    {
        /// <summary>
        /// -
        /// </summary>
        Positive,
        /// <summary>
        /// +
        /// </summary>
        Negative,
        /// <summary>
        /// !
        /// </summary>
        Not,
        /// <summary>
        /// ~
        /// </summary>
        Reverse,
        /// <summary>
        /// typeof 
        /// </summary>
        Typeof,
        /// <summary>
        /// void
        /// </summary>
        Void,
        /// <summary>
        /// delete
        /// </summary>
        Delete,
        /// <summary>
        /// ++
        /// </summary>
        AutoIncrement,
        /// <summary>
        /// --
        /// </summary>
        AutoDecrement,
    }
}
