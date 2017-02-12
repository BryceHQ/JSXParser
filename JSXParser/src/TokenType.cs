using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    public enum TokenType
    {
        Num,
        Regexp,
        String,
        Name,
        EOF,
        // Punctuation token types.
        /// <summary>
        /// [
        /// </summary>
        BracketLeft,
        /// <summary>
        /// ]
        /// </summary>
        BracketRight,
        /// <summary>
        /// {
        /// </summary>
        BraceLeft,
        /// <summary>
        /// }
        /// </summary>
        BraceRight,
        /// <summary>
        /// (
        /// </summary>
        ParenLeft,
        /// <summary>
        /// )
        /// </summary>
        ParenRight,
        /// <summary>
        /// ,
        /// </summary>
        Comma,
        /// <summary>
        /// ;
        /// </summary>
        Semi,
        /// <summary>
        /// :
        /// </summary>
        Colon,
        Dot,
        Question,
        Arrow,
        Template,
        /// <summary>
        /// ...
        /// </summary>
        Ellipsis,
        /// <summary>
        /// `
        /// </summary>
        BackQuote,
        /// <summary>
        /// ${
        /// </summary>
        DollarBraceLeft,
        // Operators. These carry several kinds of properties to help the
        // parser use them properly (the presence of these properties is
        // what categorizes them as operators).
        //
        // `binop`, when present, specifies that this operator is a binary
        // operator, and will refer to its precedence.
        //
        // `prefix` and `postfix` mark the operator as a prefix or postfix
        // unary operator.
        //
        // `isAssign` marks all of `=`, `+=`, `-=` etcetera, which act as
        // binary operators with a very low precedence, that should result
        // in AssignmentExpression nodes.
        Eq,
        Assign,
        IncDec,
        Pre,
        LogicalOR,
        LogicalAND,
        BitwiseOR,
        BitwiseXOR,
        BitwiseAND,
        Equality,
        Relational,
        BitShift,
        PlusMin,
        Modulo,
        Star,
        Slash,
        StarStar,
        // Keyword token types.
        KeywordBreak,
        KeywordCase,
        KeywordCatch,
        KeywordContinue,
        KeywordDebugger,
        KeywordDefault,
        KeywordDo,
        KeywordElse,
        KeywordFinally,
        KeywordFor,
        KeywordFunction,
        KeywordIf,
        KeywordReturn,
        KeywordSwitch,
        KeywordThrow,
        KeywordTry,
        KeywordVar,
        KeywordConst,
        KeywordWhile,
        KeywordWith,
        KeywordNew,
        KeywordThis,
        KeywordSuper,
        KeywordClass,
        KeywordExtends,
        KeywordExport,
        KeywordImport,
        KeywordNull,
        KeywordTrue,
        KeywordFalse,
        KeywordIn,
        KeywordInstanceof,
        KeywordTypeof,
        KeywordVoid,
        KeywordDelete,

        //extend
        JSXName,
        JSXText,
        JSXTagStart,
        JSXTagEnd,
    }
}
