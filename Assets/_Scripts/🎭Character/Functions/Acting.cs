using UnityEngine;

public class Acting : MonoBehaviour
{
    private NPC _npc;
    public void Initialize(NPC npc) => _npc = npc;
    public Behavior CurrentBehavior { get; set; }

    private void Update()
    {
        PerformCurrentBehavior();
    }

    private void PerformCurrentBehavior()
    {
        //map action delegate to each kind of action and then have action methods which take npc and parameter
        //then do actions like have npc do npc.MoveTo()
    }
}
