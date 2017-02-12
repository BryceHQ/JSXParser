using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    public class TokenTypeInfo
    {
        public string Label { get; set; }
        public string Keyword { get; set; }
        public bool StartsExpr { get; set; }
        public bool BeforeExpr { get; set; }
        public bool IsLoop { get; set; }
        public bool IsAssgin { get; set; }
        public bool Prefix { get; set; }
        public bool Postfix { get; set; }
        public int Binop { get; set; }
        public TokenType Type { get; set; }

        public TokenTypeInfo()
        {
        }

        public static Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>();

        private static void SetKeyword(TokenType tokenType, TokenTypeInfo info)
        {
            info.Keyword = info.Label;
            Keywords.Add(info.Keyword, tokenType);
            SetTokenTypeInfo(tokenType, info);
        }

        private static void SetBinop(TokenType tokenType, TokenTypeInfo info)
        {
            info.BeforeExpr = true;
            SetTokenTypeInfo(tokenType, info);
        }

        private static void SetTokenTypeInfo(TokenType tokenType, TokenTypeInfo info)
        {
            info.Type = tokenType;
            TokenTypes.Add(tokenType, info);
        }

        public static Dictionary<TokenType, TokenTypeInfo> TokenTypes = new Dictionary<TokenType, TokenTypeInfo>();

        static TokenTypeInfo()
        {
            SetTokenTypeInfo(TokenType.Num, new TokenTypeInfo() { Label = "num", StartsExpr = true });
            SetTokenTypeInfo(TokenType.Regexp, new TokenTypeInfo() { Label = "Regexp", StartsExpr = true });
            SetTokenTypeInfo(TokenType.String, new TokenTypeInfo() { Label = "string", StartsExpr = true });
            SetTokenTypeInfo(TokenType.Name, new TokenTypeInfo() { Label = "name", StartsExpr = true });
            SetTokenTypeInfo(TokenType.EOF, new TokenTypeInfo() { Label = "eof", StartsExpr = true });
            // Punctuation token types.
            SetTokenTypeInfo(TokenType.BracketLeft, new TokenTypeInfo() { Label = "[", BeforeExpr = true, StartsExpr = true });
            SetTokenTypeInfo(TokenType.BracketRight, new TokenTypeInfo() { Label = "]" });
            SetTokenTypeInfo(TokenType.BraceLeft, new TokenTypeInfo() { Label = "{", BeforeExpr = true, StartsExpr = true });
            SetTokenTypeInfo(TokenType.BraceRight, new TokenTypeInfo() { Label = "}" });
            SetTokenTypeInfo(TokenType.ParenLeft, new TokenTypeInfo() { Label = "(", BeforeExpr = true, StartsExpr = true });
            SetTokenTypeInfo(TokenType.ParenRight, new TokenTypeInfo() { Label = ")" });
            SetTokenTypeInfo(TokenType.Comma, new TokenTypeInfo() { Label = ",", BeforeExpr = true });
            SetTokenTypeInfo(TokenType.Semi, new TokenTypeInfo() { Label = ";", BeforeExpr = true });
            SetTokenTypeInfo(TokenType.Colon, new TokenTypeInfo() { Label = ":", BeforeExpr = true });
            SetTokenTypeInfo(TokenType.Dot, new TokenTypeInfo() { Label = "." });
            SetTokenTypeInfo(TokenType.Question, new TokenTypeInfo() { Label = "?", BeforeExpr = true });
            SetTokenTypeInfo(TokenType.Arrow, new TokenTypeInfo() { Label = "=>", BeforeExpr = true });
            SetTokenTypeInfo(TokenType.Template, new TokenTypeInfo() { Label = "template" });
            SetTokenTypeInfo(TokenType.Ellipsis, new TokenTypeInfo() { Label = "...", BeforeExpr = true });
            SetTokenTypeInfo(TokenType.BackQuote, new TokenTypeInfo() { Label = "`", BeforeExpr = true });
            SetTokenTypeInfo(TokenType.DollarBraceLeft, new TokenTypeInfo() { Label = "${", BeforeExpr = true, StartsExpr = true });
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
            SetTokenTypeInfo(TokenType.Eq, new TokenTypeInfo() { Label = "=", BeforeExpr = true, IsAssgin = true });
            SetTokenTypeInfo(TokenType.Assign, new TokenTypeInfo() { Label = "_=", BeforeExpr = true, IsAssgin = true });
            SetTokenTypeInfo(TokenType.IncDec, new TokenTypeInfo() { Label = "++/--", Prefix = true, Postfix = true, StartsExpr = true });
            SetTokenTypeInfo(TokenType.Pre, new TokenTypeInfo() { Label = "prefix", Prefix = true, BeforeExpr = true, StartsExpr = true });
            SetBinop(TokenType.LogicalOR, new TokenTypeInfo() { Label = "||", Binop = 1 });
            SetBinop(TokenType.LogicalAND, new TokenTypeInfo() { Label = "&&", Binop = 2 });
            SetBinop(TokenType.BitwiseOR, new TokenTypeInfo() { Label = "|", Binop = 3 });
            SetBinop(TokenType.BitwiseXOR, new TokenTypeInfo() { Label = "^", Binop = 4 });
            SetBinop(TokenType.BitwiseAND, new TokenTypeInfo() { Label = "&", Binop = 5 });
            SetBinop(TokenType.Equality, new TokenTypeInfo() { Label = "&", Binop = 6 });
            SetBinop(TokenType.Relational, new TokenTypeInfo() { Label = "</>", Binop = 7 });
            SetBinop(TokenType.BitShift, new TokenTypeInfo() { Label = "<</>>", Binop = 8 });
            SetTokenTypeInfo(TokenType.PlusMin, new TokenTypeInfo() { Label = "+/-", Binop = 9, BeforeExpr = true, Prefix = true, StartsExpr = true });
            SetBinop(TokenType.Modulo, new TokenTypeInfo() { Label = "%", Binop = 10 });
            SetBinop(TokenType.Star, new TokenTypeInfo() { Label = "*", Binop = 10 });
            SetBinop(TokenType.Slash, new TokenTypeInfo() { Label = "/", Binop = 10 });
            SetTokenTypeInfo(TokenType.StarStar, new TokenTypeInfo() { Label = "**", BeforeExpr = true });
            // Keyword token types.
            SetKeyword(TokenType.KeywordBreak, new TokenTypeInfo() { Label = "break", Keyword = "break" });
            SetKeyword(TokenType.KeywordCase, new TokenTypeInfo() { Label = "case", BeforeExpr = true, Keyword = "case" });
            SetKeyword(TokenType.KeywordCatch, new TokenTypeInfo() { Label = "catch", Keyword = "catch" });
            SetKeyword(TokenType.KeywordContinue, new TokenTypeInfo() { Label = "continue", Keyword = "continue" });
            SetKeyword(TokenType.KeywordDebugger, new TokenTypeInfo() { Label = "debugger", Keyword = "debugger" });
            SetKeyword(TokenType.KeywordDefault, new TokenTypeInfo() { Label = "default", BeforeExpr = true, Keyword = "default" });
            SetKeyword(TokenType.KeywordDo, new TokenTypeInfo() { Label = "do", IsLoop = true, BeforeExpr = true, Keyword = "do" });
            SetKeyword(TokenType.KeywordElse, new TokenTypeInfo() { Label = "else", BeforeExpr = true, Keyword = "else" });
            SetKeyword(TokenType.KeywordFinally, new TokenTypeInfo() { Label = "finally", Keyword = "finally" });
            SetKeyword(TokenType.KeywordFor, new TokenTypeInfo() { Label = "for", IsLoop = true, Keyword = "for" });
            SetKeyword(TokenType.KeywordFunction, new TokenTypeInfo() { Label = "function", StartsExpr = true, Keyword = "function" });
            SetKeyword(TokenType.KeywordIf, new TokenTypeInfo() { Label = "if", Keyword = "if" });
            SetKeyword(TokenType.KeywordReturn, new TokenTypeInfo() { Label = "return", BeforeExpr = true, Keyword = "return" });
            SetKeyword(TokenType.KeywordSwitch, new TokenTypeInfo() { Label = "switch", Keyword = "switch" });
            SetKeyword(TokenType.KeywordThrow, new TokenTypeInfo() { Label = "throw", Keyword = "throw" });
            SetKeyword(TokenType.KeywordTry, new TokenTypeInfo() { Label = "try", Keyword = "try" });
            SetKeyword(TokenType.KeywordVar, new TokenTypeInfo() { Label = "var", Keyword = "var" });
            SetKeyword(TokenType.KeywordConst, new TokenTypeInfo() { Label = "const", Keyword = "const" });
            SetKeyword(TokenType.KeywordWhile, new TokenTypeInfo() { Label = "while", IsLoop = true, Keyword = "while" });
            SetKeyword(TokenType.KeywordWith, new TokenTypeInfo() { Label = "with", Keyword = "with" });
            SetKeyword(TokenType.KeywordNew, new TokenTypeInfo() { Label = "new", StartsExpr = true, Keyword = "new" });
            SetKeyword(TokenType.KeywordThis, new TokenTypeInfo() { Label = "this", StartsExpr = true, Keyword = "this" });
            SetKeyword(TokenType.KeywordSuper, new TokenTypeInfo() { Label = "super", StartsExpr = true, Keyword = "super" });
            SetKeyword(TokenType.KeywordClass, new TokenTypeInfo() { Label = "class", Keyword = "class" });
            SetKeyword(TokenType.KeywordExtends, new TokenTypeInfo() { Label = "extends", BeforeExpr = true, Keyword = "extends" });
            SetKeyword(TokenType.KeywordExport, new TokenTypeInfo() { Label = "export", Keyword = "export" });
            SetKeyword(TokenType.KeywordImport, new TokenTypeInfo() { Label = "import", Keyword = "import" });
            SetKeyword(TokenType.KeywordNull, new TokenTypeInfo() { Label = "null", StartsExpr = true, Keyword = "null" });
            SetKeyword(TokenType.KeywordTrue, new TokenTypeInfo() { Label = "true", StartsExpr = true, Keyword = "true" });
            SetKeyword(TokenType.KeywordFalse, new TokenTypeInfo() { Label = "false", StartsExpr = true, Keyword = "false" });
            SetKeyword(TokenType.KeywordIn, new TokenTypeInfo() { Label = "in", BeforeExpr = true, Binop = 7, Keyword = "in" });
            SetKeyword(TokenType.KeywordInstanceof, new TokenTypeInfo() { Label = "instanceof", BeforeExpr = true, Binop = 7, Keyword = "instanceof" });
            SetKeyword(TokenType.KeywordTypeof, new TokenTypeInfo() { Label = "typeof", BeforeExpr = true, Prefix = true, StartsExpr = true, Keyword = "typeof" });
            SetKeyword(TokenType.KeywordVoid, new TokenTypeInfo() { Label = "void", BeforeExpr = true, Prefix = true, StartsExpr = true, Keyword = "void" });
            SetKeyword(TokenType.KeywordDelete, new TokenTypeInfo() { Label = "delete", BeforeExpr = true, Prefix = true, StartsExpr = true, Keyword = "delete" });

            //extend
            SetTokenTypeInfo(TokenType.JSXName, new TokenTypeInfo() { Label = "jsxName" });
            SetTokenTypeInfo(TokenType.JSXText, new TokenTypeInfo() { Label = "jsxText", BeforeExpr = true });
            SetTokenTypeInfo(TokenType.JSXTagStart, new TokenTypeInfo() { Label = "jsxTagStartt" });
            SetTokenTypeInfo(TokenType.JSXTagEnd, new TokenTypeInfo() { Label = "jsxTagEnd" });
        }

        public static TokenTypeInfo GetTokenTypeInfo(TokenType tokenType)
        {
            return TokenTypes[tokenType];
        }
    }
}
