﻿using System;
using UnityEngine;
using System.Collections;


public static class ActionsHelper
{
    //aimlessly look around
    public static void Wander(NPC npc) => npc.Movement.MoveToRandomPoints();

    //specifically go into every building to see if we can find what we are looking for
    public static void LookAroundArea(NPC npc) => npc.Logger.CurrentStepInAction = "looking around area";

    public static bool Reached(NPC npc, Vector3 destination)
    {

        var ourPosition = npc.transform.position;
        var interactionDistance = 1f;


        if (Vector3.Distance(ourPosition, destination) < interactionDistance)
        {
            return true;
        }

        npc.Movement.MoveTo(destination);

        return false;
    }



    public static IEnumerator WanderAndSearchForCharacter(NPC npc, Mind.TargetType targetType, bool alive, Action onComplete, params Mind.TraitType[] traitsToLookFor)
    {
        npc.Logger.CurrentStepInAction = "wander and search";

        while (true)
        {
            Wander(npc);


            var target = npc.Senses.SeeSomeoneWithTraits(traitsToLookFor);
            if (target != null && !(alive && target.GetComponent<Character>().Vitality.Dead))
            {

                npc.Memory.Targets[targetType] = target;

                onComplete?.Invoke();
                break;
            }

            yield return new WaitForSeconds(0.5f);
        }


    }
    //TODO: change from object type to tags so we can look up a general object of description
    //TODO: add that we can look for an object at a certain location so that we dont stray too far
    public static bool WanderAndSearchForObject(NPC npc, Mind.ObjectType objectType)
    {
        npc.Logger.CurrentStepInAction = "wander and search";


        Wander(npc);


        var target = npc.Senses.SeeObjectOfType(objectType);
        if (target != null)
        {
            npc.Logger.CurrentStepInAction = "found object";

            if (PickUpObject(npc, target))
            { return true; }
        }


        return false;


    }
    public static void EndThisBehaviour(NPC npc) => npc.Thinking.CalculateHighestScoringBehavior();

    public static bool PickUpObject(NPC npc, WorldObject targetObject)
    {

        npc.Logger.CurrentStepInAction = "pick up object";
        if (Reached(npc, targetObject.transform.position))
        {
            targetObject.gameObject.SetActive(false);
            //TODO: issue will occure if we want multiple objects of same type so change it to a list of objects <objectType, List<gameobject>>
            npc.Memory.AddToPossessions(targetObject.ObjectType, targetObject);
            npc.Memory.AddToInventory(targetObject.ObjectType, targetObject);

            npc.Logger.CurrentStepInAction = "picked up object";
            return true;

        }

        return false;
    }


}
