/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using CilTools.Reflection;

namespace CilBrowser.Core
{
    /// <summary>
    /// Provides a server that dynamically generates HTML for a disassembled IL and returns it via HTTP
    /// </summary>
    public sealed class Server : ServerBase
    {
        Assembly _ass;        
        HtmlGenerator _gen;        
        Dictionary<string, List<Type>> _typeMap;
        
        public Server(Assembly ass, string urlHost, string urlPrefix) : base(urlHost, urlPrefix)
        {
            this._ass = ass;
            this._gen = new HtmlGenerator(ass);

            Type[] types = ass.GetTypes();
            this._typeMap = Utils.GroupByNamespace(types);
        }
        
        int ResolveTokenFromUrl(string url)
        {
            string urlPart = StripURL(url);

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
            // Table of contents
            response.ContentType = "text/html; charset=utf-8";
            string[] namespaces = this._typeMap.Keys.ToArray();
            Array.Sort(namespaces);

            // Write the response to output stream.
            StreamWriter wr = new StreamWriter(response.OutputStream);
            HtmlBuilder toc = new HtmlBuilder(wr);

            using (wr)
            {
                HtmlGenerator.WriteTocStart(toc, this._ass);

                for (int i = 0; i < namespaces.Length; i++)
                {
                    string nsText = namespaces[i];
                    List<Type> nsTypes = this._typeMap[namespaces[i]];

                    if (nsTypes.Count == 0) continue;

                    if (string.IsNullOrEmpty(nsText)) nsText = "(No namespace)";
                    else nsText = nsText + " namespace";

                    toc.WriteTag("h2", nsText);

                    for (int j = 0; j < nsTypes.Count; j++)
                    {
                        string fname = HtmlGenerator.GenerateTypeFileName(nsTypes[j]);

                        //TOC entry
                        toc.StartParagraph();
                        toc.WriteHyperlink(fname, nsTypes[j].FullName);
                        toc.EndParagraph();
                    }
                }//end for

                this._gen.WriteFooter(toc);
                toc.EndDocument();
            }//end using
        }

        protected override void RenderPage(string url, HttpListenerResponse response)
        {
            string content;

            if (Utils.StrEquals(url, this._urlPrefix + "assembly.html"))
            {
                // Assembly manifest
                content = this._gen.VisualizeAssemblyManifest(this._ass);
                SendHtmlResponse(response, content);
                this.AddToCache(url, content);
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

            content = this._gen.VisualizeType(t, this._typeMap);
            SendHtmlResponse(response, content);
            this.AddToCache(url, content);
        }
    }
}
