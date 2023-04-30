/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using CilTools.Syntax;
using CilTools.SourceCode.Common;

namespace CilBrowser.Core.SyntaxModel.PowerShell
{
    /// <summary>
    /// Produces <see cref="SourceToken"/> instances for PowerShell scripting language
    /// </summary>
    public sealed class PsTokenFactory : SyntaxFactory
    {
        private PsTokenFactory() { }

        // https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_language_keywords
        static readonly HashSet<string> keywords = new HashSet<string>(new string[] {
            "begin", "break", "catch", "class", "continue", "data", "define", "do", "dynamicparam", "else", "elseif", "end",
            "enum", "exit", "filter", "finally", "for", "foreach", "from", "function", "hidden", "if", "in", "param", 
            "process", "return", "static", "switch", "throw", "trap", "try", "until", "using", "var", "while"
        }, StringComparer.OrdinalIgnoreCase);
        
        public static readonly PsTokenFactory Value = new PsTokenFactory();

        static TokenKind GetKind(string token)
        {
            if (token.Length == 0) return TokenKind.Unknown;

            if (keywords.Contains(token))
            {
                return TokenKind.Keyword;
            }
            else if (char.IsLetter(token[0]) || token[0] == '_')
            {
                return TokenKind.Name;
            }
            else if (token[0] == '#')
            {
                return TokenKind.Comment;
            }
            else if (token.StartsWith("<#", StringComparison.Ordinal))
            {
                return TokenKind.MultilineComment;
            }
            else if (token[0] == '\'')
            {
                return TokenKind.SingleQuotLiteral;
            }
            else if (token[0] == '"')
            {
                return TokenKind.DoubleQuotLiteral;
            }
            else if (char.IsPunctuation(token[0]) || char.IsSymbol(token[0]))
            {
                return TokenKind.Punctuation;
            }
            else if (char.IsDigit(token[0]))
            {
                return TokenKind.NumericLiteral;
            }
            else
            {
                return TokenKind.Unknown;
            }
        }

        public override SyntaxNode CreateNode(string content, string leadingWhitespace, string trailingWhitespace)
        {
            TokenKind kind = GetKind(content);
            return new SourceToken(content, kind, leadingWhitespace, trailingWhitespace);
        }
    }
}
