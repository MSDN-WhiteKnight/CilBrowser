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
        List<FileNode> _files;
        List<DirectoryNode> _dirs;
        string _path;

        public DirectoryNode(string name, string path)
        {
            this._name = name;
            this._path = path;
            this._files = new List<FileNode>();
            this._dirs = new List<DirectoryNode>();
        }

        public override TreeNodeKind Kind => TreeNodeKind.Directory;

        public string Path
        {
            get { return this._path; }
        }

        /// <summary>
        /// Adds a specified file node into the collection of child nodes. Also sets its parent node to this one.
        /// </summary>
        public void AddFile(FileNode file)
        {
            this._files.Add(file);
            file.Parent = this;
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
        /// Gets the collection of file child nodes for this directory
        /// </summary>
        public IEnumerable<FileNode> Files 
        {
            get { foreach (FileNode node in this._files) yield return node; }
        }

        /// <inheritdoc/>
        public override IEnumerable<TreeNode> EnumChildNodes()
        {
            foreach (DirectoryNode node in this._dirs) yield return node;

            foreach (FileNode node in this._files) yield return node;
        }

        string[] GetDirsAsStrings()
        {
            string[] ret = new string[this._dirs.Count];

            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = this._dirs[i].Path;
            }

            return ret;
        }

        /// <inheritdoc/>
        public override void Render(HtmlGenerator generator, CilBrowserOptions options, TextWriter target)
        {
            //render ToC for this directory
            int level = this.GetLevel();
            HtmlBuilder toc = new HtmlBuilder(target);
            HtmlGenerator.WriteTocStart(toc, this._name);

            //render ToC entries for subdirectories
            if (this._dirs.Count > 0) toc.WriteTag("h2", "Subdirectories");

            if (level > 0)
            {
                toc.StartParagraph();
                toc.WriteHyperlink("../index.html", "(go to parent directory)");
                toc.EndParagraph();
            }
            
            string dirIconURL = WebsiteGenerator.GetImagesURL(level) + "dir.png";
            WebsiteGenerator.RenderDirsList(this.GetDirsAsStrings(), dirIconURL, toc);
            toc.WriteRaw(Environment.NewLine);

            //render ToC entries for files
            string fileIconURL = WebsiteGenerator.GetImagesURL(level) + "file.png";

            if (this._files.Count > 0) toc.WriteTag("h2", "Files");

            toc.WriteTagStart("table", HtmlBuilder.OneAttribute("cellpadding", "2px"));

            for (int i = 0; i < this._files.Count; i++)
            {
                string name = this._files[i].Name;
                string pageName = FileUtils.FileNameToPageName(name);
                WebsiteGenerator.RenderTocEntry(name, pageName, fileIconURL, toc);
            }

            toc.WriteTagEnd("table");
            generator.WriteFooter(toc);
            toc.EndDocument();
            target.Flush();
        }
    }
}
