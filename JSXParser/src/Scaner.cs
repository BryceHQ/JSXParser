using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JSXParser
{
    public class Scaner : JSScaner
    {
        public Scaner(string text) : base(text, 5)
        {
        }

        public Scaner(string text, int ecmaVersion) : base(text, ecmaVersion)
        {
        }

        public override void ScanToken()
        {
            var context = this.CurrentContext();
            if (context == TokenContext.JSXExpr)
            {
                this.ScanJSXToken();
                return;
            }
            var ch = _text[_pos];
            if (context == TokenContext.JSXOpenTag || context == TokenContext.JSXCloseTag)
            {
                if (IdentifierHelper.IsIdentifierStart(ch, false))
                {
                    this.ScanJSXWord();
                    return;
                }
                if (ch == '>')
                {
                    _pos++;
                    this.FinishToken(TokenType.JSXTagEnd);
                    return;
                }

                if ((ch == '\'' || ch == '"') && context == TokenContext.JSXOpenTag)
                {
                    this.ScanJSXString(ch);
                    return;
                }
            }
            if (ch == '<' && _exprAllowed)
            {
                _pos++;
                this.FinishToken(TokenType.JSXTagStart);
                return;
            }
            base.ScanToken();
        }

        private void ScanJSXToken()
        {
            var sb = new StringBuilder();
            var chunkStart = _pos;
            while (_pos < _text.Length)
            {
                var ch = _text[_pos];
                switch (ch)
                {
                    case '<':
                    case '{':
                        if (_pos == _start)
                        {
                            if (ch == '<' && _exprAllowed)
                            {
                                _pos++;
                                this.FinishToken(TokenType.JSXTagStart);
                                return;
                            }
                            this.ScanTokenFromChar(ch);
                            return;
                        }
                        sb.Append(new string(_text, chunkStart, _pos - chunkStart));
                        this.FinishToken(TokenType.JSXText, sb.ToString());
                        return;

                    case '&':
                        sb.Append(new string(_text, chunkStart, _pos - chunkStart));
                        sb.Append(this.ScanJSXEntity());
                        chunkStart = _pos;
                        break;

                    default:
                        if (this.IsNewLine(ch))
                        {
                            sb.Append(new string(_text, chunkStart, _pos - chunkStart));
                            this.FinishToken(TokenType.JSXText, sb.ToString());
                            return;
                        }
                        else {
                            _pos++;
                        }
                        break;
                }
            }
            if (_pos >= _text.Length)
            {
                this.Raise("Unterminated JSX contents");
            }
        }

        private string ScanJSXNewLine(bool normalizeCRLF)
        {
            var ch = _text[_pos];
            var sb = new StringBuilder();
            _pos++;
            if (ch == '\r' && _text[_pos] == '\n')
            {
                _pos++;
                sb.Append(normalizeCRLF ? "\\n" : "\\r\\n");
            }
            else {
                sb.Append(this.GetEscapedChar(ch));
            }
            _line++;
            _lineStart = _pos;

            return sb.ToString();
        }

        private string GetEscapedChar(char ch)
        {
            switch (ch)
            {
                case '\n':
                    return "\\n";
                case '\r':
                    return "\\r";
                case '\t':
                    return "\\t";
                case '\b':
                    return "\\b";
                case '\v':
                    return "\\u000b";
                case '\f':
                    return "\\f";
            }
            return string.Empty;
        }

        private void ScanJSXString(char quote)
        {
            var sb = new StringBuilder();
            var chunkStart = ++_pos;
            while(_pos < _text.Length)
            {
                var ch = _text[_pos];
                if (ch == quote) break;
                if(ch == '&')
                {
                    sb.Append(new string(_text, chunkStart, _pos - chunkStart));
                    sb.Append(this.ScanJSXEntity());
                    chunkStart = _pos;
                }
                else if (this.IsNewLine(ch))
                {
                    sb.Append(new string(_text, chunkStart, _pos - chunkStart));
                    sb.Append(this.ScanJSXNewLine(false));
                    chunkStart = _pos;
                }
                else
                {
                    _pos++;
                }
            }
            if(_pos >= _text.Length)
            {
                this.Raise("Unterminated string constant");
            }
            sb.Append(new string(_text, chunkStart, _pos - chunkStart));
            _pos++;
            this.FinishToken(TokenType.String, sb.ToString());
        }

        private string ScanJSXEntity()
        {
            string entity = string.Empty;
            var count = 0;
            var ch = _text[_pos];
            if (ch != '&')
                this.Raise("Entity must start with an ampersand");
            var startPos = ++_pos;
            while (_pos < _text.Length && count++ < 10)
            {
                ch = _text[_pos++];
                if (ch == ';')
                {
                    if (_text[startPos] == '#')
                    {
                        var code = 0;
                        if (_text[startPos + 1] == 'x')
                        {
                            startPos += 2;
                            if(this.ScanInt(16, startPos, _pos - startPos, out code))
                            {
                                entity = ((char)code).ToString();
                            }
                        }
                        else
                        {
                            startPos += 1;
                            if (this.ScanInt(10, startPos, _pos - startPos, out code))
                            {
                                entity = ((char)code).ToString();
                            }
                        }
                    }
                    else {
                        var key = new string(_text, startPos, _pos - startPos);
                        if (HtmlEntity.ContainsEntity(key))
                        {
                            entity = (HtmlEntity.GetEntity(key)).ToString();
                        }
                    }
                    break;
                }
            }
            if (string.IsNullOrEmpty(entity))
            {
                _pos = startPos;
                return "&";
            }
            return entity;
        }

        private void ScanJSXWord()
        {
            var ch = _text[_pos];
            while(IdentifierHelper.IsIdentifierChar(ch, false) || ch == '-')
            {
                ch = _text[++_pos];
            }
            this.FinishToken(TokenType.JSXName);
        }

        protected override void UpdateContext(Token prevToken)
        {
            if (this.Token.Type == TokenType.BraceLeft)
            {
                var curContext = this.CurrentContext();
                if (curContext == TokenContext.JSXOpenTag)
                {
                    _contexts.Push(TokenContext.BraceExpression);
                }
                else if(curContext == TokenContext.JSXExpr)
                {
                    _contexts.Push(TokenContext.BraceTemplate);
                }
                else
                {
                    base.UpdateContext(prevToken);
                }
                _exprAllowed = true;
            }
            else if (this.Token.Type == TokenType.Slash && prevToken.Type == TokenType.JSXTagStart)
            {
                _contexts.Pop(); // do not consider JSX expr -> JSX open tag -> ... anymore
                _contexts.Pop(); // reconsider as closing tag context
                _contexts.Push(TokenContext.JSXCloseTag);
                _exprAllowed = false;
            }
            else
            {
                switch (this.Token.Type)
                {
                    case TokenType.JSXTagStart:
                        _contexts.Push(TokenContext.JSXExpr); // treat as beginning of JSX expression
                        _contexts.Push(TokenContext.JSXOpenTag); // start opening tag context
                        _exprAllowed = false;
                        return;
                    case TokenType.JSXTagEnd:
                        var last = _contexts.Pop();
                        if (last == TokenContext.JSXOpenTag &&
                            prevToken.Type == TokenType.Slash ||
                            last == TokenContext.JSXCloseTag)
                        {
                            _contexts.Pop();
                            _exprAllowed = this.CurrentContext() == TokenContext.JSXExpr;
                        }
                        else
                        {
                            _exprAllowed = true;
                        }
                        return;
                }
                base.UpdateContext(prevToken);
            }
        }
    }
}
