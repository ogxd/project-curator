using UnityEditor;
using System.Collections.Generic;
using System;

namespace Nanolabo
{
    public class AssetProcessor : UnityEditor.AssetModificationProcessor
    {

        [InitializeOnLoadMethod]
        public static void Init()
        {
            EditorApplication.update += OnUpdate;
        }

        /// <summary>
        /// Some callbacks must be delayed on next frame
        /// </summary>
        private static void OnUpdate()
        {
            while (Actions.Count > 0) {
                Actions.Dequeue()?.Invoke();
            }
        }

        private static Queue<Action> Actions = new Queue<Action>();

        static string[] OnWillSaveAssets(string[] paths)
        {
            if (ProjectCurator.upToDate) {
                Actions.Enqueue(() => {
                    foreach (string path in paths) {
                        ProjectCurator.RemoveAssetFromDatabase(path);
                        ProjectCurator.AddAssetToDatabase(path);
                    }
                });
            }
            return paths;
        }

        static void OnWillCreateAsset(string assetName)
        {
            if (ProjectCurator.upToDate) {
                Actions.Enqueue(() => {
                    ProjectCurator.AddAssetToDatabase(assetName);
                });
            }
        }

        static AssetDeleteResult OnWillDeleteAsset(string assetName, RemoveAssetOptions removeAssetOptions)
        {
            if (ProjectCurator.upToDate) {
                ProjectCurator.RemoveAssetFromDatabase(assetName);
            }
            return AssetDeleteResult.DidNotDelete;
        }

        static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            if (ProjectCurator.upToDate) {
                Actions.Enqueue(() => {
                    ProjectCurator.RemoveAssetFromDatabase(sourcePath);
                    ProjectCurator.AddAssetToDatabase(destinationPath);
                });
            }
            return AssetMoveResult.DidNotMove;
        }
    }
}