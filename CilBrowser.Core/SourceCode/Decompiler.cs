/* CIL Tools 
 * Copyright (c) 2022, MSDN.WhiteKnight (https://github.com/MSDN-WhiteKnight) 
 * License: BSD 2.0 */
using System;
using System.Collections.Generic;
using System.Text;

namespace CilView.SourceCode
{
    public static class Decompiler
    {
        public static bool IsCppExtension(string ext)
        {
            return ext == ".cpp" || ext == ".c" || ext == ".h" || ext == string.Empty;
        }        
    }
}
