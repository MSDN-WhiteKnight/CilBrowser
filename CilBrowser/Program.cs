/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2022,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.IO;
using System.Reflection;
using CilBrowser.Core;
using CilTools.Metadata;

namespace CilBrowser
{
    class Program
    {
        static int GenerateDemo()
        {
            AssemblyReader reader = new AssemblyReader();
            Console.WriteLine("Generating demo website...");

            using (reader)
            {
                Assembly ass = reader.LoadFrom(typeof(HtmlGenerator).Assembly.Location);
                HtmlGenerator.GenerateWebsite(ass, string.Empty, "./CilBrowser.Core/", string.Empty);
                ass = reader.LoadFrom(typeof(Program).Assembly.Location);
                HtmlGenerator.GenerateWebsite(ass, string.Empty, "./CilBrowser/", string.Empty);
            }

            HtmlGenerator.GenerateWebsite("../../../../CilBrowser.Core", "./CilBrowser.Core_Source/", 0,
                "https://github.com/MSDN-WhiteKnight/CilBrowser/tree/main/CilBrowser.Core/", string.Empty);

            HtmlGenerator.GenerateWebsite("../../../../CilBrowser", "./CilBrowser_Source/", 0,
                "https://github.com/MSDN-WhiteKnight/CilBrowser/tree/main/CilBrowser/", string.Empty);

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

            NamedArgumentDefinition[] defs = new NamedArgumentDefinition[]
            {
                new NamedArgumentDefinition("--output", true),
                new NamedArgumentDefinition("--namespace", true),
                new NamedArgumentDefinition("--footer", true),
            };

            // *** Parse command line parameters ***
            CommandLineArgs cla = new CommandLineArgs(args, defs);
            string inputPath = string.Empty;
            string outputPath = string.Empty;
            string namespaceFilter = string.Empty;
            string footerPath = string.Empty;
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
                        Server srv = new Server(ass);
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
                        HtmlGenerator.GenerateWebsite(ass, namespaceFilter, outputPath, footerContent);
                        Console.WriteLine("Generated!");
                    }
                }
            }
            else //source directory
            {
                if (server)
                {
                    Console.WriteLine("Error: Server mode is not supported for sources");
                    return 1;
                }

                //generate static website
                HtmlGenerator.GenerateWebsite(inputPath, outputPath, 0, string.Empty, footerContent);
                Console.WriteLine("Generated!");
            }

            return 0;
        }
    }
}
