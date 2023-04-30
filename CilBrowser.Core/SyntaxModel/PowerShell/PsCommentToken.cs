/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using CilTools.Syntax.Tokens;

namespace CilBrowser.Core.SyntaxModel.PowerShell
{
    class PsCommentToken : SyntaxTokenDefinition
    {
        public override bool HasContinuation(string prevPart, TokenReader reader)
        {
            return !prevPart.EndsWith("\n");
        }

        public override bool HasStart(TokenReader reader)
        {
            char c = reader.PeekChar();

            return c == '#';
        }
    }
}
