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
            return SourceDirectoryToTreeImpl(sourcesPath, options, recursive: true, 0);
        }

        /// <summary>
        /// Gets a website structure node for the specified source directory
        /// </summary>
        public static DirectoryNode CreateDirectoryNode(string sourcesPath, CilBrowserOptions options, 
            bool topLevel, string relativePath)
        {
            DirectoryNode ret = SourceDirectoryToTreeImpl(sourcesPath, options, recursive: false, 0);

            if (!topLevel)
            {
                DirectoryNode[] nodes = DirectoryNode.CreateDirectoryPath(relativePath);

                if (nodes.Length > 0)
                {
                    //connect node with the chain of parent nodes, to enable location bar rendering
                    nodes[nodes.Length - 1].AddSection(ret);
                }
                else
                {
                    //add dummy parent node, so the directory won't be treated as top-level
                    string parentName = Utils.GetParentDirectoryFromPath(sourcesPath);

                    if (string.IsNullOrWhiteSpace(parentName)) parentName = "(root)";

                    ret.Parent = new DirectoryNode(parentName);
                }
            }

            return ret;
        }

        static DirectoryNode SourceDirectoryToTreeImpl(string sourcesPath, CilBrowserOptions options, bool recursive, int level)
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

                DirectoryNode node;

                if (recursive)
                {
                    node = SourceDirectoryToTreeImpl(dirs[i], options, recursive, level + 1);
                }
                else
                {
                    node = new DirectoryNode(name);

                    //add dummy subnode so directory won't be skipped as empty
                    node.AddPage(new FileNode("empty"));
                }

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
