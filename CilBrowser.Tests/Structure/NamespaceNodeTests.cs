/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CilBrowser.Core;
using CilBrowser.Core.Structure;

namespace CilBrowser.Tests.Structure
{
    [TestClass]
    public class NamespaceNodeTests
    {
        [TestMethod]
        public void Test_Render()
        {
            // Build node
            Dictionary<string, List<Type>> typeMap = new Dictionary<string, List<Type>>();
            NamespaceNode nsNode = new NamespaceNode("CilBrowser.Core", "CilBrowser.Core");
            nsNode.AddPage(new TypeNode(typeof(HtmlGenerator), typeMap));
            nsNode.Parent = new DirectoryNode("root");
            string metadataToken = typeof(HtmlGenerator).MetadataToken.ToString("X");

            // Render node
            HtmlGenerator generator = new HtmlGenerator();
            string html = nsNode.RenderToString(generator, new CilBrowserOptions());

            // Verify output
            string expected = @"<p>.NET CIL Browser</p><h1>CilBrowser.Core</h1>
<p><a href=""../index.html"">(go to assembly)</a></p>
<h2>Types in this namespace</h2><table cellpadding=""2px""><tr><td><a href=""%METADATA_TOKEN%.html"">CilBrowser.Core.HtmlGenerator</a></td></tr></table>";

            expected = expected.Replace("%METADATA_TOKEN%", metadataToken);
            expected = expected.Replace("\r\n", "\n").Trim();
            html = html.Replace("\r\n", "\n").Trim();
            Assert.IsTrue(html.Contains(expected));
        }
    }
}
