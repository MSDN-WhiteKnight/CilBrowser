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
    public sealed class Server : IDisposable
    {
        Assembly _ass;
        string _urlHost;
        string _urlPrefix;
        HtmlGenerator _gen;
        HttpListener _listener;
        Dictionary<string, List<Type>> _typeMap;
        Dictionary<string, string> _cache = new Dictionary<string, string>(CacheCapacity);
        
        public const string DefaultUrlHost = "http://localhost:8080";
        public const string DefaultUrlPrefix = "/CilBrowser/";
        const int CacheCapacity = 200;

        public Server(Assembly ass, string urlHost, string urlPrefix)
        {
            this._ass = ass;
            this._urlHost = urlHost;
            this._urlPrefix = urlPrefix;
            this._gen = new HtmlGenerator(ass);

            Type[] types = ass.GetTypes();
            this._typeMap = Utils.GroupByNamespace(types);

            // Create a listener.
            this._listener = new HttpListener();
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

        int ResolveTokenFromUrl(string url)
        {
            int index = this._urlPrefix.Length;

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
            response.ContentType = "text/html; charset=utf-8";
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
            HttpListener listener = this._listener;
            StreamWriter wr;

            // Add the prefixes.
            listener.Prefixes.Add(this._urlHost + this._urlPrefix);
            listener.Start();
            Console.WriteLine("Assembly: " + this._ass.GetName().Name);
            Console.WriteLine("Displaying web UI on " + this._urlHost + this._urlPrefix);
            Console.WriteLine("Press E to stop server");

            while (true)
            {
                HttpListenerContext context = listener.GetContext();

                if (!listener.IsListening)
                {
                    Console.WriteLine("Server was stopped");
                    break;
                }

                HttpListenerRequest request = context.Request;
                string url = request.RawUrl;

                HttpListenerResponse response = context.Response;

                // Construct a response.

                if (!url.StartsWith(this._urlPrefix))
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
                
                if (Utils.StrEquals(url, this._urlPrefix) || Utils.StrEquals(url, this._urlPrefix + "index.html"))
                {
                    // Table of contents
                    response.ContentType = "text/html; charset=utf-8";
                    string[] namespaces = this._typeMap.Keys.ToArray();
                    Array.Sort(namespaces);

                    // Get a response stream and write the response to it.
                    wr = new StreamWriter(response.OutputStream);
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

                    response.Close();
                    continue;
                }

                string content;

                if (Utils.StrEquals(url, this._urlPrefix + "assembly.html"))
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

        public void Stop()
        {
            this._listener.Stop();
        }

        public void Dispose()
        {
            this._listener.Close();
        }

        public void RunInBackground()
        {
            Thread th = new Thread(Run);
            th.IsBackground = true;
            th.Start();
        }
    }
}
