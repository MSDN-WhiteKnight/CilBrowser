/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CilBrowser.Core;

namespace CilBrowser.Tests
{
    [TestClass]
    public class ConfigurationTests
    {
        [TestMethod]
        public void Test_CilBrowserOptions()
        {
            string[] lines = { 
                ";Test",
                "SourceControlURL=http://example.com/",
                "SourceExtensions=cs,vb,cpp",
                "SourceEncoding=cp1251"
            };

            CilBrowserOptions options = CilBrowserOptions.ReadValues(lines);

            Assert.AreEqual("http://example.com/", options.SourceControlURL);
            CollectionAssert.AreEquivalent(new string[] { ".cs", ".vb", ".cpp" }, options.SourceExtensions.ToArray());
            Assert.IsTrue(options.UseAnsiEncoding);
            Assert.AreEqual(1251, options.AnsiCodepage);
        }

        [TestMethod]
        public void Test_CilBrowserOptions_UTF8()
        {
            string[] lines = {
                "SourceExtensions=js,py",
                "SourceEncoding=utf8"
            };

            CilBrowserOptions options = CilBrowserOptions.ReadValues(lines);

            CollectionAssert.AreEquivalent(new string[] { ".js", ".py"}, options.SourceExtensions.ToArray());
            Assert.IsFalse(options.UseAnsiEncoding);
        }

        [TestMethod]
        public void Test_CilBrowserOptions_Empty()
        {
            string[] lines = new string[0];
            CilBrowserOptions options = CilBrowserOptions.ReadValues(lines);

            Assert.AreEqual(string.Empty, options.SourceControlURL);
            Assert.IsFalse(options.UseAnsiEncoding);
        }
    }
}
