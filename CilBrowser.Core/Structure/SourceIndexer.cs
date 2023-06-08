/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CilBrowser.Core.Structure
{
    public static class SourceIndexer
    {
        public static DirectoryNode SourceDirectoryToTree(string sourcesPath, CilBrowserOptions options)
        {
            return SourceDirectoryToTreeImpl(sourcesPath, options, 0);
        }

        static DirectoryNode SourceDirectoryToTreeImpl(string sourcesPath, CilBrowserOptions options, int level)
        {
            string dirName = Utils.GetDirectoryNameFromPath(sourcesPath);
            DirectoryNode ret = new DirectoryNode(dirName, sourcesPath);

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
                ret.AddDirectory(node);
            }

            string[] files = Directory.GetFiles(sourcesPath);
            Array.Sort(files);

            for (int i = 0; i < files.Length; i++)
            {
                if (!FileUtils.IsSourceFile(files[i], sourceExtensions)) continue;

                FileNode node = new FileNode(files[i]);
                ret.AddFile(node);
            }

            return ret;
        }
    }
}
