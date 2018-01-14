using System.IO;
using UnityEngine;

internal class StreamingAssets {
    internal static string ReadFile(string folder, string file) {
        return Path.Combine(Path.Combine(Application.streamingAssetsPath, folder), file);
    }
}