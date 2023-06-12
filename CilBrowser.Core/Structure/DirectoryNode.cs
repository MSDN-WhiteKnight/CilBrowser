/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CilBrowser.Core.Structure
{
    /// <summary>
    /// Represents a directory in website structure. Directory can contain child nodes for files and subdirectories.
    /// </summary>
    public sealed class DirectoryNode : SectionNode
    {
        TreeNodeKind _kind;

        public DirectoryNode(string name, TreeNodeKind kind)
        {
            this._name = name;
            this._displayName = name;
            this._kind = kind;
        }

        public override TreeNodeKind Kind => this._kind;
        
        /// <inheritdoc/>
        public override void Render(HtmlGenerator generator, CilBrowserOptions options, TextWriter target)
        {
            //render ToC for this directory
            int level = this.GetLevel();
            HtmlBuilder toc = new HtmlBuilder(target);

            if (this._kind == TreeNodeKind.Directory)
            {
                HtmlGenerator.WriteTocStart(toc, this._name);
            }
            else if (this._kind == TreeNodeKind.Assembly)
            {
                HtmlGenerator.WriteTocStart(toc, this._displayName, "Assembly: " + this._displayName);
            }
            else
            {
                HtmlGenerator.WriteTocStart(toc, this._displayName, this._displayName);
            }

            //render ToC entries for subdirectories
            if (this._sections.Count > 0)
            {
                if (this.Kind == TreeNodeKind.Directory) toc.WriteTag("h2", "Subdirectories");
                else if (this.Kind == TreeNodeKind.Assembly) toc.WriteTag("h2", "Namespaces");
                else toc.WriteTag("h2", "Sections");
            }

            if (level > 0)
            {
                toc.StartParagraph();
                toc.WriteHyperlink("../index.html", "(go to parent directory)");
                toc.EndParagraph();
            }
            
            string dirIconURL = WebsiteGenerator.GetImagesURL(level) + "dir.png";
            WebsiteGenerator.RenderDirsList(this._sections.ToArray(), dirIconURL, toc);
            toc.WriteRaw(Environment.NewLine);

            //render ToC entries for files
            string fileIconURL = WebsiteGenerator.GetImagesURL(level) + "file.png";

            if (this._pages.Count > 0)
            {
                if (this.Kind == TreeNodeKind.Directory) toc.WriteTag("h2", "Files");
                else if(this.Kind == TreeNodeKind.Namespace) toc.WriteTag("h2", "Types");
                else toc.WriteTag("h2", "Pages");
            }

            toc.WriteTagStart("table", HtmlBuilder.OneAttribute("cellpadding", "2px"));

            for (int i = 0; i < this._pages.Count; i++)
            {
                string name = this._pages[i].Name;
                string pageName = FileUtils.FileNameToPageName(name);
                WebsiteGenerator.RenderTocEntry(this._pages[i].DisplayName, pageName, fileIconURL, this._pages[i].Kind, toc);
            }

            toc.WriteTagEnd("table");
            generator.WriteFooter(toc);
            toc.EndDocument();
            target.Flush();
        }
    }
}
