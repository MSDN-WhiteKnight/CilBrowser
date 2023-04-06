/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using CilTools.Syntax.Tokens;

namespace CilBrowser.Core.SyntaxModel.Markup
{
    class XmlCommentToken : SyntaxTokenDefinition
    {
        public override bool HasContinuation(string prevPart, TokenReader reader)
        {
            return !prevPart.EndsWith("-->");
        }

        public override bool HasStart(TokenReader reader)
        {
            string str = reader.PeekString(4);

            if (str.Length < 4) return false;

            return str[0] == '<' && str[1] == '!' && str[2] == '-' && str[3] == '-';
        }
    }
}
