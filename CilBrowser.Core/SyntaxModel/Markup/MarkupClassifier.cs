/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using CilView.Core.Syntax;

namespace CilView.SourceCode
{
    public sealed class MarkupClassifier : TokenClassifier
    {
        private MarkupClassifier() { }

        public static readonly MarkupClassifier Value = new MarkupClassifier();

        public override TokenKind GetKind(string token)
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
            else return GetKindCommon(token);
        }
    }
}
