/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using CilTools.Syntax;
using CilTools.SourceCode.Common;

namespace CilBrowser.Core.SyntaxModel.Java
{
    public sealed class JavaTokenFactory : SyntaxFactory
    {
        private JavaTokenFactory() { }

        // https://docs.oracle.com/javase/tutorial/java/nutsandbolts/_keywords.html
        static readonly HashSet<string> keywords = new HashSet<string>(new string[] {
            "abstract", "assert", "boolean", "break", "byte", "case", "catch", "char", "class", "continue", 
            "default", "do", "double", "else", "enum", "extends", "final", "finally", "float", "for", 
            "if", "implements", "import", "instanceof", "int", "interface", "long", "native", "new", 
            "package", "private", "protected", "public", "return", 
            "short", "static", "strictfp", "super", "switch", "synchronized",
            "this", "throw", "throws", "transient", "try", "void", "volatile", "while",
            "true", "false", "null"
        });

        public static readonly JavaTokenFactory Value = new JavaTokenFactory();

        static TokenKind GetKind(string token)
        {
            if (token.Length == 0) return TokenKind.Unknown;

            if (char.IsLetter(token[0]) || token[0] == '_')
            {
                if (keywords.Contains(token)) return TokenKind.Keyword;
                else return TokenKind.Name;
            }
            else
            {
                return SourceParser.GetKindCommon(token);
            }
        }

        public override SyntaxNode CreateNode(string content, string leadingWhitespace, string trailingWhitespace)
        {
            TokenKind kind = GetKind(content);
            return new SourceToken(content, kind, leadingWhitespace, trailingWhitespace);
        }
    }
}
