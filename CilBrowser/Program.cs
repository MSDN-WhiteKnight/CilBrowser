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
                "https://github.com/MSDN-WhiteKnight/CilBrowser/tree/main/CilBrowser.Core/");

            HtmlGenerator.GenerateWebsite("../../../../CilBrowser", "./CilBrowser_Source/", 0,
                "https://github.com/MSDN-WhiteKnight/CilBrowser/tree/main/CilBrowser/");

            Console.WriteLine("Generated!");
            return 0;
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                GenerateDemo();
                return;
            }

            NamedArgumentDefinition[] defs = new NamedArgumentDefinition[]
            {
                new NamedArgumentDefinition("--output", true),
                new NamedArgumentDefinition("--namespace", true),
            };

            //parse command line parameters
            CommandLineArgs cla = new CommandLineArgs(args, defs);
            string inputPath = string.Empty;
            string outputPath = string.Empty;
            string namespaceFilter = string.Empty;

            if (cla.HasNamedArgument("--output"))
            {
                outputPath = cla.GetNamedArgument("--output");
            }

            if (cla.HasNamedArgument("--namespace"))
            {
                namespaceFilter = cla.GetNamedArgument("--namespace");
            }

            if (cla.PositionalArgumentsCount > 0)
            {
                inputPath = cla.GetPositionalArgument(0);
            }
            
            if (string.IsNullOrEmpty(inputPath))
            {
                GenerateDemo();
                return;
            }

            if (string.IsNullOrEmpty(outputPath))
            {
                outputPath = "./Output/";
            }

            //generate website
            string ext = Path.GetExtension(inputPath);

            if (Utils.StrEquals(ext, ".dll") || Utils.StrEquals(ext, ".exe") || Utils.StrEquals(ext, ".winmd"))
            {
                AssemblyReader reader = new AssemblyReader();

                using (reader)
                {
                    Assembly ass = reader.LoadFrom(inputPath);
                    HtmlGenerator.GenerateWebsite(ass, namespaceFilter, outputPath, string.Empty);
                }
            }
            else //source directory
            {
                HtmlGenerator.GenerateWebsite(inputPath, outputPath, 0, string.Empty);
            }

            Console.WriteLine("Generated!");
        }
    }
}
