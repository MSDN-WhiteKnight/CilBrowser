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
    public sealed class DirectoryNode : TreeNode
    {
        List<PageNode> _pages;
        List<DirectoryNode> _dirs;
        TreeNodeKind _kind;

        public DirectoryNode(string name, TreeNodeKind kind)
        {
            this._name = name;
            this._displayName = name;
            this._pages = new List<PageNode>();
            this._dirs = new List<DirectoryNode>();
            this._kind = kind;
        }

        public override TreeNodeKind Kind => this._kind;

        /// <summary>
        /// Adds a specified page node into the collection of child nodes. Also sets its parent node to this one.
        /// </summary>
        public void AddPage(PageNode page)
        {
            this._pages.Add(page);
            page.Parent = this;
        }

        /// <summary>
        /// Adds a specified directory node into the collection of child nodes. Also sets its parent node to this one.
        /// </summary>
        public void AddDirectory(DirectoryNode dir)
        {
            this._dirs.Add(dir);
            dir.Parent = this;
        }

        /// <summary>
        /// Gets the collection of subdirectory child nodes for this directory
        /// </summary>
        public IEnumerable<DirectoryNode> Directories
        {
            get { foreach (DirectoryNode node in this._dirs) yield return node; }
        }

        /// <summary>
        /// Gets the collection of page child nodes for this directory
        /// </summary>
        public IEnumerable<PageNode> Pages 
        {
            get { foreach (PageNode node in this._pages) yield return node; }
        }

        public int PagesCount
        {
            get { return this._pages.Count; }
        }

        public int DirectoriesCount
        {
            get { return this._dirs.Count; }
        }

        /// <inheritdoc/>
        public override IEnumerable<TreeNode> EnumChildNodes()
        {
            foreach (DirectoryNode node in this._dirs) yield return node;

            foreach (FileNode node in this._pages) yield return node;
        }

        /// <inheritdoc/>
        public override void Render(HtmlGenerator generator, CilBrowserOptions options, TextWriter target)
        {
            //render ToC for this directory
            int level = this.GetLevel();
            HtmlBuilder toc = new HtmlBuilder(target);
            HtmlGenerator.WriteTocStart(toc, this._name);

            //render ToC entries for subdirectories
            if (this._dirs.Count > 0)
            {
                if(this.Kind == TreeNodeKind.Directory) toc.WriteTag("h2", "Subdirectories");
                else toc.WriteTag("h2", "Sections");
            }

            if (level > 0)
            {
                toc.StartParagraph();
                toc.WriteHyperlink("../index.html", "(go to parent directory)");
                toc.EndParagraph();
            }
            
            string dirIconURL = WebsiteGenerator.GetImagesURL(level) + "dir.png";
            WebsiteGenerator.RenderDirsList(this._dirs.ToArray(), dirIconURL, toc);
            toc.WriteRaw(Environment.NewLine);

            //render ToC entries for files
            string fileIconURL = WebsiteGenerator.GetImagesURL(level) + "file.png";

            if (this._pages.Count > 0)
            {
                if (this.Kind == TreeNodeKind.Directory) toc.WriteTag("h2", "Files");
                else toc.WriteTag("h2", "Pages");
            }

            toc.WriteTagStart("table", HtmlBuilder.OneAttribute("cellpadding", "2px"));

            for (int i = 0; i < this._pages.Count; i++)
            {
                string name = this._pages[i].Name;
                string pageName = FileUtils.FileNameToPageName(name);
                WebsiteGenerator.RenderTocEntry(this._pages[i].DisplayName, pageName, fileIconURL, toc);
            }

            toc.WriteTagEnd("table");
            generator.WriteFooter(toc);
            toc.EndDocument();
            target.Flush();
        }
    }
}
