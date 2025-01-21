using Mind;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEngine.GraphicsBuffer;

public static class BaseAction
{
    private static void SetState(Character character, StateType stateType)
    {
        character.State.SetState(stateType);
    }


    public static void MoveTo(Character subject, Vector3 destination)
    {
        SetState(subject, StateType.normal);
        subject.Movement.MoveTo(destination);
    }
    public static void RentItem(ObjectType objectType, Character buyer, Character seller)
    {
        SetState(buyer, StateType.normal);
        int price = seller.Memory.GetPrice(objectType);
        var workLocation = WorldManager.Instance.GetLocation(seller.Memory.GetLocationTarget(TargetLocationType.work));
        var canGetPossession = workLocation.GetPossession(objectType);

        if (canGetPossession == null)
        {
            seller.Ui.Speak($"There are no more rooms available");
        }
        else if (!ActionsHelper.FinancialTransaction(buyer, seller, price))
        {
            seller.Ui.Speak($"That's not enough coin");
        }
        else
        {

            var boughtItem = workLocation.RemoveFromPossessions(objectType);
            buyer.Memory.AddToPossessions(objectType, boughtItem);
            boughtItem.ObjectActions.TryRenting(buyer, workLocation, 24);
            seller.Ui.Speak($"Here is the key to your room");

        }

    }


    public static void BuyItem(ObjectType objectType, Character buyer, Character seller)
    {
        SetState(buyer, StateType.normal);
        int price = seller.Memory.GetPrice(objectType);

        bool canGetPossession = seller.Memory.Possessions.ContainsKey(objectType);

        if (canGetPossession)
        {
            seller.Ui.Speak($"I don't have any more");
        }
        else if (!ActionsHelper.FinancialTransaction(buyer, seller, price))
        {
            seller.Ui.Speak($"That's not enough coin");
        }
        else
        {

            var boughtItem = seller.Memory.RemoveFromPossessions(objectType);
            buyer.Memory.AddToPossessions(objectType, boughtItem);
            buyer.Memory.AddToInventory(objectType, boughtItem);
            seller.Ui.Speak($"Good doing business with you");

        }

    }





    public static void InteractWithObject(WorldObject objectToUse, Character userOfObject, ObjectInteractionType interactionType)
    {
        if (!(userOfObject.State.ActionState == StateType.sleeping && objectToUse.ObjectType == ObjectType.bed))
            SetState(userOfObject, StateType.normal);
        if (ActionsHelper.Reached(userOfObject, objectToUse.transform.position, 0.3f))
        {
            objectToUse.ObjectActions.TryInteractWithObject(userOfObject, interactionType);

        }



    }

    public static void HurtSomeone(Character subject, Character target, float dammage)
    {
        SetState(subject, StateType.normal);
        target.Vitality.Hurt(dammage);
    }

}
