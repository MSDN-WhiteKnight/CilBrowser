/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using CilTools.SourceCode.Common;
using CilTools.Syntax;
using CilTools.Syntax.Tokens;

namespace CilBrowser.Core.SyntaxModel.Java
{
    class JavaSyntaxProvider : SyntaxProvider
    {
        public override SyntaxNode[] GetNodes(string sourceText)
        {
            return SyntaxReader.ReadAllNodes(sourceText, SourceCodeUtils.GetTokenDefinitions(".java"), 
                JavaTokenFactory.Value);
        }
    }
}
