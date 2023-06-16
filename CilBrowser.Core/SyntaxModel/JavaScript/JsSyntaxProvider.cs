/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using CilTools.SourceCode.Common;
using CilTools.Syntax;
using CilTools.Syntax.Tokens;

namespace CilBrowser.Core.SyntaxModel.JavaScript
{
    class JsSyntaxProvider : SyntaxProvider
    {
        static readonly SyntaxTokenDefinition[] s_jsDefinitions = new SyntaxTokenDefinition[] {
            new CommonNameToken(), new JsRegexLiteralToken(), new JsTemplateLiteralToken(), new PunctuationToken(), new WhitespaceToken(),
            new NumericLiteralToken(), new DoubleQuotLiteralToken(), new SingleQuotLiteralToken(), new CommentToken(),
            new MultilineCommentToken()
        };

        public override SyntaxNode[] GetNodes(string sourceText)
        {
            return SyntaxReader.ReadAllNodes(sourceText, s_jsDefinitions, JsTokenFactory.Value);
        }
    }
}
