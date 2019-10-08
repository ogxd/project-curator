using System;
using System.IO;
using UnityEngine;

[Serializable]
public class ProjectCuratorData {

    private const string JSON_PATH = "ProjectSettings/ProjectCuratorSettings.json";

    [SerializeField]
    private AssetInfo[] assetInfos;
    public static AssetInfo[] AssetInfos {
        get => Instance.assetInfos;
        set => Instance.assetInfos = value;
    }

    private static ProjectCuratorData instance;
    public static ProjectCuratorData Instance {
        get {
            if (instance == null) {
                if (File.Exists(JSON_PATH)) {
                    instance = JsonUtility.FromJson<ProjectCuratorData>(File.ReadAllText(JSON_PATH));
                }
                else {
                    instance = new ProjectCuratorData();
                    File.WriteAllText(JSON_PATH, JsonUtility.ToJson(instance));
                }
            }
            return instance;
        }
    }

    public static void Save() {
        File.WriteAllText(JSON_PATH, JsonUtility.ToJson(Instance));
    }
}