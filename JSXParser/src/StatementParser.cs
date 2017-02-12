using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    public partial class JSParser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="declaration">是否为declaration</param>
        /// <returns></returns>
        public IStatement ParseStatement(bool declaration)
        {
            this._startType= this._scaner.Token.Type;
            switch (this._startType)
            {
                case TokenType.KeywordBreak:
                case TokenType.KeywordContinue:
                    return this.ParseBreakContinueStatement(this._scaner.Token.Info.Keyword);
                case TokenType.KeywordDebugger:
                    return this.ParseDebuggerStatement();
                case TokenType.KeywordDo:
                    return this.ParseDoStatement();
                case TokenType.KeywordFor:
                    return this.ParseForStatement();
                case TokenType.KeywordFunction:
                    if (!declaration)
                    {
                        _scaner.Unexpected();
                    }
                    return this.ParseFunctionDeclaration();
                case TokenType.KeywordClass:
                    _scaner.Unexpected();
                    break;
                case TokenType.KeywordIf:
                    return this.ParseIfStatement();
                case TokenType.KeywordReturn:
                    return this.ParseReturnStatement();
                case TokenType.KeywordSwitch:
                    return this.ParseSwitchStatement();
                case TokenType.KeywordTry:
                    return this.ParseTryStatement();
                case TokenType.KeywordThrow:
                    return this.ParseThrowStatement();
                case TokenType.KeywordConst:
                case TokenType.KeywordVar:
                    var kind = _scaner.Token.Value;
                    if(!declaration && kind !="var")
                    {
                        _scaner.Unexpected();
                    }
                    return this.ParseVarDeclaration(kind);
                case TokenType.KeywordWhile:
                    return this.ParseWhileStatement();
                case TokenType.KeywordWith:
                    return this.ParseWithStatement();
                case TokenType.BraceLeft:
                    return this.ParseBlock();
                case TokenType.Semi:
                    return this.ParseEmptyStatement();
            }
            return this.ParseMayBeExpression();
        }

        protected IStatement ParseBreakContinueStatement(string keyword)
        {
            var startPos = _scaner.CurrentPosition();
            var isBreak = keyword == "break";
            _scaner.Next();
            Identifier label = null;
            if(_scaner.Eat(TokenType.Semi) || _scaner.InsertSemicolon())
            {
            }
            else if(_scaner.Token.Type == TokenType.Name)
            {
                label = this.ParseIdent(false);
                _scaner.Semicolon();
            }
            else
            {
                _scaner.Unexpected();
            }

            // Verify that there is an actual destination to break or
            // continue to.
            var valid = false;
            foreach(var lab in _labels)
            {
                if(label == null || label.Name == lab.Name)
                {
                    if (!string.IsNullOrEmpty(lab.Kind) && (isBreak || lab.Kind == "loop"))
                    {
                        valid = true;
                        break;
                    }
                    if(label != null && isBreak)
                    {
                        valid = true;
                        break;
                    }
                }
            }
            if (!valid)
            {
                _scaner.Raise(string.Format("Unsyntactic {0}", keyword));
            }
            var loc = new Location(startPos, _scaner.CurrentPosition());
            if (isBreak)
            {
                return new BreakStatement(label, loc);
            }
            return new ContinueStatement(label, loc);
        }

        protected IStatement ParseDebuggerStatement()
        {
            _scaner.Next();
            _scaner.Semicolon();
            return new DebuggerStatement(_scaner.Token.SourceLocation);
        }

        protected DoWhileStatement ParseDoStatement()
        {
            var startPos = _scaner.CurrentPosition();
            _scaner.Next();
            _labels.Push(LabelInfo.Loop);
            var body = this.ParseStatement(false);
            _labels.Pop();
            _scaner.Expect(TokenType.KeywordWhile);
            var test = this.ParseParenExpression();
            if(_scaner.EcmaVersion >= 6)
            {
                _scaner.Eat(TokenType.Semi);
            }
            else
            {
                _scaner.Semicolon();
            }
            return new DoWhileStatement(test, body, new Location(startPos, _scaner.CurrentPosition()));
        }

        // for, for/in, for/of
        protected IStatement ParseForStatement()
        {
            var startPos = _scaner.CurrentPosition();
            _scaner.Next();
            _labels.Push(LabelInfo.Loop);
            _scaner.Expect(TokenType.ParenLeft);
            if(_scaner.Token.Type == TokenType.Semi)
            {
                return this.ParseFor(null);
            }
            if(_scaner.Token.Type == TokenType.KeywordVar || _scaner.Token.Type == TokenType.KeywordConst)
            {
                var pos = _scaner.CurrentPosition();
                _scaner.Next();
                var init = new VariableDeclaration(this.ParseVar(true), new Location(pos, _scaner.CurrentPosition()));
                if(_scaner.Token.Type == TokenType.KeywordIn)
                {
                    return this.ParseForIn(init);
                }
                return this.ParseFor(init);
            }
            var init1 = this.ParseExpression(true);
            if(_scaner.Token.Type == TokenType.KeywordIn)
            {
                this.CheckLeftValue(init1, false);
                return this.ParseForIn(init1);
            }
            return this.ParseFor(init1);
        }

        protected ForStatement ParseFor(INode init)
        {
            var startPos = _scaner.CurrentPosition();
            _scaner.Expect(TokenType.Semi);
            var test = _scaner.Token.Type == TokenType.Semi ? null : this.ParseExpression(false);
            _scaner.Expect(TokenType.Semi);
            var update = _scaner.Token.Type == TokenType.Semi ? null : this.ParseExpression(false);
            _scaner.Expect(TokenType.ParenRight);
            var body = this.ParseStatement(false);
            return new ForStatement(init, test, update, body, new Location(startPos, _scaner.CurrentPosition()));
        }

        /// <summary>
        /// for/in for/of
        /// </summary>
        /// <returns></returns>
        protected ForInStatement ParseForIn(INode left)
        {
            var startPos = _scaner.CurrentPosition();
            _scaner.Expect(TokenType.KeywordIn);
            var right = this.ParseExpression(false);
            _scaner.Expect(TokenType.ParenRight);
            var body = this.ParseStatement(false);
            _labels.Pop();
            return new ForInStatement(left, right, body, new Location(startPos, _scaner.CurrentPosition()));
        }

        protected IfStatement ParseIfStatement()
        {
            var startPos = _scaner.CurrentPosition();
            _scaner.Next();
            var test = this.ParseParenExpression();
            var consequent = this.ParseStatement(false);
            IStatement alternate = null;
            if (_scaner.Eat(TokenType.KeywordElse))
            {
                alternate = this.ParseStatement(false);
            }
            return new IfStatement(test, consequent, alternate, new Location(startPos, _scaner.CurrentPosition()));
        }

        protected ReturnStatement ParseReturnStatement()
        {
            if (!this._inFunction)
            {
                _scaner.Raise("'return' outside of function");
            }
            var startPos = _scaner.CurrentPosition();
            _scaner.Next();
            IExpression argument = null;
            if (!_scaner.Eat(TokenType.Semi) && !_scaner.InsertSemicolon())
            {
                argument = this.ParseExpression(false);
                _scaner.Semicolon();
            }
            return new ReturnStatement(argument, new Location(startPos, _scaner.CurrentPosition()));
        }

        protected SwitchStatement ParseSwitchStatement()
        {
            var startPos = _scaner.CurrentPosition();
            _labels.Push(LabelInfo.Switch);
            _scaner.Next();
            var test = this.ParseParenExpression();
            _scaner.Expect(TokenType.BraceLeft);
            var cases = new List<SwitchCase>();
            while(_scaner.Token.Type == TokenType.KeywordCase ||
                _scaner.Token.Type == TokenType.KeywordDefault)
            {
                if(_scaner.Token.Type == TokenType.KeywordDefault)
                {
                    var switchCase1 = this.ParseSwitchCase(true);
                    cases.Add(switchCase1);
                    break;
                }
                var switchCase = this.ParseSwitchCase(false);
                cases.Add(switchCase);
            }
            _scaner.Expect(TokenType.BraceRight);
            _labels.Pop();
            return new SwitchStatement(test, cases.ToArray(), new Location(startPos, _scaner.CurrentPosition()));
        }

        protected SwitchCase ParseSwitchCase(bool isDefault)
        {
            var startPos = _scaner.CurrentPosition();
            _scaner.Next();
            IExpression test = null;
            if (!isDefault)
            {
                test = this.ParseExpression(false);
            }
            _scaner.Expect(TokenType.Colon);
            var statements = new List<IStatement>();
            while (_scaner.Token.Type != TokenType.KeywordCase && 
                _scaner.Token.Type != TokenType.KeywordDefault &&
                _scaner.Token.Type != TokenType.BraceRight)
            {
                var s = this.ParseStatement(false);
                statements.Add(s);
            }
            return new SwitchCase(test, statements.ToArray(), new Location(startPos, _scaner.CurrentPosition()));
        }

        protected TryStatement ParseTryStatement()
        {
            var startPos = _scaner.CurrentPosition();
            _scaner.Next();
            var block = this.ParseBlock();
            CatchClause handler = null;
            BlockStatement finalizer = null;
            if (_scaner.Eat(TokenType.KeywordCatch))
            {
                handler = this.ParseCatchClause();
            }
            if(_scaner.Eat(TokenType.KeywordFinally))
            {
                finalizer = this.ParseBlock();
            }
            if(handler == null &&  finalizer == null)
            {
                _scaner.Raise("Missing catch or finally clause");
            }
            return new TryStatement(block, handler, finalizer, new Location(startPos, _scaner.CurrentPosition()));
        }

        protected ThrowStatement ParseThrowStatement()
        {
            var startPos = _scaner.CurrentPosition();
            _scaner.Next();
            var argument = this.ParseExpression(false);
            return new ThrowStatement(argument, new Location(startPos, _scaner.CurrentPosition()));
        }

        protected CatchClause ParseCatchClause()
        {
            var startPos = _scaner.CurrentPosition();
            _scaner.Expect(TokenType.ParenLeft);
            var p = this.ParseBindingAtom();
            _scaner.Expect(TokenType.ParenRight);
            var body = this.ParseBlock();
            return new CatchClause(p, body, new Location(startPos, _scaner.CurrentPosition()));
        }

        protected WhileStatement ParseWhileStatement()
        {
            var startPos = _scaner.CurrentPosition();
            _scaner.Next();
            var test = this.ParseParenExpression();
            _labels.Push(LabelInfo.Loop);
            var body = this.ParseStatement(false);
            _labels.Pop();
            return new WhileStatement(test, body, new Location(startPos, _scaner.CurrentPosition()));
        }

        protected WithStatement ParseWithStatement()
        {
            var startPos = _scaner.CurrentPosition();
            _scaner.Next();
            var ob = this.ParseParenExpression();
            var body = this.ParseStatement(false);
            return new WithStatement(ob, body, new Location(startPos, _scaner.CurrentPosition()));
        }

        protected EmptyStatement ParseEmptyStatement()
        {
            var startPos = _scaner.CurrentPosition();
            _scaner.Next();
            return new EmptyStatement(new Location(startPos, _scaner.CurrentPosition()));
        }

        protected VariableDeclaration ParseVarDeclaration(string kind)
        {
            var startPos = _scaner.CurrentPosition();
            _scaner.Next();
            var declarations = this.ParseVar(false);
            _scaner.Semicolon();
            return new VariableDeclaration(declarations, new Location(startPos, _scaner.CurrentPosition()));            
        }

        protected VariableDeclarator[] ParseVar(bool isFor)
        {
            var vars = new List<VariableDeclarator>();
            do
            {
                var startPos = _scaner.CurrentPosition();
                var id = this.ParseVarId();
                IExpression init = null;
                if(_scaner.Eat(TokenType.Eq))
                {
                    init = this.ParseMaybeAssign(isFor);
                }
                vars.Add(new VariableDeclarator(id, init, new Location(startPos, _scaner.CurrentPosition())));
            } while (_scaner.Eat(TokenType.Comma));
            return vars.ToArray();
        }

        protected Identifier ParseVarId()
        {
            var id = this.ParseBindingAtom();
            this.CheckLeftValue(id, true);
            return id;
        }

        protected BlockStatement ParseBlock()
        {
            var startPos = _scaner.CurrentPosition();
            _scaner.Expect(TokenType.BraceLeft);
            var body = new List<IStatement>();
            while (!_scaner.Eat(TokenType.BraceRight))
            {
                body.Add(this.ParseStatement(true));
            }
            return new BlockStatement(body.ToArray(), new Location(startPos, _scaner.CurrentPosition()));
        }

        protected FunctionDeclaration ParseFunctionDeclaration()
        {
            _scaner.Next(); 
            return this.ParseFunctionStatement(false);
        }

        protected FunctionDeclaration ParseFunctionStatement(bool allowExpressionBody)
        {
            var startPos = _scaner.CurrentPosition();
            var id = this.ParseIdent(false);
            var p = this.ParseFunctionParams();
            var body = this.ParseFunctionBody(allowExpressionBody);
            return new FunctionDeclaration(id, p, body, new Location(startPos, _scaner.CurrentPosition()));
        }

        protected IPattern[] ParseFunctionParams()
        {
            _scaner.Expect(TokenType.ParenLeft);
            var arguments = this.ParseBindingList(TokenType.ParenRight, false, true);
            if (!this.IsSimpleParamList(arguments))
            {
                this.CheckParams(arguments);
            }
            return arguments;
        }

        protected IStatement ParseFunctionBody(bool allowExpressionBody)
        {
            var startPos = _scaner.CurrentPosition();
            var isExpression = _scaner.Token.Type != TokenType.BraceLeft;
            IStatement body;
            if (isExpression)
            {
                var expr = this.ParseMaybeAssign(false);
                body = new ExpressionStatement(expr, new Location(startPos, _scaner.CurrentPosition()));
            }
            else
            {
                var labels = _labels;
                var inFunction = _inFunction;
                _inFunction = true;
                _labels = new Stack<LabelInfo>();
                body = this.ParseBlock();
                _labels = labels;
                _inFunction = inFunction;
            }
            return body;
        }

        // Checks function params for various disallowed patterns such as using "eval"
        // or "arguments" and duplicate parameters.
        protected void CheckParams(IEnumerable<IPattern> arguments)
        {
            foreach (var a in arguments)
            {
                this.CheckLeftValue(a, true);
            }
        }

        protected bool IsSimpleParamList(IEnumerable<IPattern> arguments)
        {
            foreach (var a in arguments)
            {
                if (a.Type != NodeType.Identifier) return false;
            }
            return true;
        }

        protected IStatement ParseMayBeExpression()
        {
            var startPos = _scaner.CurrentPosition();
            var expr = this.ParseExpression(false);
            if (_startType == TokenType.Name && expr.Type == NodeType.Identifier 
                && _scaner.Eat(TokenType.Colon))
            {
                //return this.parseLabeledStatement(node, maybeName, expr);
            }
            _scaner.Semicolon();
            return new ExpressionStatement(expr, new Location(startPos, _scaner.CurrentPosition()));
        }

    }
}
