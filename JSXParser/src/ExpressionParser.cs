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
        protected TokenType _startType;
        protected bool _inGenerator;
        protected bool _strict;
        protected bool _preserveParens;
        
        /// <summary>
        /// parse expression
        /// </summary>
        /// <param name="noIn">true，则不解析in关键字。因为在for/in中，包含in的部分不是一个expression，而是两个部分。</param>
        /// <returns></returns>
        protected IExpression ParseExpression(bool noIn)
        {
            var startPos = this._scaner.CurrentPosition();
            var expr = this.ParseMaybeAssign(noIn);
            if (this._scaner.Token.Type == TokenType.Comma)
            {
                var list = new List<IExpression>();
                while (this._scaner.Eat(TokenType.Comma))
                {
                    list.Add(this.ParseMaybeAssign(noIn));
                }
                return new SequenceExpression(list.ToArray(), new Location(startPos, this._scaner.CurrentPosition()));
            }
            return expr;
        }

        protected IExpression ParseMaybeAssign(bool noIn)
        {
            //if (this._inGenerator && this._scaner.IsContextual("yield")) {
            //    return this.ParseYield();
            //}
            //var ownDestructuringErrors = false;

            //if (!refDestructuringErrors)
            //{
            //  refDestructuringErrors = new DestructuringErrors
            //  ownDestructuringErrors = true
            //}
            var startPos = this._scaner.CurrentPosition();
            //if(this._scaner.Token.Type == TokenType.ParenLeft || this._scaner.Token.Type == TokenType.Comma)
            //{
            //    this.potentialArrowAt = this.start;
            //}
            var left = this.ParseMaybeConditional(noIn);
            //if (afterLeftParse) left = afterLeftParse.call(this, left, startPos, startLoc)
            if (this._scaner.Token.Info.IsAssgin)
            {
                this.CheckLeftValue(left, false);
                var op = this._scaner.Token.Value;
                this._scaner.Next();
                var right = this.ParseMaybeAssign(noIn);
                return new AssignmentExpression(op, left, right, new Location(startPos, this._scaner.CurrentPosition()));
            }
            return left;
        }

        // Parse a ternary conditional (`?:`) operator.
        protected IExpression ParseMaybeConditional(bool noIn)
        {
            var startPos = this._scaner.CurrentPosition();
            var expr = this.ParseExprOps(noIn);
            //if (this.CheckExpressionErrors())
            //{
            //    return expr;
            //}
            if (this._scaner.Eat(TokenType.Question))
            {
                var consequent = this.ParseMaybeAssign(false);
                this._scaner.Expect(TokenType.Colon);
                var alternate = this.ParseMaybeAssign(noIn);
                return new ConditionalExpression(expr, consequent, alternate, new Location(startPos, this._scaner.CurrentPosition()));
            }
            return expr;
        }

        // Start the precedence parser.
        protected IExpression ParseExprOps(bool noIn)
        {
            var startPos = this._scaner.CurrentPosition();
            var expr = this.ParseMaybeUnary(false);
            return this.ParseExprOp(expr, startPos, -1, noIn);
        }
        // Parse binary operators with the operator precedence parsing
        // algorithm. `left` is the left-hand side of the operator.
        // `minPrec` provides context that allows the function to stop and
        // defer further parser to one of its callers when it encounters an
        // operator that has a lower precedence than the set it is parsing.
        protected IExpression ParseExprOp(IExpression left, Position leftStartPos, int minPrec, bool noIn)
        {
            var prec = this._scaner.Token.Info.Binop;
            if(prec != 0 && (!noIn || this._scaner.Token.Type != TokenType.KeywordIn))
            {
                if(prec > minPrec)
                {
                    var logical = this._scaner.Token.Type == TokenType.LogicalAND ||
                        this._scaner.Token.Type == TokenType.LogicalOR;
                    var op = this._scaner.Token.Value;
                    this._scaner.Next();
                    var startPos = this._scaner.CurrentPosition();
                    var right = this.ParseExprOp(this.ParseMaybeUnary(false), startPos, prec, noIn);
                    var node = this.BuildBinary(leftStartPos, left, right, op, logical);
                    return this.ParseExprOp(node, leftStartPos, minPrec, noIn);
                }
            }
            return left;
        }

        protected IExpression BuildBinary(Position startPos, IExpression left, IExpression right, string op, bool logical)
        {
            var location = new Location(startPos, this._scaner.CurrentPosition());
            if (logical)
            {
                return new LogicalExpression(op, left, right, location);
            }
            return new BinaryExpression(op, left, right, location);
        }

        // Parse unary operators, both prefix and postfix.
        protected IExpression ParseMaybeUnary(bool sawUnary)
        {
            var startPos = this._scaner.CurrentPosition();
            IExpression expr;
            var op = this._scaner.Token.Value;
            if (this._scaner.Token.Info.Prefix)
            {
                var update = this._scaner.Token.Type == TokenType.IncDec;
                _scaner.Next();
                var arguemnt = this.ParseMaybeUnary(true);
                if(update)
                {
                    this.CheckLeftValue(arguemnt, false);
                }
                else if (this._strict && op == "delete" && arguemnt.Type == NodeType.Identifier)
                {
                    this._scaner.Raise("Deleting local variable in strict mode");
                }
                else
                {
                    sawUnary = true;
                }
                var location = new Location(startPos, this._scaner.CurrentPosition());
                if (update)
                {
                    expr = new UpdateExpression(op, true, arguemnt, location);
                }
                else
                {
                    expr = new UnaryExpression(op, true, arguemnt, location);
                }
            }
            else
            {
                expr = this.ParseExprSubscripts();
                while(this._scaner.Token.Info.Postfix && !this._scaner.CanInsertSemicolon())
                {
                    var argument = expr;
                    this.CheckLeftValue(argument, false);
                    op = _scaner.Token.Value;
                    this._scaner.Next();
                    expr = new UpdateExpression(op, false, argument, new Location(startPos, this._scaner.CurrentPosition()));
                }
            }
            if(!sawUnary && this._scaner.Eat(TokenType.StarStar))
            {
                return this.BuildBinary(startPos, expr, this.ParseMaybeUnary(false), "**", false);
            }
            else
            {
                return expr;
            }
        }
        // Parse call, dot, and `[]`-subscript expressions.
        protected IExpression ParseExprSubscripts()
        {
            var startPos = this._scaner.CurrentPosition();
            var expr = this.ParseExprAtom();
            // todo: ArrowFunctionExpression
            return this.ParseSubscripts(expr, startPos, false);
        }

        protected IExpression ParseSubscripts(IExpression parent, Position startPos, bool noCalls)
        {
            var scaner = this._scaner;
            while (true)
            {
                if (scaner.Eat(TokenType.Dot))
                {
                    parent = new MemberExpression(
                        parent, this.ParseIdent(true), false, 
                        new Location(startPos, scaner.CurrentPosition())
                    );
                }
                else if (scaner.Eat(TokenType.BracketLeft))
                {
                    var proerty = this.ParseExpression(false);
                    scaner.Expect(TokenType.BracketRight);
                    parent = new MemberExpression(
                        parent, proerty, true,
                        new Location(startPos, scaner.CurrentPosition())
                    );
                }
                else if(!noCalls && scaner.Eat(TokenType.ParenLeft))
                {
                    var exprList = this.ParseExprList(TokenType.ParenRight, false);
                    parent = new CallExpression(
                        parent, exprList, 
                        new Location(startPos, scaner.CurrentPosition())
                    );
                }
                else
                {
                    break;
                }
            }
            return parent;
        }

        // Parses a comma-separated list of expressions, and returns them as
        // an array. `close` is the token type that ends the list, and
        // `allowEmpty` can be turned on to allow subsequent commas with
        // nothing in between them to be parsed as `null` (which is needed
        // for array literals).
        protected IExpression[] ParseExprList(TokenType close, bool allowEmpty)
        {
            var list = new List<IExpression>();
            var first = true;
            while (!this._scaner.Eat(close))
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    this._scaner.Expect(TokenType.Comma);
                }
                IExpression expr;
                if (allowEmpty && this._scaner.Token.Type == TokenType.Comma)
                {
                    expr = null;
                } else
                {
                    expr = this.ParseMaybeAssign(false);
                }
                if(expr != null)
                {
                    list.Add(expr);
                }
            }
            return list.ToArray();
        }
        // Parse an atomic expression — either a single token that is an
        // expression, an expression started by a keyword like `function` or
        // `new`, or an expression wrapped in punctuation like `()`, `[]`,
        // or `{}`.
        protected virtual IExpression ParseExprAtom()
        {
            var startPos = this._scaner.CurrentPosition();
            switch (this._scaner.Token.Type)
            {
                //case TokenType.KeywordSuper:
                case TokenType.KeywordThis:
                    var endPos = this._scaner.CurrentPosition();
                    this._scaner.Next();
                    return new ThisExpression(new Location(startPos, endPos));

                case TokenType.Name:
                    var id = this.ParseIdent(false);
                    return id;
                case TokenType.Regexp:
                case TokenType.Num:
                case TokenType.String:
                    return this.ParseLiteral(this._scaner.Token.Value);

                case TokenType.KeywordNull:
                case TokenType.KeywordTrue:
                case TokenType.KeywordFalse:
                    return this.ParseLiteral(this._scaner.Token.Value);

                case TokenType.ParenLeft:
                    return this.ParseParenAndDistinguishExpression();

                case TokenType.BracketLeft:
                    this._scaner.Next();
                    var elements = this.ParseExprList(TokenType.BracketRight, true);
                    return new ArrayExpression(
                        elements,
                        new Location(startPos, this._scaner.CurrentPosition())
                    );
                case TokenType.BraceLeft:
                    return this.ParseObj();

                case TokenType.KeywordFunction:
                    this._scaner.Next();
                    return this.ParseFunctionExpression(false);
                //case TokenType.KeywordClass:
                //    return this.parseClass(this.startNode(), false)

                case TokenType.KeywordNew:
                    return this.ParseNew();

                //case TokenType.BackQuote:
                //    return this.parseTemplate()

                default:
                    this._scaner.Unexpected();
                    break;
            }
            return null;
        }

        protected NewExpression ParseNew()
        {
            var startPos = _scaner.CurrentPosition();
            _scaner.Next();
            var callee = this.ParseSubscripts(this.ParseExprAtom(), startPos, true);
            var arguments = new IExpression[0];
            if (_scaner.Eat(TokenType.ParenLeft))
            {
                arguments = this.ParseExprList(TokenType.ParenRight, false);
            }
            return new NewExpression(callee, arguments, new Location(startPos, _scaner.CurrentPosition()));
        }

        // Parse an object literal or binding pattern.
        protected ObjectExpression ParseObj()
        {
            var properties = new List<Property>();
            var first = true;
            var startPos = this._scaner.CurrentPosition();
            this._scaner.Next();
            while (!this._scaner.Eat(TokenType.BraceRight))
            {
                if (!first)
                {
                    this._scaner.Expect(TokenType.Comma);
                    if (this._scaner.AfterTrailingComma(TokenType.BraceRight, false)) break;
                }
                else {
                    first = false;
                }
                var pos = this._scaner.CurrentPosition();
                var prop = new Property(new Location(pos, pos));
                this.ParsePropertyName(prop);
                this.ParsePropertyValue(prop);
                //this.checkPropClash(prop, propHash)
                properties.Add(prop);
            }
            return new ObjectExpression(properties.ToArray(), new Location(startPos, this._scaner.CurrentPosition()));
        }
        protected void ParsePropertyValue(Property prop)
        {
            if (this._scaner.Eat(TokenType.Colon))
            {
                prop.Value = this.ParseMaybeAssign(false);
                prop.Kind = PropertyKind.Init;
            } else
            {
                this._scaner.Unexpected();
            }
        }
        protected void ParsePropertyName(Property prop)
        {
            if(this._scaner.Token.Type == TokenType.Num ||
                this._scaner.Token.Type == TokenType.String)
            {
                prop.Key = this.ParseExprAtom();
            }
            else
            {
                prop.Key = this.ParseIdent(true);
            }
        }

        // Parse the next token as an identifier. If `liberal` is true (used
        // when parsing properties), it will also convert keywords into
        // identifiers.
        protected Identifier ParseIdent(bool liberal)
        {
            string value = string.Empty;
            var startPos = this._scaner.CurrentPosition();
            if(this._scaner.Token.Type == TokenType.Name)
            {
                value = this._scaner.Token.Value;
                
            } else if(liberal && !string.IsNullOrEmpty(this._scaner.Token.Info.Keyword))
            {
                value = this._scaner.Token.Info.Keyword;
            }
            else
            {
                this._scaner.Unexpected();
            }
            var endPos = this._scaner.CurrentPosition();
            this._scaner.Next();
            return new Identifier(value, new Location(startPos, endPos));
        }
        protected Literal ParseLiteral(string value)
        {
            var pos = this._scaner.CurrentPosition();
            var literal = new Literal(value, new string(_scaner.Text, _scaner.Token.Start, _scaner.Token.End - _scaner.Token.Start), new Location(pos, pos));
            this._scaner.Next();
            return literal;
        }
        protected IExpression ParseParenAndDistinguishExpression()
        {
            var startPos = this._scaner.CurrentPosition();
            var expr = this.ParseParenExpression();
            if (this._preserveParens)
            {
                return new ParenthesizedExpression(expr, new Location(startPos, this._scaner.CurrentPosition()));
            }
            else
            {
                return expr;
            }
        }
        protected IExpression ParseParenExpression()
        {
            this._scaner.Expect(TokenType.ParenLeft);
            var val = this.ParseExpression(false);
            this._scaner.Expect(TokenType.ParenRight);
            return val;
        }
        protected FunctionExpression ParseFunctionExpression(bool allowExpressionBody)
        {
            var startPos = _scaner.CurrentPosition();
            Identifier id = null;
            if (_scaner.Token.Type == TokenType.Name)
            {
                id = this.ParseIdent(false);
            }
            var p = this.ParseFunctionParams();
            var body = this.ParseFunctionBody(allowExpressionBody);
            return new FunctionExpression(id, p, body, new Location(startPos, _scaner.CurrentPosition()));
        }

        #region left value
        // Verify that a node is an lval — something that can be assigned to.
        protected void CheckLeftValue(INode expr, bool isBinding)
        {
            switch (expr.Type)
            {
                case NodeType.Identifier:
                    // todo: check reserved words
                    break;
                case NodeType.MemberExpression:
                    if (isBinding)
                    {
                        this._scaner.Raise(string.Format("{0} member expression", isBinding ? "Binding" : "Assigning to"));
                    }
                    break;
                //case "ObjectPattern":
                //case "ArrayPattern":
                //case "AssignmentPattern":
                //case "RestElement":
                //case "ParenthesizedExpression":
                
                default:
                    this._scaner.Raise(string.Format("{0}  rvalue", isBinding ? "Binding" : "Assigning to"));
                    break;
            }
        }

        /// <summary>
        /// Parses lvalue (assignable) atom.
        /// </summary>
        /// <returns></returns>
        protected Identifier ParseBindingAtom()
        {
            return this.ParseIdent(false);
        }

        protected IPattern[] ParseBindingList(TokenType close, bool allowEmpty, bool allowNonIdent)
        {
            var patterns = new List<IPattern>();
            var first = true;
            while (!_scaner.Eat(close))
            {
                if (first) first = false;
                else _scaner.Expect(TokenType.Comma);
                if (allowEmpty && _scaner.Token.Type == TokenType.Comma)
                {
                    patterns.Add(null);
                }
                else
                {
                    var elem = this.ParseBindingAtom();
                    patterns.Add(elem);
                }
            }
            return patterns.ToArray();
        }
        #endregion
    }
}
