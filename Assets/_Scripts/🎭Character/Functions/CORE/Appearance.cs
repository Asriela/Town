using UnityEngine;

public class Appearance : MonoBehaviour
{
    private Character _npc;
    public void Initialize(Character npc) => _npc = npc;

    public void LookDead()
    {
        var currentRotation = _npc.SpriteRenderer.transform.localRotation;
        _npc.SpriteRenderer.transform.localRotation = currentRotation * Quaternion.Euler(0, 0, 90);
    }
}
