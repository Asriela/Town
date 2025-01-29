using UnityEngine;
using System.IO;

public static class DiaImporter
{
    public static string GetDiaFileText(string fileName)
    {
        // Build the path using Application.streamingAssetsPath and manually add the subdirectories
        string path = Application.streamingAssetsPath + "/DiaFiles/" + fileName + ".dia";

        // Check if the file exists
        if (!File.Exists(path))
        {
            Debug.LogError($"DiaImporter: File not found at {path}");
            return string.Empty;
        }

        // Read the file content
        return File.ReadAllText(path);
    }


}
