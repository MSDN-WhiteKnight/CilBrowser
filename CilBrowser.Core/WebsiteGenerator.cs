/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        static HashSet<string> s_srcExtensions = new HashSet<string>(new string[] {
            ".il", ".cil", ".cs", ".vb", ".c", ".cpp", ".h", ".hpp", ".js", ".ts", ".fs", ".txt", ".md", ".htm", ".html",
            ".css", ".xml", ".csproj", ".vbproj", ".vcxproj", ".proj", ".rc", ".cmd", ".bat", ".sh", ".ps1", ".xaml",
            ".config", ".json", ".yml", ".sln", ".props", ".targets"
        });

        static HashSet<string> s_excludedDirs = new HashSet<string>(new string[] {
            ".git", "bin", "obj", "packages", ".vs", "TestResults", "Debug", "Release"
        });

        static bool IsSourceFile(string name)
        {
            string ext = Path.GetExtension(name).ToLower();

            return ext == string.Empty || s_srcExtensions.Contains(ext);
        }

        static bool IsDirectoryExcluded(string name)
        {
            return s_excludedDirs.Contains(name);
        }

        /// <summary>
        /// Generates a static website that contains disassembled CIL for the specified assembly
        /// </summary>
        public static void GenerateFromAssembly(Assembly ass, string nsFilter, string outputPath, string customFooter)
        {
            HtmlGenerator generator = new HtmlGenerator(ass, nsFilter, customFooter);
            Directory.CreateDirectory(outputPath);

            //write assembly manifest
            string html = generator.VisualizeAssemblyManifest(ass);
            File.WriteAllText(Path.Combine(outputPath, "assembly.html"), html);

            //create Table of contents builder
            StringBuilder sb = new StringBuilder(1000);
            HtmlBuilder toc = new HtmlBuilder(sb);
            AssemblyName an = ass.GetName();
            Console.WriteLine("Generating website for " + an.Name);

            if (!string.IsNullOrEmpty(nsFilter)) Console.WriteLine("Namespace filter: " + nsFilter);

            Console.WriteLine("Output path: " + outputPath);
            HtmlGenerator.WriteTocStart(toc, an);

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

                    File.WriteAllText(Path.Combine(outputPath, fname), html);
                }
            }

            //write TOC
            generator.WriteFooter(toc);
            toc.EndDocument();
            File.WriteAllText(Path.Combine(outputPath, "index.html"), sb.ToString());
        }

        static string VisualizeNavigationPanel(string filename, string dirName, string[] dirFiles)
        {
            StringBuilder sb = new StringBuilder(1000);
            HtmlBuilder html = new HtmlBuilder(sb);
            html.WriteParagraph("Files in " + dirName + " directory:");

            //list of files
            for (int i = 0; i < dirFiles.Length; i++)
            {
                if (!IsSourceFile(dirFiles[i])) continue;

                html.StartParagraph();
                string currFileName = Path.GetFileName(dirFiles[i]);

                if (Utils.StrEquals(currFileName, filename))
                {
                    html.WriteTag("b", filename);
                }
                else
                {
                    html.WriteHyperlink(currFileName + ".html", currFileName);
                }

                html.EndParagraph();
            }

            return sb.ToString();
        }
        
        static void GenerateFromSourcesImpl(string sourcesPath, string outputPath, int level,
            string sourceControlUrl, string customFooter)
        {
            if (level > 50)
            {
                Console.WriteLine("Error: directory recursion is too deep!");
                return;
            }

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

            for (int i = 0; i < dirs.Length; i++)
            {
                string name = Utils.GetDirectoryNameFromPath(dirs[i]);

                if (IsDirectoryExcluded(name)) continue;

                //TOC entry
                toc.StartParagraph();
                toc.WriteHyperlink("./" + name + "/index.html", name);
                toc.EndParagraph();
            }

            //write source files
            toc.WriteRaw(Environment.NewLine);
            string[] files = Directory.GetFiles(sourcesPath);
            Array.Sort(files);

            if (files.Length > 0) toc.WriteTag("h2", "Files");

            for (int i = 0; i < files.Length; i++)
            {
                if (!IsSourceFile(files[i])) continue;

                string name = Path.GetFileName(files[i]);
                string html;

                try
                {
                    string content = File.ReadAllText(files[i]);
                    string navigation = VisualizeNavigationPanel(name, dirName, files);
                    html = generator.VisualizeSourceFile(content, name, navigation, sourceControlUrl);
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
                    File.WriteAllText(Path.Combine(outputPath, name + ".html"), html);

                    //TOC entry
                    toc.StartParagraph();
                    toc.WriteHyperlink(name + ".html", name);
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

                if (IsDirectoryExcluded(name)) continue;

                if (!string.IsNullOrEmpty(sourceControlUrl)) urlNew = Utils.UrlAppend(sourceControlUrl, name);
                else urlNew = string.Empty;

                GenerateFromSourcesImpl(dirs[i], Path.Combine(outputPath, name), level + 1, urlNew, customFooter);
            }

            //write TOC
            generator.WriteFooter(toc);
            toc.EndDocument();
            File.WriteAllText(Path.Combine(outputPath, "index.html"), sb.ToString());
        }

        /// <summary>
        /// Generates a static website for the source code in the specified directory
        /// </summary>
        public static void GenerateFromSources(string sourcesPath, string outputPath, string sourceControlUrl,
            string customFooter)
        {
            GenerateFromSourcesImpl(sourcesPath, outputPath, 0, sourceControlUrl, customFooter);
        }
    }
}
