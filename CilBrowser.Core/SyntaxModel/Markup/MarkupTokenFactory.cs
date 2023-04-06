/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using CilTools.Syntax;
using CilTools.SourceCode.Common;

namespace CilBrowser.Core.SyntaxModel.Markup
{
    /// <summary>
    /// Produces <see cref="SourceToken"/> instances for markup languages (XML, HTML)
    /// </summary>
    public sealed class MarkupTokenFactory : SyntaxFactory
    {
        private MarkupTokenFactory() { }

        public static readonly MarkupTokenFactory Value = new MarkupTokenFactory();

        static TokenKind GetKindCommon(string token)
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

        static TokenKind GetKind(string token)
        {
            if (token.Length == 0) return TokenKind.Unknown;

            if (char.IsLetter(token[0]) || token[0] == '_')
            {
                return TokenKind.Name;
            }
            else if (token.StartsWith("<!--"))
            {
                return TokenKind.Comment;
            }
            else return GetKindCommon(token);
        }

        public override SyntaxNode CreateNode(string content, string leadingWhitespace, string trailingWhitespace)
        {
            TokenKind kind = GetKind(content);
            return new SourceToken(content, kind, leadingWhitespace, trailingWhitespace);
        }
    }
}
