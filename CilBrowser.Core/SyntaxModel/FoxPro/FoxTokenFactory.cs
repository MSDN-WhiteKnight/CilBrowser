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

        // https://learn.microsoft.com/en-us/sql/t-sql/language-elements/reserved-keywords-transact-sql
        static readonly HashSet<string> sqlKeywords = new HashSet<string>(new string[] {
            "ALL", "ALTER", "AND", "ANY", "AS", "ASC", "BETWEEN", "BY", "CREATE", "COUNT", "DATABASE", "DELETE", 
            "DESC", "DISTINCT", "DROP", "EXISTS", "FROM", "GROUP", "HAVING", "IN", "IS", "INDEX", "INSERT", "INTO", 
            "JOIN", "KEY", "LIKE", "NOT", "NULL", "ON", "OR", "ORDER", "SELECT", "TABLE", "VALUES", "VIEW", "UNION", 
            "UPDATE", "WHERE", "WITH"
        }, StringComparer.OrdinalIgnoreCase);

        // https://jeffpar.github.io/kbarchive/kb/130/Q130440/
        static readonly HashSet<string> foxKeywords = new HashSet<string>(new string[] {
            "ARRAY", "CALL", "CASE", "CATCH", "CLASS", "CLOSE", "CONTINUE", "CURSOR", "DECLARE", "DO", "ELSE", "ENDCASE", 
            "ENDIF", "ENDSCAN", "ENDTRY", "ENDWITH", "EXIT", "FORM", "FUNCTION", "IF", "GO", "GOTO", "NEXT", "PARAMETERS",
            "PROCEDURE", "REPLACE", "RETURN", "SCAN", "SET", "SKIP", "STORE", "TABLES", "TO", "TRY", "USE", "WAIT"
        }, StringComparer.OrdinalIgnoreCase);

        public static readonly FoxTokenFactory Value = new FoxTokenFactory();
        
        static TokenKind GetKind(string token)
        {
            if (token.Length == 0) return TokenKind.Unknown;

            if (sqlKeywords.Contains(token) || foxKeywords.Contains(token))
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
