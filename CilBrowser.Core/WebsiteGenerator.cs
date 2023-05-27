/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace CilBrowser.Core
{
    /// <summary>
    /// Indexes contents of assemblies or source directories and invokes <see cref="HtmlGenerator"/> to visualize
    /// them as a static website
    /// </summary>
    public static class WebsiteGenerator
    {
        /// <summary>
        /// Generates a static website that contains disassembled CIL for the specified assembly
        /// </summary>
        public static void GenerateFromAssembly(Assembly ass, string nsFilter, string outputPath, string customFooter)
        {
            HtmlGenerator generator = new HtmlGenerator(ass, nsFilter, customFooter);
            Directory.CreateDirectory(outputPath);

            //write assembly manifest
            string html = generator.VisualizeAssemblyManifest(ass);
            File.WriteAllText(Path.Combine(outputPath, "assembly.html"), html, Encoding.UTF8);

            //create Table of contents builder
            StringBuilder sb = new StringBuilder(1000);
            HtmlBuilder toc = new HtmlBuilder(sb);
            AssemblyName an = ass.GetName();
            Console.WriteLine("Generating website for " + an.Name);

            if (!string.IsNullOrEmpty(nsFilter)) Console.WriteLine("Namespace filter: " + nsFilter);

            Console.WriteLine("Output path: " + outputPath);
            HtmlGenerator.WriteTocStart(toc, ass);

            //write types
            Type[] types = ass.GetTypes();
            Dictionary<string, List<Type>> typeMap = Utils.GroupByNamespace(types);
            string[] namespaces = typeMap.Keys.ToArray();
            Array.Sort(namespaces);

            for (int i = 0; i < namespaces.Length; i++)
            {
                string nsText = namespaces[i];

                if (!string.IsNullOrEmpty(nsFilter))
                {
                    if (!Utils.StrEquals(nsText, nsFilter)) continue;
                }

                List<Type> nsTypes = typeMap[namespaces[i]];

                if (nsTypes.Count == 0) continue;

                if (nsTypes.Count == 1)
                {
                    BindingFlags all = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                        BindingFlags.NonPublic;

                    if (Utils.StrEquals(nsTypes[0].FullName, "<Module>") &&
                        nsTypes[0].GetFields(all).Length == 0 && nsTypes[0].GetMethods(all).Length == 0)
                    {
                        continue; //ignore namespace consisting only from empty <Module> type
                    }
                }

                if (string.IsNullOrEmpty(nsText)) nsText = "(No namespace)";
                else nsText = nsText + " namespace";

                toc.WriteTag("h2", nsText);

                for (int j = 0; j < nsTypes.Count; j++)
                {
                    //file content
                    try
                    {
                        html = generator.VisualizeType(nsTypes[j], typeMap);
                    }
                    catch (NotSupportedException ex)
                    {
                        html = HtmlGenerator.VisualizeException(ex);
                        Console.WriteLine("Failed to generate HTML for " + nsTypes[j].FullName);
                        Console.WriteLine(ex.ToString());
                    }

                    string fname = HtmlGenerator.GenerateTypeFileName(nsTypes[j]);

                    if (!string.IsNullOrWhiteSpace(html))
                    {
                        Console.WriteLine(nsTypes[j].FullName);

                        //TOC entry
                        toc.StartParagraph();
                        toc.WriteHyperlink(fname, nsTypes[j].FullName);
                        toc.EndParagraph();
                    }
                    else
                    {
                        Console.WriteLine(nsTypes[j].FullName + " - empty");
                    }

                    File.WriteAllText(Path.Combine(outputPath, fname), html, Encoding.UTF8);
                }
            }

            //write TOC
            generator.WriteFooter(toc);
            toc.EndDocument();
            File.WriteAllText(Path.Combine(outputPath, "index.html"), sb.ToString(), Encoding.UTF8);
        }

        static string VisualizeNavigationPanel(string filename, string dirName, string[] dirFiles, HashSet<string> sourceExtensions)
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
        
        internal static string RenderNavigationPanel(string filepath, string filename, HashSet<string> sourceExtensions)
        {
            string dirpath = Path.GetDirectoryName(filepath);
            string dirname = Utils.GetDirectoryNameFromPath(dirpath);
            string[] files = Directory.GetFiles(dirpath);

            if (files.Length > 1) return VisualizeNavigationPanel(filename, dirname, files, sourceExtensions);
            else return string.Empty;
        }

        static string GetImagesURL(int level)
        {
            StringBuilder sb = new StringBuilder(level * 3);

            for (int i = 0; i < level; i++) sb.Append("../");

            sb.Append("img/");
            return sb.ToString();
        }

        internal static void RenderDirsList(string[] dirs, string dirIconURL, HtmlBuilder toc)
        {
            toc.WriteTagStart("table", HtmlBuilder.OneAttribute("cellpadding", "2px"));

            for (int i = 0; i < dirs.Length; i++)
            {
                string name = Utils.GetDirectoryNameFromPath(dirs[i]);

                if (FileUtils.IsDirectoryExcluded(name)) continue;

                //TOC entry
                toc.WriteTagStart("tr");
                toc.WriteTagStart("td");
                toc.WriteTag("img", string.Empty, HtmlBuilder.OneAttribute("src", dirIconURL));
                toc.WriteTagEnd("td");
                toc.WriteTagStart("td");
                toc.WriteHyperlink("./" + WebUtility.UrlEncode(name) + "/index.html", name);
                toc.WriteTagEnd("td");
                toc.WriteTagEnd("tr");
                toc.EndParagraph();
            }

            toc.WriteTagEnd("table");
        }

        static void GenerateFromSourcesImpl(string sourcesPath, string outputPath, CilBrowserOptions options, 
            string customFooter, int level)
        {
            if (level > 50)
            {
                Console.WriteLine("Error: directory recursion is too deep!");
                return;
            }

            HashSet<string> sourceExtensions;

            if (options.SourceExtensions.Length > 0) sourceExtensions = new HashSet<string>(options.SourceExtensions);
            else sourceExtensions = FileUtils.GetDefaultExtensions();

            HtmlGenerator generator = new HtmlGenerator();
            generator.CustomFooter = customFooter;
            Directory.CreateDirectory(outputPath);

            //create Table of contents builder
            StringBuilder sb = new StringBuilder(1000);
            HtmlBuilder toc = new HtmlBuilder(sb);
            Console.WriteLine("Generating website for source directory: " + sourcesPath);
            Console.WriteLine("Output path: " + outputPath);

            string dirName = Utils.GetDirectoryNameFromPath(sourcesPath);
            HtmlGenerator.WriteTocStart(toc, dirName);

            string[] dirs = Directory.GetDirectories(sourcesPath);
            Array.Sort(dirs);

            if (dirs.Length > 0) toc.WriteTag("h2", "Subdirectories");

            if (level > 0)
            {
                toc.StartParagraph();
                toc.WriteHyperlink("../index.html", "(go to parent directory)");
                toc.EndParagraph();
            }

            //render TOC entries for subdirectories
            string dirIconURL = GetImagesURL(level) + "dir.png";
            RenderDirsList(dirs, dirIconURL, toc);
            toc.WriteRaw(Environment.NewLine);

            //write source files
            string[] files = Directory.GetFiles(sourcesPath);
            Array.Sort(files);

            if (files.Length > 0) toc.WriteTag("h2", "Files");

            for (int i = 0; i < files.Length; i++)
            {
                if (!FileUtils.IsSourceFile(files[i], sourceExtensions)) continue;

                string name = Path.GetFileName(files[i]);
                string html;

                try
                {
                    string content = File.ReadAllText(files[i], options.GetEncoding());
                    string navigation = VisualizeNavigationPanel(name, dirName, files, sourceExtensions);
                    html = generator.VisualizeSourceFile(content, name, navigation, options.SourceControlURL);
                }
                catch (IOException ex)
                {
                    html = HtmlGenerator.VisualizeException(ex);
                    Console.WriteLine("Failed to generate HTML for " + name);
                    Console.WriteLine(ex.ToString());
                }

                if (!string.IsNullOrWhiteSpace(html))
                {
                    Console.WriteLine(name);

                    //file content
                    string pageName = FileUtils.FileNameToPageName(name);
                    File.WriteAllText(Path.Combine(outputPath, pageName), html, Encoding.UTF8);

                    //TOC entry
                    toc.StartParagraph();
                    toc.WriteHyperlink(WebUtility.UrlEncode(pageName), name);
                    toc.EndParagraph();
                }
                else
                {
                    Console.WriteLine(name + " - empty");
                }
            }//end for

            //subdirectories
            for (int i = 0; i < dirs.Length; i++)
            {
                string name = Utils.GetDirectoryNameFromPath(dirs[i]);
                string urlNew;

                if (FileUtils.IsDirectoryExcluded(name)) continue;

                if (!string.IsNullOrEmpty(options.SourceControlURL)) urlNew = Utils.UrlAppend(options.SourceControlURL, name);
                else urlNew = string.Empty;

                CilBrowserOptions optionsNew = options.Copy();
                optionsNew.SourceControlURL = urlNew;
                GenerateFromSourcesImpl(dirs[i], Path.Combine(outputPath, name), optionsNew, customFooter, level + 1);
            }

            //write TOC
            generator.WriteFooter(toc);
            toc.EndDocument();
            File.WriteAllText(Path.Combine(outputPath, "index.html"), sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Generates a static website for the source code in the specified directory
        /// </summary>
        public static void GenerateFromSources(string sourcesPath, string outputPath, CilBrowserOptions options,
            string customFooter)
        {
            //generate HTML files
            GenerateFromSourcesImpl(sourcesPath, outputPath, options, customFooter, 0);

            //write directory icon
            Directory.CreateDirectory(Path.Combine(outputPath, "img"));
            Assembly ass = typeof(WebsiteGenerator).Assembly;
            byte[] imgContent = FileUtils.ReadFromResource(ass, "CilBrowser.Core.Images", "dir.png");
            File.WriteAllBytes(Path.Combine(outputPath, "img/dir.png"), imgContent);
        }
    }
}
