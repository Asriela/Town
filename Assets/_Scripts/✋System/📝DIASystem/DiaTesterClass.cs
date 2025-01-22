using UnityEngine;

public class DiaTesterClass : MonoBehaviour
{
    private void Start()
    {
        DiaReader.SetCurrentDialogueFile("basic");
        BasicFunctions.Log(DiaReader.GetDialogue(), LogType.dia);
    }
}
