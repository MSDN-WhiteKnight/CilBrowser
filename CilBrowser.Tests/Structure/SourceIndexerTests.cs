/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CilBrowser.Core;
using CilBrowser.Core.Structure;

namespace CilBrowser.Tests.Structure
{
    [TestClass]
    public class SourceIndexerTests
    {
        [TestMethod]
        public void Test_SourceDirectoryToTree()
        {
            string dir = Utils.CreateTempDir(string.Empty);

            try
            {
                // Create files in temp directory
                Directory.CreateDirectory(Path.Combine(dir, "subdir1"));
                Directory.CreateDirectory(Path.Combine(dir, "subdir2"));
                File.WriteAllText(Path.Combine(dir, "subdir1", "foo.txt"), "foo");
                File.WriteAllText(Path.Combine(dir, "subdir1", "bar.txt"), "bar");
                File.WriteAllText(Path.Combine(dir, "subdir2", "alice.txt"), "Alice");
                File.WriteAllText(Path.Combine(dir, "subdir2", "bob.txt"), "Bob");
                File.WriteAllText(Path.Combine(dir, "frobby"), "Bobby");

                // Index files
                DirectoryNode tree = SourceIndexer.SourceDirectoryToTree(dir, new CilBrowserOptions());

                // Validate results
                SectionNode[] dirs = tree.Sections.ToArray();
                Assert.AreEqual(2, dirs.Length);
                Assert.AreEqual("subdir1", dirs[0].Name);
                Assert.AreEqual("subdir2", dirs[1].Name);

                PageNode[] files = tree.Pages.ToArray();
                Assert.AreEqual(1, files.Length);
                Assert.AreEqual("frobby", files[0].Name);

                files = dirs[0].Pages.ToArray();
                Assert.AreEqual(2, files.Length);
                Assert.AreEqual("bar.txt", files[0].Name);
                Assert.AreEqual("foo.txt", files[1].Name);

                files = dirs[1].Pages.ToArray();
                Assert.AreEqual(2, files.Length);
                Assert.AreEqual("alice.txt", files[0].Name);
                Assert.AreEqual("bob.txt", files[1].Name);
            }
            finally
            {
                Utils.DeleteTempDirRecursive(dir, 0);
            }
        }
    }
}
