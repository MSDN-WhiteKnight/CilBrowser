/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CilBrowser.Core.SyntaxModel.FoxPro;
using CilBrowser.Core.SyntaxModel.JavaScript;
using CilBrowser.Core.SyntaxModel.Markup;
using CilBrowser.Core.SyntaxModel.PowerShell;
using CilTools.SourceCode.Common;
using CilTools.Syntax;
using CilTools.Syntax.Tokens;

namespace CilBrowser.Core.SyntaxModel
{
    public static class SourceParser
    {
        static readonly SyntaxTokenDefinition[] s_markupDefinitions = new SyntaxTokenDefinition[] {
            new CommonNameToken(), new XmlCommentToken(), new PunctuationToken(), new WhitespaceToken(),
            new NumericLiteralToken(), new DoubleQuotLiteralToken()
        };

        static readonly SyntaxTokenDefinition[] s_foxDefinitions = new SyntaxTokenDefinition[] {
            new CommonNameToken(), new FoxCommentToken(), new FoxTextLiteralToken(), new PunctuationToken(),
            new WhitespaceToken(),new NumericLiteralToken()
        };

        static readonly SyntaxTokenDefinition[] s_psDefinitions = new SyntaxTokenDefinition[] {
            new CommonNameToken(), new PsCommentToken(), new PsMultilineCommentToken(), new PsTextLiteralToken(), 
            new PunctuationToken(), new WhitespaceToken(),new NumericLiteralToken()
        };

        static readonly SyntaxTokenDefinition[] s_jsDefinitions = new SyntaxTokenDefinition[] {
            new CommonNameToken(), new JsRegexLiteralToken(), new PunctuationToken(), new WhitespaceToken(), 
            new NumericLiteralToken(), new DoubleQuotLiteralToken(), new SingleQuotLiteralToken(), new CommentToken(),
            new MultilineCommentToken()
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
                    UnknownTokenFactory.Value);
            }
            else if (s_markupExts.Contains(ext))
            {
                SourceToken[] tokens = ParseXmlTokens(content);
                return SyntaxElementReader.ParseElements(tokens, SyntaxElementDefinition.GetMarkupDefs());
            }
            else if (Utils.StrEquals(ext, ".ps1"))
            {
                return SyntaxReader.ReadAllNodes(content, s_psDefinitions, PsTokenFactory.Value);
            }
            else if (Utils.StrEquals(ext, ".js"))
            {
                return SyntaxReader.ReadAllNodes(content, s_jsDefinitions, JsTokenFactory.Value);
            }
            else if (Utils.StrEquals(ext, ".prg"))
            {
                return SyntaxReader.ReadAllNodes(content, s_foxDefinitions, FoxTokenFactory.Value);
            }
            else
            {
                return SyntaxReader.ReadAllNodes(content, SourceCodeUtils.GetTokenDefinitions(ext),
                    SourceCodeUtils.GetFactory(ext));
            }
        }

        internal static TokenKind GetKindCommon(string token)
        {
            //common logic for C-like languages
            //From: https://github.com/MSDN-WhiteKnight/CilTools/blob/master/CilTools.SourceCode/Common/SourceCodeUtils.cs
            if (token.Length == 0) return TokenKind.Unknown;

            if (char.IsDigit(token[0]))
            {
                return TokenKind.NumericLiteral;
            }
            else if (token[0] == '"')
            {
                if (token.Length < 2 || token[token.Length - 1] != '"')
                {
                    return TokenKind.Unknown;
                }
                else
                {
                    return TokenKind.DoubleQuotLiteral;
                }
            }
            else if (token[0] == '\'')
            {
                if (token.Length < 2 || token[token.Length - 1] != '\'')
                {
                    return TokenKind.Unknown;
                }
                else
                {
                    return TokenKind.SingleQuotLiteral;
                }
            }
            else if (token[0] == '/')
            {
                if (token.Length == 1)
                {
                    return TokenKind.Punctuation;
                }
                else if (token[1] == '*')
                {
                    if (!token.EndsWith("*/", StringComparison.Ordinal))
                    {
                        return TokenKind.Unknown;
                    }
                    else
                    {
                        return TokenKind.MultilineComment;
                    }
                }
                else if (token[1] == '/')
                {
                    return TokenKind.Comment;
                }
                else
                {
                    return TokenKind.Unknown;
                }
            }
            else if (char.IsPunctuation(token[0]) || char.IsSymbol(token[0]))
            {
                return TokenKind.Punctuation;
            }
            else
            {
                return TokenKind.Unknown;
            }
        }
    }
}
