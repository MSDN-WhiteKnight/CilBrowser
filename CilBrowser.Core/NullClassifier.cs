/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using CilTools.Syntax;
using CilTools.SourceCode.Common;

namespace CilBrowser.Core
{
    class NullClassifier : SyntaxFactory
    {
        internal static readonly NullClassifier Value = new NullClassifier();

        private NullClassifier() { }

        public override SyntaxNode CreateNode(string content, string leadingWhitespace, string trailingWhitespace)
        {
            return new SourceToken(content, TokenKind.Unknown, leadingWhitespace, trailingWhitespace);
        }
    }
}
