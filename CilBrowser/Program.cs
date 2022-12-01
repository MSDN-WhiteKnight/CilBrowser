using System;
using System.IO;
using System.Reflection;

namespace CilBrowser
{
    class Program
    {
        static void Main(string[] args)
        {
            MethodBase mb = MethodBase.GetCurrentMethod();
            string html = HtmlGenerator.VisualizeMethod(mb);
            File.WriteAllText("il.html", html);

            Console.WriteLine("Generated!");
            Console.ReadLine();
        }
    }
}
