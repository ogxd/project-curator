using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ogxd.DependencyHighlighter
{

    public enum IncludedInBuild
    {
        Unknown,
        NotIncluded,
        Resource,
        Referenced,
        RuntimeScript,
        SceneInBuild,
    }

    public static partial class DependencyInfoHighlighter
    {
        public class DependencyInfo
        {

            public string path;
            public string Guid => AssetDatabase.AssetPathToGUID(path);
            public UnityObject Asset => AssetDatabase.LoadAssetAtPath<UnityObject>(path); 
            public List<DependencyInfo> dependencies = new List<DependencyInfo>();
            public List<DependencyInfo> referencers = new List<DependencyInfo>();

            private IncludedInBuild included;
            public IncludedInBuild Included {
                get {
                    if (included == IncludedInBuild.Unknown)
                    {
                        for (int r = 0; r < referencers.Count; r++)
                        {
                            if (referencers[r].Included != IncludedInBuild.NotIncluded)
                            {
                                included = IncludedInBuild.Referenced;
                                break;
                            }
                        }

                        bool isInEditor = false;

                        string[] directories = path.ToLower().Split('/');
                        for (int i = 0; i < directories.Length - 1; i++)
                        {
                            switch (directories[i])
                            {
                                case "editor":
                                    isInEditor = true;
                                    break;
                                case "resources":
                                    included = IncludedInBuild.Resource;
                                    break;
                                case "plugins":
                                    break;
                                default:
                                    break;
                            }
                        }

                        string extension = System.IO.Path.GetExtension(path);
                        switch (extension)
                        {
                            case ".cs":
                                if (isInEditor)
                                {
                                    included = IncludedInBuild.NotIncluded;
                                }
                                else
                                {
                                    included = IncludedInBuild.RuntimeScript;
                                }
                                break;
                            case ".unity":
                                if (sceneGuids.Contains(Guid))
                                    included = IncludedInBuild.SceneInBuild;
                                break;
                            default:
                                break;
                        }

                        if (included == IncludedInBuild.Unknown)
                        {
                            included = IncludedInBuild.NotIncluded;
                        }
                    }
                    return included;
                }
            }

            public DependencyInfo(string path)
            {
                this.path = path;

                string guid = Guid;
                if (!string.IsNullOrEmpty(guid) && !guidToNode.ContainsKey(guid))
                {
                    guidToNode.Add(guid, this);
                }
                else
                {
                    return;
                }

                var deps = AssetDatabase.GetDependencies(path);

                for (int i = 0; i < deps.Length; i++)
                {
                    if (deps[i] == path)
                        continue;

                    DependencyInfo node;
                    if (!guidToNode.TryGetValue(deps[i], out node))
                    {
                        node = new DependencyInfo(deps[i]);
                    }
                    if (node != this)
                    {
                        node.referencers.Add(this);
                        dependencies.Add(node);
                    }
                }
            }
        }
    }
}