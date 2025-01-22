using UnityEngine;

public static class DiaReader
{
    private static string _currentFile;

    public static void SetCurrentDialogueFile(string fileName) =>
        _currentFile = DiaImporter.GetDiaFileText(fileName);

    public static string GetDialogue()
    {
        return _currentFile;
    }


}
