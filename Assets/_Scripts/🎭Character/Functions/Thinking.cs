using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thinking : MonoBehaviour
{
    [SerializeField]
    private List<Trait> _traits = new();

    private NPC _npc;

    public void Initialize(NPC npc) => _npc = npc;

    public List<Trait> Traits => _traits;


    [SerializeField] private float _checkInterval = 3f;
    private void Start()
    {

        StartCoroutine(CheckBestBehaviorCoroutine());
    }

    private IEnumerator CheckBestBehaviorCoroutine()
    {
        while (true)
        {

            CalculateHighestScoringBehavior();

            yield return new WaitForSeconds(_checkInterval);
        }
    }



    public void CalculateHighestScoringBehavior()
    {
        if (_npc.Vitality.Dead)
        { return; }
        Behavior highestScoringBehavior = null;
        int highestScore = 0;


        for (int i = 0; i < _traits.Count; i++)
        {
            var trait = _traits[i];
            print($"inspecting trait: {trait.name}");


            for (int j = 0; j < trait.Behaviors.Count; j++)
            {

                var behavior = trait.Behaviors[j];
                print($"inspecting behaviour: {behavior.Name}");


                print($"are conditions met: {AreConditionsMet(behavior.Conditions)}");
                if (AreConditionsMet(behavior.Conditions))
                {

                    int score = behavior.Priority + j + 1 + i + 1;


                    if (score > highestScore)
                    {
                        highestScore = score;
                        highestScoringBehavior = behavior;
                    }
                }
            }
        }

        if (highestScoringBehavior != null)
        {
            _npc.Acting.CurrentBehavior = highestScoringBehavior;
            print($"Selected Behavior: {highestScoringBehavior.Name}");
        }
        else
        {
            print("No valid behaviour found.");
        }

    }


    private bool AreConditionsMet(List<Condition> conditions)
    {
        foreach (var condition in conditions)
        {
            if (!Conditions.CheckCondition(condition, _npc))
            {
                return false;
            }
        }
        return true;
    }
}
