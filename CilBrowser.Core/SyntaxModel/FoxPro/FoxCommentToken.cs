/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using CilTools.Syntax.Tokens;

namespace CilBrowser.Core.SyntaxModel.FoxPro
{
    class FoxCommentToken : SyntaxTokenDefinition
    {
        public override bool HasContinuation(string prevPart, TokenReader reader)
        {
            return !prevPart.EndsWith("\n");
        }

        public override bool HasStart(TokenReader reader)
        {
            string str = reader.PeekString(2);

            if (Utils.StrEquals(str, "&&")) return true;

            if (!str.StartsWith("*")) return false;
            
            char prevChar = reader.GetPreviousChar(1);

            return prevChar == '\n' || prevChar == 0;
        }
    }
}

