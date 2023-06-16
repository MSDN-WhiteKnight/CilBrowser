/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CilBrowser.Core.SyntaxModel.JavaScript;
using CilTools.SourceCode.Common;

namespace CilBrowser.Tests.SyntaxModel
{
    [TestClass]
    public class JsTokenFactoryTests
    {
        [DataRow("x", TokenKind.Name)]
        [DataRow("class", TokenKind.Keyword)]
        [DataRow("+", TokenKind.Punctuation)]
        [DataRow("//\"Commented\"", TokenKind.Comment)]
        [DataRow("/*'Commented'*/", TokenKind.MultilineComment)]
        [DataRow("'Hello world'", TokenKind.SingleQuotLiteral)]
        [DataRow("\"String\"", TokenKind.DoubleQuotLiteral)]
        [DataRow("123", TokenKind.NumericLiteral)]
        [DataRow("`Template literal`", TokenKind.SpecialTextLiteral)]
        [DataTestMethod]
        public void Test_CreateNode(string token, TokenKind expected)
        {
            JsTokenFactory factory = JsTokenFactory.Value;
            SourceToken st = (SourceToken)factory.CreateNode(token, string.Empty, string.Empty);

            Assert.AreEqual(token, st.Content);
            Assert.AreEqual(expected, st.Kind);
            Assert.AreEqual(string.Empty, st.LeadingWhitespace);
            Assert.AreEqual(string.Empty, st.TrailingWhitespace);
        }
    }
}
