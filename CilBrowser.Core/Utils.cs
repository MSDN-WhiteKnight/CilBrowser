/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2022,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CilBrowser.Core
{
    public static class Utils
    {
        public static bool StrEquals(string left, string right)
        {
            return string.Equals(left, right, StringComparison.Ordinal);
        }

        public static string GetDirectoryNameFromPath(string path)
        {
            if (path.EndsWith("\\") || path.EndsWith("/"))
            {
                return Path.GetFileName(path.Substring(0, path.Length - 1));
            }
            else
            {
                return Path.GetFileName(path);
            }
        }
    }
}
