/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using CilTools.Syntax;
using CilTools.SourceCode.Common;

namespace CilBrowser.Core.SyntaxModel.Markup
{
    /// <summary>
    /// Produces <see cref="SourceToken"/> instances for markup languages (XML, HTML)
    /// </summary>
    public sealed class MarkupTokenFactory : SyntaxFactory
    {
        private MarkupTokenFactory() { }

        public static readonly MarkupTokenFactory Value = new MarkupTokenFactory();

        static TokenKind GetKind(string token)
        {
            if (token.Length == 0) return TokenKind.Unknown;

            if (char.IsLetter(token[0]) || token[0] == '_')
            {
                return TokenKind.Name;
            }
            else if (token.StartsWith("<!--"))
            {
                return TokenKind.Comment;
            }
            else return SourceParser.GetKindCommon(token);
        }

        public override SyntaxNode CreateNode(string content, string leadingWhitespace, string trailingWhitespace)
        {
            TokenKind kind = GetKind(content);
            return new SourceToken(content, kind, leadingWhitespace, trailingWhitespace);
        }
    }
}
