using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Vitality : MonoBehaviour
{
    private NPC _npc;
    public void Initialize(NPC npc)
    {
        _npc = npc;
        _npc.Needs.Add(Mind.NeedType.sleep, 100);
        _npc.Needs.Add(Mind.NeedType.eat, 100);
        _npc.Needs.Add(Mind.NeedType.makeUndead, 100);
        _npc.Needs.Add(Mind.NeedType.murder, 100);
    }

    private void Update()
    {
        ManageNeeds();
    }

    private void ManageNeeds()
    {
        var needs = _npc.Needs;
        foreach (var needType in needs.Keys.ToList())
        {
            needs[needType] += 0.1f;

           // if (needs[needType] > 80)
              //  print($"Need to: {needType}");
        }
    }
}
