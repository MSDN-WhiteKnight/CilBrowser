/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2022,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;

namespace CilBrowser.Core
{
    internal static class Utils
    {
        public static bool StrEquals(string left, string right)
        {
            return string.Equals(left, right, StringComparison.Ordinal);
        }
    }
}
