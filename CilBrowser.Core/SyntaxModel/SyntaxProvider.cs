/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using CilTools.SourceCode.Common;
using CilTools.Syntax;

namespace CilBrowser.Core.SyntaxModel
{
    /// <summary>
    /// Provides a base class for classes that convert a source text into a collection of syntax nodes. It is used
    /// to implement syntax highlighting for different programming languages.
    /// </summary>
    abstract class SyntaxProvider
    {
        /// <summary>
        /// Converts a specified source text into an array of syntax nodes. The output nodes could be <see cref="SourceToken"/>
        /// instances (if this provider supports only tokenization) or some high-level objects consisting of tokens if
        /// it implements a syntax parser.
        /// </summary>
        public abstract SyntaxNode[] GetNodes(string sourceText);
    }
}
