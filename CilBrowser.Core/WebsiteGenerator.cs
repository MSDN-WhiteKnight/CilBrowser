﻿/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using CilBrowser.Core.Structure;

namespace CilBrowser.Core
{
    /// <summary>
    /// Indexes contents of assemblies or source directories and invokes <see cref="HtmlGenerator"/> to visualize
    /// them as a static website
    /// </summary>
    public static class WebsiteGenerator
    {
        public static void GenerateFromAssembly(Assembly ass, string nsFilter, string outputPath, string customFooter)
        {
            AssemblyName an = ass.GetName();
            Console.WriteLine("Generating website for " + an.Name);

            if (!string.IsNullOrEmpty(nsFilter)) Console.WriteLine("Namespace filter: " + nsFilter);

            Console.WriteLine("Output path: " + outputPath);
            HtmlGenerator generator = new HtmlGenerator(ass, nsFilter, customFooter);
            SectionNode root = AssemblyIndexer.AssemblyToTree(ass, nsFilter);
            GenerateFromTreeImpl(root, outputPath, new CilBrowserOptions(), generator, 0);
        }

        internal static string VisualizeNavigationPanel(string filename, string dirName, string[] dirFiles, 
            HashSet<string> sourceExtensions)
        {
            StringBuilder sb = new StringBuilder(1000);
            HtmlBuilder html = new HtmlBuilder(sb);
            html.WriteParagraph("Files in " + dirName + " directory:");

            //list of files
            for (int i = 0; i < dirFiles.Length; i++)
            {
                if (!FileUtils.IsSourceFile(dirFiles[i], sourceExtensions)) continue;

                html.StartParagraph();
                string currFileName = Path.GetFileName(dirFiles[i]);

                if (Utils.StrEquals(currFileName, filename))
                {
                    html.WriteTag("b", filename);
                }
                else
                {
                    string pageName = FileUtils.FileNameToPageName(currFileName);
                    html.WriteHyperlink(WebUtility.UrlEncode(pageName), currFileName);
                }

                html.EndParagraph();
            }

            return sb.ToString();
        }

        internal static string VisualizeNavigationPanel(PageNode fileNode, string dirName, PageNode[] dirFiles, TreeNodeKind kind)
        {
            StringBuilder sb = new StringBuilder(1000);
            HtmlBuilder html = new HtmlBuilder(sb);

            if (kind == TreeNodeKind.Directory) html.WriteParagraph("Files in " + dirName + " directory:");
            else html.WriteParagraph("Pages in " + dirName + ":");

            //list of pages
            for (int i = 0; i < dirFiles.Length; i++)
            {
                html.StartParagraph();
                string currFileName = dirFiles[i].Name;

                if (Utils.StrEquals(currFileName, fileNode.Name))
                {
                    html.WriteTag("b", fileNode.DisplayName);
                }
                else
                {
                    string pageName = FileUtils.FileNameToPageName(currFileName);
                    html.WriteHyperlink(WebUtility.UrlEncode(pageName), dirFiles[i].DisplayName);
                }

                html.EndParagraph();
            }

            return sb.ToString();
        }

        internal static string RenderNavigationPanel(string filepath, string filename, HashSet<string> sourceExtensions)
        {
            string dirpath = Path.GetDirectoryName(filepath);
            string dirname = Utils.GetDirectoryNameFromPath(dirpath);
            string[] files = Directory.GetFiles(dirpath);

            if (files.Length > 1) return VisualizeNavigationPanel(filename, dirname, files, sourceExtensions);
            else return string.Empty;
        }

        internal static string GetImagesURL(int level)
        {
            StringBuilder sb = new StringBuilder(level * 3);

            for (int i = 0; i < level; i++) sb.Append("../");

            sb.Append("img/");
            return sb.ToString();
        }
        
        internal static void RenderDirsList(SectionNode[] dirs, string dirIconURL, HtmlBuilder toc)
        {
            if (dirs.Length == 0) return;

            toc.WriteTagStart("table", HtmlBuilder.OneAttribute("cellpadding", "2px"));

            for (int i = 0; i < dirs.Length; i++)
            {
                if (dirs[i].PagesCount + dirs[i].SectionsCount == 0)
                {
                    continue; //not interested in empty directories
                }

                string name = dirs[i].Name;
                
                //TOC entry
                toc.WriteTagStart("tr");

                if (dirs[i].Kind == TreeNodeKind.Directory)
                {
                    toc.WriteTagStart("td");
                    toc.WriteTag("img", string.Empty, HtmlBuilder.OneAttribute("src", dirIconURL));
                    toc.WriteTagEnd("td");
                }
                
                toc.WriteTagStart("td");
                toc.WriteHyperlink("./" + WebUtility.UrlEncode(name) + "/index.html", dirs[i].DisplayName);
                toc.WriteTagEnd("td");
                toc.WriteTagEnd("tr");
            }

            toc.WriteTagEnd("table");
        }

        internal static void RenderTocEntry(string displayName, string pageName, string fileIconURL, TreeNodeKind kind,
            HtmlBuilder toc)
        {
            toc.WriteTagStart("tr");

            if (kind == TreeNodeKind.File)
            {
                toc.WriteTagStart("td");
                toc.WriteTag("img", string.Empty, HtmlBuilder.OneAttribute("src", fileIconURL));
                toc.WriteTagEnd("td");
            }

            toc.WriteTagStart("td");
            toc.WriteHyperlink(WebUtility.UrlEncode(pageName), displayName);
            toc.WriteTagEnd("td");
            toc.WriteTagEnd("tr");
        }

        internal static void RenderNodePath(TreeNode[] path, int baseLevel, HtmlBuilder target)
        {
            if (path.Length == 0) return;

            target.StartParagraph();

            for (int i = 0; i < path.Length; i++)
            {
                int level = path.Length - baseLevel - i; //first node is on highest level
                target.WriteRaw("<a href=\"");

                for (int j = 0; j < level; j++) target.WriteRaw("../");

                target.WriteRaw("index.html\">");
                target.WriteEscaped(path[i].DisplayName);
                target.WriteTagEnd("a");
                target.WriteRaw(" / ");
            }

            target.EndParagraph();
        }

        /// <summary>
        /// Generates a static website for the source code in the specified directory
        /// </summary>
        public static void GenerateFromSources(string sourcesPath, string outputPath, CilBrowserOptions options,
            string customFooter)
        {
            Console.WriteLine("Generating website for source directory: " + sourcesPath);
            Console.WriteLine("Output path: " + outputPath);

            //generate HTML files
            HtmlGenerator generator = new HtmlGenerator();
            generator.CustomFooter = customFooter;
            DirectoryNode root = SourceIndexer.SourceDirectoryToTree(sourcesPath, options);
            GenerateFromTreeImpl(root, outputPath, options, generator, 0);

            //write icons
            Directory.CreateDirectory(Path.Combine(outputPath, "img"));
            Assembly ass = typeof(WebsiteGenerator).Assembly;
            byte[] imgContent = FileUtils.ReadFromResource(ass, "CilBrowser.Core.Images", "dir.png");
            File.WriteAllBytes(Path.Combine(outputPath, "img/dir.png"), imgContent);
            imgContent = FileUtils.ReadFromResource(ass, "CilBrowser.Core.Images", "file.png");
            File.WriteAllBytes(Path.Combine(outputPath, "img/file.png"), imgContent);
        }

        static void GenerateFromTreeImpl(SectionNode currentNode, string outputPath, CilBrowserOptions options,
            HtmlGenerator generator, int level)
        {
            if (level > 50)
            {
                Console.WriteLine("Error: Website structure tree is too deep!");
                return;
            }

            if (currentNode.PagesCount + currentNode.SectionsCount == 0)
            {
                return; //this directory does not have anything interesting
            }
            
            Directory.CreateDirectory(outputPath);

            //files
            foreach (PageNode fileNode in currentNode.Pages)
            {
                string pageName = FileUtils.FileNameToPageName(fileNode.Name);
                string html = fileNode.RenderToString(generator, options);
                File.WriteAllText(Path.Combine(outputPath, pageName), html, Encoding.UTF8);
            }

            //subdirectories
            foreach(SectionNode dir in currentNode.Sections)
            {
                string name = dir.Name;
                string urlNew;

                if (!string.IsNullOrEmpty(options.SourceControlURL)) urlNew = Utils.UrlAppend(options.SourceControlURL, name);
                else urlNew = string.Empty;

                CilBrowserOptions optionsNew = options.Copy();
                optionsNew.SourceControlURL = urlNew;
                GenerateFromTreeImpl(dir, Path.Combine(outputPath, name), optionsNew, generator, level + 1);
            }

            //TOC for current directory
            string tocHTML = currentNode.RenderToString(generator, options);
            File.WriteAllText(Path.Combine(outputPath, "index.html"), tocHTML, Encoding.UTF8);
        }
    }
}
