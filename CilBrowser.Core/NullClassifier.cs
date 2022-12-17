/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2022,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using CilView.Core.Syntax;
using CilView.SourceCode;

namespace CilBrowser.Core
{
    class NullClassifier : TokenClassifier
    {
        internal static readonly NullClassifier Value = new NullClassifier();

        private NullClassifier() { }

        public override TokenKind GetKind(string token)
        {
            return TokenKind.Unknown;
        }
    }
}
