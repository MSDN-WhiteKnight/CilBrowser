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
    /// Represents a file in website structure
    /// </summary>
    public sealed class FileNode : TreeNode
    {
        string _filepath;

        public FileNode(string filePath)
        {
            this._filepath = filePath;
            this._name = Path.GetFileName(filePath);
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
        public override IEnumerable<TreeNode> EnumChildNodes()
        {
            return TreeNode.EmptyArray;
        }

        /// <inheritdoc/>
        public override void Render(HtmlGenerator generator, CilBrowserOptions options, TextWriter target)
        {
            string content = File.ReadAllText(this._filepath, options.GetEncoding());

            // DRAFT: Add navigation panel
            string html = generator.VisualizeSourceFile(content, this._name, string.Empty, options.SourceControlURL);
            target.Write(html);
        }
    }
}
