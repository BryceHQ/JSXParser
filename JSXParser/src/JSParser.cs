using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /// <summary>
    /// 不支持 es6
    /// </summary>
    public partial class JSParser
    {
        protected JSScaner _scaner;
        protected Stack<LabelInfo> _labels;
        protected bool _inFunction;

        protected JSParser()
        {
        }

        public JSParser(string text, int ecmaVersion)
        {
            _scaner = new JSScaner(text, ecmaVersion);
            _preserveParens = true;
            _labels = new Stack<LabelInfo>();
        }

        public JSParser(string text) : this(text, 5)
        {
        }

        public virtual void Reset(string text)
        {
            _scaner.Reset(text);
        }

        public Program Parse()
        {
            var startPos = this._scaner.CurrentPosition();
            this._scaner.Next();
            //var first = true;
            var body = new List<IStatement>();
            while(this._scaner.Token.Type != TokenType.EOF)
            {
                body.Add(this.ParseStatement(true));
            }
            return new Program(body.ToArray(), new Location(startPos, this._scaner.CurrentPosition()));
        }
    }
}
