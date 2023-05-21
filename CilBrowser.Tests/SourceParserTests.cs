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

        [TestMethod]
        public void Test_JsFunction()
        {
            string src = "function foo(event){return 0;}";
            SyntaxNode[] nodes = SourceParser.Parse(src, ".js");

            Assert.AreEqual(10, nodes.Length);
            TestUtils.VerifySourceToken(nodes[0], "function", TokenKind.Keyword, leadingWhitespace: string.Empty,
                trailingWhitespace: " ");
            TestUtils.VerifySourceToken(nodes[1], "foo", TokenKind.Name, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[2], "(", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[3], "event", TokenKind.Name, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[4], ")", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[5], "{", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[6], "return", TokenKind.Keyword, leadingWhitespace: string.Empty,
                trailingWhitespace: " ");
            TestUtils.VerifySourceToken(nodes[7], "0", TokenKind.NumericLiteral, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[8], ";", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
            TestUtils.VerifySourceToken(nodes[9], "}", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                trailingWhitespace: string.Empty);
        }

        [TestMethod]
        public void Test_Java_Class()
        {
            string src = "class MyWindowAdapter extends WindowAdapter implements ActionListener {";
            SyntaxNode[] nodes = SourceParser.Parse(src, ".java");
            
            TestUtils.AssertSourceTokens(new SourceToken[] { 
                new SourceToken("class", TokenKind.Keyword, leadingWhitespace: string.Empty, 
                    trailingWhitespace: " "),
                new SourceToken("MyWindowAdapter", TokenKind.Name, leadingWhitespace: string.Empty, 
                    trailingWhitespace: " "),
                new SourceToken("extends", TokenKind.Keyword, leadingWhitespace: string.Empty,
                    trailingWhitespace: " "),
                new SourceToken("WindowAdapter", TokenKind.Name, leadingWhitespace: string.Empty,
                    trailingWhitespace: " "),
                new SourceToken("implements", TokenKind.Keyword, leadingWhitespace: string.Empty,
                    trailingWhitespace: " "),
                new SourceToken("ActionListener", TokenKind.Name, leadingWhitespace: string.Empty,
                    trailingWhitespace: " "),
                new SourceToken("{", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                    trailingWhitespace: string.Empty),
            }, nodes);
        }

        [TestMethod]
        public void Test_Java_Arithmetic()
        {
            string src = "x*=(x-k)/(k+1.0);";
            SyntaxNode[] nodes = SourceParser.Parse(src, ".java");

            TestUtils.AssertSourceTokens(new SourceToken[] {
                new SourceToken("x", TokenKind.Name, leadingWhitespace: string.Empty,
                    trailingWhitespace: string.Empty),
                new SourceToken("*", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                    trailingWhitespace: string.Empty),
                new SourceToken("=", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                    trailingWhitespace: string.Empty),
                new SourceToken("(", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                    trailingWhitespace: string.Empty),
                new SourceToken("x", TokenKind.Name, leadingWhitespace: string.Empty,
                    trailingWhitespace: string.Empty),
                new SourceToken("-", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                    trailingWhitespace: string.Empty),
                new SourceToken("k", TokenKind.Name, leadingWhitespace: string.Empty,
                    trailingWhitespace: string.Empty),
                new SourceToken(")", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                    trailingWhitespace: string.Empty),
                new SourceToken("/", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                    trailingWhitespace: string.Empty),
                new SourceToken("(", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                    trailingWhitespace: string.Empty),
                new SourceToken("k", TokenKind.Name, leadingWhitespace: string.Empty,
                    trailingWhitespace: string.Empty),
                new SourceToken("+", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                    trailingWhitespace: string.Empty),
                new SourceToken("1.0", TokenKind.NumericLiteral, leadingWhitespace: string.Empty,
                    trailingWhitespace: string.Empty),
                new SourceToken(")", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                    trailingWhitespace: string.Empty),
                new SourceToken(";", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                    trailingWhitespace: string.Empty),
            }, nodes);
        }

        [TestMethod]
        public void Test_Java_StringLiteral()
        {
            string src = "fr = new Window(\"'Window title'\"); //create window";
            SyntaxNode[] nodes = SourceParser.Parse(src, ".java");

            TestUtils.AssertSourceTokens(new SourceToken[] {
                new SourceToken("fr", TokenKind.Name, leadingWhitespace: string.Empty,
                    trailingWhitespace: " "),
                new SourceToken("=", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                    trailingWhitespace: " "),
                new SourceToken("new", TokenKind.Keyword, leadingWhitespace: string.Empty,
                    trailingWhitespace: " "),
                 new SourceToken("Window", TokenKind.Name, leadingWhitespace: string.Empty,
                    trailingWhitespace: string.Empty),
                 new SourceToken("(", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                    trailingWhitespace: string.Empty),
                 new SourceToken("\"'Window title'\"", TokenKind.DoubleQuotLiteral, leadingWhitespace: string.Empty,
                    trailingWhitespace: string.Empty),
                 new SourceToken(")", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                    trailingWhitespace: string.Empty),
                 new SourceToken(";", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                    trailingWhitespace: " "),
                 new SourceToken("//create window", TokenKind.Comment, leadingWhitespace: string.Empty,
                    trailingWhitespace: string.Empty),
            }, nodes);
        }

        [TestMethod]
        public void Test_Xml_AttributeValue()
        {
            string src = "<Folder Include=\"Properties\\\" />";
            SyntaxNode[] nodes = SourceParser.Parse(src, ".xml");

            Assert.AreEqual(1, nodes.Length);
            Assert.IsTrue(nodes[0] is SyntaxElement);
            Assert.AreEqual(SyntaxKind.TagStart, ((SyntaxElement)nodes[0]).Kind);
            SyntaxNode[] tokens = nodes[0].GetChildNodes();
            
            TestUtils.AssertSourceTokens(new SourceToken[] {
                new SourceToken("<", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                    trailingWhitespace: string.Empty),
                new SourceToken("Folder", TokenKind.Name, leadingWhitespace: string.Empty,
                    trailingWhitespace: " "),
                new SourceToken("Include", TokenKind.Name, leadingWhitespace: string.Empty,
                    trailingWhitespace: string.Empty),
                new SourceToken("=", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                    trailingWhitespace: string.Empty),
                new SourceToken("\"Properties\\\"", TokenKind.DoubleQuotLiteral, leadingWhitespace: string.Empty,
                    trailingWhitespace: " "),
                new SourceToken("/", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                    trailingWhitespace: string.Empty),
                new SourceToken(">", TokenKind.Punctuation, leadingWhitespace: string.Empty,
                    trailingWhitespace: string.Empty),
            }, tokens);
        }
    }
}
