using System;
using System.Reflection;
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
                Assembly ass = reader.LoadFrom(typeof(Program).Assembly.Location);
                HtmlGenerator.GenerateWebsite(ass);
            }

            Console.WriteLine("Generated!");
            Console.ReadLine();
        }
    }
}
