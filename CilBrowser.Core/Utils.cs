/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2022,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace CilBrowser.Core
{
    public static class Utils
    {
        public static bool StrEquals(string left, string right)
        {
            return string.Equals(left, right, StringComparison.Ordinal);
        }

        public static string GetDirectoryNameFromPath(string path)
        {
            if (path.EndsWith("\\") || path.EndsWith("/"))
            {
                return Path.GetFileName(path.Substring(0, path.Length - 1));
            }
            else
            {
                return Path.GetFileName(path);
            }
        }

        public static string UrlAppend(string url, string str)
        {
            if (url.EndsWith("/", StringComparison.Ordinal)) return url + str;
            else return url + "/" + str;
        }

        public static int CompareTypes(Type x, Type y)
        {
            string s1 = x.FullName;
            string s2 = y.FullName;
            return string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase);
        }

        public static Dictionary<string, List<Type>> GroupByNamespace(Type[] types)
        {
            Dictionary<string, List<Type>> ret = new Dictionary<string, List<Type>>();

            for (int i = 0; i < types.Length; i++)
            {
                string ns = types[i].Namespace;

                if (ns == null) ns = string.Empty;

                List<Type> list;

                if (ret.ContainsKey(ns))
                {
                    list = ret[ns];
                }
                else
                {
                    list = new List<Type>();
                    ret[ns] = list;
                }

                list.Add(types[i]);
            }

            foreach (string key in ret.Keys)
            {
                List<Type> list = ret[key];
                list.Sort(CompareTypes);
            }

            return ret;
        }

        public static bool IsMatchingNamespaceFilter(Type t, string nsFilter)
        {
            if (nsFilter.Length == 0) return true;
            if (t == null) return false;

            return StrEquals(t.Namespace, nsFilter);
        }

        public static bool IsTypeInAssembly(Type t, Assembly ass)
        {
            if (t == null || ass == null) return false;

            Assembly typeAssembly = t.Assembly;

            if (typeAssembly == null) return false;

            string name1 = typeAssembly.GetName().Name;
            string name2 = ass.GetName().Name;
            return string.Equals(name1, name2, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
