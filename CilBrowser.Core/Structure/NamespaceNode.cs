/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CilBrowser.Core.Structure
{
    public sealed class NamespaceNode : SectionNode
    {
        public NamespaceNode(string name, string displayName)
        {
            this._name = name;
            this._displayName = displayName;
        }

        public override TreeNodeKind Kind => TreeNodeKind.Namespace;

        public override void Render(HtmlGenerator generator, CilBrowserOptions options, TextWriter target)
        {
            HtmlBuilder toc = new HtmlBuilder(target);
            HtmlGenerator.WriteTocStart(toc, this._displayName, this._displayName);

            toc.StartParagraph();
            toc.WriteHyperlink("../index.html", "(go to assembly)");
            toc.EndParagraph();

            //render ToC entries for subsections
            WebsiteGenerator.RenderDirsList(this._sections.ToArray(), string.Empty, toc);
            toc.WriteRaw(Environment.NewLine);

            //render ToC entries for types
            toc.WriteTag("h2", "Types in this namespace");
            toc.WriteTagStart("table", HtmlBuilder.OneAttribute("cellpadding", "2px"));

            for (int i = 0; i < this._pages.Count; i++)
            {
                string name = this._pages[i].Name;
                string pageName = FileUtils.FileNameToPageName(name);
                WebsiteGenerator.RenderTocEntry(this._pages[i].DisplayName, pageName, string.Empty, TreeNodeKind.Unknown, toc);
            }

            toc.WriteTagEnd("table");
            toc.WriteRaw(Environment.NewLine);

            generator.WriteFooter(toc);
            toc.EndDocument();
            target.Flush();
        }
    }
}
