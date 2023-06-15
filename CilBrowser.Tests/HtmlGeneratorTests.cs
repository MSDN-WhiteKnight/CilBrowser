/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CilBrowser.Core;
using CilTools.Metadata;
using CilTools.Syntax;
using CilTools.SourceCode.Common;

namespace CilBrowser.Tests
{
    [TestClass]
    public class HtmlGeneratorTests
    {
        static string Visualize(IEnumerable<SyntaxNode> nodes)
        {
            StringBuilder sb = new StringBuilder(1000);
            HtmlBuilder html = new HtmlBuilder(sb);
            HtmlGenerator generator = new HtmlGenerator();
            generator.VisualizeSyntaxNodes(nodes, 0, html);
            return sb.ToString();
        }

        static string Preformatted(string html)
        {
            return "<pre style=\"white-space: pre-wrap;\"><code>" + html + "</code></pre>";
        }

        [TestMethod]
        public void Test_VisualizeSyntaxNodes_SourceToken()
        {
            SyntaxNode[] nodes = new SyntaxNode[]
            {
                new SourceToken("string", TokenKind.Keyword, string.Empty, " "),
                new SourceToken("str", TokenKind.Name, string.Empty, " "),
                new SourceToken("=", TokenKind.Punctuation, string.Empty, " "),
                new SourceToken("\"Hello, world!\"", TokenKind.DoubleQuotLiteral, string.Empty, string.Empty),
                new SourceToken(";", TokenKind.Punctuation, string.Empty, " "),
                new SourceToken("//Comment", TokenKind.Comment, string.Empty, string.Empty),
            };

            string expected = Preformatted("<span style=\"color: blue;\">string </span>" + 
                "str = <span style=\"color: red;\">&quot;Hello, world!&quot;</span>;" + 
                " <span style=\"color: green;\">//Comment</span>");

            string str = Visualize(nodes);
            Assert.AreEqual(expected, str);
        }

        [TestMethod]
        public void Test_VisualizeSyntaxNodes_SpecialTextLiteral()
        {
            SyntaxNode[] nodes = new SyntaxNode[]
            {
                new SourceToken("string", TokenKind.Keyword, string.Empty, " "),
                new SourceToken("s", TokenKind.Name, string.Empty, " "),
                new SourceToken("=", TokenKind.Punctuation, string.Empty, " "),
                new SourceToken("@\"Quick brown fox\"", TokenKind.SpecialTextLiteral, string.Empty, string.Empty),
                new SourceToken(";", TokenKind.Punctuation, string.Empty, string.Empty),
            };

            string expected = Preformatted("<span style=\"color: blue;\">string </span>" +
                "s = <span style=\"color: red;\">@&quot;Quick brown fox&quot;</span>;");

            string str = Visualize(nodes);
            Assert.AreEqual(expected, str);
        }

        [TestMethod]
        public void Test_VisualizeSyntaxNodes_MethodSignature()
        {
            SyntaxNode[] nodes = new SyntaxNode[]
            {
                SyntaxFactory.CreateFromToken(".method", string.Empty, " "),
                SyntaxFactory.CreateFromToken("public", string.Empty, " "),
                SyntaxFactory.CreateFromToken("static", string.Empty, " "),
                SyntaxFactory.CreateFromToken("void", string.Empty, " "),
                SyntaxFactory.CreateFromToken("Foo", string.Empty, " "),
                SyntaxFactory.CreateFromToken("(", string.Empty, string.Empty),
                SyntaxFactory.CreateFromToken(")", string.Empty, string.Empty),
            };

            string expected = Preformatted("<span style=\"color: magenta;\">.method </span>" +
                "<span style=\"color: blue;\">public </span><span style=\"color: blue;\">static </span>" +
                "<span style=\"color: blue;\">void </span><span>Foo </span>()");

            string str = Visualize(nodes);
            Assert.AreEqual(expected, str);
        }

        [TestMethod]
        public void Test_VisualizeSyntaxNodes_StringLiteral()
        {
            SyntaxNode[] nodes = new SyntaxNode[]
            {
                SyntaxFactory.CreateFromToken("ldstr", string.Empty, " "),
                SyntaxFactory.CreateFromToken("\"Rabbits\"", string.Empty, " "),
                SyntaxFactory.CreateFromToken("// Load string", string.Empty, string.Empty),
            };

            string expected = Preformatted("<span>ldstr </span>" +
                "<span style=\"color: red;\">&quot;Rabbits&quot; </span>" +
                "<span style=\"color: green;\">// Load string</span>");

            string str = Visualize(nodes);
            Assert.AreEqual(expected, str);
        }

        [TestMethod]
        public void Test_VisualizeType()
        {
            HtmlGenerator gen = new HtmlGenerator(typeof(SampleType).Assembly);
            AssemblyReader reader = new AssemblyReader();
            string fname;
            string str;

            using (reader)
            {
                Assembly ass = reader.LoadFrom(typeof(SampleType).Assembly.Location);
                Type t = ass.GetType(typeof(SampleType).FullName);
                fname = HtmlGenerator.GenerateTypeFileName(t);
                str = gen.VisualizeType(t, new Dictionary<string, List<Type>>());
            }

            Assert.IsTrue(str.Contains("<span style=\"color: magenta;\">.class </span>" +
                "<span style=\"color: blue;\">public </span><span style=\"color: blue;\">auto </span>" +
                "<span style=\"color: blue;\">ansi </span><span style=\"color: blue;\">beforefieldinit </span>" +
                "<a href=\""+ fname + "\" class=\"memberid\">CilBrowser.Tests.SampleType</a>"));

            Assert.IsTrue(str.Contains("<span style=\"color: magenta;\"> .field </span>" +
                "<span style=\"color: blue;\">public </span><span style=\"color: blue;\">float32</span>" +
                "<span class=\"memberid\"> X</span>"));

            Assert.IsTrue(str.Contains("<span style=\"color: magenta;\"> .field </span>" +
                "<span style=\"color: blue;\">public </span><span style=\"color: blue;\">float32</span>" +
                "<span class=\"memberid\"> Y</span>"));
        }

        [TestMethod]
        public void Test_VisualizeSourceFile()
        {
            string content = @"using System;

public class Program
{
    public static void Main() 
    {
        Console.WriteLine(""Hello, world!"");
    }
}";

            string expected = @"<span style=""color: blue;"">using </span>System;

<span style=""color: blue;"">public </span><span style=""color: blue;"">class </span>Program
{
    <span style=""color: blue;"">public </span><span style=""color: blue;"">static </span><span style=""color: blue;"">void </span>Main() 
    {
        Console.WriteLine(<span style=""color: red;"">&quot;Hello, world!&quot;</span>);
    }
}";

            HtmlGenerator gen = new HtmlGenerator();            
            string str = gen.VisualizeSourceFile(content, "file.cs", string.Empty, string.Empty);
            
            Assert.IsTrue(str.Contains(Preformatted(expected)));
        }

        [TestMethod]
        public void Test_RenderSourceText()
        {
            const string sourceText = @"#include <stdio.h>
int main(int argc, char* argv[])
{
    printf(""Hello, world!"");
    getchar();
    return 0;
}
";
            string expected = @"<pre style=""white-space: pre-wrap;""><code>#include &lt;stdio.h&gt;
<span style=""color: blue;"">int </span>main(<span style=""color: blue;"">int </span>argc, <span style=""color: blue;"">char</span>* argv[])
{
    printf(<span style=""color: red;"">&quot;Hello, world!&quot;</span>);
    getchar();
    <span style=""color: blue;"">return </span>0;
}
</code></pre>";

            StringBuilder sb = new StringBuilder();
            StringWriter wr = new StringWriter(sb);
            HtmlGenerator.RenderSourceText(sourceText, ".cpp", wr);
            string html = sb.ToString().Trim();
            expected = expected.Trim();

            //normalize line endings
            html = html.Replace("\r\n", "\n");
            expected = expected.Replace("\r\n", "\n");

            Assert.AreEqual(expected, html);
        }
    }
}
