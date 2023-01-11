/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using CilBrowser.Core.SyntaxModel.Markup;
using CilTools.Syntax;
using CilView.Core.Syntax;
using CilView.SourceCode;
using CilView.SourceCode.Common;

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
                return TokenParser.ParseTokens(content, TokenParser.GetDefinitions(ext),
                    NullClassifier.Value);
            }
            else if (s_markupExts.Contains(ext))
            {
                return TokenParser.ParseTokens(content, s_markupDefinitions, MarkupClassifier.Value);
            }
            else
            {
                return TokenParser.ParseTokens(content, TokenParser.GetDefinitions(ext),
                    TokenClassifier.Create(ext));
            }
        }
    }
}
