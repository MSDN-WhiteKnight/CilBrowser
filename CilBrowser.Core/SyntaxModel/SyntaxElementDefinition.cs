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
    public class SyntaxElementDefinition
    {
        SyntaxFilter[] start;
        SyntaxFilter[] end;
        SyntaxKind kind;

        public SyntaxElementDefinition(IEnumerable<SyntaxFilter> startSequence,
            IEnumerable<SyntaxFilter> endSequence,
            SyntaxKind syntaxKind)
        {
            this.start = startSequence.ToArray();
            this.end = endSequence.ToArray();
            this.kind = syntaxKind;
        }

        public IEnumerable<SyntaxFilter> Start
        {
            get
            {
                for (int i = 0; i < start.Length; i++)
                {
                    yield return start[i];
                }
            }
        }

        public IEnumerable<SyntaxFilter> End
        {
            get
            {
                for (int i = 0; i < end.Length; i++)
                {
                    yield return end[i];
                }
            }
        }

        public SyntaxKind Kind
        {
            get { return this.kind; }
        }
        
        static bool IsMatch(SourceToken[] arr, int index, SyntaxFilter[] filters)
        {
            if (index + filters.Length >= arr.Length) return false;
            if (index < 0) return false;

            for (int i = 0; i < filters.Length; i++)
            {
                SourceToken tok = arr[index + i];
                SyntaxFilter filter = filters[i];

                if (!filter.IsMatch(tok)) return false;
            }

            return true;
        }

        public bool HasStart(SourceToken[] arr, int index)
        {
            return IsMatch(arr, index, this.start);
        }

        public bool HasEnd(SourceToken[] arr, int index)
        {
            return IsMatch(arr, index - this.end.Length, this.end);
        }

        public static SyntaxElementDefinition[] GetMarkupDefs()
        {
            //HTML/XML syntax element definitions
            SyntaxElementDefinition tagStart = new SyntaxElementDefinition(
            startSequence: new SyntaxFilter[] {
                new SyntaxFilter(TokenKind.Punctuation, "<"),
                new SyntaxFilter(TokenKind.Name, null)
            },
            endSequence: new SyntaxFilter[] {
                new SyntaxFilter(TokenKind.Punctuation, ">")
            },
            syntaxKind: SyntaxKind.TagStart);

            SyntaxElementDefinition tagEnd = new SyntaxElementDefinition(
            startSequence: new SyntaxFilter[] {
                new SyntaxFilter(TokenKind.Punctuation, "<"),
                new SyntaxFilter(TokenKind.Punctuation, "/"),
                new SyntaxFilter(TokenKind.Name, null)
            },
            endSequence: new SyntaxFilter[] {
                new SyntaxFilter(TokenKind.Punctuation, ">")
            },
            syntaxKind: SyntaxKind.TagEnd);

            return new SyntaxElementDefinition[] { tagStart, tagEnd };
        }
    }

    public class SyntaxFilter
    {
        public TokenKind? Kind { get; set; }
        public string Content { get; set; }

        public SyntaxFilter(TokenKind? kind, string content)
        {
            this.Kind = kind;
            this.Content = content;
        }

        public bool IsMatch(SourceToken token)
        {
            bool ret = true;

            if (this.Kind.HasValue) ret &= token.Kind == this.Kind.Value;

            if (this.Content != null) ret &= Utils.StrEquals(token.Content, this.Content);

            return ret;
        }
    }
}
