using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /*
    enum BinaryOperator {
        "==" | "!=" | "===" | "!=="
                | "<" | "<=" | ">" | ">="
                | "<<" | ">>" | ">>>"
                | "+" | "-" | "*" | "/" | "%"
                | "|" | "^" | "&" | "in"
                | "instanceof"
    }
    */
    public enum BinaryOperator
    {
        /// <summary>
        /// ==
        /// </summary>
        Equal,
        /// <summary>
        /// !=
        /// </summary>
        NotEqual,
        /// <summary>
        /// ===
        /// </summary>
        StrictEqual,
        /// <summary>
        /// !==
        /// </summary>
        StrictNotEqual,
        /// <summary>
        /// <
        /// </summary>
        Less,
        /// <summary>
        /// <=
        /// </summary>
        LessOrEqual,
        /// <summary>
        /// >
        /// </summary>
        More,
        /// <summary>
        /// >=
        /// </summary>
        MoreOrEqual,
        /// <summary>
        /// << Arithmetic Shift Left 算术左移
        /// </summary>
        ASL,
        /// <summary>
        /// >> Arithmetic Shift Right 算术右移
        /// </summary>
        ASR,
        /// <summary>
        /// >>> Logical Shift Left 逻辑右移
        /// </summary>
        LSR,
        /// <summary>
        /// +
        /// </summary>
        Plus,
        /// <summary>
        /// -
        /// </summary>
        Minus,
        /// <summary>
        /// *
        /// </summary>
        Multiply,
        /// <summary>
        /// /
        /// </summary>
        Division,
        /// <summary>
        /// %
        /// </summary>
        Modular,
        /// <summary>
        /// |
        /// </summary>
        Or,
        /// <summary>
        /// ^
        /// </summary>
        XOR,
        /// <summary>
        /// &
        /// </summary>
        And,
        /// <summary>
        /// in
        /// </summary>
        In,
        /// <summary>
        /// instanceof
        /// </summary>
        Instanceof,
    }
}
