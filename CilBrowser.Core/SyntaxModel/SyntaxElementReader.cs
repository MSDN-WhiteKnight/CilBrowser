/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CilTools.SourceCode.Common;
using CilTools.Syntax;

namespace CilBrowser.Core.SyntaxModel
{
    public class SyntaxElementReader
    {
        SourceToken[] source;
        int pos = 0;
        SyntaxElementDefinition[] definitions;

        public SyntaxElementReader(IEnumerable<SourceToken> src, IEnumerable<SyntaxElementDefinition> defs)
        {
            this.source = src.ToArray();
            this.definitions = defs.ToArray();
        }

        SourceToken Read()
        {
            if (pos >= source.Length) return null;

            SourceToken ret = source[pos];
            pos++;
            return ret;
        }

        public SyntaxElement ReadElement()
        {
            if (pos >= source.Length) return null;

            SyntaxElement ret = new SyntaxElement();
            SyntaxElementDefinition currentElement = null;

            for (int i = 0; i < definitions.Length; i++)
            {
                if (definitions[i].HasStart(source, pos))
                {
                    currentElement = definitions[i];
                    break;
                }
            }

            if (currentElement == null)
            {
                //unknown element

                while (true)
                {
                    SourceToken tok = this.Read();

                    if (tok == null) break;

                    ret.Add(tok);

                    for (int i = 0; i < definitions.Length; i++)
                    {
                        if (definitions[i].HasStart(source, pos)) return ret;
                    }
                }//end while
            }
            else
            {
                ret.Kind = currentElement.Kind;

                while (true)
                {
                    SourceToken tok = this.Read();

                    if (tok == null) break;

                    ret.Add(tok);

                    if (currentElement.HasEnd(source, pos))
                    {
                        break;
                    }
                }//end while
            }

            return ret;
        }

        public IEnumerable<SyntaxElement> ReadAll()
        {
            while (true)
            {
                SyntaxElement x = this.ReadElement();
                if (x == null) break;
                yield return x;
            }
        }

        public static SyntaxElement[] ParseElements(IEnumerable<SourceToken> src, IEnumerable<SyntaxElementDefinition> defs)
        {
            SyntaxElementReader reader = new SyntaxElementReader(src, defs);
            return reader.ReadAll().ToArray();
        }
    }
}
