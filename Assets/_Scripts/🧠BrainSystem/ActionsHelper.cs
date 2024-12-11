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


        if (Vector3.Distance(ourPosition, destination) < interactionDistance)
        {
            return true;
        }
        else
        { npc.Movement.MoveTo(destination); }

        return false;
    }



    public static IEnumerator WanderAndSearchForCharacter(NPC npc, Mind.TargetType targetType, bool alive, params Mind.TraitType[] traitsToLookFor)
    {
        npc.Logger.CurrentStepInAction = "wander and search";

        while (true)
        {
            Wander(npc);


            var target = npc.Senses.SeeSomeoneWithTraits(traitsToLookFor);
            if (target != null && !(alive && target.GetComponent<Character>().Vitality.Dead))
            {

                npc.Memory.Targets[targetType] = target;

                EndThisBehaviour(npc);
                break;
            }

            yield return new WaitForSeconds(0.5f);
        }


    }
    //TODO: change from object type to tags so we can look up a general object of description
    //TODO: add that we can look for an object at a certain location so that we dont stray too far
    public static IEnumerator WanderAndSearchForObject(NPC npc, Mind.ObjectType objectType)
    {
        npc.Logger.CurrentStepInAction = "wander and search";

        while (true)
        {
            Wander(npc);


            var target = npc.Senses.SeeObjectOfType(objectType);
            if (target != null)
            {
                npc.Logger.CurrentStepInAction = "found object";
                PickUpObject(npc, target);
                EndThisBehaviour(npc);

                break;
            }

            yield return new WaitForSeconds(0.5f);
        }


    }
    public static void EndThisBehaviour(NPC npc) => npc.Thinking.CalculateHighestScoringBehavior();

    public static void PickUpObject(NPC npc, WorldObject targetObject)
    {

        npc.Logger.CurrentStepInAction = "pick up object";
        if (Reached(npc, targetObject.transform.position))
        {

            //TODO: issue will occure if we want multiple objects of same type so change it to a list of objects <objectType, List<gameobject>>
            npc.Memory.Possessions[targetObject.ObjectType] = targetObject;
            npc.Memory.Inventory[targetObject.ObjectType] = targetObject;

            targetObject.gameObject.SetActive(false);

        }
    }
}
