/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;

namespace CilBrowser.Core.Structure
{
    /// <summary>
    /// Represents a page in website structure. Page does not have child nodes.
    /// </summary>
    public abstract class PageNode : TreeNode
    {
        /// <inheritdoc/>
        public override IEnumerable<TreeNode> EnumChildNodes()
        {
            return TreeNode.EmptyArray;
        }
    }
}
