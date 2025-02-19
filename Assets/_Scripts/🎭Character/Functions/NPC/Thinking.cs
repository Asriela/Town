﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thinking : MonoBehaviour
{

    private NPC _npc;

    public void Initialize(NPC npc)
    {
        _npc = npc;
        StartCoroutine(CheckBestBehaviorCoroutine());
    }


    [SerializeField] private float _checkInterval = 3f;

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
        int highestScore = int.MinValue; // To accommodate negative scores if needed
        if (_npc.Vitality.Dead)
        {
            return;
        }

        Behavior highestScoringBehavior = null;

        for (int i = 0; i < _npc.Memory.Traits.Count; i++)
        {
            var trait = _npc.Memory.Traits[i];
            BasicFunctions.Log($"Inspecting trait: {trait.name}",LogType.thinking);

            for (int j = 0; j < trait.Behaviors.Count; j++)
            {
                var behavior = trait.Behaviors[j];
                BasicFunctions.Log($"Inspecting behavior: {behavior.Name}", LogType.thinking);
                BasicFunctions.Log($"Are conditions met: {AreConditionsMet(behavior.Conditions)}", LogType.thinking);

                if (AreConditionsMet(behavior.Conditions))
                {
                    // Reverse the priority of behaviors (lower index = higher score)
                    int reversePriority = j + 1;

                    // Calculate the score
                    int score = behavior.Priority + reversePriority + i + 1;

                    // Update if the score is higher OR if there's a tie, pick the one later in the list
                    if (score > highestScore ||
                        (score == highestScore && highestScoringBehavior == null))
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
            BasicFunctions.Log($"Selected Behavior: {highestScoringBehavior.Name}", LogType.thinking);
        }
        else
        {
            BasicFunctions.Log("No valid behavior found.", LogType.thinking);
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
