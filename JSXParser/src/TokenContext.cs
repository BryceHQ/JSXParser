using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    public class TokenContext
    {
        public TokenType TokenType
        {
            get;
        }

        public bool IsExpr
        {
            get;
        }

        public bool PreserveSpace
        {
            get;
        }

        public TokenContext(TokenType tokenType, bool isExpr, bool preserveSpace)
        {
            this.TokenType = tokenType;
            this.IsExpr = isExpr;
            this.PreserveSpace = preserveSpace;
        }

        public bool NextToken(JSScaner scaner)
        {
            //if(scaner.Token.Info.Type == TokenType.BackQuote)
            //{
            //    scaner.re
            //    return true;
            //}
            return false;
        }

        public static TokenContext BraceStatement = new TokenContext(TokenType.BraceLeft, false, false);
        public static TokenContext BraceExpression = new TokenContext(TokenType.BraceLeft, true, false);
        public static TokenContext BraceTemplate = new TokenContext(TokenType.DollarBraceLeft, true, false);
        public static TokenContext ParenStatement = new TokenContext(TokenType.ParenLeft, false, false);
        public static TokenContext ParenExpression = new TokenContext(TokenType.ParenLeft, true, false);
        public static TokenContext QuoteTemplate = new TokenContext(TokenType.BackQuote, true, true);
        public static TokenContext FunctionExpression = new TokenContext(TokenType.BackQuote, true, false);

        //extend
        public static TokenContext JSXOpenTag = new TokenContext(TokenType.JSXTagStart, false, false);
        public static TokenContext JSXCloseTag = new TokenContext(TokenType.JSXTagEnd, false, false);
        public static TokenContext JSXExpr = new TokenContext(TokenType.JSXName, true, false);
    }
}
