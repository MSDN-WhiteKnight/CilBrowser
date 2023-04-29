/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CilBrowser.Core.Configuration;

namespace CilBrowser.Core
{
    public class CilBrowserOptions
    {
        public CilBrowserOptions()
        {
            this.SourceExtensions = new string[0];
            this.UseAnsiEncoding = false;
        }

        /// <summary>
        /// Gets or sets the URL of directory on source control server (for example, on GitHub) where the sources for this 
        /// project are stored.
        /// </summary>
        public string SourceControlURL { get; set; }

        /// <summary>
        /// Gets or sets the array of file extensions (with leading dot) that are considered source files. When empty, 
        /// the default set of extensions is used.
        /// </summary>
        public string[] SourceExtensions { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether ANSI encoding is used when reading source files. When set to true, 
        /// the <see cref="AnsiCodepage"/> property should contain codepage number. When set to false, UTF-8 is used.
        /// </summary>
        public bool UseAnsiEncoding { get; set; }

        /// <summary>
        /// Gets or sets the codepage number when ANSI encoding is used for source files
        /// </summary>
        public int AnsiCodepage { get; set; }

        /// <summary>
        /// Reads options from the text configuration file with "key=value" lines
        /// </summary>        
        public static CilBrowserOptions ReadFromFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
            return ReadValues(lines);
        }

        public static CilBrowserOptions ReadValues(string[] lines)
        {
            Dictionary<string, string> cfg = ConfigReader.ReadValues(lines);
            CilBrowserOptions ret = new CilBrowserOptions();

            string str;

            //source control URL
            if (!cfg.TryGetValue("SourceControlURL", out str)) str = string.Empty;

            ret.SourceControlURL = str;

            //extensions
            if (!cfg.TryGetValue("SourceExtensions", out str)) str = string.Empty;

            str = str.Trim();

            if (str.Length > 0)
            {
                string[] exts = str.Split(',');
                List<string> exts_list = new List<string>(exts.Length);

                for (int i = 0; i < exts.Length; i++)
                {
                    string item = exts[i].Trim();

                    if (item.Length == 0) continue;

                    if (!item.StartsWith(".", StringComparison.Ordinal))
                    {
                        item = "." + item;
                    }

                    exts_list.Add(item);
                }

                if(exts_list.Count>0) ret.SourceExtensions = exts_list.ToArray();
            }

            //encoding
            if (!cfg.TryGetValue("SourceEncoding", out str)) str = string.Empty;

            str = str.Trim();

            if (str.StartsWith("cp") && str.Length > 2) //ANSI codepage by number
            {
                string cpn = str.Substring(2);
                int codepage;

                if (int.TryParse(cpn, out codepage))
                {
                    ret.UseAnsiEncoding = true;
                    ret.AnsiCodepage = codepage;
                }

                // Everything else is assumed to be UTF-8
            }

            return ret;
        }

        public CilBrowserOptions Copy()
        {
            CilBrowserOptions optionsNew = new CilBrowserOptions();
            optionsNew.SourceControlURL = this.SourceControlURL;
            optionsNew.UseAnsiEncoding = this.UseAnsiEncoding;
            optionsNew.AnsiCodepage = this.AnsiCodepage;
            optionsNew.SourceExtensions = this.SourceExtensions;
            return optionsNew;
        }

        public Encoding GetEncoding()
        {
            if (!this.UseAnsiEncoding) return Encoding.UTF8;
            else return Encoding.GetEncoding(this.AnsiCodepage);
        }
    }
}
