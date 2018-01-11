using UnityEngine;

public class PathUtilities
{
    static public string GetPath(string directory, string filename)
    {
        return System.IO.Path.Combine(
            Application.streamingAssetsPath, 
            System.IO.Path.Combine(directory, filename)
            );
    }
}