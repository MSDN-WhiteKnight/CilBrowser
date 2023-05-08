/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace CilBrowser.Core
{
    /// <summary>
    /// Provides a base class for servers that dynamically generate HTML and return it via HTTP. Derived classes 
    /// should be sealed and not add any more disposable resources!
    /// </summary>
    public abstract class ServerBase : IDisposable
    {
        protected string _urlHost;
        protected string _urlPrefix;
        HttpListener _listener;
        Dictionary<string, string> _cache = new Dictionary<string, string>(CacheCapacity);

        public const string DefaultUrlHost = "http://localhost:8080";
        public const string DefaultUrlPrefix = "/CilBrowser/";
        const int CacheCapacity = 200;

        protected ServerBase(string urlHost, string urlPrefix)
        {
            this._urlHost = urlHost;
            this._urlPrefix = urlPrefix;
            
            // Create a listener.
            this._listener = new HttpListener();
        }

        protected abstract void OnStart();

        protected abstract void RenderFrontPage(HttpListenerResponse response);

        protected abstract void RenderPage(string url, HttpListenerResponse response);

        internal void AddToCache(string url, string content)
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

        /// <summary>
        /// Strips prefix and file extension from the specified URL
        /// </summary>
        internal string StripURL(string url)
        {
            int index = this._urlPrefix.Length;

            if (index >= url.Length) return string.Empty;

            //Strip prefix
            string urlPart = url.Substring(index);
            index = urlPart.IndexOf('.');

            if (index < 0) index = urlPart.Length;

            //Strip extension
            urlPart = urlPart.Substring(0, index);

            return urlPart;
        }

        internal static void SendHtmlResponse(HttpListenerResponse response, string content)
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

        internal static void SendErrorResponse(HttpListenerResponse response, int code, string status)
        {
            response.StatusCode = code;
            response.StatusDescription = status;
            response.Close();
        }
        
        public void Run()
        {
            HttpListener listener = this._listener;
            
            // Add the prefixes.
            listener.Prefixes.Add(this._urlHost + this._urlPrefix);
            listener.Start();
            this.OnStart();
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

                // Write output to the response.
                if (Utils.StrEquals(url, this._urlPrefix) || Utils.StrEquals(url, this._urlPrefix + "index.html"))
                {
                    // Table of contents
                    this.RenderFrontPage(response);
                    response.Close();
                }
                else
                {
                    // Regular page
                    this.RenderPage(url, response);
                    response.Close();
                }
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
