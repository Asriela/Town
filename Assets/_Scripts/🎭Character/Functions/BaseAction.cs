using Mind;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEngine.GraphicsBuffer;

public static class BaseAction
{
    public static void RentItem(ObjectType objectType, Character buyer , Character seller)
    {
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
            boughtItem.StartRenting(buyer, workLocation, 24);
            seller.Ui.Speak($"Here is the key to your room");

        }

    }

    
    public static void BuyItem(ObjectType objectType, Character buyer, Character seller)
    {
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
        objectToUse.InteractWithObject(userOfObject, interactionType);

    }

    public static void HurtSomeone(Character target, float dammage)
    {
        target.Vitality.Hurt(dammage);
    }

}
