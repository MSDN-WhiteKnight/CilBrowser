/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using CilBrowser.Core;
using CilTools.Metadata;
using Integration.Git;

namespace CilBrowser
{
    class Program
    {
        static NamedArgumentDefinition[] defs;

        static int GenerateDemo()
        {
            string outputDir;
            string pathCilBrowser;
            string pathCilBrowserCore;
            AssemblyReader reader = new AssemblyReader();
            Console.WriteLine("Generating demo website...");

            // Determine paths
            if (File.Exists("./CilBrowser.csproj")) // Launched from project root (like for 'dotnet run')
            {
                pathCilBrowser = ".";
                pathCilBrowserCore = "../CilBrowser.Core";
                outputDir = "./bin";
            }
            else // Launched from bin/(config)/(tfm) under project root
            {
                pathCilBrowser = "../../../../CilBrowser";
                pathCilBrowserCore = "../../../../CilBrowser.Core";
                outputDir = ".";
            }

            // Generate websites: Disassembled CIL for binaries
            using (reader)
            {
                Assembly ass = reader.LoadFrom(typeof(HtmlGenerator).Assembly.Location);
                WebsiteGenerator.GenerateFromAssembly(ass, string.Empty, outputDir + "/CilBrowser.Core/", string.Empty);
                ass = reader.LoadFrom(typeof(Program).Assembly.Location);
                WebsiteGenerator.GenerateFromAssembly(ass, string.Empty, outputDir + "/CilBrowser/", string.Empty);
                Console.WriteLine();
            }

            // Generate websites: Source code
            const string MsgTemplate = "Error: Failed to generate demo website for {0} sources. Directory not found. " +
                "Run program from bin/(config)/(tfm) subridectory of source directory to fix this.";
            CilBrowserOptions options = new CilBrowserOptions();
            
            if (Directory.Exists(pathCilBrowserCore))
            {
                options.SourceControlURL = "https://github.com/MSDN-WhiteKnight/CilBrowser/tree/main/CilBrowser.Core/";

                WebsiteGenerator.GenerateFromSources(pathCilBrowserCore, outputDir + "/CilBrowser.Core_Source/",
                    options, string.Empty);
            }
            else
            {
                Console.WriteLine(MsgTemplate, "CilBrowser.Core");
            }

            if (Directory.Exists(pathCilBrowser))
            {
                options.SourceControlURL = "https://github.com/MSDN-WhiteKnight/CilBrowser/tree/main/CilBrowser/";

                WebsiteGenerator.GenerateFromSources(pathCilBrowser, outputDir + "/CilBrowser_Source/",
                    options, string.Empty);
            }
            else
            {
                Console.WriteLine(MsgTemplate, "CilBrowser");
            }

            Console.WriteLine("Generated!");
            return 0;
        }

        static void PrintHelp()
        {
            Console.WriteLine("*** CIL Browser {0} ***", typeof(Program).Assembly.GetName().Version);
            Console.WriteLine();
            Console.WriteLine("Command line syntax:");
            Console.WriteLine("  CilBrowser [Options] <InputPath>");
            Console.WriteLine();
            Console.WriteLine("Options:");

            for (int i = 0; i < defs.Length; i++)
            {
                Console.WriteLine(defs[i].Name + ": " + defs[i].Description);
            }
        }

        static int GenerateFromSourceDirectory(string sourcesPath, string outputPath, string footerContent)
        {
            string cfgPath = Path.Combine(sourcesPath, "browser.cfg");
            CilBrowserOptions options;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //try to read config from file
            if (File.Exists(cfgPath))
            {
                try
                {
                    options = CilBrowserOptions.ReadFromFile(cfgPath);
                    Console.WriteLine("Using config from " + cfgPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error when trying to read browser.cfg!");
                    Console.WriteLine(ex.ToString());
                    options = new CilBrowserOptions();
                }
            }
            else
            {
                options = new CilBrowserOptions();
            }

            //run generation process
            WebsiteGenerator.GenerateFromSources(sourcesPath, outputPath, options, footerContent);
            return 0;
        }

        static int GenerateFromGitRepository(string url, string outputPath, string footerContent)
        {
            string repoName = Path.GetFileNameWithoutExtension(url);

            if (string.IsNullOrEmpty(repoName)) repoName = "repo";
            
            // Clone remote to temp directory
            string cloneDir = Utils.CreateTempDir(repoName);
            string cloneDirLinux = cloneDir.Replace('\\', '/'); //git bash uses Linux paths

            try
            {
                Console.WriteLine("Cloning git repository to " + cloneDirLinux + "...");
                string output = GitBash.ExecuteCommand("git clone " + url + " " + cloneDirLinux);
                Console.WriteLine(output);

                Console.WriteLine("Generating website from cloned sources...");
                GenerateFromSourceDirectory(cloneDir, outputPath, footerContent);
            }
            finally
            {
                // Try to delete cloned sources. This will most likely fail because git has some background process
                // that keeps objects open even after we've finished working with repository.
                Thread.Sleep(500);
                Utils.DeleteTempDirRecursive(cloneDir, 0);
            }

            Console.WriteLine("Generated!");
            return 0;
        }

        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                GenerateDemo();
                return 0;
            }

            defs = new NamedArgumentDefinition[]
            {
                new NamedArgumentDefinition("--output", true, "Output directory"),
                new NamedArgumentDefinition("--namespace", true, "Namespace filter"),
                new NamedArgumentDefinition("--footer", true, "Custom footer file path"),
                new NamedArgumentDefinition("--host", true, "URL host (default is " + Server.DefaultUrlHost + ")"),
                new NamedArgumentDefinition("--prefix", true, "URL prefix (default is " + Server.DefaultUrlPrefix + ")"),
            };

            if (Utils.StrEquals(args[0], "help") || Utils.StrEquals(args[0], "-?") || Utils.StrEquals(args[0], "/?"))
            {
                PrintHelp();
                return 0;
            }

            // *** Parse command line parameters ***
            CommandLineArgs cla = new CommandLineArgs(args, defs);
            string inputPath = string.Empty;
            string outputPath = string.Empty;
            string namespaceFilter = string.Empty;
            string footerPath = string.Empty;
            string urlHost = string.Empty;
            string urlPrefix = string.Empty;
            bool server = false;

            //named parameters
            if (cla.HasNamedArgument("--output"))
            {
                outputPath = cla.GetNamedArgument("--output");
            }

            if (cla.HasNamedArgument("--namespace"))
            {
                namespaceFilter = cla.GetNamedArgument("--namespace");
            }

            if (cla.HasNamedArgument("--footer"))
            {
                footerPath = cla.GetNamedArgument("--footer");
            }

            if (cla.HasNamedArgument("--host"))
            {
                urlHost = cla.GetNamedArgument("--host");
            }

            if (cla.HasNamedArgument("--prefix"))
            {
                urlPrefix = cla.GetNamedArgument("--prefix");
            }

            //positional parameter: input path
            if (cla.PositionalArgumentsCount > 0)
            {
                inputPath = cla.GetPositionalArgument(0);
            }
            
            if (string.IsNullOrEmpty(inputPath))
            {
                //Input path not specified, generate demo website
                GenerateDemo();
                return 0;
            }

            if (string.IsNullOrEmpty(outputPath))
            {
                //Output path is not specified, use server mode instead of generating website
                server = true;
            }

            string footerContent = string.Empty;

            if (!string.IsNullOrEmpty(footerPath))
            {
                if (server)
                {
                    Console.WriteLine("Error: Custom footer is not supported in server mode");
                    return 1;
                }

                //read custom footer file
                footerContent = File.ReadAllText(footerPath);
            }

            if (!string.IsNullOrEmpty(namespaceFilter) && server)
            {
                Console.WriteLine("Error: Namespace filter is not supported in server mode");
                return 1;
            }

            if (server)
            {
                if (string.IsNullOrEmpty(urlHost))
                {
                    urlHost = Server.DefaultUrlHost;
                }

                if (string.IsNullOrEmpty(urlPrefix))
                {
                    urlPrefix = Server.DefaultUrlPrefix;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(urlHost) || !string.IsNullOrEmpty(urlPrefix))
                {
                    Console.WriteLine("Error: --host or --prefix should not be specified in website generator mode");
                    return 1;
                }
            }
            
            // *** Run program ***
            string ext = Path.GetExtension(inputPath);

            if (Utils.StrEquals(ext, ".dll") || Utils.StrEquals(ext, ".exe") || Utils.StrEquals(ext, ".winmd"))
            {
                //assembly file
                AssemblyReader reader = new AssemblyReader();

                using (reader)
                {
                    Assembly ass = reader.LoadFrom(inputPath);

                    if (server)
                    {
                        //run server
                        Console.WriteLine("Running server...");
                        Server srv = new Server(ass, urlHost, urlPrefix);
                        srv.RunInBackground();

                        while (true)
                        {
                            ConsoleKeyInfo key = Console.ReadKey();

                            if (key.Key == ConsoleKey.E)
                            {
                                srv.Stop();
                                srv.Dispose();
                                break;
                            }
                        }
                    }
                    else
                    {
                        //generate static website
                        WebsiteGenerator.GenerateFromAssembly(ass, namespaceFilter, outputPath, footerContent);
                        Console.WriteLine("Generated!");
                    }
                }
            }
            else if (inputPath.EndsWith(".git"))
            {
                return GenerateFromGitRepository(inputPath, outputPath, footerContent);
            }
            else //source directory
            {
                if (server)
                {
                    Console.WriteLine("Error: Server mode is not supported for sources");
                    return 1;
                }

                //generate static website
                GenerateFromSourceDirectory(inputPath, outputPath, footerContent);
                Console.WriteLine("Generated!");
            }

            return 0;
        }
    }
}
