/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CilBrowser.Core.Structure
{
    public abstract class TreeNode
    {
        protected string _name;
        protected TreeNode _parent;

        internal static readonly TreeNode[] EmptyArray = new TreeNode[0];

        protected TreeNode(){}

        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        public TreeNode Parent
        {
            get { return this._parent; }
            set { this._parent = value; }
        }

        public virtual TreeNodeKind Kind { get; } 

        public abstract IEnumerable<TreeNode> EnumChildNodes();

        public abstract void Render(HtmlGenerator generator, CilBrowserOptions options, TextWriter target);
    }

    public enum TreeNodeKind
    {
        Unknown = 0, File, Directory
    }
}
