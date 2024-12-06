using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Mind;

public class Vitality : MonoBehaviour
{
    private NPC _npc;
    public Dictionary<NeedType, float> Needs { get; set; } = new();


    public void Initialize(NPC npc)
    {
        _npc = npc;
        Needs.Add(Mind.NeedType.sleep, 100);
        Needs.Add(Mind.NeedType.eat, 100);
        Needs.Add(Mind.NeedType.makeUndead, 100);
        Needs.Add(Mind.NeedType.murder, 100);
    }

    private void Update()
    {
        ManageNeeds();
    }

    private void ManageNeeds()
    {
        var needs = Needs;
        foreach (var needType in needs.Keys.ToList())
        {
            needs[needType] += 0.001f;

           // if (needs[needType] > 80)
              //  print($"Need to: {needType}");
        }
    }
}
