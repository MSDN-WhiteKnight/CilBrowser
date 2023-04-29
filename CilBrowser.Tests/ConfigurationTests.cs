/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
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
            CollectionAssert.AreEqual(new string[] { ".cs", ".vb", ".cpp" }, options.SourceExtensions);
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

            CollectionAssert.AreEqual(new string[] { ".js", ".py"}, options.SourceExtensions);
            Assert.IsFalse(options.UseAnsiEncoding);
        }

        [TestMethod]
        public void Test_CilBrowserOptions_Empty()
        {
            string[] lines = new string[0];
            CilBrowserOptions options = CilBrowserOptions.ReadValues(lines);

            Assert.AreEqual(string.Empty, options.SourceControlURL);
            Assert.AreEqual(0, options.SourceExtensions.Length);
            Assert.IsFalse(options.UseAnsiEncoding);
        }
    }
}
