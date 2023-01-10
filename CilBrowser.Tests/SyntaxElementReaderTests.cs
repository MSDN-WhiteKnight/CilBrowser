/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CilBrowser.Core.SyntaxModel;
using CilView.Core.Syntax;
using CilView.SourceCode;

namespace CilBrowser.Tests
{
    [TestClass]
    public class SyntaxElementReaderTests
    {
        [TestMethod]
        public void Test_ParseElements_Tags()
        {
            //element definitions
            SyntaxElementDefinition[] defs = SyntaxElementDefinition.GetMarkupDefs();

            //source tokens
            SourceToken[] tokens = new SourceToken[]
            {
                new SourceToken("<", TokenKind.Punctuation),
                new SourceToken("b", TokenKind.Name),
                new SourceToken(">", TokenKind.Punctuation),
                new SourceToken("hello", TokenKind.Name),
                new SourceToken("<", TokenKind.Punctuation),
                new SourceToken("/", TokenKind.Punctuation),
                new SourceToken("b", TokenKind.Name),
                new SourceToken(">", TokenKind.Punctuation),
                new SourceToken(",", TokenKind.Punctuation),
                new SourceToken("<", TokenKind.Punctuation),
                new SourceToken("i", TokenKind.Name),
                new SourceToken(">", TokenKind.Punctuation),
                new SourceToken("world", TokenKind.Name),
                new SourceToken("<", TokenKind.Punctuation),
                new SourceToken("/", TokenKind.Punctuation),
                new SourceToken("i", TokenKind.Name),
                new SourceToken(">", TokenKind.Punctuation),
            };

            //parse elements
            SyntaxElement[] elems = SyntaxElementReader.ParseElements(tokens, defs);

            //verify elements
            Assert.AreEqual(7, elems.Length);
            Assert.AreEqual("<b>", elems[0].ToString());
            Assert.AreEqual(SyntaxKind.TagStart, elems[0].Kind);
            Assert.AreEqual("hello", elems[1].ToString());
            Assert.AreEqual(SyntaxKind.Unknown, elems[1].Kind);
            Assert.AreEqual("</b>", elems[2].ToString());
            Assert.AreEqual(SyntaxKind.TagEnd, elems[2].Kind);
            Assert.AreEqual(",", elems[3].ToString());
            Assert.AreEqual(SyntaxKind.Unknown, elems[3].Kind);
            Assert.AreEqual("<i>", elems[4].ToString());
            Assert.AreEqual(SyntaxKind.TagStart, elems[4].Kind);
            Assert.AreEqual("world", elems[5].ToString());
            Assert.AreEqual(SyntaxKind.Unknown, elems[5].Kind);
            Assert.AreEqual("</i>", elems[6].ToString());
            Assert.AreEqual(SyntaxKind.TagEnd, elems[6].Kind);
        }

        [TestMethod]
        public void Test_ParseElements_Negative()
        {
            //element definitions
            SyntaxElementDefinition[] defs = SyntaxElementDefinition.GetMarkupDefs();

            //source tokens
            SourceToken[] tokens = new SourceToken[]
            {
                new SourceToken("b", TokenKind.Punctuation),
                new SourceToken("=", TokenKind.Punctuation),
                new SourceToken("i", TokenKind.Name),
                new SourceToken("<", TokenKind.Punctuation),
                new SourceToken("10", TokenKind.NumericLiteral),
                new SourceToken(";", TokenKind.Punctuation),
                new SourceToken("c", TokenKind.Name),
                new SourceToken("=", TokenKind.Punctuation),
                new SourceToken("j", TokenKind.Name),
                new SourceToken(">", TokenKind.Punctuation),
                new SourceToken("5", TokenKind.NumericLiteral),
            };

            //parse elements
            SyntaxElement[] elems = SyntaxElementReader.ParseElements(tokens, defs);

            //verify elements
            Assert.AreEqual(1, elems.Length);
            Assert.AreEqual("b=i<10;c=j>5", elems[0].ToString());
            Assert.AreEqual(SyntaxKind.Unknown, elems[0].Kind);
        }
    }
}
