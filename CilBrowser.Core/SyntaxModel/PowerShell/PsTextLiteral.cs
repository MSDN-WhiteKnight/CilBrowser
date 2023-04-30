/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using CilTools.Syntax.Tokens;

namespace CilBrowser.Core.SyntaxModel.PowerShell
{
    public class PsTextLiteralToken : SyntaxTokenDefinition
    {
        public override bool HasStart(TokenReader reader)
        {
            char c = reader.PeekChar();
            return c == '"' || c == '\'';
        }

        public override bool HasContinuation(string prevPart, TokenReader reader)
        {
            if (prevPart.Length <= 1) return true;

            char c = prevPart[prevPart.Length - 1];

            if (c == '"' && prevPart[0] == '"')
            {
                return false;
            }
            else if (c == '\'' && prevPart[0] == '\'')
            {
                return false;
            }
            else return true;
        }
    }
}
