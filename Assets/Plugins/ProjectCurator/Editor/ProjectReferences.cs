using UnityEngine;

namespace Ogxd.DependencyHighlighter
{
    public class ProjectReferences<T> : ScriptableObject where T : ScriptableObject
    {
        private static T instance;
        public static T Instance {
            get {
                if (instance == null)
                {
                    instance = Resources.Load<T>(typeof(T).Name);
                    if (instance == null)
                    {
                        instance = CreateInstance<T>();
#if UNITY_EDITOR
                        UnityEditor.AssetDatabase.CreateAsset(instance, $"Assets/Resources/{typeof(T).Name}.asset");
#else
                        Debug.LogError($"There is no '{typeof(T).Name}' in your resources. Access {typeof(T).Name} class in the Editor to initialize it.");
#endif
                    }
                }
                return instance;
            }
        }
    }
}