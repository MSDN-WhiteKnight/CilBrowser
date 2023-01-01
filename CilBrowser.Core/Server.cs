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
using System.Threading;
using CilTools.Reflection;

namespace CilBrowser.Core
{
    /// <summary>
    /// Provides a server that dynamically generates HTML for a disassembled IL and returns it via HTTP
    /// </summary>
    public class Server
    {
        Assembly _ass;
        HtmlGenerator _gen;
        Dictionary<string, List<Type>> _typeMap;
        Dictionary<string, string> _cache = new Dictionary<string, string>(CacheCapacity);
        
        const string UrlHost = "http://localhost:8080";
        const string UrlPrefix = "/CilBrowser/";
        const int CacheCapacity = 200;

        public Server(Assembly ass)
        {
            this._ass = ass;
            this._gen = new HtmlGenerator(ass);

            Type[] types = ass.GetTypes();
            this._typeMap = Utils.GroupByNamespace(types);
        }

        void AddToCache(string url, string content)
        {
            if (string.IsNullOrEmpty(content) || content.Length < 20) return;

            if (this._cache.Count >= CacheCapacity) this._cache.Clear();
            
            this._cache[url] = content;
        }

        string TryGetFromCache(string url)
        {
            string ret;

            if (this._cache.TryGetValue(url, out ret)) return ret;
            else return string.Empty;
        }

        static int ResolveTokenFromUrl(string url)
        {
            int index = UrlPrefix.Length;

            if (index >= url.Length) return 0;

            //Strip prefix
            string urlPart = url.Substring(index);
            index = urlPart.IndexOf('.');

            if (index < 0) index = urlPart.Length;

            //Strip extension
            urlPart = urlPart.Substring(0, index);
            int ret;

            //Parse hexadecimal metadata token from URL
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

        static void SendHtmlResponse(HttpListenerResponse response, string content)
        {
            // Get a response stream and write the response to it.
            response.ContentType = "text/html";
            Stream output = response.OutputStream;
            StreamWriter wr = new StreamWriter(output);

            using (wr)
            {
                wr.Write(content);
            }

            response.Close();
        }

        static void SendErrorResponse(HttpListenerResponse response, int code, string status)
        {
            response.StatusCode = code;
            response.StatusDescription = status;
            response.Close();
        }

        public void Run()
        {
            // Create a listener.
            HttpListener listener = new HttpListener();
            StreamWriter wr;

            // Add the prefixes.
            listener.Prefixes.Add(UrlHost + UrlPrefix);
            listener.Start();
            Console.WriteLine("Assembly: " + this._ass.GetName().Name);
            Console.WriteLine("Displaying web UI on " + UrlHost + UrlPrefix);

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                string url = request.RawUrl;

                HttpListenerResponse response = context.Response;

                // Construct a response.

                if (!url.StartsWith(UrlPrefix))
                {
                    //вернуть ошибку при неверном URL
                    SendErrorResponse(response, 404, "Not found");
                    continue;
                }

                response.Headers.Add("Expires: Tue, 01 Jul 2000 06:00:00 GMT");
                response.Headers.Add("Cache-Control: max-age=0, no-cache, must-revalidate");

                // Try from cache
                string cached = this.TryGetFromCache(url);

                if (cached.Length > 0)
                {
                    SendHtmlResponse(response, cached);
                    continue;
                }
                
                if (Utils.StrEquals(url, UrlPrefix) || Utils.StrEquals(url, UrlPrefix + "index.html"))
                {
                    // Table of contents
                    response.ContentType = "text/html";
                    string[] namespaces = this._typeMap.Keys.ToArray();
                    Array.Sort(namespaces);

                    // Get a response stream and write the response to it.
                    wr = new StreamWriter(response.OutputStream);
                    HtmlBuilder toc = new HtmlBuilder(wr);

                    using (wr)
                    {
                        HtmlGenerator.WriteTocStart(toc, this._ass.GetName());

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

                        HtmlGenerator.WriteFooter(toc);
                        toc.EndDocument();
                    }//end using

                    response.Close();
                    continue;
                }

                string content;

                if (Utils.StrEquals(url, UrlPrefix + "assembly.html"))
                {
                    // Assembly manifest
                    content = this._gen.VisualizeAssemblyManifest(this._ass);
                    SendHtmlResponse(response, content);
                    this.AddToCache(url, content);
                    continue;
                }

                // Type
                int metadataToken = ResolveTokenFromUrl(url);

                if (metadataToken == 0)
                {
                    //вернуть ошибку при неверном URL
                    SendErrorResponse(response, 404, "Not found");
                    continue;
                }

                Type t = ResolveType(this._ass, metadataToken);

                if (t == null)
                {
                    //вернуть ошибку при неверном URL
                    SendErrorResponse(response, 404, "Not found");
                    continue;
                }

                content = this._gen.VisualizeType(t, this._typeMap);
                SendHtmlResponse(response, content);
                this.AddToCache(url, content);
            }//end while
        }

        public void RunInBackground()
        {
            Thread th = new Thread(Run);
            th.IsBackground = true;
            th.Start();
        }
    }
}
