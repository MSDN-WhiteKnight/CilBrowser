/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CilBrowser.Core.SyntaxModel;
using CilBrowser.Core.SyntaxModel.PowerShell;
using CilTools.Syntax;
using CilTools.SourceCode.Common;

namespace CilBrowser.Tests.SyntaxModel
{
    [TestClass]
    public class PsTokenFactoryTests
    {
        [DataRow("Rabbit31", TokenKind.Name)]
        [DataRow("for", TokenKind.Keyword)]
        [DataRow("+", TokenKind.Punctuation)]
        [DataRow("# $Commented", TokenKind.Comment)]
        [DataRow("<# 'Commented'#>", TokenKind.MultilineComment)]
        [DataRow("'Hello world'", TokenKind.SingleQuotLiteral)]
        [DataRow("\"Another string literal\"", TokenKind.DoubleQuotLiteral)]
        [DataTestMethod]
        public void Test_CreateNode(string token, TokenKind expected)
        {
            PsTokenFactory factory = PsTokenFactory.Value;
            SourceToken st = (SourceToken)factory.CreateNode(token, string.Empty, string.Empty);

            Assert.AreEqual(token, st.Content);
            Assert.AreEqual(expected, st.Kind);
            Assert.AreEqual(string.Empty, st.LeadingWhitespace);
            Assert.AreEqual(string.Empty, st.TrailingWhitespace);
        }

        [DataRow("New-Item -Path \".\" -Name 'obj' -ItemType 'directory' -ErrorAction:SilentlyContinue")]
        [DataRow("$filePath = [System.IO.Path]::GetFullPath('..\\foo\\bin.zip')")]
        [DataRow("foreach ($file in $fileList)")]
        [DataRow("<# Quick brown fox #>")]
        [DataTestMethod]
        public void Test_Parse_Roundtrip(string src)
        {
            SyntaxNode[] nodes = SourceParser.Parse(src, ".ps1");
            Assert.AreEqual(src, TestUtils.SyntaxToString(nodes));
        }

        [TestMethod]
        public void Test_SourceParser_PowerShell()
        {
            string src = "$animal = 'whale' # set variable";
            SyntaxNode[] nodes = SourceParser.Parse(src, ".ps1");

            Assert.AreEqual(5, nodes.Length);
            TestUtils.VerifySourceToken(nodes[0], "$", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[1], "animal", TokenKind.Name, leadingWhitespace: string.Empty,
                trailingWhitespace: " ");
            TestUtils.VerifySourceToken(nodes[2], "=", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                trailingWhitespace: " ");
            TestUtils.VerifySourceToken(nodes[3], "'whale'", TokenKind.SingleQuotLiteral, leadingWhitespace: string.Empty,
                trailingWhitespace: " ");
            TestUtils.VerifySourceToken(nodes[4], "# set variable", TokenKind.Comment, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
        }
    }
}
