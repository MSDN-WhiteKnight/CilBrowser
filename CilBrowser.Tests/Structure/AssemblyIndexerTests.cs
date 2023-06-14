/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CilBrowser.Core;
using CilBrowser.Core.Structure;

namespace CilBrowser.Tests.Structure
{
    [TestClass]
    public class AssemblyIndexerTests
    {
        static void VerifyTypeNode(PageNode node)
        {
            Assert.IsTrue(node is TypeNode);
            Assert.AreEqual(0, node.EnumChildNodes().Count());
        }

        static void VerifyNamespaceNode(SectionNode node)
        {
            Assert.AreEqual(TreeNodeKind.Namespace, node.Kind);
            Assert.AreEqual(0, node.SectionsCount);

            foreach (PageNode subnode in node.Pages)
            {
                Assert.AreSame(node, subnode.Parent);
                VerifyTypeNode(subnode);
            }
        }

        [TestMethod]
        public void Test_AssemblyToTree()
        {
            AssemblySectionNode tree = AssemblyIndexer.AssemblyToTree(typeof(HtmlGenerator).Assembly, string.Empty);
            Assert.AreEqual("CilBrowser.Core", tree.DisplayName);
            Assert.AreEqual("CilBrowser.Core", tree.Name);
            Assert.IsNull(tree.Parent);
            Assert.AreEqual(TreeNodeKind.Assembly, tree.Kind);
            Assert.AreEqual(1, tree.PagesCount);

            // Assembly manifest
            PageNode page = tree.Pages.First();
            Assert.AreEqual("(Assembly manifest)", page.DisplayName);
            Assert.AreEqual("assembly", page.Name);
            Assert.AreEqual(0, page.EnumChildNodes().Count());
            Assert.AreSame(tree, page.Parent);

            // Namespaces
            SectionNode[] sections = tree.Sections.ToArray();
            List<string> namespaces = new List<string>(sections.Length);

            for (int i = 0; i < sections.Length; i++)
            {
                Assert.AreSame(tree, sections[i].Parent);
                VerifyNamespaceNode(sections[i]);
                namespaces.Add(sections[i].DisplayName);
            }

            Assert.IsTrue(namespaces.Contains("CilBrowser.Core"));
            Assert.IsTrue(namespaces.Contains("CilBrowser.Core.Configuration"));
            Assert.IsTrue(namespaces.Contains("CilBrowser.Core.Structure"));
            Assert.IsTrue(namespaces.Contains("CilBrowser.Core.SyntaxModel"));
        }

        [TestMethod]
        public void Test_AssemblyToTree_NamespaceFilter()
        {
            AssemblySectionNode tree = AssemblyIndexer.AssemblyToTree(typeof(HtmlGenerator).Assembly, "CilBrowser.Core");
            Assert.AreEqual("CilBrowser.Core", tree.DisplayName);
            
            // Namespaces
            SectionNode[] sections = tree.Sections.ToArray();
            Assert.AreEqual(1, sections.Length);
            Assert.AreEqual("CilBrowser.Core", sections[0].DisplayName);
            VerifyNamespaceNode(sections[0]);
        }
    }
}
