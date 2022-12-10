/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2022,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;

namespace CilBrowser.Core
{
    /// <summary>
    /// Represents an attribute on HTML tag
    /// </summary>
    public class HtmlAttribute
    {
        public HtmlAttribute()
        {
            this.Value = string.Empty;
        }

        public HtmlAttribute(string name, string val)
        {
            this.Name = name;
            this.Value = val;
        }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}
