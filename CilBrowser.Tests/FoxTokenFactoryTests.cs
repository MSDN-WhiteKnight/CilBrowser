/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CilBrowser.Core.SyntaxModel;
using CilBrowser.Core.SyntaxModel.FoxPro;
using CilTools.Syntax;
using CilTools.SourceCode.Common;

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
    }
}
