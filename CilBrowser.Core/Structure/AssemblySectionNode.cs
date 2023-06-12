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
    /// Provides a section node that represents an assembly in website structure
    /// </summary>
    public sealed class AssemblySectionNode : SectionNode
    {
        Assembly _ass;

        public AssemblySectionNode(Assembly ass)
        {
            this._ass = ass;
            string assName = ass.GetName().Name;
            this._name = Utils.StrToIdentifier(assName);
            this._displayName = assName;
        }

        public override TreeNodeKind Kind => TreeNodeKind.Assembly;
        
        public override void Render(HtmlGenerator generator, CilBrowserOptions options, TextWriter target)
        {
            HtmlBuilder toc = new HtmlBuilder(target);
            HtmlGenerator.WriteTocStart(toc, this._ass);

            //render ToC entries for pages
            toc.WriteTagStart("table", HtmlBuilder.OneAttribute("cellpadding", "2px"));

            for (int i = 0; i < this._pages.Count; i++)
            {
                string name = this._pages[i].Name;
                string pageName = FileUtils.FileNameToPageName(name);
                WebsiteGenerator.RenderTocEntry(this._pages[i].DisplayName, pageName, string.Empty, TreeNodeKind.Unknown, toc);
            }

            toc.WriteTagEnd("table");

            //render ToC entries for namespaces
            if (this._sections.Count > 0)
            {
                toc.WriteTag("h2", "Namespaces");
            }
            
            WebsiteGenerator.RenderDirsList(this._sections.ToArray(), string.Empty, toc);

            toc.WriteRaw(Environment.NewLine);
            generator.WriteFooter(toc);
            toc.EndDocument();
            target.Flush();
        }
    }
}
