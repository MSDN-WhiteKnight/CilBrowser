/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text;
using CilBrowser.Core.Structure;
using CilTools.Reflection;

namespace CilBrowser.Core
{
    /// <summary>
    /// Provides a server that dynamically generates HTML for a disassembled IL and returns it via HTTP
    /// </summary>
    public sealed class AssemblyServer : ServerBase
    {
        Assembly _ass;        
        HtmlGenerator _gen;
        AssemblySectionNode _tree;
        
        public AssemblyServer(Assembly ass, string urlHost, string urlPrefix) : base(urlHost, urlPrefix)
        {
            this._ass = ass;
            this._gen = new HtmlGenerator(ass);
            this._tree = AssemblyIndexer.AssemblyToTree(ass, string.Empty);
        }
        
        int ResolveTokenFromUrl(string url)
        {
            string urlPart = StripURL(url);

            int index = urlPart.LastIndexOf('/');

            //If there're slashes, take the part after last slash
            if (index >= 0 && index < urlPart.Length - 1)
            {
                urlPart = urlPart.Substring(index + 1, urlPart.Length - (index + 1)).Trim();
            }
            
            //Parse hexadecimal metadata token from URL
            int ret;

            if (int.TryParse(urlPart, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ret))
            {
                return ret;
            }
            else
            {
                return 0;
            }
        }

        static Type ResolveType(Assembly ass, int metadataToken)
        {
            if (ass is ITokenResolver)
            {
                ITokenResolver resolver = (ITokenResolver)ass;
                return resolver.ResolveType(metadataToken);
            }
            else
            {
                return ass.ManifestModule.ResolveType(metadataToken);
            }
        }
        
        protected override void OnStart()
        {
            Console.WriteLine("Assembly: " + this._ass.GetName().Name);
        }

        protected override void RenderFrontPage(HttpListenerResponse response)
        {
            // Assembly ToC
            response.ContentType = "text/html; charset=utf-8";
            StreamWriter wr = new StreamWriter(response.OutputStream);
            
            using (wr)
            {
                this._tree.Render(this._gen, new CilBrowserOptions(), wr);
            }
        }

        string RenderNamespaceToc(string ns)
        {
            SectionNode node = this._tree.FindSection(ns);

            if (node == null) return string.Empty;
            
            return node.RenderToString(this._gen, new CilBrowserOptions());
        }

        void SendContent(string url, string content, HttpListenerResponse response)
        {
            if (string.IsNullOrEmpty(content))
            {
                SendErrorResponse(response, 404, "Not found");
                return;
            }

            SendHtmlResponse(response, content);
            this.AddToCache(url, content);
        }

        protected override void RenderPage(string url, HttpListenerResponse response)
        {
            string content;
            string relativePath;

            if (Utils.StrEquals(url, this._urlPrefix + "assembly.html"))
            {
                // Assembly manifest
                content = this._gen.VisualizeAssemblyManifest(this._ass);
                SendHtmlResponse(response, content);
                this.AddToCache(url, content);
                return;
            }
            else if (url.EndsWith("index.html", StringComparison.OrdinalIgnoreCase))
            {
                // Namespace ToC
                relativePath = StripURL(url);

                if (relativePath.Length < 6)
                {
                    SendErrorResponse(response, 404, "Not found");
                    return;
                }

                string ns = relativePath.Substring(0, relativePath.Length - 6);
                content = RenderNamespaceToc(ns);
                this.SendContent(url, content, response);
                return;
            }
            else if (url.Length > 0 && url[url.Length - 1] == '/')
            {
                // Namespace ToC
                relativePath = StripPrefix(url);
                
                if (relativePath.Length < 1)
                {
                    SendErrorResponse(response, 404, "Not found");
                    return;
                }
                
                string ns = relativePath.Substring(0, relativePath.Length - 1);
                content = RenderNamespaceToc(ns);
                this.SendContent(url, content, response);
                return;
            }

            // Type
            int metadataToken = ResolveTokenFromUrl(url);

            if (metadataToken == 0)
            {
                //вернуть ошибку при неверном URL
                SendErrorResponse(response, 404, "Not found");
                return;
            }

            Type t = ResolveType(this._ass, metadataToken);

            if (t == null)
            {
                //вернуть ошибку при неверном URL
                SendErrorResponse(response, 404, "Not found");
                return;
            }

            content = this._gen.VisualizeType(t, this._tree.TypeMap);
            SendHtmlResponse(response, content);
            this.AddToCache(url, content);
        }
    }
}
