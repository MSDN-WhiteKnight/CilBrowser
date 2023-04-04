/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using CilTools.Syntax.Tokens;

namespace CilTools.SourceCode.Common
{
    /// <summary>
    /// Defines a name (identifier or keyword) token common to multiple C-like programming languages
    /// </summary>
    internal class CommonNameToken : SyntaxTokenDefinition
    {
        // From: https://github.com/MSDN-WhiteKnight/CilTools/blob/master/CilTools.SourceCode/Common/CommonNameToken.cs

        public override bool HasStart(TokenReader reader)
        {
            char c = reader.PeekChar();

            return char.IsLetter(c) || c == '_';
        }

        public override bool HasContinuation(string prevPart, TokenReader reader)
        {
            char c = reader.PeekChar();
            return char.IsLetterOrDigit(c) || c == '_';
        }
    }
}
