using UnityEditor;
using UnityEngine;

namespace Ogxd.DependencyHighlighter {

    [InitializeOnLoad]
    public static partial class ProjectWindowOverlay
    {
        static ProjectWindowOverlay()
        {
            enabled = Enabled;
            EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGUI;
        }

        private static void ProjectWindowItemOnGUI(string guid, Rect rect)
        {
            if (enabled) {

                Rect r = new Rect(rect.width + rect.x - 20, rect.y + 1, 16, 16);

                AssetInfo assetInfo = ProjectCurator.GetAsset(AssetDatabase.GUIDToAssetPath(guid));
                if (assetInfo != null) {
                    if (GUI.Button(r, assetInfo.IsIncludedInBuild ? ProjectIcons.LinkBlue : ProjectIcons.LinkBlack, GUIStyle.none)) {
                        //Rect tooltipRect = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 100, 20);
                        //PopupWindow.Show(tooltipRect, new DependencyInfoPopup(dependencyInfo));
                    }
                } else {

                }
            }
        }

        private static bool enabled;

        public static bool Enabled {
            get {
                return enabled = EditorPrefs.GetBool("ProjectCurator_PWO");
            }
            set {
                EditorPrefs.SetBool("ProjectCurator_PWO", enabled = value);
            }
        }
    }
}