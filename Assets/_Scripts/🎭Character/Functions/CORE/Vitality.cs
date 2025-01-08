using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Mind;

public class Vitality : MonoBehaviour
{
    private Character _npc;
    public Dictionary<NeedType, float> Needs { get; set; } = new();

    public bool Dead { get; set; }

    public float Health { get; set; }

    public void Initialize(Character npc)
    {
        Dead = false;
        _npc = npc;
        Needs.Add(NeedType.sleep, 100);
        Needs.Add(NeedType.eat, 100);
        Needs.Add(NeedType.makeUndead, 100);
        Needs.Add(NeedType.murder, 100);
    }

    private void Update() => ManageNeeds();

    private void Die()
    {
        Dead = true;
        _npc.Appearance.State = AppearanceState.dead;
    }


    public void Hurt(float dammage)
    {
        Health -= dammage;

        if (Health < 0)
        {
            Health = 0;
            Die();
        }
    }

    private void ManageNeeds()
    {
        var needs = Needs;
        foreach (var needType in needs.Keys.ToList())
        {
            needs[needType] += 0.001f;

           // if (needs[needType] > 80)
              //  BasicFunctions.Log($"Need to: {needType}");
        }
    }
}
