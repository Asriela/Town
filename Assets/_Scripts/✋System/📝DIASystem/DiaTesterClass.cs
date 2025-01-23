using UnityEngine;

public class DiaTesterClass : MonoBehaviour
{
    private void Start()
    {
        DiaReader.OpenNewDialogue("basic");
        DiaReader.ChooseOption(0);
        DiaReader.ChooseOption(0);
    }

    private void Update()
    {
        
    }
}
