/* CIL Browser 
 * Copyright (c) 2023,  MSDN.WhiteKnight (https://github.com/MSDN-WhiteKnight) 
 * License: BSD 2.0 */
using System;
using System.Collections.Generic;
using System.Text;
using CilView.Core.Syntax;

namespace CilBrowser.Core.SyntaxModel.Markup
{
    class XmlCommentToken : SyntaxTokenDefinition
    {
        public override TokenKind Kind => TokenKind.Comment;

        public override bool HasContinuation(string prevPart, TokenReader reader)
        {
            return !prevPart.EndsWith("-->");
        }

        public override bool HasStart(TokenReader reader)
        {
            char[] arr = reader.PeekChars(4);

            if (arr.Length < 4) return false;

            return arr[0] == '<' && arr[1] == '!' && arr[2] == '-' && arr[3] == '-';
        }
    }
}
