/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CilBrowser.Core.Configuration
{
    public static class ConfigReader
    {
        /// <summary>
        /// Reads configuration values from the text file containing "key=value" lines
        /// </summary>        
        public static Dictionary<string, string> ReadFromFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
            return ReadValues(lines);
        }

        /// <summary>
        /// Reads configuration values from the array of "key=value" lines
        /// </summary>
        public static Dictionary<string, string> ReadValues(string[] lines)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                if (line.Length == 0) continue;

                if (line[0] == ';') continue;

                int index = line.IndexOf('=');

                if (index <= 0) continue;

                string key = line.Substring(0, index).Trim();
                string val = line.Substring(index + 1).Trim();
                                
                if (key.Length == 0) continue;

                ret[key] = val;
            }

            return ret;
        }
    }
}
