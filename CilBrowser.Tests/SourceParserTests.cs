/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CilBrowser.Core;
using CilBrowser.Core.SyntaxModel;
using CilTools.Metadata;
using CilTools.Syntax;
using CilTools.SourceCode.Common;

namespace CilBrowser.Tests
{
    [TestClass]
    public class SourceParserTests
    {
        [TestMethod]
        public void Test_JsArithmetic()
        {
            string src = "x=a/2\ny=x+1";
            SyntaxNode[] nodes = SourceParser.Parse(src, ".js");

            Assert.AreEqual(10, nodes.Length);
            TestUtils.VerifySourceToken(nodes[0], "x", TokenKind.Name, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[1], "=", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[2], "a", TokenKind.Name, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[3], "/", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[4], "2", TokenKind.NumericLiteral, leadingWhitespace: string.Empty,
                trailingWhitespace: "\n");
            TestUtils.VerifySourceToken(nodes[5], "y", TokenKind.Name, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[6], "=", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[7], "x", TokenKind.Name, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[8], "+", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[9], "1", TokenKind.NumericLiteral, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
        }

        [TestMethod]
        public void Test_JsRegexLiteral()
        {
            string src = "replace(/a/g, 'b')";
            SyntaxNode[] nodes = SourceParser.Parse(src, ".js");

            Assert.AreEqual(7, nodes.Length);
            TestUtils.VerifySourceToken(nodes[0], "replace", TokenKind.Name, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[1], "(", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[2], "/a/", TokenKind.Unknown, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[3], "g", TokenKind.Name, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[4], ",", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                trailingWhitespace: " ");
            TestUtils.VerifySourceToken(nodes[5], "'b'", TokenKind.SingleQuotLiteral, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[6], ")", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
        }
    }
}
