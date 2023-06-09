/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CilBrowser.Core.Structure
{
    public sealed class TypeNode : PageNode
    {
        Type _t;
        Dictionary<string, List<Type>> _typeMap;

        public TypeNode(Type t, Dictionary<string, List<Type>> typeMap)
        {
            this._name = HtmlGenerator.GenerateTypeFileNameShort(t);
            this._displayName = t.Name;
            this._t = t;
            this._typeMap = typeMap;
        }

        public override void Render(HtmlGenerator generator, CilBrowserOptions options, TextWriter target)
        {
            string html;
            
            try
            {
                html = generator.VisualizeType(this._t, this._typeMap);
            }
            catch (NotSupportedException ex)
            {
                html = HtmlGenerator.VisualizeException(ex);
                Console.WriteLine("Failed to render HTML for " + this._t.FullName);
                Console.WriteLine(ex.ToString());
            }

            target.Write(html);
        }
    }
}
