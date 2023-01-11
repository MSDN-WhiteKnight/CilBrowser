/* CIL Browser 
 * Copyright (c) 2023, MSDN.WhiteKnight (https://github.com/MSDN-WhiteKnight) 
 * License: BSD 2.0 */
using System;
using System.Collections.Generic;
using System.Text;
using CilView.Core.Syntax;

namespace CilView.SourceCode
{
    public class MarkupClassifier : TokenClassifier
    {
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
