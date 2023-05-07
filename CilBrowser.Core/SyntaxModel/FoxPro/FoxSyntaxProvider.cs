/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using CilTools.SourceCode.Common;
using CilTools.Syntax;
using CilTools.Syntax.Tokens;

namespace CilBrowser.Core.SyntaxModel.FoxPro
{
    class FoxSyntaxProvider : SyntaxProvider
    {
        static readonly SyntaxTokenDefinition[] s_foxDefinitions = new SyntaxTokenDefinition[] {
            new CommonNameToken(), new FoxCommentToken(), new FoxTextLiteralToken(), new PunctuationToken(),
            new WhitespaceToken(),new NumericLiteralToken()
        };

        public override SyntaxNode[] GetNodes(string sourceText)
        {
            return SyntaxReader.ReadAllNodes(sourceText, s_foxDefinitions, FoxTokenFactory.Value);
        }
    }
}
