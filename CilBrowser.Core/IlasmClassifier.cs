/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2022,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using CilTools.Syntax;
using CilView.Core.Syntax;
using CilView.SourceCode;

namespace CilBrowser.Core
{
    class IlasmClassifier : TokenClassifier
    {
        public override TokenKind GetKind(string token)
        {
            SyntaxNode node = SyntaxFactory.CreateFromToken(token, string.Empty, string.Empty);

            if (node is KeywordSyntax) return TokenKind.Keyword;
            else if (node is IdentifierSyntax) return TokenKind.Name;
            else if (node is CommentSyntax) return TokenKind.Comment;
            else if (node is PunctuationSyntax) return TokenKind.Punctuation;
            else if (node is LiteralSyntax)
            {
                if (token.StartsWith("\"", StringComparison.Ordinal)) return TokenKind.DoubleQuotLiteral;
                else if (token.StartsWith("'", StringComparison.Ordinal)) return TokenKind.SingleQuotLiteral;
                else return TokenKind.NumericLiteral;
            }
            else return TokenKind.Unknown;
        }
    }
}
