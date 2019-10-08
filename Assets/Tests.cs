using UnityEditor;
using UnityEngine;

public static class Tests
{
    [MenuItem("Tests/GetDependencies For Selected")]
    static void GetDependencies()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeGameObject);
        var dependencies = AssetDatabase.GetDependencies(path);
        foreach (var dep in dependencies) {
            Debug.Log(AssetDatabase.LoadAssetAtPath(dep, typeof(Object)).name);
        }
    }

    [MenuItem("Tests/Get Dependencies For All")]
    static void GetDependenciesForAll() {
        string path = AssetDatabase.GetAssetPath(Selection.activeGameObject);
        var dependencies = AssetDatabase.GetDependencies(path);
        foreach (var dep in dependencies) {
            Debug.Log(AssetDatabase.LoadAssetAtPath(dep, typeof(Object)).name);
        }
    }
}