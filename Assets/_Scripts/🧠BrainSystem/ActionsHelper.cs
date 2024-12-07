using System;
using UnityEngine;
using System.Collections;
using Mind;

public static class ActionsHelper
{
    //aimlessly look around
    public static void Wander(NPC npc)
    {


        npc.Movement.MoveToRandomPoints();
    }
    //specifically go into every building to see if we can find what we are looking for
    public static void LookAroundArea(NPC npc)
    {
        npc.Logger.CurrentStepInAction = "looking around area";


    }

    public static bool Reached(NPC npc, Vector3 destination)
    {

        var ourPosition = npc.transform.position;
        var interactionDistance = 2f;
        npc.Movement.MoveTo(destination);

        if (Vector3.Distance(ourPosition, destination) < interactionDistance)
        {
            return true;
        }


        return false;
    }

    public static IEnumerator WanderAndSearch(NPC npc, Mind.TargetType targetType, bool alive, params Mind.TraitType[] traitsToLookFor)
    {
        npc.Logger.CurrentStepInAction = "wander and search";
        // Continuously wander until the NPC sees someone with the desired traits
        while (true)
        {
            Wander(npc); // Perform wandering behavior

            // Check if the NPC's senses detect someone with the desired traits
            var target = npc.Senses.SeeSomeoneWithTraits(traitsToLookFor); // Replace with actual parameters
            if (target != null && !(alive && target.GetComponent<Character>().Vitality.Dead))
            {

                npc.Memory.Targets[targetType] = target;
                Debug.Log($"Target found and assigned to memory: {target}");
                npc.Thinking.CalculateHighestScoringBehavior();
                break; // Exit the loop once the condition is met
            }

            yield return new WaitForSeconds(0.5f);
        }


    }

}
