/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace CilBrowser.Core
{
    static class FileUtils
    {
        // Default source file extensions list
        static HashSet<string> s_srcExtensions = new HashSet<string>(new string[] {
            ".il", ".cil", ".cs", ".vb", ".c", ".cpp", ".h", ".hpp", ".js", ".ts", ".fs", ".txt", ".md", ".htm", ".html",
            ".css", ".xml", ".csproj", ".vbproj", ".vcxproj", ".proj", ".rc", ".cmd", ".bat", ".sh", ".ps1", ".xaml",
            ".config", ".json", ".yml", ".sln", ".props", ".targets"
        });

        static HashSet<string> s_excludedDirs = new HashSet<string>(new string[] {
            ".git", "bin", "obj", "packages", ".vs", "TestResults", "Debug", "Release"
        });

        internal static bool IsSourceFile(string name, HashSet<string> srcExtensions)
        {
            string ext = Path.GetExtension(name).ToLower();

            return ext == string.Empty || srcExtensions.Contains(ext);
        }

        internal static bool IsSourceFileDefault(string name)
        {
            return IsSourceFile(name, s_srcExtensions);
        }

        internal static bool IsDirectoryExcluded(string name)
        {
            return s_excludedDirs.Contains(name);
        }

        internal static HashSet<string> GetDefaultExtensions()
        {
            return s_srcExtensions;
        }

        internal static string FileNameToPageName(string filename)
        {
            return filename + ".html";
        }
    }
}
