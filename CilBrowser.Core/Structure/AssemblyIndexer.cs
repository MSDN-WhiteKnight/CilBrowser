/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CilBrowser.Core.Structure
{
    /// <summary>
    /// Indexes assembly contents and provides a tree structure that can be used to generate website
    /// </summary>
    public static class AssemblyIndexer
    {
        /// <summary>
        /// Gets a website structure tree for the disassembled code of the specified assembly
        /// </summary>
        public static DirectoryNode AssemblyToTree(Assembly ass, string nsFilter)
        {
            string name = ass.GetName().Name;
            DirectoryNode ret = new DirectoryNode(Utils.StrToIdentifier(name), TreeNodeKind.Assembly);
            ret.DisplayName = name;

            // Assembly manifest
            AssemblyManifestNode am = new AssemblyManifestNode(ass);
            ret.AddPage(am);

            // Namespaces
            Type[] types = ass.GetTypes();
            Dictionary<string, List<Type>> typeMap = Utils.GroupByNamespace(types);
            string[] namespaces = typeMap.Keys.ToArray();
            Array.Sort(namespaces);

            for (int i = 0; i < namespaces.Length; i++)
            {
                string nsid = Utils.StrToIdentifier(namespaces[i]);
                string nsText = namespaces[i];

                if (!string.IsNullOrEmpty(nsFilter))
                {
                    if (!Utils.StrEquals(nsText, nsFilter)) continue;
                }

                List<Type> nsTypes = typeMap[namespaces[i]];

                if (nsTypes.Count == 0) continue;

                if (nsTypes.Count == 1)
                {
                    BindingFlags all = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                        BindingFlags.NonPublic;

                    if (Utils.StrEquals(nsTypes[0].FullName, "<Module>") &&
                        nsTypes[0].GetFields(all).Length == 0 && nsTypes[0].GetMethods(all).Length == 0)
                    {
                        continue; //ignore namespace consisting only from empty <Module> type
                    }
                }

                if (string.IsNullOrEmpty(nsText)) nsText = "(No namespace)";

                if (string.IsNullOrEmpty(nsid)) nsid = "no-namespace";

                DirectoryNode nsNode = new DirectoryNode(nsid, TreeNodeKind.Namespace);
                nsNode.DisplayName = nsText;

                for (int j = 0; j < nsTypes.Count; j++)
                {
                    TypeNode typeNode = new TypeNode(nsTypes[j], typeMap);
                    nsNode.AddPage(typeNode);
                }

                ret.AddDirectory(nsNode);
            }

            return ret;
        }
    }
}
