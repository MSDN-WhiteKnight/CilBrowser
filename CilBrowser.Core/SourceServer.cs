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

                // Index files in base directory
                string[] files = Directory.GetFiles(dir);
                Array.Sort(files);

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

        protected override void RenderPage(string url, HttpListenerResponse response)
        {
            string relativePath = StripURL(url);
            string filepath = Path.Combine(this._baseDirectory, relativePath);
            string src = File.ReadAllText(filepath);
            string filename = Path.GetFileName(relativePath);
            string html = this._gen.VisualizeSourceFile(src, filename, string.Empty, string.Empty);
            this.AddToCache(url, html);
            SendHtmlResponse(response, html);
        }
    }
}
