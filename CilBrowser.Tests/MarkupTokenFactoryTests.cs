/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CilBrowser.Core.SyntaxModel;
using CilBrowser.Core.SyntaxModel.Markup;
using CilTools.Syntax;
using CilTools.SourceCode.Common;

namespace CilBrowser.Tests
{
    [TestClass]
    public class MarkupTokenFactoryTests
    {
        [DataRow("Project", TokenKind.Name)]
        [DataRow("<", TokenKind.Punctuation)]
        [DataRow("<!-- <Commented> -->", TokenKind.Comment)]
        [DataRow("<!-- \"Commented\" -->", TokenKind.Comment)]
        [DataTestMethod]
        public void Test_CreateNode(string token, TokenKind expected)
        {
            MarkupTokenFactory factory = MarkupTokenFactory.Value;
            SourceToken st = (SourceToken)factory.CreateNode(token, string.Empty, string.Empty);

            Assert.AreEqual(token, st.Content);
            Assert.AreEqual(expected, st.Kind);
            Assert.AreEqual(string.Empty, st.LeadingWhitespace);
            Assert.AreEqual(string.Empty, st.TrailingWhitespace);
        }

        [DataRow("int x = 0; Console.WriteLine(\"Test\"); //comment", ".cs")]
        [DataRow("<Project Sdk=\"Microsoft.Net.Sdk\"><Description>Hello, world!</Description></Project>", ".csproj")]
        [DataRow("<foo/><!--comment-->Text<bar></bar>", ".xml")]
        [DataTestMethod]
        public void Test_SourceParser_Roundtrip(string src, string ext)
        {
            SyntaxNode[] nodes = SourceParser.Parse(src, ext);
            StringBuilder sb = new StringBuilder(src.Length * 2);
            StringWriter wr = new StringWriter(sb);

            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].ToText(wr);
            }

            Assert.AreEqual(src, sb.ToString());
        }

        [TestMethod]
        public void Test_SourceParser_Xml()
        {
            string src = "<Application Name=\"Foo\"><!-- 'application definition' --></Application>";
            SyntaxNode[] nodes = SourceParser.ParseXmlTokens(src);

            Assert.AreEqual(11, nodes.Length);
            Assert.AreEqual("<", (nodes[0] as SourceToken).Content);
            Assert.AreEqual(TokenKind.Punctuation, (nodes[0] as SourceToken).Kind);
            Assert.AreEqual("Application", (nodes[1] as SourceToken).Content);
            Assert.AreEqual(" ", nodes[1].TrailingWhitespace);
            Assert.AreEqual(TokenKind.Name, (nodes[1] as SourceToken).Kind);
            Assert.AreEqual("Name", (nodes[2] as SourceToken).Content);
            Assert.AreEqual(TokenKind.Name, (nodes[2] as SourceToken).Kind);
            Assert.AreEqual("=", (nodes[3] as SourceToken).Content);
            Assert.AreEqual(TokenKind.Punctuation, (nodes[3] as SourceToken).Kind);
            Assert.AreEqual("\"Foo\"", (nodes[4] as SourceToken).Content);
            Assert.AreEqual(TokenKind.DoubleQuotLiteral, (nodes[4] as SourceToken).Kind);
            Assert.AreEqual(">", (nodes[5] as SourceToken).Content);
            Assert.AreEqual(TokenKind.Punctuation, (nodes[5] as SourceToken).Kind);
            Assert.AreEqual("<!-- 'application definition' -->", (nodes[6] as SourceToken).Content);
            Assert.AreEqual(TokenKind.Comment, (nodes[6] as SourceToken).Kind);
            Assert.AreEqual("<", (nodes[7] as SourceToken).Content);
            Assert.AreEqual(TokenKind.Punctuation, (nodes[7] as SourceToken).Kind);
            Assert.AreEqual("/", (nodes[8] as SourceToken).Content);
            Assert.AreEqual(TokenKind.Punctuation, (nodes[8] as SourceToken).Kind);
            Assert.AreEqual("Application", (nodes[9] as SourceToken).Content);
            Assert.AreEqual(TokenKind.Name, (nodes[9] as SourceToken).Kind);
            Assert.AreEqual(">", (nodes[10] as SourceToken).Content);
            Assert.AreEqual(TokenKind.Punctuation, (nodes[10] as SourceToken).Kind);
        }
    }
}
