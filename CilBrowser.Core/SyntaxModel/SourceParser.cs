/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CilBrowser.Core.SyntaxModel.Markup;
using CilTools.Syntax;
using CilTools.Syntax.Tokens;
using CilTools.SourceCode.Common;

namespace CilBrowser.Core.SyntaxModel
{
    public static class SourceParser
    {
        static readonly SyntaxTokenDefinition[] s_markupDefinitions = new SyntaxTokenDefinition[] {
            new CommonNameToken(), new XmlCommentToken(), new PunctuationToken(), new WhitespaceToken(),
            new NumericLiteralToken(), new DoubleQuotLiteralToken()
        };

        static readonly HashSet<string> s_markupExts = new HashSet<string>(new string[] {
            ".xml", ".csproj", ".vbproj", ".vcxproj", ".proj", ".ilproj", ".htm", ".html", ".config", ".xaml"
        });

        public static SourceToken[] ParseXmlTokens(string content)
        {
            return SyntaxReader.ReadAllNodes(content, s_markupDefinitions, MarkupTokenFactory.Value).
                Cast<SourceToken>().ToArray();
        }

        public static SyntaxNode[] Parse(string content, string ext)
        {
            ext = ext.ToLower();

            if (Utils.StrEquals(ext, ".il") || Utils.StrEquals(ext, ".cil"))
            {
                return SyntaxReader.ReadAllNodes(content);
            }
            else if (Utils.StrEquals(ext, ".txt") || Utils.StrEquals(ext, ".md"))
            {
                //disable syntax highlighting for plaintext files
                return SyntaxReader.ReadAllNodes(content, SourceCodeUtils.GetTokenDefinitions(ext),
                    NullClassifier.Value);
            }
            else if (s_markupExts.Contains(ext))
            {
                SourceToken[] tokens = ParseXmlTokens(content);
                return SyntaxElementReader.ParseElements(tokens, SyntaxElementDefinition.GetMarkupDefs());
            }
            else
            {
                return SyntaxReader.ReadAllNodes(content, SourceCodeUtils.GetTokenDefinitions(ext),
                    SourceCodeUtils.GetFactory(ext));
            }
        }
    }
}
