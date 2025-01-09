
using UnityEngine;



public static class ActionsHelper
{
    //aimlessly look around
    public static void Wander(Character character) => character.Movement.MoveToRandomPoints();

    //specifically go into every building to see if we can find what we are looking for
    public static void LookAroundArea(Character character) => character.Ui.CurrentStepInAction = "looking around area";

    public static bool FinancialTransaction(Character buyer, Character seller, int price)
    {
        if (buyer.Memory.Coin >= price)
        {
            buyer.Memory.Coin -= price;
            seller.Memory.Coin += price;
            return true;
        }

        return false;
    }

    public static bool Reached(Character character, Vector3 destination, float distance)
    {
        BaseAction.MoveTo(character, destination);
 
        float remainingDistance = character.Movement.Agent.remainingDistance;



        if (!character.Movement.Agent.pathPending &&  remainingDistance < distance)
        {
            character.Movement.Stop();
            return true;
        }


  

        return false;
    }



    public static bool WanderAndSearchForCharacter(NPC character, Mind.TargetType targetType, bool alive, params Mind.TraitType[] traitsToLookFor)
    {
        character.Ui.CurrentStepInAction = "wander and search";


        Wander(character);


        var target = character.Senses.SeeSomeoneWithTraits(traitsToLookFor);
        if (target != null && !(alive && target.State.CurrentState!=StateType.dead))
        {

            character.Memory.Targets[targetType] = target;
            return true;


        }

        return false;

    }
    //TODO: change from object type to tags so we can look up a general object of description
    //TODO: add that we can look for an object at a certain location so that we dont stray too far
    public static bool WanderAndSearchForObject(NPC character, Mind.ObjectType objectType)
    {
        character.Ui.CurrentStepInAction = "wander and search";


        Wander(character);


        var target = character.Senses.SeeObjectOfType(objectType);
        if (target != null)
        {
            character.Ui.CurrentStepInAction = "found object";

            if (PickUpObject(character, target))
            { return true; }
        }


        return false;


    }
    public static void EndThisBehaviour(NPC character)
    {
        character.Acting.StepInAction = 0;
        (character as NPC).Thinking.CalculateHighestScoringBehavior();
    }

    public static bool PickUpObject(Character character, WorldObject targetObject)
    {

        character.Ui.CurrentStepInAction = "pick up object";
        if (Reached(character, targetObject.transform.position, 1f))
        {

            //TODO: issue will occure if we want multiple objects of same type so change it to a list of objects <objectType, List<gameobject>>
            character.Memory.AddToPossessions(targetObject.ObjectType, targetObject);
            character.Memory.AddToInventory(targetObject.ObjectType, targetObject);

            character.Ui.CurrentStepInAction = "picked up object";
            return true;

        }

        return false;
    }

    public static void DestroyObject(Character character, WorldObject objectToDestroy)
    {
        character.Memory.RemoveObjectFromPossessions(objectToDestroy);
        character.Memory.RemoveObjectFromInventory(objectToDestroy);
    }


}
