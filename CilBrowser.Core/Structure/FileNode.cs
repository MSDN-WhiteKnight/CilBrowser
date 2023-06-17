/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CilBrowser.Core.Structure
{
    /// <summary>
    /// Represents a file in website structure
    /// </summary>
    public sealed class FileNode : PageNode
    {
        string _filepath;

        public FileNode(string filePath)
        {
            this._filepath = filePath;
            this._name = Path.GetFileName(filePath);
            this._displayName = this._name;
        }

        /// <summary>
        /// Gets a full path to file represented by this node
        /// </summary>
        public string FilePath
        {
            get { return this._filepath; }
        }

        public override TreeNodeKind Kind => TreeNodeKind.File;

        /// <inheritdoc/>
        public override void Render(HtmlGenerator generator, CilBrowserOptions options, TextWriter target)
        {
            HtmlBuilder builder = new HtmlBuilder(target);
            HtmlGenerator.StartDocument(builder, "Source file: " + this._displayName);

            //navigation panel
            string navigation = string.Empty;
            DirectoryNode dir = this._parent as DirectoryNode;

            if (dir != null)
            {
                PageNode[] files = dir.Pages.ToArray();

                if (files.Length > 1)
                {
                    navigation = WebsiteGenerator.VisualizeNavigationPanel(this, dir.Name, files, dir.Kind);
                }
            }

            //location bar
            generator.WriteHeaderHTML(builder);
            TreeNode[] path = this.GetPathFromRoot();
            builder.StartParagraph();

            for (int i = 0; i < path.Length; i++)
            {
                int level = path.Length - 1 - i;
                builder.WriteRaw("<a href=\"");

                for (int j = 0; j < level; j++) builder.WriteRaw("../");

                builder.WriteRaw("index.html\">");
                builder.WriteEscaped(path[i].DisplayName);
                builder.WriteTagEnd("a");
                builder.WriteRaw(" / ");
            }

            builder.EndParagraph();
            builder.WriteTag("h2", "Source file: " + this._displayName);
            HtmlGenerator.StartContentSection(builder, navigation);

            try
            {
                //content
                string content = File.ReadAllText(this._filepath, options.GetEncoding());
                HtmlGenerator.RenderSourceText(content, Path.GetExtension(this._name), target);
                
                if (!string.IsNullOrEmpty(options.SourceControlURL))
                {
                    string url = Utils.UrlAppend(options.SourceControlURL, this._name);
                    builder.WriteHyperlink(url, "View in source control");
                }
            }
            catch (IOException ex)
            {
                string html = HtmlGenerator.VisualizeException(ex);
                Console.WriteLine("Failed to render HTML for: " + this._name);
                Console.WriteLine(ex.ToString());
                target.Write(html);
            }

            generator.EndDocument(builder);
        }
    }
}
