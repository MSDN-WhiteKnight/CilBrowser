/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace CilBrowser.Core
{
    /// <summary>
    /// Provides a server that dynamically generates HTML for source code files in the specified directory 
    /// and returns it via HTTP
    /// </summary>
    public sealed class SourceServer : ServerBase
    {
        string _baseDirectory;
        HtmlGenerator _gen;
        CilBrowserOptions _options;
        HashSet<string> _sourceExtensions;
        Dictionary<string, byte[]> _resources;

        public SourceServer(string baseDirectory, CilBrowserOptions options, string urlHost, string urlPrefix) : 
            base(urlHost, urlPrefix)
        {
            this._baseDirectory = baseDirectory;
            this._options = options;
            this._gen = new HtmlGenerator();

            if (options.SourceExtensions.Length > 0)
            {
                this._sourceExtensions = new HashSet<string>(this._options.SourceExtensions);
            }
            else
            {
                this._sourceExtensions = FileUtils.GetDefaultExtensions();
            }

            this._resources = new Dictionary<string, byte[]>(2);
            Assembly ass = typeof(WebsiteGenerator).Assembly;
            byte[] imageData = FileUtils.ReadFromResource(ass, "CilBrowser.Core.Images", "dir.png");
            this._resources["dir.png"] = imageData;
            imageData = FileUtils.ReadFromResource(ass, "CilBrowser.Core.Images", "file.png");
            this._resources["file.png"] = imageData;
        }

        protected override void OnStart()
        {
            Console.WriteLine("Base source directory: " + this._baseDirectory);
        }

        void RenderToc(string dir, bool topLevel, HttpListenerResponse response)
        {
            if (!Directory.Exists(dir))
            {
                SendErrorResponse(response, 404, "Not found");
                return;
            }
            
            // Render table of contents
            response.ContentType = "text/html; charset=utf-8";
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

                if (!topLevel)
                {
                    toc.StartParagraph();
                    toc.WriteHyperlink("../index.html", "(go to parent directory)");
                    toc.EndParagraph();
                }

                WebsiteGenerator.RenderDirsList(dirs, "dir.png", toc);

                // Index files in directory
                string[] files = Directory.GetFiles(dir);
                Array.Sort(files);

                if (files.Length > 0) toc.WriteTag("h2", "Files");

                toc.WriteTagStart("table", HtmlBuilder.OneAttribute("cellpadding", "2px"));

                for (int i = 0; i < files.Length; i++)
                {
                    if (!FileUtils.IsSourceFile(files[i], this._sourceExtensions)) continue;

                    string name = Path.GetFileName(files[i]);
                    string pageName = FileUtils.FileNameToPageName(name);
                    WebsiteGenerator.RenderTocEntry(name, pageName, "file.png", toc);
                }

                toc.WriteTagEnd("table");

                this._gen.WriteFooter(toc);
                toc.EndDocument();
            }
        }

        protected override void RenderFrontPage(HttpListenerResponse response)
        {
            try
            {
                this.RenderToc(this._baseDirectory, true, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SourceServer error when rendering frontpage!");
                Console.WriteLine(ex.ToString());
                Console.WriteLine();
                SendErrorResponse(response, 500, "Internal server error");
            }
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
            try
            {
                url = WebUtility.UrlDecode(url);
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
                    this.RenderToc(Path.Combine(this._baseDirectory, dir), false, response);
                }
                else if (url.Length > 0 && url[url.Length - 1] == '/')
                {
                    //directory table of contents
                    this.RenderToc(Path.Combine(this._baseDirectory, StripPrefix(url)), false, response);
                }
                else if (url.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                {
                    //image
                    string imageName = Path.GetFileName(StripPrefix(url)).ToLower();
                    byte[] imageContent;

                    if (this._resources.TryGetValue(imageName, out imageContent))
                    {
                        response.ContentType = "image/png";
                        response.OutputStream.Write(imageContent, 0, imageContent.Length);
                        response.Close();
                    }
                    else
                    {
                        SendErrorResponse(response, 404, "Not found");
                    }
                }
                else
                {
                    //regular page
                    string filepath = Path.Combine(this._baseDirectory, relativePath);

                    if (!File.Exists(filepath))
                    {
                        SendErrorResponse(response, 404, "Not found");
                        return;
                    }

                    string src = File.ReadAllText(filepath, this._options.GetEncoding());
                    string filename = Path.GetFileName(relativePath);

                    //render left navigation panel
                    string navigation = WebsiteGenerator.RenderNavigationPanel(filepath, filename, this._sourceExtensions);

                    //render page body
                    string html = this._gen.VisualizeSourceFile(src, filename, navigation, string.Empty);

                    this.AddToCache(url, html);
                    SendHtmlResponse(response, html);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SourceServer error when rendering page!");
                Console.WriteLine(ex.ToString());
                Console.WriteLine();
                SendErrorResponse(response, 500, "Internal server error");
            }
        }
    }
}
