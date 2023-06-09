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
    /// Provides a base class for nodes in the website structure tree. Each node represents a page in generated website and 
    /// its correspoding Table of contents (ToC) entry.
    /// </summary>
    public abstract class TreeNode
    {
        protected string _name;
        protected string _displayName;
        protected TreeNode _parent;

        internal static readonly TreeNode[] EmptyArray = new TreeNode[0];

        protected TreeNode(){}

        /// <summary>
        /// Gets or sets URL name part of this node
        /// </summary>
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        /// <summary>
        /// Gets or sets a ToC display name of this node
        /// </summary>
        public string DisplayName
        {
            get { return this._displayName; }
            set { this._displayName = value; }
        }

        /// <summary>
        /// Gets or sets a parent node of this node, or <c>null</c> if this node is a root.
        /// </summary>
        public TreeNode Parent
        {
            get { return this._parent; }
            set { this._parent = value; }
        }

        public virtual TreeNodeKind Kind { get; } 

        /// <summary>
        /// Gets a collection of this node's child nodes, or an empty collection if it's a leaf node.
        /// </summary>
        public abstract IEnumerable<TreeNode> EnumChildNodes();

        /// <summary>
        /// Generates HTML output for this node into the specified TextWriter
        /// </summary>
        public abstract void Render(HtmlGenerator generator, CilBrowserOptions options, TextWriter target);

        public string RenderToString(HtmlGenerator generator, CilBrowserOptions options)
        {
            StringBuilder sb = new StringBuilder(5000);
            StringWriter wr = new StringWriter(sb);
            this.Render(generator, options, wr);
            return sb.ToString();
        }

        internal int GetLevel()
        {
            int level = 0;
            TreeNode root = this._parent;

            while (true)
            {
                if (root == null) return level;

                root = root.Parent;
                level++;
            }
        }
    }

    public enum TreeNodeKind
    {
        Unknown = 0, File, Directory, Namespace, Assembly
    }
}
