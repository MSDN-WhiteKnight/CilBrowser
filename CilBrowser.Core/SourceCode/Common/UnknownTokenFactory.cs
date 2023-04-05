/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using CilTools.Syntax;

namespace CilTools.SourceCode.Common
{
    /// <summary>
    /// Produces <see cref="SourceToken"/> instances classified as <see cref="TokenKind.Unknown"/> for all inputs 
    /// (useful to disable syntax highlighting).
    /// </summary>
    class UnknownTokenFactory : SyntaxFactory
    {
        internal static readonly UnknownTokenFactory Value = new UnknownTokenFactory();

        private UnknownTokenFactory() { }

        public override SyntaxNode CreateNode(string content, string leadingWhitespace, string trailingWhitespace)
        {
            return new SourceToken(content, TokenKind.Unknown, leadingWhitespace, trailingWhitespace);
        }
    }
}
