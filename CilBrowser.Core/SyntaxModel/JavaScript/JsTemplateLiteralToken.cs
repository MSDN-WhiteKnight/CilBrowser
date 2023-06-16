/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using CilTools.Syntax.Tokens;

namespace CilBrowser.Core.SyntaxModel.JavaScript
{
    class JsTemplateLiteralToken : SyntaxTokenDefinition
    {
        // https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Template_literals

        public override bool HasContinuation(string prevPart, TokenReader reader)
        {
            if (prevPart.Length <= 1) return true;

            char c = prevPart[prevPart.Length - 1];

            if (c == '`')
            {
                if (SourceParser.IsEscaped(prevPart, prevPart.Length - 1)) return true;
                else return false;
            }
            else return true;
        }

        public override bool HasStart(TokenReader reader)
        {
            char c = reader.PeekChar();
            return c == '`';
        }
    }
}
