/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using CilTools.Syntax;
using CilTools.SourceCode.Common;

namespace CilBrowser.Core.SyntaxModel.FoxPro
{
    public sealed class FoxTokenFactory : SyntaxFactory
    {
        private FoxTokenFactory() { }

        static readonly HashSet<string> keywords = new HashSet<string>(new string[] {
            "SET", "DO", "IF", "ELSE", "ENDIF", "DECLARE", "PROCEDURE", "SELECT", "FROM", "WHERE", "INTO",
            "GROUP", "ORDER", "BY", "CASE", "ENDCASE", "SCAN", "ENDSCAN", "NEXT", "SKIP", "GO", "USE", "REPLACE",
            "WITH", "IN", "WAIT", "PARAMETERS", "CLOSE", "TABLES", "ALL", "EXISTS", "TRY", "CATCH", "ENDTRY", "CREATE", "TABLE",
            "DROP", "INSERT", "DELETE", "AND", "OR", "VALUES"
        }, StringComparer.OrdinalIgnoreCase);

        public static readonly FoxTokenFactory Value = new FoxTokenFactory();
        
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
            else if (token.StartsWith("&&"))
            {
                return TokenKind.Comment;
            }
            else if (token[0] == '*' && token.Length >= 2)
            {
                return TokenKind.Comment;
            }
            else if (token[0] == '\'' || token[0] == '"')
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
