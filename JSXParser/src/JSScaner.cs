using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JSXParser
{
    public class JSScaner 
    {
        protected char[] _text;
        protected int _pos;
        protected int _start;
        protected int _end;
        protected int _lastTokenStart;
        protected int _lastTokenEnd;
        protected Position _startPos;
        protected int _line;
        protected int _column;
        protected int _lineStart;
        protected bool _exprAllowed;
        protected int _ecmaVersion;
        protected Stack<TokenContext> _contexts;
        protected Token _lastToken;
        protected Token _token;

        protected bool _containsEsc = false;
        public char[] Text
        {
            get
            {
                return this._text;
            }
        }
        public Token Token
        {
            get
            {
                return this._token;
            }
        }
        public int Start
        {
            get
            {
                return this._start;
            }
        }
        public int EcmaVersion
        {
            get
            {
                return this._ecmaVersion;
            }
        }

        public JSScaner(string text):this(text, 5)
        {
        }

        public JSScaner(string text, int ecmaVersion)
        {
            this.Reset(text);
            _ecmaVersion = ecmaVersion;
        }

        public virtual void Reset(string text)
        {
            int length;
            if (text == null)
            {
                length = 0;
                _text = new char[length + 1];
            }
            else
            {
                length = text.Length;
                _text = new char[length + 1];
                text.CopyTo(0, _text, 0, length);
            }
            _text[length] = '\0';

            _pos = 0;
            _start = 0;
            _end = 0;
            _lastTokenStart = 0;
            _lastTokenEnd = 0;
            _line = 0;
            _column = 0;
            _lineStart = 0;

            _token = new Token(null, TokenTypeInfo.GetTokenTypeInfo(TokenType.EOF), _pos, _pos, new Location(this.CurrentPosition(), this.CurrentPosition()));

            // The context stack is used to superficially track syntactic
            // context to predict whether a regular expression is allowed in a
            // given position.
            _contexts = new Stack<TokenContext>();
            _contexts.Push(TokenContext.BraceStatement);
            _exprAllowed = true;
        }

        public void Next()
        {
            _lastTokenStart = _start;
            _lastTokenEnd = _end;
            _lastToken = _token;
            NextToken();
        }

        public void NextToken()
        {
            var curContext = this.CurrentContext();
            if (curContext == null || !curContext.PreserveSpace) this.SkipSpace();
            _start = _pos;
            _startPos = CurrentPosition();
            this.ScanToken();
        }

        public virtual void ScanToken()
        {
            var ch = _text[_pos];
            if (IdentifierHelper.IsIdentifierStart(ch, _ecmaVersion >= 6) || ch == '\\' /* '\' */)
            {
                this.ScanWord();
                return;
            }
            this.ScanTokenFromChar(ch);
        }

        protected void ScanTokenFromChar(char ch)
        {
            switch (ch)
            {
                case '\0':
                    this.FinishToken(TokenType.EOF);
                    return;
                case '.':
                    this.ScanDot();
                    return;

                // Punctuation tokens.
                case '(':
                    this._pos++;
                    this.FinishToken(TokenType.ParenLeft, null);
                    return;
                case ')':
                    this._pos++;
                    this.FinishToken(TokenType.ParenRight, null);
                    return;
                case '[':
                    this._pos++;
                    this.FinishToken(TokenType.BracketLeft, null);
                    return;
                case ']':
                    this._pos++;
                    this.FinishToken(TokenType.BracketRight, null);
                    return;
                case '{':
                    this._pos++;
                    this.FinishToken(TokenType.BraceLeft, null);
                    return;
                case '}':
                    this._pos++;
                    this.FinishToken(TokenType.BraceRight, null);
                    return;
                case ':':
                    this._pos++;
                    this.FinishToken(TokenType.Colon, null);
                    return;
                case ';':
                    this._pos++;
                    this.FinishToken(TokenType.Semi, null);
                    return;
                case ',':
                    this._pos++;
                    this.FinishToken(TokenType.Comma, null);
                    return;
                case '?':
                    this._pos++;
                    this.FinishToken(TokenType.Question, null);
                    return;
                case '`':
                    if (this._ecmaVersion < 6)
                    {
                        break;
                    }
                    this._pos++;
                    this.FinishToken(TokenType.BackQuote, null);
                    return;

                case '\'':
                case '\"':
                    this.ScanString(ch);
                    return;
                case '/':
                    this.ScanSlash();
                    return;
                case '*':
                case '%':
                    this.ScanModuloExp(ch);
                    return;
                case '|':
                case '&':
                    this.ScanLogical(ch);
                    return;
                case '^':
                    this.ScanCaret();
                    return;
                case '+':
                case '-':
                    this.ScanPluMin(ch);
                    return;
                case '>':
                case '<':
                    this.ScanRelational(ch);
                    return;
                case '!':
                case '=':
                    this.ScanEqulity(ch);
                    return;
                case '~':
                    this._pos++;
                    this.FinishToken(TokenType.Pre);
                    return;

                default:
                    if ((ch == '0') && (
                        _text[_pos] == 'b' || _text[_pos] == 'B' ||
                        _text[_pos] == 'o' || _text[_pos] == 'O' ||
                        _text[_pos] == 'x' || _text[_pos] == 'X')
                        )
                    {
                        this.ScanNumber(_text[_pos]);
                        return;
                    }
                    else if (this.IsNumber(ch))
                    {
                        this.ScanNumber();
                        return;
                    }
                    break;
            }
            this.Raise(string.Format("Unexpected character '{0}'", ch));
        }

        protected void ScanDot()
        {
            var next = this._text[this._pos + 1];
            if (this.IsNumber(next))
            {
                this.ScanNumber();
                return;
            }
            var next2 = this._text[this._pos + 2];
            if(this._ecmaVersion >= 6 && next == '.' && next2 == '.')
            {
                this._pos += 3;
                this.FinishToken(TokenType.Ellipsis);
                return;
            }
            this._pos++;
            this.FinishToken(TokenType.Dot);
        }
        protected void ScanNumber()
        {
            while (IsNumber(this._text[this._pos]))
            {
                this._pos++;
            }
            if (this._text[this._pos] == '.')
            {
                this._pos++;
            }
            while (IsNumber(this._text[this._pos]))
            {
                this._pos++;
            }
            this.FinishToken(TokenType.Num);
        }

        /// <summary>
        /// 扫描二进制的数字
        /// </summary>
        /// <param name="prefix"></param>
        protected void ScanNumber(char prefix)
        {
            var numEnd = 0;
            var flag = false;
            switch (prefix)
            {
                case 'b'://2
                case 'B':
                    numEnd = 49;
                    break;
                case 'o'://8
                case 'O':
                    numEnd = 55;
                    break;
                default://16
                    flag = true;
                    numEnd = 58;
                    break;
            }
            this._pos += 2;
            var ch = _text[this._pos];
            var invalid = true;
            while (ch >= 48 && ch <= numEnd || (flag && (ch >= 65 && ch <= 70 || ch >= 97 && ch <= 102)))
            {
                invalid = true;
                ch = _text[this._pos++];
            }
            if (invalid)
            {
                this.Raise("Invalid or unexpected token");
            }
            this.FinishToken(TokenType.Num);
        }

        protected void ScanString(char quote)
        {
            var sb = new StringBuilder();
            var chunkStart = ++_pos;
            while (_pos < _text.Length)
            {
                char ch = _text[_pos];
                if (ch == quote) break;
                if(ch == '\\')
                {
                    sb.Append(new string(_text, chunkStart, _pos - chunkStart));
                    sb.Append(this.ScanEscapedChar(false));
                    chunkStart = _pos;
                }
                else if (this.IsNewLine(ch))
                {
                    this.Raise("Unterminated string constant");
                }
                else
                {
                    _pos++;
                }
            }
            if (this._pos >= this._text.Length)
            {
                this.Raise("Unterminated string constant");
            }
            sb.Append(new string(_text, chunkStart, _pos - chunkStart));
            _pos++;
            this.FinishToken(TokenType.String, sb.ToString());
        }

        // Used to read escaped characters
        protected string ScanEscapedChar(bool inTemplate)
        {
            var ch = this._text[++this._pos];
            ++this._pos;
            switch (ch)
            {
                case 'n':
                    return "\n";
                case 'r':
                    return "\r";
                case 't':
                    return "\t";
                case 'b':
                    return "\b";
                case 'v':
                    return "\u000b";
                case 'f':
                    return "\f";
                case 'x':
                    return ((char)this.ScanHexChar(2)).ToString();
                case 'u':
                    return this.CodePointToString(this.ScanCodePoint());
                case (char)13:
                    if (this.NextChar(0) == (char)10)// "\r\n"
                    {
                        this._pos++;
                    }
                    return string.Empty;
                case (char)10:
                    return string.Empty;
                default:
                    // \xxx
                    if (ch >= 48 && ch <= 55)
                    {
                        var end = this._pos;
                        var next = this.NextChar(1);
                        if (next >= 48 && next <= 55)
                        {
                            end++;
                            next = this.NextChar(2);
                            if (next >= 48 && next <= 55)
                            {
                                end++;
                            }
                        }

                        var octalStr = new string(this._text, this._pos - 1, end);
                        var octal = System.Convert.ToInt32(octalStr, 8);
                        if (octal > 255)
                        {
                            octalStr = new string(this._text, this._pos - 1, end - 1);
                            octal = System.Convert.ToInt32(octalStr, 8);
                        }
                        if (octalStr != "0" && inTemplate)
                        {
                            this.Raise("Octal literal in strict mode");
                        }
                        this._pos += octalStr.Length - 1;
                        return octal.ToString();
                    }
                    return ch.ToString();
            }
        }

        // Read an integer in the given radix. Return `false` if zero digits
        // were read, return `true` otherwise. When `len` is not zero, this
        // will return `false` unless the integer has exactly `len` digits.
        protected bool ScanInt(int radix, int len, out int result)
        {
            return ScanInt(radix, _pos, len, out result);
        }

        protected bool ScanInt(int radix, int start, int len, out int result)
        {
            result = 0;
            var i = 0;
            while (true)
            {
                var code = (int)this.NextChar(start, i);
                int value = 0;
                var flag = false;
                if (code >= 97) //a
                {
                    value = code - 97 + 10;
                }
                else if (code >= 65) //A
                {
                    value = code - 65 + 10;
                }
                else if (code >= 48 && code <= 57)
                {
                    value = code - 48;
                }
                else
                {
                    flag = true;
                }
                if (value >= radix || flag)
                {
                    break;
                }
                i++;
                result = result * radix + value;
            }
            if (i == 0 || len != 0 && i != len)
            {
                return false;
            }
            return true;
        }

        // Used to read character escape sequences ('\x', '\u', '\U').
        protected int ScanHexChar(int len)
        {
            var start = this._pos;
            int hex;
            var success = this.ScanInt(16, len, out hex);
            if (!success)
            {
                this.Raise("Bad character escape sequence");
            }
            return hex;
        }

        // Read a string value, interpreting backslash-escapes.
        protected int ScanCodePoint()
        {
            var ch = this.NextChar(0);
            int code;
            if (ch == '{')
            {
                if (this._ecmaVersion < 6)
                {
                    this.Unexpected();
                }
                var codePos = this._pos++;
                code = this.ScanHexChar(0);
                // ?
                if (code > 0x10FFFF)
                {
                    this.Raise("Code point out of bounds");
                }
            }
            else
            {
                code = this.ScanHexChar(4);
            }
            return code;
        }
        protected string CodePointToString(int code)
        {
            // UTF-16(0 - 0xffff) Decoding 
            if(code <= 0xffff)
            {
                return ((char)code).ToString();
            }
            //http://www.cnblogs.com/skynet/archive/2011/05/03/2035105.html
            code -= 0x10000;
            return string.Format(
                "{0}{1}",
                ((char)((code >> 10) + 0xD800)).ToString(),
                ((char)((code & 1023) + 0xDC00)).ToString()
            ); 
        }

        protected string ScanEscapedWord()
        {
            _containsEsc = false;
            var astral = _ecmaVersion >= 6;
            var chunkStart = _pos;
            var first = true;
            var word = new StringBuilder();
            while(_pos < _text.Length)
            {
                var ch = _text[_pos];
                if(IdentifierHelper.IsIdentifierChar(ch, astral))
                {
                    _pos += ch <= 0xffff ? 1 : 2;
                }
                else if(ch == '\\')
                {
                    _containsEsc = true;
                    word.Append(new string(_text, chunkStart, _pos));
                    var escStart = _pos;
                    if(_text[++_pos] != 'u')
                    {
                        this.Raise("Expecting Unicode escape sequence \\uXXXX");
                    }
                    var esc = this.ScanCodePoint();
                    var valid = first ? 
                        IdentifierHelper.IsIdentifierStart((char)esc, astral) :
                        IdentifierHelper.IsIdentifierChar((char)esc, astral);
                    if (!valid)
                    {
                        this.Raise("Invalid Unicode escape");
                    }
                    word.Append(this.CodePointToString(esc));
                    chunkStart = _pos;
                } 
                else
                {
                    break;
                }
            }
            return word.Append(new string(_text, chunkStart, _pos - chunkStart)).ToString();
        }
        protected void ScanWord()
        {
            var word = this.ScanEscapedWord();
            var type = TokenType.Name;
            if((_ecmaVersion >= 6 || !_containsEsc)&& TokenTypeInfo.Keywords.ContainsKey(word))
            {
                type = TokenTypeInfo.Keywords[word];
            }
            this.FinishToken(type, word);
        }

        protected void ScanRegexp()
        {
            var start = _pos;
            var escaped = false;
            var inClass = false;
            while (_pos < _text.Length)
            {
                var ch = _text[_pos];
                if (this.IsNewLine(ch))
                {
                    this.Raise("Unterminated regular expression");
                }
                if (!escaped)
                {
                    if (ch == '[')
                    {
                        inClass = true;
                    }
                    else if (ch == ']')
                    {
                        if(inClass) inClass = false;
                    }
                    else if (ch == '/' && !inClass)
                    {
                        break;
                    }
                    escaped = ch == '\\';
                }
                else
                {
                    escaped = false;
                }
                _pos++;
            }
            if (_pos >= _text.Length) this.Raise("Unterminated regular expression");
            var content = new string(_text, start, _pos - start);
            _pos++;
            // Need to use `ScanEscapedWord` because '\uXXXX' sequences are allowed
            // here (don't ask).
            var mods = this.ScanEscapedWord();
            var tmp = content;
            var tmpFlags = string.Empty;
            if (!string.IsNullOrEmpty(mods))
            {
                var validFlags = new Regex("^[gim]*$");
                if (_ecmaVersion >= 6) validFlags = new Regex("^[gimuy]*$");
                if (!validFlags.IsMatch(mods))
                {
                    this.Raise("Invalid regular expression flag");
                }
            }
            this.FinishToken(TokenType.Regexp, content + mods);
        }

        protected void ScanSlash()
        {
            var next = this._text[this._pos + 1];
            if (this._exprAllowed)
            {
                this._pos++;
                this.ScanRegexp();
                return;
            }
            if (next == '=')
            {
                this.FinishOp(TokenType.Assign, 2);
                return;
            }
            this.FinishOp(TokenType.Slash, 1);
        }
        protected void ScanModuloExp(char ch)
        {
            var next = this._text[this._pos + 1];
            var size = 1;
            var token = ch == '%' ? TokenType.Modulo : TokenType.Star;
            // exponentiation operator ** and **=
            if (this._ecmaVersion >= 7 && next == '*')
            {
                size++;
                token = TokenType.StarStar;
                next = this._text[this._pos + 2];
            }
            if(next == '=')
            {
                this.FinishOp(TokenType.Assign, size + 1);
                return;
            }
            this.FinishOp(token, size);
        }
        protected void ScanLogical(char ch)
        {
            var next = this._text[this._pos + 1];
            if (next == ch)
            {
                this.FinishOp(ch == '|' ? TokenType.LogicalOR : TokenType.LogicalAND, 2);
            }
            else if (next == '=')
            {
                this.FinishOp(TokenType.Assign, 2);
            }
            else
            {
                this.FinishOp(ch == '|' ? TokenType.BitwiseOR : TokenType.BitwiseAND, 1);
            }
        }
        protected void ScanCaret()
        {
            var next = this._text[this._pos + 1];
            if (next == '=')
            {
                this.FinishOp(TokenType.Assign, 2);
                return;
            }
            this.FinishOp(TokenType.BitwiseXOR, 1);
        }
        protected void ScanPluMin(char ch)
        {
            var next = this._text[this._pos + 1];
            if(next == ch)
            {
                if(next == '-' && this._text[this._pos + 2] == '>')
                {
                    //this.SkipLineComment(2);
                    //this.SkipSpace();
                    this.NextToken();
                    return;
                }
                this.FinishOp(TokenType.IncDec, 2);
                return;
            }
            if (next == '=')
            {
                this.FinishOp(TokenType.Assign, 2);
                return;
            }
            this.FinishOp(TokenType.PlusMin, 1);
        }
        // <>
        protected void ScanRelational(char ch)
        {
            var next = this.NextChar(1);
            var size = 1;
            if(next == ch)
            {
                size = ch == '>' && this.NextChar(2) == '>' ? 3 : 2;
                if(this.NextChar(size) == '=')
                {
                    this.FinishOp(TokenType.Assign, size + 1);
                    return;
                }
                this.FinishOp(TokenType.BitShift, size);
                return;
            }
            if(next == '!' && ch == '<' && this.NextChar(2) == '-' && this.NextChar(3) == '-')
            {
                //if (this.inModule) this.unexpected()
                // `<!--`, an XML-style comment that should be interpreted as a line comment
                this.SkipLineComment(4);
                this.SkipSpace();
                this.NextToken();
                return;
            }
            if(next == '=')
            {
                size = 2;
            }
            this.FinishOp(TokenType.Relational, size);
        }
        protected void ScanEqulity(char ch)
        {
            var next = this.NextChar(1);
            if(next == '=')
            {
                this.FinishOp(TokenType.Equality, this.NextChar(2) == '=' ? 3 : 2);
                return;
            }
            if(ch == '=' && next == '>' && this._ecmaVersion >= 6)
            {
                this._pos += 2;
                this.FinishToken(TokenType.Arrow);
                return;
            }
            this.FinishOp(ch == '=' ? TokenType.Eq : TokenType.Pre, 1);
        }

        protected void FinishToken(TokenType type, string value)
        {
            _end = _pos;
            var preToken = this._token;
            _token = new Token(value, TokenTypeInfo.GetTokenTypeInfo(type), _start, _end, new Location(this._startPos, this.CurrentPosition()));
            this.UpdateContext(preToken);
        }
        protected void FinishToken(TokenType type)
        {
            this.FinishToken(type, this.CurrentText());
        }
        protected void FinishOp(TokenType type, int size)
        {
            this._pos += size;
            this.FinishToken(type);
        }

        protected void SkipLineComment(int offset)
        {
            //var strat = this._pos;
            //var startLoc = this.CurrentPosition();
            this._pos += offset;
            var ch = this._text[this._pos];
            while(this._pos < this._text.Length && !this.IsNewLine(ch))
            {
                ch = this._text[++this._pos];
            }
        }
        protected void SkipBlockComment()
        {
            //var start = this._pos;
            //var startLoc = this.CurrentPosition();
            while (this._pos < this._text.Length)
            {
                var ch = this._text[this._pos];
                switch (ch)
                {
                    case '*':
                        if (this.NextChar(1) == '/')
                        {
                            this._pos += 2;
                            goto End;
                        }
                        this._pos++;
                        break;
                    default:
                        if (this.IsNewLine(ch))
                        {
                            this._pos++;
                            this._line++;
                            this._lineStart = this._pos;
                            break;
                        }
                        else
                        {
                            this._pos++;
                        }
                        break;
                }
            }
            End:
            if(this._pos == this._text.Length)
            {
                this.Raise("Unterminated comment");
            }
        }

        // Called at the start of the parse and after every token. Skips
        // whitespace and comments, and.
        public void SkipSpace()
        {
            while (_pos < _text.Length)
            {
                var ch = _text[_pos];
                switch ((int)ch)
                {
                    case 32: 
                    case 160:
                        _pos++;
                        break;
                    case 13:// \r
                        if (this.NextChar(1) == '\n')
                        {
                            _pos += 2;
                        }
                        else
                        {
                            _pos++;
                        }
                        break;
                    case 47:
                        switch (this.NextChar(1))
                        {
                            case '*':
                                this.SkipBlockComment();
                                break;
                            case '/':
                                this.SkipLineComment(2);
                                break;
                            default:
                                goto End;
                        }
                        break;
                    //Special Characters 
                    //https://msdn.microsoft.com/en-us/library/2yfce773(v=vs.94).aspx
                    case 0x1680:
                    case 0x180e:
                    case 0x202f:
                    case 0x205f:
                    case 0x3000:
                    case 0xfeff:
                        _pos++;
                        break;
                    default:
                        if (this.IsNewLine(ch))
                        {
                            _pos++;
                            _line++;
                            _column = 0;
                            _lineStart = _pos;
                            break;
                        }
                        if (!(ch > 8 && ch<14 || ch>=0x2000 && ch <= 0x200a))
                        {
                            goto End;
                        }
                        _pos++;
                        break;
                }
            }
        End:
            return;
        }

        protected char NextChar(int offset)
        {
            return this.NextChar(_pos, offset);
        }
        protected char NextChar(int start, int offset)
        {
            return this._text[start + offset];
        }

        public bool Eat(TokenType tokenType)
        {
            if(this._token.Info.Type == tokenType)
            {
                this.Next();
                return true;
            }
            return false;
        }

        public bool CanInsertSemicolon()
        {
            return this.Token.Info.Type == TokenType.EOF || 
                this.Token.Info.Type == TokenType.BraceRight ||
                this.TestLineBreak(_lastTokenEnd, _start);
        }
        public bool InsertSemicolon()
        {
            if (this.CanInsertSemicolon())
            {
                return true;
            }
            return false;
        }
        public void Semicolon()
        {
            if (!this.Eat(TokenType.Semi) && !this.InsertSemicolon())
            {
                this.Unexpected();
            }
        }
        public bool IsContextual(string name)
        {
            return this.Token.Type == TokenType.Name && this.Token.Value == name;
        }

        // Consumes contextual keyword if possible.
        public bool EatContextual(string name)
        {
            return this.Token.Value == name && this.Eat(TokenType.Name);
        }

        public void Expect(TokenType type)
        {
            if (!this.Eat(type))
            {
                this.Unexpected();
            }
        }
        public void Unexpected()
        {
            this.Raise(string.Format("Unexpected token {0}", this.Token.Type));
        }

        public void Raise(string message)
        {
            throw new SyntaxException(this.CurrentPosition(), message);
        }

        public bool AfterTrailingComma(TokenType type, bool notNext)
        {
            if (this.Token.Type == type)
            {
                if (!notNext)
                {
                    this.Next();
                }
                return true;
            }
            return false;
        }

        protected TokenContext CurrentContext()
        {
            if(_contexts.Count > 0)
            {
                return this._contexts.Peek();
            }
            return null;
        }

        protected virtual void UpdateContext(Token prevToken)
        {
            var prevType = prevToken.Info.Type;
            if (string.IsNullOrEmpty(this._token.Info.Keyword) && prevType == TokenType.Dot)
            {
                this._exprAllowed = false;
                return;
            }
            switch (this._token.Info.Type)
            {
                case TokenType.ParenRight:
                case TokenType.BraceRight:
                    if(this._contexts.Count == 1)
                    {
                        this._exprAllowed = true;
                        break;
                    }
                    var last = this._contexts.Pop();
                    if (last == TokenContext.BraceStatement && this.CurrentContext() == TokenContext.FunctionExpression)
                    {
                        this._contexts.Pop();
                        this._exprAllowed = false;
                    }
                    else if (last == TokenContext.BraceTemplate)
                    {
                        this._exprAllowed = true;
                    }
                    else {
                        this._exprAllowed = !last.IsExpr;
                    }
                    break;
                case TokenType.BraceLeft:
                    this._contexts.Push(
                        this.BraceIsBlock(prevType) ? 
                            TokenContext.BraceStatement : 
                            TokenContext.BraceExpression
                    );
                    this._exprAllowed = true;
                    break;
                case TokenType.DollarBraceLeft:
                    this._contexts.Push(TokenContext.BraceTemplate);
                    this._exprAllowed = true;
                    break;
                case TokenType.ParenLeft:
                    var statementParens = prevType == TokenType.KeywordIf ||
                        prevType == TokenType.KeywordFor ||
                        prevType == TokenType.KeywordWith ||
                        prevType == TokenType.KeywordWhile;
                    this._contexts.Push(
                        statementParens ? TokenContext.ParenStatement : TokenContext.ParenExpression
                    );
                    this._exprAllowed = true;
                    break;
                case TokenType.KeywordFunction:
                    if(prevToken.Info.BeforeExpr && 
                        prevType != TokenType.Semi && 
                        prevType != TokenType.KeywordElse && 
                        !(
                            (prevType == TokenType.Colon || prevType == TokenType.BraceLeft) &&
                            this.CurrentContext() == TokenContext.BraceStatement
                        ))
                    {
                        this._contexts.Push(TokenContext.FunctionExpression);
                    }
                    this._exprAllowed = false;
                    break;
                case TokenType.BackQuote:
                    if(this.CurrentContext() == TokenContext.QuoteTemplate)
                    {
                        this._contexts.Pop();
                    } else
                    {
                        this._contexts.Push(TokenContext.QuoteTemplate);
                    }
                    this._exprAllowed = false;
                    break;
                default:
                    this._exprAllowed = this._token.Info.BeforeExpr;
                    break;
            }
        }

        protected bool BraceIsBlock(TokenType prevType)
        {
            if(prevType== TokenType.Colon)
            {
                var parent = this.CurrentContext();
                if(parent == TokenContext.BraceStatement || parent == TokenContext.BraceExpression)
                {
                    return !parent.IsExpr;
                }
            }
            if(prevType == TokenType.KeywordReturn)
            {
                return this.TestLineBreak(this._lastTokenEnd, this._start);
            }
            if(prevType == TokenType.KeywordElse || prevType == TokenType.Semi || 
                prevType == TokenType.EOF || prevType == TokenType.ParenRight)
            {
                return true;
            }
            if(prevType == TokenType.BraceLeft)
            {
                return this.CurrentContext() == TokenContext.BraceStatement;
            }
            return !this._exprAllowed;
        }
 
        #region Helper
        public Position CurrentPosition()
        {
            return new Position(this._line, this._column);
        }
        protected string CurrentText()
        {
            return new string(_text, _start, (_pos) - _start);
        }

        protected bool IsAlpha(char ch)
        {
            switch (ch)
            {
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'G':
                case 'H':
                case 'I':
                case 'J':
                case 'K':
                case 'L':
                case 'M':
                case 'N':
                case 'O':
                case 'P':
                case 'Q':
                case 'R':
                case 'S':
                case 'T':
                case 'U':
                case 'V':
                case 'W':
                case 'X':
                case 'Y':
                case 'Z':
                case '_':
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                    return true;
            }
            return false;
        }

        protected bool IsNumber(char ch)
        {
            switch (ch)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return true;
            }
            return false;
        }

        protected bool IsAlphaNumber(char ch)
        {
            if (IsAlpha(ch) || IsNumber(ch))
            {
                return true;
            }
            if (ch == '$')
            {
                return true;
            }
            return (ch > '\x007f');
        }

        //https://msdn.microsoft.com/zh-cn/library/aa664664(v=vs.71).aspx
        protected bool IsNewLine(char ch)
        {
            var code = (int)ch;
            return code == 10 || code == 13 || code == 8232 || code == 8233;
        }

        protected bool TestLineBreak(int start, int end)
        {
            var offset = end - start;
            var i = 0;
            while (i < offset)
            {
                var ch = this.NextChar(start, i++);
                if (this.IsNewLine(ch))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
