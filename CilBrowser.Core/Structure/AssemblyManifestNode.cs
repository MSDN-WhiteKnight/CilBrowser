/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace CilBrowser.Core.Structure
{
    /// <summary>
    /// Represents a disassembled assembly manifest in website structure
    /// </summary>
    public sealed class AssemblyManifestNode : PageNode
    {
        Assembly _ass;

        public AssemblyManifestNode(Assembly ass)
        {
            this._name = "assembly";
            this._displayName = "(Assembly manifest)";
            this._ass = ass;
        }

        public override void Render(HtmlGenerator generator, CilBrowserOptions options, TextWriter target)
        {
            string html = generator.VisualizeAssemblyManifest(this._ass);
            target.Write(html);
        }
    }
}
