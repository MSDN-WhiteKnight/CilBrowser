/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2022,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using CilBrowser.Core.SyntaxModel;
using CilTools.BytecodeAnalysis;
using CilTools.Syntax;
using CilView.Core.Syntax;
using CilView.SourceCode;
using CilView.SourceCode.Common;

namespace CilBrowser.Core
{
    /// <summary>
    /// Visualizes CIL or source text as HTML in the context of the specified assembly
    /// </summary>
    public class HtmlGenerator
    {
        Assembly _ass; //could be null when unknown
        string _nsFilter; //could be empty string when there's no namespace filter
        string _customFooter; //could be empty string when there's no custom footer

        const string Footer = "Generated by <a href=\"https://github.com/MSDN-WhiteKnight/CilBrowser\">CIL Browser</a>";
        const string GlobalStyles = ".memberid { color: rgb(43, 145, 175); text-decoration: none; }";
        
        public HtmlGenerator()
        {
            this._ass = null;
            this._nsFilter = string.Empty;
            this._customFooter = string.Empty;
        }

        public HtmlGenerator(Assembly ass)
        {
            this._ass = ass;
            this._nsFilter = string.Empty;
            this._customFooter = string.Empty;
        }

        public HtmlGenerator(Assembly ass, string ns, string footer)
        {
            if (ns == null) ns = string.Empty;
            if (footer == null) footer = string.Empty;

            this._ass = ass;
            this._nsFilter = ns;
            this._customFooter = footer;
        }

        public string CustomFooter
        {
            get { return this._customFooter; }
            set { this._customFooter = value; }
        }
        
        static bool IsMethodInAssembly(MethodBase mb, Assembly ass)
        {
            if (mb == null || ass == null) return false;

            Type tDecl = mb.DeclaringType;

            return Utils.IsTypeInAssembly(tDecl, ass);
        }

        static bool IsNameToken(SyntaxNode node)
        {
            SourceToken tok = node as SourceToken;

            if (tok == null) return false;
            else return tok.Kind == TokenKind.Name;
        }

        void VisualizeNode(SyntaxNode node, HtmlBuilder target)
        {
            SyntaxNode[] children = node.GetChildNodes();
            HtmlAttribute[] attrs;

            if (children.Length > 0)
            {
                if (node is SyntaxElement)
                {
                    SyntaxElement elem = (SyntaxElement)node;

                    if (elem.Kind == SyntaxKind.TagStart || elem.Kind == SyntaxKind.TagEnd)
                    {
                        SyntaxNode[] tokens = elem.GetChildNodes();
                        int nameIndex;

                        if (elem.Kind == SyntaxKind.TagStart) nameIndex = 1;
                        else nameIndex = 2;

                        for (int i = 0; i < tokens.Length; i++)
                        {
                            if (i == nameIndex && IsNameToken(tokens[i]))
                            {
                                //highlighted tag name
                                target.WriteTag("span", tokens[i].ToString(),
                                    HtmlBuilder.OneAttribute("style", "color: blue;"));
                            }
                            else
                            {
                                //rest of the content
                                VisualizeNode(tokens[i], target);
                            }
                        }
                    }
                    else
                    {
                        foreach (SyntaxNode child in children) VisualizeNode(child, target);
                    }
                }
                else
                {
                    //other syntax nodes
                    foreach (SyntaxNode child in children) VisualizeNode(child, target);
                }
            }
            else if (node is KeywordSyntax)
            {
                KeywordSyntax ks = (KeywordSyntax)node;

                if (ks.Kind == KeywordKind.DirectiveName)
                {
                    attrs = new HtmlAttribute[1];
                    attrs[0] = new HtmlAttribute("style", "color: magenta;");
                }
                else if (ks.Kind == KeywordKind.Other)
                {
                    attrs = new HtmlAttribute[1];
                    attrs[0] = new HtmlAttribute("style", "color: blue;");
                }
                else attrs = new HtmlAttribute[0];

                target.WriteTag("span", node.ToString(), attrs);
            }
            else if (node is IdentifierSyntax)
            {
                IdentifierSyntax ids = (IdentifierSyntax)node;
                string tagName;
                List<HtmlAttribute> attrList = new List<HtmlAttribute>(5);

                if (ids.IsMemberName)
                {
                    if (ids.TargetMember is Type)
                    {
                        Type targetType = (Type)ids.TargetMember;

                        if (Utils.IsTypeInAssembly(targetType, this._ass) &&
                            Utils.IsMatchingNamespaceFilter(targetType, this._nsFilter))
                        {
                            //hyperlink for types in the same assembly
                            tagName = "a";
                            attrList.Add(new HtmlAttribute("href", GenerateTypeFileName(targetType)));
                        }
                        else
                        {
                            //plaintext name for other types
                            tagName = "span";
                        }
                    }
                    else if (ids.TargetMember is MethodBase)
                    {
                        MethodBase mb = (MethodBase)ids.TargetMember;

                        if (node.Parent is DirectiveSyntax &&
                            Utils.StrEquals(((DirectiveSyntax)node.Parent).Name, "method"))
                        {
                            //anchor when in method signature
                            tagName = "a";
                            attrList.Add(new HtmlAttribute("name", GenerateMethodAnchor(mb)));
                            attrList.Add(new HtmlAttribute("href", GenerateMethodURL(mb)));
                        }
                        else if (IsMethodInAssembly(mb, this._ass) &&
                            Utils.IsMatchingNamespaceFilter(mb.DeclaringType, this._nsFilter))
                        {
                            //hyperlink for methods in the same assembly
                            tagName = "a";
                            attrList.Add(new HtmlAttribute("href", GenerateMethodURL(mb)));
                        }
                        else
                        {
                            //plaintext name for other methods
                            tagName = "span";
                        }
                    }
                    else tagName = "span";

                    attrList.Add(new HtmlAttribute("class", "memberid"));
                }
                else tagName = "span";
                
                target.WriteTag(tagName, node.ToString(), attrList.ToArray());
            }
            else if (node is LiteralSyntax)
            {
                LiteralSyntax ls = (LiteralSyntax)node;

                if (ls.Value is string)
                {
                    attrs = new HtmlAttribute[1];
                    attrs[0] = new HtmlAttribute("style", "color: red;");
                }
                else attrs = new HtmlAttribute[0];

                target.WriteTag("span", node.ToString(), attrs);
            }
            else if (node is CommentSyntax)
            {
                attrs = new HtmlAttribute[1];
                attrs[0] = new HtmlAttribute("style", "color: green;");
                target.WriteTag("span", node.ToString(), attrs);
            }
            else if (node is SourceToken)
            {
                SourceToken token = (SourceToken)node;
                
                switch (token.Kind)
                {
                    case TokenKind.Keyword:
                        target.WriteTag("span", node.ToString(), HtmlBuilder.OneAttribute("style", "color: blue;"));
                        break;
                    case TokenKind.DoubleQuotLiteral:
                        target.WriteTag("span", node.ToString(), HtmlBuilder.OneAttribute("style", "color: red;"));
                        break;
                    case TokenKind.SingleQuotLiteral:
                        target.WriteTag("span", node.ToString(), HtmlBuilder.OneAttribute("style", "color: red;"));
                        break;
                    case TokenKind.Comment:
                        target.WriteTag("span", node.ToString(), HtmlBuilder.OneAttribute("style", "color: green;"));
                        break;
                    case TokenKind.MultilineComment:
                        target.WriteTag("span", node.ToString(), HtmlBuilder.OneAttribute("style", "color: green;"));
                        break;
                    default:
                        target.WriteEscaped(node.ToString());
                        break;
                }
            }
            else
            {
                target.WriteEscaped(node.ToString());
            }
        }

        void WriteHeaderHTML(HtmlBuilder target)
        {
            target.StartParagraph();
            target.WriteEscaped(".NET CIL Browser");
            target.WriteRaw("&nbsp;-&nbsp;");

            if (this._ass != null)
            {
                target.WriteHyperlink("index.html", this._ass.GetName().Name);
            }
            else
            {
                target.WriteHyperlink("index.html", "Back to table of contents");
            }

            target.EndParagraph();
            target.WriteRaw(Environment.NewLine);
        }

        void WriteLayoutStart(HtmlBuilder target, string title, string navigation)
        {
            target.StartDocument(title, GlobalStyles);
            this.WriteHeaderHTML(target);
            target.WriteTag("h2", title);

            target.WriteTagStart("table", new HtmlAttribute[] {
                new HtmlAttribute("width", "100%"), new HtmlAttribute("cellpadding", "5"),
                new HtmlAttribute("cellspacing", "5")
            });

            target.WriteTagStart("tr");

            if (!string.IsNullOrEmpty(navigation))
            {
                target.WriteTagStart("td", new HtmlAttribute[] {
                    new HtmlAttribute("width", "200"), new HtmlAttribute("valign", "top"),
                    new HtmlAttribute("style", "border: thin solid;"),
                });

                target.WriteRaw(navigation);
                target.WriteTagEnd("td");
                target.WriteRaw(Environment.NewLine);
            }

            target.WriteTagStart("td", HtmlBuilder.OneAttribute("valign", "top"));
        }

        void WriteLayoutEnd(HtmlBuilder target)
        {
            target.WriteTagEnd("td");
            target.WriteTagEnd("tr");
            target.WriteTagEnd("table");

            target.StartParagraph();
            target.WriteHyperlink("index.html", "Back to table of contents");
            target.EndParagraph();

            WriteFooter(target);
            target.EndDocument();
        }
        
        public void VisualizeSyntaxNodes(IEnumerable<SyntaxNode> nodes, HtmlBuilder target)
        {
            target.WriteTagStart("pre", HtmlBuilder.OneAttribute("style", "white-space: pre-wrap;"));
            target.WriteTagStart("code");

            //content
            foreach (SyntaxNode node in nodes) this.VisualizeNode(node, target);

            target.WriteTagEnd("code");
            target.WriteTagEnd("pre");
        }

        public static string VisualizeMethod(MethodBase mb)
        {
            CilGraph gr = CilGraph.Create(mb);
            SyntaxNode[] nodes = new SyntaxNode[] { gr.ToSyntaxTree() };
            HtmlGenerator generator = new HtmlGenerator();
            StringBuilder sb = new StringBuilder(5000);
            HtmlBuilder builder = new HtmlBuilder(sb);

            generator.WriteLayoutStart(builder, "Method: " + mb.Name, string.Empty);
            generator.VisualizeSyntaxNodes(nodes, builder);
            generator.WriteLayoutEnd(builder);

            return sb.ToString();
        }

        static string VisualizeNavigationPanel(Type t, Dictionary<string, List<Type>> typeMap)
        {
            StringBuilder sb = new StringBuilder(1000);
            HtmlBuilder html = new HtmlBuilder(sb);            
            string ns = t.Namespace;
            List<Type> types;

            if (ns == null) ns = string.Empty;

            if (typeMap.ContainsKey(ns)) types = typeMap[ns];
            else types = new List<Type>();

            if (ns.Length > 0)
            {
                html.WriteParagraph("Types in " + ns + " namespace:");
            }
            else
            {
                html.WriteParagraph("Types without namespace:");
            }

            //list of types
            for (int i = 0; i < types.Count; i++)
            {
                html.StartParagraph();

                if (Utils.StrEquals(types[i].FullName, t.FullName))
                {
                    html.WriteTag("b", types[i].Name);
                }
                else
                {
                    html.WriteHyperlink(GenerateTypeFileName(types[i]), types[i].Name);
                }

                html.EndParagraph();
            }

            return sb.ToString();
        }
        
        public string VisualizeType(Type t, Dictionary<string, List<Type>> typeMap)
        {
            SyntaxNode[] nodes = SyntaxNode.GetTypeDefSyntax(t, true, new DisassemblerParams()).ToArray();

            if (nodes.Length == 0) return string.Empty;

            if (nodes.Length == 1)
            {
                if (string.IsNullOrWhiteSpace(nodes[0].ToString())) return string.Empty;
            }

            StringBuilder sb = new StringBuilder(5000);
            HtmlBuilder html = new HtmlBuilder(sb);

            string navigation = VisualizeNavigationPanel(t, typeMap);
            this.WriteLayoutStart(html, "Type: " + t.FullName, navigation);
            this.VisualizeSyntaxNodes(nodes, html);
            WriteLayoutEnd(html);

            return sb.ToString();
        }

        public string VisualizeAssemblyManifest(Assembly ass)
        {
            IEnumerable<SyntaxNode> nodes = Disassembler.GetAssemblyManifestSyntaxNodes(ass);
            StringBuilder sb = new StringBuilder(5000);
            HtmlBuilder html = new HtmlBuilder(sb);

            this.WriteLayoutStart(html, "Assembly: " + ass.GetName().Name, string.Empty);
            this.VisualizeSyntaxNodes(nodes, html);
            WriteLayoutEnd(html);

            return sb.ToString();
        }
        
        public string VisualizeSourceFile(string content, string filename, string navigation, string sourceControlUrl)
        {
            string ext = Path.GetExtension(filename);
            StringBuilder sb = new StringBuilder(5000);
            HtmlBuilder html = new HtmlBuilder(sb);

            //convert source text into tokens
            SyntaxNode[] nodes = SourceParser.Parse(content, ext);

            //convert tokens to HTML
            this.WriteLayoutStart(html, "Source file: " + filename, navigation);
            this.VisualizeSyntaxNodes(nodes, html);

            if (!string.IsNullOrEmpty(sourceControlUrl))
            {
                html.WriteHyperlink(Utils.UrlAppend(sourceControlUrl, filename), "View in source control");
            }

            WriteLayoutEnd(html);

            return sb.ToString();
        }

        public static string GenerateTypeFileName(Type t)
        {
            return ((uint)t.MetadataToken).ToString("X", CultureInfo.InvariantCulture) + ".html";
        }

        static string GenerateMethodAnchor(MethodBase mb)
        {
            return "M"+((uint)mb.MetadataToken).ToString("X", CultureInfo.InvariantCulture);
        }

        static string GenerateMethodURL(MethodBase mb)
        {
            if (mb.DeclaringType == null) return string.Empty;

            return GenerateTypeFileName(mb.DeclaringType) + "#" + GenerateMethodAnchor(mb);
        }
        
        public void WriteFooter(HtmlBuilder target)
        {
            target.WriteTag("hr", string.Empty);

            if (!string.IsNullOrEmpty(this._customFooter))
            {
                target.StartParagraph();
                target.WriteRaw(this._customFooter);
                target.EndParagraph();
            }

            target.StartParagraph();
            target.WriteRaw(Footer);
            target.EndParagraph();
        }
        
        public static string VisualizeException(Exception ex)
        {
            StringBuilder sb = new StringBuilder(500);
            sb.Append("<pre>");
            sb.Append(WebUtility.HtmlEncode(ex.GetType().ToString()));
            sb.Append(": ");
            sb.Append(WebUtility.HtmlEncode(ex.Message));
            sb.Append("</pre>");
            return sb.ToString();
        }

        public static void WriteTocStart(HtmlBuilder toc, AssemblyName an)
        {
            toc.StartDocument(".NET CIL Browser - " + an.Name, GlobalStyles);
            toc.WriteParagraph(".NET CIL Browser");
            toc.WriteTag("h1", an.Name);
            toc.WriteRaw(Environment.NewLine);
            toc.StartParagraph();
            toc.WriteHyperlink("assembly.html", "(Assembly manifest)");
            toc.EndParagraph();
            toc.WriteParagraph("Types in assembly: ");
        }

        public static void WriteTocStart(HtmlBuilder toc, string dirName)
        {
            toc.StartDocument(".NET CIL Browser - " + dirName, GlobalStyles);
            toc.WriteParagraph(".NET CIL Browser");
            toc.WriteTag("h1", "Source directory: " + dirName);
            toc.WriteRaw(Environment.NewLine);
        }
    }
}
