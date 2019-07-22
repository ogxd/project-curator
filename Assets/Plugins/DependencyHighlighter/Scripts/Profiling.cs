using System;
using System.Collections.Generic;
using System.Diagnostics;

public static class Profiling {

    private static Dictionary<string, Stopwatch> stopwatches = new Dictionary<string, Stopwatch>();

    public static void Start(string key) {
        if (!stopwatches.ContainsKey(key))
            stopwatches.Add(key, Stopwatch.StartNew());
        else {
            stopwatches[key] = Stopwatch.StartNew();
        }
    }

    public static void End(string key) {
        TimeSpan time = EndTimer(key);
        UnityEngine.Debug.Log($"<color=#8000ff>{key} done in {time.ToString("mm':'ss':'fff")}</color>");
    }

    private static TimeSpan EndTimer(string key) {
        if (!stopwatches.ContainsKey(key))
            return TimeSpan.MinValue;
        Stopwatch sw = stopwatches[key];
        sw.Stop();
        stopwatches.Remove(key);
        return sw.Elapsed;
    }
}