using System;
using System.IO;
using UnityEngine;

internal class StreamingAssets {
    internal static string ReadFile(string folder, string file) {
        return Path.Combine(Path.Combine(Application.streamingAssetsPath, folder), file);
    }

    internal static string GetPath(string folder) {
        return Path.Combine(Application.streamingAssetsPath, folder);
    }
}