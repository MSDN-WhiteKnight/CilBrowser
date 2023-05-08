/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using CilTools.Syntax.Tokens;

namespace CilBrowser.Core.SyntaxModel.JavaScript
{
    class JsRegexLiteralToken : SyntaxTokenDefinition
    {
        static bool IsEscaped(string str, int i)
        {
            if (i <= 0) return false;

            char c1 = str[i - 1];

            if (c1 != '\\') return false;

            //check if the slash itself is not escaped
            return !IsEscaped(str, i - 1);
        }

        public override bool HasContinuation(string prevPart, TokenReader reader)
        {
            if (prevPart.Length <= 1) return true;

            char c = prevPart[prevPart.Length - 1];

            if (c == '/')
            {
                if (IsEscaped(prevPart, prevPart.Length - 1)) return true;
                else return false;
            }
            else return true;
        }

        public override bool HasStart(TokenReader reader)
        {
            char c = reader.PeekChar();

            if (c != '/') return false; //JS regex literal starts with slash

            string s = reader.PeekString(2);

            if (s.Length < 2) return false; //too short for regex literal

            if (s[1] == '/' || s[1] == '*') return false; //comments

            // Now the interesting part begins... Fully detecting division vs. regex literals is too complex, but if 
            // there's a newline after the starting slash and before the following slash, likely it is NOT a regex literal.

            string str = reader.GetSourceString();

            for (int i = reader.Position + 1; i < str.Length; i++)
            {
                if (i - reader.Position > 2000)
                {
                    // Line is too long - exit early to prevent slowing down on minified JS files that consist of a
                    // single huge line.
                    return false;
                }

                if (str[i] == '\r' || str[i] == '\n') return false;
                else if(str[i] == '/') return true;
            }

            return false;
        }
    }
}
