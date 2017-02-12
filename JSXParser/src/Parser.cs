using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    public class Parser : JSParser
    {
        public Parser(string text, int ecmaVersion)
        {
            _scaner = new Scaner(text, ecmaVersion);
            _preserveParens = true;
            _labels = new Stack<LabelInfo>();
        }

        public Parser(string text) : this(text, 5)
        {
        }

        protected override IExpression ParseExprAtom()
        {
            var startPos = this._scaner.CurrentPosition();
            if (_scaner.Token.Type == TokenType.JSXText)
            {
                return this.ParseLiteral(_scaner.Token.Value);
            }
            else if (_scaner.Token.Type == TokenType.JSXTagStart)
            {
                return this.ParseJSXElement();
            }
            return base.ParseExprAtom();
        }

        private IExpression ParseJSXElement()
        {
            var startPos = _scaner.CurrentPosition();
            _scaner.Next();
            return this.ParseJSXElementAt(startPos);
        }

        // Parse next token as JSX identifier
        private JSXIdentifier ParseJSXIdentifier()
        {
            var startPos = _scaner.CurrentPosition();
            var name = string.Empty;
            if (_scaner.Token.Type == TokenType.JSXName)
            {
                name = _scaner.Token.Value;
            }
            else if (string.IsNullOrEmpty(_scaner.Token.Info.Keyword))
            {
                name = _scaner.Token.Info.Keyword;
            }
            else
            {
                _scaner.Unexpected();
            }
            _scaner.Next();
            return new JSXIdentifier(name, new Location(startPos, _scaner.CurrentPosition()));
        }

        // Parse namespaced identifier.
        private IExpression ParseJSXNamespacedName()
        {
            var startPos = _scaner.CurrentPosition();
            var name = this.ParseJSXIdentifier();
            if (!_scaner.Eat(TokenType.Colon))
            {
                return name;
            }
            var name1 = this.ParseJSXIdentifier();
            return new JSXNamespacedName(name, name1, new Location(startPos, _scaner.CurrentPosition()));
        }

        // Parses element name in any form - namespaced, member
        // or single identifier.
        private IExpression ParseJSXElementName()
        {
            var startPos = _scaner.CurrentPosition();
            var node = this.ParseJSXNamespacedName();
            while (_scaner.Eat(TokenType.Dot))
            {
                var property = this.ParseJSXIdentifier();
                node = new JSXMemberExpression(node, property, new Location(startPos, _scaner.CurrentPosition()));
            }
            return node;
        }

        // Parses any type of JSX attribute value.
        private INode ParseJSXAttributeValue()
        {
            switch (_scaner.Token.Type)
            {
                case TokenType.BraceLeft:
                    var node = this.ParseJSXExpressionContainer();
                    if (node.Expression.Type == NodeType.JSXEmptyExpression)
                    {
                        _scaner.Raise("JSX attributes must only be assigned a non-empty expression");
                    }
                    return node;
                case TokenType.JSXTagStart:
                case TokenType.String:
                    return this.ParseExprAtom();
                default:
                    _scaner.Raise("JSX value should be either an expression or a quoted JSX text");
                    break;
            }
            return null;
        }

        // JSXEmptyExpression is unique type since it doesn't actually parse anything,
        // and so it should start at the end of last read token (left brace) and finish
        // at the beginning of the next one (right brace).
        private JSXEmptyExpression ParseJSXEmptyExpression()
        {
            var startPos = _scaner.CurrentPosition();
            return new JSXEmptyExpression(new Location(startPos, startPos));
        }

        // Parses JSX expression enclosed into curly brackets.
        private JSXExpressionContainer ParseJSXExpressionContainer()
        {
            var startPos = _scaner.CurrentPosition();
            _scaner.Next();
            INode expr;
            if (_scaner.Token.Type == TokenType.BraceRight)
            {
                expr = this.ParseJSXEmptyExpression();
            }
            else
            {
                expr = this.ParseExpression(false);
            }
            _scaner.Expect(TokenType.BraceRight);
            return new JSXExpressionContainer(expr, new Location(startPos, _scaner.CurrentPosition()));
        }

        // Parses following JSX attribute name-value pair.
        private JSXAttribute ParseJSXAttribute()
        {
            var startPos = _scaner.CurrentPosition();
            var name = this.ParseJSXNamespacedName();
            var value = _scaner.Eat(TokenType.Eq) ? this.ParseJSXAttributeValue() : null;
            return new JSXAttribute(name, value, new Location(startPos, _scaner.CurrentPosition()));
        }

        // Parses JSX opening tag starting after '<'.
        private JSXOpeningElement ParseJSXOpeningElementAt(Position startPos)
        {
            var name = this.ParseJSXElementName();
            var attributes = new List<JSXAttribute>();
            while (_scaner.Token.Type != TokenType.Slash && _scaner.Token.Type != TokenType.JSXTagEnd)
            {
                attributes.Add(this.ParseJSXAttribute());
            }
            var selfClosing = _scaner.Eat(TokenType.Slash);
            _scaner.Expect(TokenType.JSXTagEnd);
            return new JSXOpeningElement(attributes.ToArray(), selfClosing, name, new Location(startPos, _scaner.CurrentPosition()));
        }

        // Parses JSX closing tag starting after '</'.
        private JSXClosingElement ParseJSXClosingElementAt(Position startPos)
        {
            var name = this.ParseJSXElementName();
            _scaner.Expect(TokenType.JSXTagEnd);
            return new JSXClosingElement(name, new Location(startPos, _scaner.CurrentPosition()));
        }
        // Parses entire JSX element, including it's opening tag
        // (starting after '<'), attributes, contents and closing tag.
        private JSXElement ParseJSXElementAt(Position startPos)
        {
            var opening = this.ParseJSXOpeningElementAt(startPos);
            JSXClosingElement closing = null;
            var children = new List<INode>();
            if (!opening.SelfClosing)
            {
                while (true)
                {
                    switch (_scaner.Token.Type)
                    {
                        case TokenType.JSXTagStart:
                            startPos = _scaner.Token.SourceLocation.Start;
                            _scaner.Next();
                            if (_scaner.Eat(TokenType.Slash))
                            {
                                closing = this.ParseJSXClosingElementAt(startPos);
                                goto OutsideWhile;
                            }
                            children.Add(this.ParseJSXElementAt(startPos));
                            break;
                        case TokenType.JSXText:
                            children.Add(this.ParseExprAtom());
                            break;

                        case TokenType.BraceLeft:
                            children.Add(this.ParseJSXExpressionContainer());
                            break;

                        default:
                            _scaner.Unexpected();
                            break;
                    }
                }
                OutsideWhile:
                if (this.GetQualifiedJSXName(closing.Name) != this.GetQualifiedJSXName(opening.Name))
                {
                    _scaner.Raise(
                        string.Format(
                            "Expected corresponding JSX closing tag for <{0}>",
                            this.GetQualifiedJSXName(opening.Name)
                        )
                    );
                }
            }
            if (_scaner.Token.Type == TokenType.Relational && _scaner.Token.Value == "<")
            {
                _scaner.Raise("Adjacent JSX elements must be wrapped in an enclosing tag");
            }
            return new JSXElement(opening, children.ToArray(), closing, new Location(startPos, _scaner.CurrentPosition()));
        }

        private string GetQualifiedJSXName(INode node)
        {
            if (node is JSXIdentifier)
            {
                return ((JSXIdentifier)node).Name;
            }
            if (node is JSXNamespacedName)
            {
                var jsxNamespace = (JSXNamespacedName)node;
                return string.Format("{0}:{1}", jsxNamespace.Namespace, jsxNamespace.Name);
            }
            if (node is JSXMemberExpression)
            {
                var member = (JSXMemberExpression)node;
                return string.Format(
                    "{0}.{1}",
                    this.GetQualifiedJSXName(member.Object),
                    this.GetQualifiedJSXName(member.Property)
                );
            }
            return string.Empty;
        }
    }
}
