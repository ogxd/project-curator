using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public static class ProjectCurator {

    private const bool ONLY_ASSETS = true;

    public static bool upToDate = false;

    [NonSerialized]
    private static Dictionary<string, AssetInfo> pathToAssetInfo;

    static ProjectCurator() {
        pathToAssetInfo = new Dictionary<string, AssetInfo>();
        var assetInfos = ProjectCuratorData.AssetInfos;
        for (int i = 0; i < assetInfos.Length; i++) {
            pathToAssetInfo.Add(assetInfos[i].path, assetInfos[i]);
        }
    }

    public static AssetInfo GetAsset(string path) {
        AssetInfo assetInfo = null;
        pathToAssetInfo.TryGetValue(path, out assetInfo);
        return assetInfo;
    }

    public static void AddAssetToDatabase(string path) {

        if (ONLY_ASSETS && !path.StartsWith("Assets"))
            return;

        AssetInfo assetInfo;
        if (!pathToAssetInfo.TryGetValue(path, out assetInfo)) {
            pathToAssetInfo.Add(path, assetInfo = new AssetInfo(path));
        }

        var dependencies = assetInfo.GetDependencies();

        foreach (string dependency in dependencies) {
            if (ONLY_ASSETS && !dependency.StartsWith("Assets"))
                continue;
            if (dependency == assetInfo.path)
                continue;
            assetInfo.dependencies.Add(dependency);
            pathToAssetInfo[dependency].referencers.Add(assetInfo.path);
            // Included status may have changed and need to be recomputed
            pathToAssetInfo[dependency].ClearIncludedStatus();
        }
    }

    public static void RemoveAssetFromDatabase(string asset) {

        if (pathToAssetInfo.TryGetValue(asset, out AssetInfo assetInfo)) {
            foreach (string referencer in assetInfo.referencers) {
                if (pathToAssetInfo.TryGetValue(referencer, out AssetInfo referencerAssetInfo)) {
                    if (referencerAssetInfo.dependencies.Remove(asset)) {
                        referencerAssetInfo.ClearIncludedStatus();
                    } else {
                        // Non-Reciprocity Error
                        Debug.LogWarning($"Asset '{referencer}' that depends on '{asset}' doesn't have it as a dependency");
                    }
                } else {
                    Debug.LogWarning($"Asset '{referencer}' that depends on '{asset}' is not present in the database");
                }
            }
            foreach (string dependency in assetInfo.dependencies) {
                if (pathToAssetInfo.TryGetValue(dependency, out AssetInfo dependencyAssetInfo)) {
                    if (dependencyAssetInfo.referencers.Remove(asset)) {
                        dependencyAssetInfo.ClearIncludedStatus();
                    } else {
                        // Non-Reciprocity Error
                        Debug.LogWarning($"Asset '{dependency}' that is referenced by '{asset}' doesn't have it as a referencer");
                    }
                } else {
                    Debug.LogWarning($"Asset '{dependency}' that is referenced by '{asset}' is not present in the database");
                }
            }
            pathToAssetInfo.Remove(asset);
        } else {
            Debug.LogWarning($"Asset '{asset}' is not present in the database");
        }
    }

    public static void ClearDatabase() {
        pathToAssetInfo.Clear();
    }

    public static void RebuildDatabase()
	{
        pathToAssetInfo = new Dictionary<string, AssetInfo>();

		var allAssetPaths = AssetDatabase.GetAllAssetPaths();

        EditorUtility.DisplayProgressBar("Building Dependency Database", "Gathering All Assets...", 0f);

        // Gather all assets
        for (int p = 0; p < allAssetPaths.Length; p++) {
            if (ONLY_ASSETS && !allAssetPaths[p].StartsWith("Assets"))
                continue;
            AssetInfo assetInfo = new AssetInfo(allAssetPaths[p]);
            pathToAssetInfo.Add(assetInfo.path, assetInfo);
		}

        // Find links between assets
        for (int p = 0; p < allAssetPaths.Length; p++) {
            if (p % 5 == 0) {
                var cancel = EditorUtility.DisplayCancelableProgressBar("Building Dependency Database", allAssetPaths[p], (float)p / allAssetPaths.Length);
                if (cancel) {
                    pathToAssetInfo = null;
                    break;
                }
            }
            AddAssetToDatabase(allAssetPaths[p]);
        }

		EditorUtility.ClearProgressBar();

        upToDate = true;

        SaveDatabase();
    }

    public static void SaveDatabase() {
        if (pathToAssetInfo == null)
            return;
        var assetInfos = new AssetInfo[pathToAssetInfo.Count];
        int i = 0;
        foreach (var pair in pathToAssetInfo) {
            assetInfos[i] = pair.Value;
            i++;
        }
        ProjectCuratorData.AssetInfos = assetInfos;
        ProjectCuratorData.Save();
    }
}