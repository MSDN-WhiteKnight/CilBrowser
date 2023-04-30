/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CilTools.SourceCode.Common;
using CilTools.Syntax;

namespace CilBrowser.Tests
{
    static class TestUtils
    {
        /// <summary>
        /// Gets the string that contains joined text of all syntax nodes in the specified collection
        /// </summary>
        public static string SyntaxToString(IEnumerable<SyntaxNode> nodes)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter wr = new StringWriter(sb);

            foreach (SyntaxNode node in nodes) node.ToText(wr);

            return sb.ToString();
        }

        /// <summary>
        /// Verifies that the specified <see cref="SyntaxNode"/> is a <see cref="SourceToken"/> with specified 
        /// properties. Throws exception if it's false.
        /// </summary>
        public static void VerifySourceToken(SyntaxNode node, string content, TokenKind kind, string leadingWhitespace,
            string trailingWhitespace)
        {
            //expected to be called only on SourceToken, but parameter is typed SyntaxNode to reduce casts on 
            //callsites
            SourceToken tok = (SourceToken)node;
            Assert.AreEqual(content, tok.Content);
            Assert.AreEqual(kind, tok.Kind);
            Assert.AreEqual(leadingWhitespace, node.LeadingWhitespace);
            Assert.AreEqual(trailingWhitespace, node.TrailingWhitespace);
        }
    }
}
