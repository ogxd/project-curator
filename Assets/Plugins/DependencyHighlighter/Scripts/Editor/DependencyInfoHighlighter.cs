using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityObject = UnityEngine.Object;

[InitializeOnLoad]
public static partial class DependencyInfoHighlighter
{
    private static Dictionary<string, DependencyInfo> guidToNode = new Dictionary<string, DependencyInfo>();

    private static HashSet<string> sceneGuids;

    static DependencyInfoHighlighter()
    {
        CheckDependencies();
        EditorApplication.projectChanged += ProjectChanged;
        //EditorApplication.hierarchyChanged += HierarchyChanged;
        EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGUI;
        EditorSceneManager.sceneSaved += EditorSceneManager_sceneSaved;
    }

    private static void EditorSceneManager_sceneSaved(Scene scene) {
        CheckDependencies();
    }

    private static void ProjectChanged() {
        CheckDependencies();
    }

    private static void HierarchyChanged() {
        CheckDependencies();
    }

    private static void CheckDependencies() {

        Profiling.Start("CheckDependencies");

        guidToNode.Clear();

        // Add info for each object in scene included in build
        var scenePaths = EditorBuildSettings.scenes.Select(x => x.path).ToArray();
        sceneGuids = new HashSet<string>(scenePaths.Select(x => AssetDatabase.AssetPathToGUID(x)));
        string[] dependencies = AssetDatabase.GetDependencies(scenePaths, true);
        for (int i = 0; i < dependencies.Length; i++) {
            new DependencyInfo(dependencies[i]);
        }

        // Add info for each object in project folder
        string[] paths = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);
        for (int i = 0; i < paths.Length; i++) {
            string localPath = paths[i].Replace(Application.dataPath, "Assets");
            if (Path.GetExtension(localPath) == ".meta")
                continue;
            new DependencyInfo(localPath);
        }

        Profiling.End("CheckDependencies");
    }

    private static void ProjectWindowItemOnGUI(string guid, Rect rect) {

        guidToNode.TryGetValue(guid, out DependencyInfo dependencyInfo);

        if (dependencyInfo != null) {
            Rect r = new Rect(rect.width + rect.x - 20, rect.y + 1, 16, 16);

            if (GUI.Button(r, (dependencyInfo.Included == IncludedInBuild.NotIncluded) ? ProjectIcons.LinkBlack : ProjectIcons.LinkBlue, GUIStyle.none)) {
                Rect tooltipRect = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 100, 20);
                PopupWindow.Show(tooltipRect, new DependencyInfoPopup(dependencyInfo));
            }
        }
    }
}