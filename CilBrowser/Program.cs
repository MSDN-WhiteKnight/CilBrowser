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
        static void Main(string[] args)
        {
            AssemblyReader reader = new AssemblyReader();

            using (reader)
            {
                Assembly ass = reader.LoadFrom(typeof(HtmlGenerator).Assembly.Location);
                HtmlGenerator.GenerateWebsite(ass);
            }

            Console.WriteLine("Generated!");
            Console.ReadLine();
        }
    }
}
