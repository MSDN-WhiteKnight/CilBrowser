/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using CilTools.Syntax;
using CilTools.SourceCode.Common;

namespace CilBrowser.Core.SyntaxModel.JavaScript
{
    /// <summary>
    /// Produces <see cref="SourceToken"/> instances for JavaScript
    /// </summary>
    public sealed class JsTokenFactory : SyntaxFactory
    {
        private JsTokenFactory() { }

        // https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Lexical_grammar#keywords
        static readonly HashSet<string> keywords = new HashSet<string>(new string[] {
            "break", "case", "catch", "class", "const", "continue", "debugger", "default", "delete", "do", "else",
            "export", "extends", "false", "finally", "for", "function", "if", "import", "in", "instanceof", "new", "null",
            "return", "super", "switch", "this", "throw", "true", "try", "typeof", "var", "void", "while", "with"
        });

        public static readonly JsTokenFactory Value = new JsTokenFactory();

        static TokenKind GetKind(string token)
        {
            if (token.Length == 0) return TokenKind.Unknown;

            if (char.IsLetter(token[0]) || token[0] == '_')
            {
                if (keywords.Contains(token)) return TokenKind.Keyword;
                else return TokenKind.Name;
            }
            else if (token[0] == '`')
            {
                return TokenKind.SpecialTextLiteral; //template literal
            }
            else
            {
                return SourceParser.GetKindCommon(token);
            }
        }

        public override SyntaxNode CreateNode(string content, string leadingWhitespace, string trailingWhitespace)
        {
            TokenKind kind = GetKind(content);
            return new SourceToken(content, kind, leadingWhitespace, trailingWhitespace);
        }
    }
}
