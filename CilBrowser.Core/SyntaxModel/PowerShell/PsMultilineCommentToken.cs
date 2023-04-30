/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using CilTools.Syntax.Tokens;

namespace CilBrowser.Core.SyntaxModel.PowerShell
{
    class PsMultilineCommentToken : SyntaxTokenDefinition
    {
        public override bool HasContinuation(string prevPart, TokenReader reader)
        {
            return !prevPart.EndsWith("#>", StringComparison.Ordinal);
        }

        public override bool HasStart(TokenReader reader)
        {
            string s = reader.PeekString(2);

            return Utils.StrEquals(s, "<#");
        }
    }
}
