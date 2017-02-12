using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    enum AssignmentOperator {
        "=" | "+=" | "-=" | "*=" | "/=" | "%="
            | "<<=" | ">>=" | ">>>="
            | "|=" | "^=" | "&="
    }
    */
    public enum AssignmentOperator
    {
        /// <summary>
        /// =
        /// </summary>
        Assignment,
        /// <summary>
        /// +=
        /// </summary>
        Plus,
        /// <summary>
        /// -=
        /// </summary>
        Minus,
        /// <summary>
        /// *=
        /// </summary>
        Multiply,
        /// <summary>
        /// /=
        /// </summary>
        Division,
        /// <summary>
        /// %=
        /// </summary>
        Modular,
        /// <summary>
        /// <<=
        /// </summary>
        ASL,
        /// <summary>
        /// >>=
        /// </summary>
        ASR,
        /// <summary>
        /// >>>=
        /// </summary>
        LSR,
        /// <summary>
        /// |=
        /// </summary>
        Or,
        /// <summary>
        /// ^=
        /// </summary>
        XOR,
        /// <summary>
        /// &=
        /// </summary>
        And,
    }
}
