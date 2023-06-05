/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CilBrowser.Core.Structure
{
    public sealed class DirectoryNode : TreeNode
    {
        List<FileNode> _files;

        public DirectoryNode(string name, TreeNode parent)
        {
            this._name = name;
            this._parent = parent;
            this._files = new List<FileNode>();
        }

        public override TreeNodeKind Kind => TreeNodeKind.Directory;

        public void AddFile(FileNode file)
        {
            this._files.Add(file);
            file.Parent = this;
        }
        
        public override IEnumerable<TreeNode> EnumChildNodes()
        {
            foreach (FileNode node in this._files) yield return node;
        }

        public override void Render(HtmlGenerator generator, CilBrowserOptions options, TextWriter target)
        {
            //empty by design
        }
    }
}
