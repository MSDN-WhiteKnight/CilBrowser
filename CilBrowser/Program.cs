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
        static bool TryReadExpectedParameter(string[] args, int pos, string expected)
        {
            if (pos >= args.Length) return false;

            if (args[pos] == expected) return true;
            else return false;
        }

        static string ReadCommandParameter(string[] args, int pos)
        {
            if (pos >= args.Length) return null;

            return args[pos];
        }

        static int GenerateDemo()
        {
            AssemblyReader reader = new AssemblyReader();
            Console.WriteLine("Generating demo website...");

            using (reader)
            {
                Assembly ass = reader.LoadFrom(typeof(HtmlGenerator).Assembly.Location);
                HtmlGenerator.GenerateWebsite(ass, "./CilBrowser.Core/");
                ass = reader.LoadFrom(typeof(Program).Assembly.Location);
                HtmlGenerator.GenerateWebsite(ass, "./CilBrowser/");
            }

            HtmlGenerator.GenerateWebsite("../../../../CilBrowser.Core", "./CilBrowser.Core_Source/", 0);
            HtmlGenerator.GenerateWebsite("../../../../CilBrowser", "./CilBrowser_Source/", 0);

            Console.WriteLine("Generated!");
            return 0;
        }

        static void Main(string[] args)
        {
            //parse command line parameters
            string inputPath;
            string outputPath = null;

            if (args.Length == 0)
            {
                GenerateDemo();
                return;
            }

            int pos = 0;

            if (TryReadExpectedParameter(args, pos, "--output"))
            {
                pos++;
                outputPath = ReadCommandParameter(args, pos);
                pos++;
            }

            inputPath = ReadCommandParameter(args, pos);

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
                    HtmlGenerator.GenerateWebsite(ass, outputPath);
                }
            }
            else //source directory
            {
                HtmlGenerator.GenerateWebsite(inputPath, outputPath, 0);
            }

            Console.WriteLine("Generated!");
        }
    }
}
