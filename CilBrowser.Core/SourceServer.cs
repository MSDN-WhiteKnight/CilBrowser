/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace CilBrowser.Core
{
    public sealed class SourceServer : ServerBase
    {
        string _baseDirectory;
        HtmlGenerator _gen;

        public SourceServer(string baseDirectory, string urlHost, string urlPrefix) : base(urlHost, urlPrefix)
        {
            this._baseDirectory = baseDirectory;
            this._gen = new HtmlGenerator();
        }

        protected override void OnStart()
        {
            Console.WriteLine("Base directory: " + this._baseDirectory);
        }

        void RenderToc(string dir, HttpListenerResponse response)
        {
            // Render table of contents
            StreamWriter wr = new StreamWriter(response.OutputStream);
            HtmlBuilder toc = new HtmlBuilder(wr);

            using (wr)
            {
                string dirName = Utils.GetDirectoryNameFromPath(dir);
                HtmlGenerator.WriteTocStart(toc, dirName);

                // Index subdirectories
                string[] dirs = Directory.GetDirectories(dir);
                Array.Sort(dirs);

                if (dirs.Length > 0) toc.WriteTag("h2", "Subdirectories");

                for (int i = 0; i < dirs.Length; i++)
                {
                    string name = Utils.GetDirectoryNameFromPath(dirs[i]);

                    if (WebsiteGenerator.IsDirectoryExcluded(name)) continue;
                    
                    toc.StartParagraph();
                    toc.WriteHyperlink("./" + name + "/index.html", name);
                    toc.EndParagraph();
                }

                // Index files in directory
                string[] files = Directory.GetFiles(dir);
                Array.Sort(files);

                if (files.Length > 0) toc.WriteTag("h2", "Files");

                for (int i = 0; i < files.Length; i++)
                {
                    if (!WebsiteGenerator.IsSourceFileDefault(files[i])) continue;

                    string name = Path.GetFileName(files[i]);
                    toc.StartParagraph();
                    toc.WriteHyperlink(name + ".html", name);
                    toc.EndParagraph();
                }

                this._gen.WriteFooter(toc);
                toc.EndDocument();
            }
        }

        protected override void RenderFrontPage(HttpListenerResponse response)
        {
            this.RenderToc(this._baseDirectory, response);
        }

        static bool IsUrlInvalid(string url)
        {
            string[] parts = url.Split('/');

            for (int i = 0; i < parts.Length; i++)
            {
                if (Utils.StrEquals(parts[i], "..")) return true;

                if (Utils.StrEquals(parts[i], "\\")) return true;
            }

            return false;
        }

        protected override void RenderPage(string url, HttpListenerResponse response)
        {
            string relativePath = StripURL(url);

            if (IsUrlInvalid(relativePath))
            {
                SendErrorResponse(response, 404, "Not found");
                return;
            }

            if (url.EndsWith("index.html", StringComparison.OrdinalIgnoreCase))
            {
                //directory table of contents
                if (relativePath.Length < 5)
                {
                    SendErrorResponse(response, 404, "Not found");
                    return;
                }

                string dir = relativePath.Substring(0, relativePath.Length - 5);
                this.RenderToc(Path.Combine(this._baseDirectory, dir), response);
            }
            else if (url.Length > 0 && url[url.Length - 1] == '/')
            {
                //directory table of contents
                this.RenderToc(Path.Combine(this._baseDirectory, StripPrefix(url)), response);
            }
            else
            {
                //regular page
                string filepath = Path.Combine(this._baseDirectory, relativePath);
                string src = File.ReadAllText(filepath);
                string filename = Path.GetFileName(relativePath);
                string html = this._gen.VisualizeSourceFile(src, filename, string.Empty, string.Empty);
                this.AddToCache(url, html);
                SendHtmlResponse(response, html);
            }
        }
    }
}
