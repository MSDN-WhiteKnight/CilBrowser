/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CilTools.SourceCode.Common;
using CilTools.Syntax;

namespace CilBrowser.Core.SyntaxModel
{
    public class SyntaxElement : SyntaxNode
    {
        List<SourceToken> tokens = new List<SourceToken>();

        public SyntaxKind Kind { get; set; }

        public void Add(SourceToken token)
        {
            this.tokens.Add(token);
        }

        public override IEnumerable<SyntaxNode> EnumerateChildNodes()
        {
            for (int i = 0; i < this.tokens.Count; i++)
            {
                yield return this.tokens[i];
            }
        }

        public override void ToText(TextWriter target)
        {
            for (int i = 0; i < this.tokens.Count; i++)
            {
                this.tokens[i].ToText(target);
            }

            target.Flush();
        }
    }
}
