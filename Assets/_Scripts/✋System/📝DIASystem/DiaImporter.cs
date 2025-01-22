using UnityEngine;
using System.IO;

public static class DiaImporter
{
    public static string GetDiaFileText(string file)
    {
        string path = Path.Combine(Application.dataPath, "Resources/DiaFiles", file + ".dia");

        if (!File.Exists(path))
        {
            Debug.LogError($"DiaImporter: File not found at {path}");
            return string.Empty;
        }

        return File.ReadAllText(path); // Read file directly, tabs preserved
    }
}
