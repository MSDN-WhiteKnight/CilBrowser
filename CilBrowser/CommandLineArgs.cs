/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2022,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;
using CilBrowser.Core;

namespace CilBrowser
{
    class CommandLineArgs
    {
        Dictionary<string, string> namedArgs;
        string[] posArgs;

        public CommandLineArgs(string[] args, NamedArgumentDefinition[] defs)
        {
            namedArgs = new Dictionary<string, string>(args.Length);
            List<string> positional = new List<string>(args.Length);
            int i = 0;

            while(true)
            {
                if (i >= args.Length) break;

                if (IsSwitch(args[i]))
                {
                    if (NamedArgumentDefinition.IsArgumentWithoutValue(args[i], defs) || 
                        i == args.Length - 1 || IsSwitch(args[i + 1]))
                    {
                        //switch without value
                        namedArgs[args[i]] = string.Empty;
                    }
                    else
                    {
                        //switch with value
                        string name = args[i];
                        i++;
                        
                        string val = args[i];

                        if (val == null) val = string.Empty;

                        namedArgs[name] = val;
                    }
                }
                else
                {
                    //positional argument
                    string val = args[i];

                    if (val == null) val = string.Empty;

                    positional.Add(val);
                }

                i++;
            }

            this.posArgs = positional.ToArray();
        }

        static bool IsSwitch(string s)
        {
            if (s == null) return false;

            return s.StartsWith("-") || s.StartsWith("--");
        }

        public bool HasNamedArgument(string name)
        {
            return this.namedArgs.ContainsKey(name);
        }

        public string GetNamedArgument(string name)
        {
            string ret;
            if (this.namedArgs.TryGetValue(name, out ret)) return ret;
            else return null;
        }

        public int PositionalArgumentsCount
        {
            get { return this.posArgs.Length; }
        }

        public string GetPositionalArgument(int index)
        {
            return this.posArgs[index];
        }

        public int Count
        {
            get { return this.namedArgs.Count + this.posArgs.Length; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(1000);
            sb.AppendLine("Named: " + this.namedArgs.Count.ToString());

            foreach (string key in this.namedArgs.Keys)
            {
                sb.AppendFormat(" {0} = {1}", key, this.GetNamedArgument(key));
                sb.AppendLine();
            }

            sb.AppendLine();
            sb.AppendLine("Positional: " + this.posArgs.Length.ToString());

            for (int i = 0; i < this.posArgs.Length; i++)
            {
                sb.AppendFormat(" {0} = {1}", i, this.posArgs[i]);
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }

    class NamedArgumentDefinition
    {
        public NamedArgumentDefinition(string name, bool hasValue)
        {
            this.Name = name;
            this.HasValue = hasValue;
        }

        public string Name { get; set; }
        public bool HasValue { get; set; }

        public static bool IsArgumentWithValue(string name, IEnumerable<NamedArgumentDefinition> defs)
        {
            foreach (NamedArgumentDefinition def in defs)
            {
                if (def.HasValue && Utils.StrEquals(def.Name, name)) return true;
            }

            return false;
        }

        public static bool IsArgumentWithoutValue(string name, IEnumerable<NamedArgumentDefinition> defs)
        {
            foreach (NamedArgumentDefinition def in defs)
            {
                if (!def.HasValue && Utils.StrEquals(def.Name, name)) return true;
            }

            return false;
        }
    }
}
