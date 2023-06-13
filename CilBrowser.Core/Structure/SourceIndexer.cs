/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CilBrowser.Core.Structure
{
    /// <summary>
    /// Indexes source code in file system directories and provides a tree structure that can be used to generate website
    /// </summary>
    public static class SourceIndexer
    {
        /// <summary>
        /// Gets a website structure tree for the specified source directory and all its subdirectories
        /// </summary>
        public static DirectoryNode SourceDirectoryToTree(string sourcesPath, CilBrowserOptions options)
        {
            return SourceDirectoryToTreeImpl(sourcesPath, options, 0);
        }

        static DirectoryNode SourceDirectoryToTreeImpl(string sourcesPath, CilBrowserOptions options, int level)
        {
            string dirName = Utils.GetDirectoryNameFromPath(sourcesPath);
            DirectoryNode ret = new DirectoryNode(dirName);

            if (level > 50)
            {
                Console.WriteLine("Error: directory recursion is too deep!");
                return ret;
            }

            HashSet<string> sourceExtensions = options.SourceExtensions;
            
            string[] dirs = Directory.GetDirectories(sourcesPath);
            Array.Sort(dirs);

            for (int i = 0; i < dirs.Length; i++)
            {
                string name = Utils.GetDirectoryNameFromPath(dirs[i]);

                if (FileUtils.IsDirectoryExcluded(name)) continue;

                DirectoryNode node = SourceDirectoryToTreeImpl(dirs[i], options, level + 1);

                if (node.SectionsCount + node.PagesCount == 0)
                {
                    continue; //not interested in empty directories
                }

                ret.AddSection(node);
            }

            string[] files = Directory.GetFiles(sourcesPath);
            Array.Sort(files);

            for (int i = 0; i < files.Length; i++)
            {
                if (!FileUtils.IsSourceFile(files[i], sourceExtensions)) continue;

                FileNode node = new FileNode(files[i]);
                ret.AddPage(node);
            }

            return ret;
        }
    }
}
