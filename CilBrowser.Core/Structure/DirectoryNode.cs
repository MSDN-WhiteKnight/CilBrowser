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
        public DirectoryNode(string name)
        {
            this._name = name;
            this._displayName = name;
        }

        public override TreeNodeKind Kind => TreeNodeKind.Directory;
        
        /// <inheritdoc/>
        public override void Render(HtmlGenerator generator, CilBrowserOptions options, TextWriter target)
        {
            //render ToC for this directory
            int level = this.GetLevel();
            HtmlBuilder toc = new HtmlBuilder(target);
            HtmlGenerator.StartDocument(toc, HtmlGenerator.AppName + " - " + this._displayName);
            toc.WriteParagraph(HtmlGenerator.AppName);

            //location bar
            TreeNode[] path = this.GetPathFromRoot();
            WebsiteGenerator.RenderNodePath(path, 0, toc);
            toc.WriteTag("h1", "Source directory: " + this._displayName);
            toc.WriteRaw(Environment.NewLine);

            //render ToC entries for subdirectories
            if (this._sections.Count > 0)
            {
                toc.WriteTag("h2", "Subdirectories");
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
                toc.WriteTag("h2", "Files");
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
