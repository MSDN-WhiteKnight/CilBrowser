/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;

namespace CilBrowser.Core.Structure
{
    /// <summary>
    /// Provides a base class for nodes that represent sections in website structure. Section can contains pages and subsections.
    /// </summary>
    public abstract class SectionNode : TreeNode
    {
        protected List<PageNode> _pages = new List<PageNode>();
        protected List<SectionNode> _sections = new List<SectionNode>();
        
        /// <summary>
        /// Adds a specified page node into the collection of child nodes. Also sets its parent node to this one.
        /// </summary>
        public void AddPage(PageNode page)
        {
            this._pages.Add(page);
            page.Parent = this;
        }

        /// <summary>
        /// Adds a specified section node into the collection of child nodes. Also sets its parent node to this one.
        /// </summary>
        public void AddSection(SectionNode section)
        {
            this._sections.Add(section);
            section.Parent = this;
        }

        /// <summary>
        /// Gets the collection of subsection child nodes for this section
        /// </summary>
        public IEnumerable<SectionNode> Sections
        {
            get { foreach (SectionNode node in this._sections) yield return node; }
        }

        /// <summary>
        /// Gets the collection of page child nodes for this section
        /// </summary>
        public IEnumerable<PageNode> Pages
        {
            get { foreach (PageNode node in this._pages) yield return node; }
        }

        public int PagesCount
        {
            get { return this._pages.Count; }
        }

        public int SectionsCount
        {
            get { return this._sections.Count; }
        }

        /// <inheritdoc/>
        public override IEnumerable<TreeNode> EnumChildNodes()
        {
            foreach (SectionNode node in this._sections) yield return node;

            foreach (FileNode node in this._pages) yield return node;
        }

        public SectionNode FindSection(string name)
        {
            for (int i = 0; i < this._sections.Count; i++)
            {
                if (Utils.StrEquals(this._sections[i].Name, name)) return this._sections[i];
            }

            return null;
        }
    }
}
