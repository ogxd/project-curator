using System.Collections.Generic;
using UnityEditor;
using UnityObject = UnityEngine.Object;

public enum IncludedInBuild {
    Unknown,
    NotIncluded,
    Resource,
    Referenced,
    RuntimeScript,
    SceneInBuild,
}

public static partial class DependencyInfoHighlighter
{
    public class DependencyInfo {

        public string Path => AssetDatabase.GetAssetPath(item);
        public string Guid => AssetDatabase.AssetPathToGUID(Path);
        public UnityObject item;
        public List<DependencyInfo> dependencies = new List<DependencyInfo>();
        public List<DependencyInfo> referencers = new List<DependencyInfo>();

        private IncludedInBuild included;
        public IncludedInBuild Included {
            get {
                if (included == IncludedInBuild.Unknown) {
                    for (int r = 0; r < referencers.Count; r++) {
                        if (referencers[r].Included != IncludedInBuild.NotIncluded) {
                            included = IncludedInBuild.Referenced;
                            break;
                        }
                    }

                    bool isInEditor = false;

                    string[] directories = Path.ToLower().Split('/');
                    for (int i = 0; i < directories.Length - 1; i++) {
                        switch (directories[i]) {
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

                    string extension = System.IO.Path.GetExtension(Path);
                    switch (extension) {
                        case ".cs":
                            if (isInEditor) {
                                included = IncludedInBuild.NotIncluded;
                            }
                            else {
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

                    if (included == IncludedInBuild.Unknown) {
                        included = IncludedInBuild.NotIncluded;
                    }
                }
                return included;
            }
        }

        public DependencyInfo(UnityObject item) {
            this.item = item;

            if (!item || item == null || itemToNode.ContainsKey(item)) {
                return;
            }

            itemToNode.Add(item, this);

            string guid = Guid;
            if (!string.IsNullOrEmpty(guid)) {
                if (!guidToNode.ContainsKey(guid)) {
                    guidToNode.Add(guid, this);
                }
            }

            var deps = EditorUtility.CollectDependencies(new[] { item });
            for (int i = 0; i < deps.Length; i++) {
                DependencyInfo node;
                if (!itemToNode.TryGetValue(deps[i], out node)) {
                    node = new DependencyInfo(deps[i]);
                }
                if (node != this) {
                    node.referencers.Add(this);
                    dependencies.Add(node);
                }
            }
        }
    }
}