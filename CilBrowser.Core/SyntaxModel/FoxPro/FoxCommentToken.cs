/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Reflection;
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

            MethodInfo mi = typeof(TokenReader).GetMethod(
                "GetPreviousChar", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            char prevChar = (char)mi.Invoke(reader, new object[] { 1 });

            return prevChar == '\n' || prevChar == 0;
        }
    }
}

