/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CilBrowser.Core.SyntaxModel;
using CilBrowser.Core.SyntaxModel.FoxPro;
using CilTools.SourceCode.Common;
using CilTools.Syntax;

namespace CilBrowser.Tests
{
    [TestClass]
    public class FoxTokenFactoryTests
    {
        [DataRow("SELECT", TokenKind.Keyword)]
        [DataRow("From", TokenKind.Keyword)]
        [DataRow("x", TokenKind.Name)]
        [DataRow("+", TokenKind.Punctuation)]
        [DataRow("'String'", TokenKind.SingleQuotLiteral)]
        [DataRow("\"One more string\"", TokenKind.DoubleQuotLiteral)]
        [DataRow("* Hello from comment", TokenKind.Comment)]
        [DataRow("&&Hello from another comment", TokenKind.Comment)]
        [DataTestMethod]
        public void Test_CreateNode(string token, TokenKind expected)
        {
            FoxTokenFactory factory = FoxTokenFactory.Value;
            SourceToken st = (SourceToken)factory.CreateNode(token, string.Empty, string.Empty);

            Assert.AreEqual(token, st.Content);
            Assert.AreEqual(expected, st.Kind);
            Assert.AreEqual(string.Empty, st.LeadingWhitespace);
            Assert.AreEqual(string.Empty, st.TrailingWhitespace);
        }

        [DataRow("INSERT INTO Animals (Name,Class) VALUES (\"Cat\",\"Mammal\") && insert line")]
        [DataRow("s='hello'+[ world]\nx=y+1")]
        [DataRow("* Русский\n* Ελληνικά")]
        [DataTestMethod]
        public void Test_FoxPro_Roundtrip(string src)
        {
            SyntaxNode[] nodes = SourceParser.Parse(src, ".prg");
            Assert.AreEqual(src, TestUtils.SyntaxToString(nodes));
        }
        
        [TestMethod]
        public void Test_Parse_FoxPro()
        {
            string src = "SELECT Name FROM Users WHERE LEN(name)>0 && query";
            SyntaxNode[] nodes = SourceParser.Parse(src, ".prg");

            Assert.AreEqual(12, nodes.Length);
            TestUtils.VerifySourceToken(nodes[0], "SELECT", TokenKind.Keyword, leadingWhitespace: string.Empty,
                trailingWhitespace: " ");
            TestUtils.VerifySourceToken(nodes[1], "Name", TokenKind.Name, leadingWhitespace: string.Empty,
                trailingWhitespace: " ");
            TestUtils.VerifySourceToken(nodes[2], "FROM", TokenKind.Keyword, leadingWhitespace: string.Empty,
                trailingWhitespace: " ");
            TestUtils.VerifySourceToken(nodes[3], "Users", TokenKind.Name, leadingWhitespace: string.Empty,
                trailingWhitespace: " ");
            TestUtils.VerifySourceToken(nodes[4], "WHERE", TokenKind.Keyword, leadingWhitespace: string.Empty,
                trailingWhitespace: " ");
            TestUtils.VerifySourceToken(nodes[5], "LEN", TokenKind.Name, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[6], "(", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[7], "name", TokenKind.Name, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[8], ")", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[9], ">", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[10], "0", TokenKind.NumericLiteral, leadingWhitespace: string.Empty,
                trailingWhitespace: " ");
            TestUtils.VerifySourceToken(nodes[11], "&& query", TokenKind.Comment, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
        }

        [TestMethod]
        public void Test_Parse_FoxPro_StartingComment()
        {
            string src = "* Quick brown fox";
            SyntaxNode[] nodes = SourceParser.Parse(src, ".prg");

            Assert.AreEqual(1, nodes.Length);
            TestUtils.VerifySourceToken(nodes[0], "* Quick brown fox", TokenKind.Comment, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
        }

        [TestMethod]
        public void Test_Parse_FoxPro_LineComment()
        {
            string src = "path='C:\\dir'\n* set path";
            SyntaxNode[] nodes = SourceParser.Parse(src, ".prg");

            Assert.AreEqual(4, nodes.Length);
            TestUtils.VerifySourceToken(nodes[0], "path", TokenKind.Name, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[1], "=", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[2], "'C:\\dir'", TokenKind.SingleQuotLiteral, leadingWhitespace: string.Empty,
                trailingWhitespace: "\n");
            TestUtils.VerifySourceToken(nodes[3], "* set path", TokenKind.Comment, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
        }
    }
}
