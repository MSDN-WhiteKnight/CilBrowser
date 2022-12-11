/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2022,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
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

            Console.WriteLine("Generated!");
            return 0;
        }

        static void Main(string[] args)
        {
            Console.WriteLine(string.Join(",", args));
            //parse command line parameters
            string assemblyPath;
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

            assemblyPath = ReadCommandParameter(args, pos);

            if (string.IsNullOrEmpty(assemblyPath))
            {
                GenerateDemo();
                return;
            }

            if (string.IsNullOrEmpty(outputPath))
            {
                outputPath = "./Output/";
            }

            //generate website
            AssemblyReader reader = new AssemblyReader();

            using (reader)
            {
                Assembly ass = reader.LoadFrom(assemblyPath);
                HtmlGenerator.GenerateWebsite(ass, outputPath);
            }

            Console.WriteLine("Generated!");
        }
    }
}
