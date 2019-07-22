using System;
using System.Collections.Concurrent;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public sealed class MonoContext : MonoBehaviour {

    public static UnityEvent OnUpdate;

    public static UnityEvent OnApplicationFocused;

    // Name of MonoContext GameObject (Unique)
    const string NAME = "__MonoContext__";

    // Actions remaining to be dispatched in an Unity Thread
    private static ConcurrentQueue<Action> actionsToDispatch = new ConcurrentQueue<Action>();

    // Reference to unique MonoContext GameObject
    private static GameObject monoContextObject;

    private static MonoContext instance;
    public static MonoContext Instance {
		get {
            if (instance == null || monoContextObject == null) {
                // Takes existing context
                GetMonoContextObject();
                if (monoContextObject == null) {
                    // If no context exists, create one.
                    monoContextObject = new GameObject(NAME);
                    monoContextObject.hideFlags = HideFlags.HideInHierarchy;
                    instance = monoContextObject.AddComponent<MonoContext>();
                    if (Application.isPlaying) DontDestroyOnLoad(monoContextObject);
                } else {
                    instance = monoContextObject.GetComponent<MonoContext>();
                    if (instance == null)
                        instance = monoContextObject.AddComponent<MonoContext>();
                }
            }
            return instance;
        }
	}

    public static void GetMonoContextObject() {
        foreach (GameObject mcinst in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().Where(x => x.name == NAME)) {
            if (monoContextObject == null) {
                monoContextObject = mcinst;
            } else {
                int ccount = mcinst.GetComponents<MonoContext>().Count();
                DestroyImmediate(mcinst);
            }
        }
    }

    public static void Clear() {
        foreach (GameObject mcinst in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().Where(x => x.name == NAME)) {
            DestroyImmediate(mcinst);
        }
        monoContextObject = null;
        instance = null;
    }

    public static bool CheckInstance() {
        return Instance != null;
    }

    public static void DispatchInMainThread(Action action) {
        if (action != null) {
            actionsToDispatch.Enqueue(action);
        }
    }

    public void Update() {
        OnUpdate?.Invoke();
        dispatch();
    }

    private void dispatch() {
        while (actionsToDispatch.Count != 0) {
            actionsToDispatch.TryDequeue(out Action action);
            if (action != null) {
                action.Invoke();
            }
        }
    }

    private void OnApplicationFocus(bool focus) {
        if (focus) {
            OnApplicationFocused?.Invoke();
        }
    }
}