using System.Collections.Generic;
using Mind;
using NUnit.Framework;
using UnityEngine;


public static class DiaMenuHelper
{
    public static DiaPackage OpenInteractionWithCharacter(Character characerWeAreSpeakingTo, DialogueFileType fileType)
    {
        string charactersFileName = DiaCharacterFileHelper.GetFileName(characerWeAreSpeakingTo, fileType);
        return DiaReader.OpenNewDialogue(charactersFileName);
    }


    public static List<MenuOption> ConvertDiaOptionToMenuOptions(List<DiaOption> diaOptions)
    {
        List<MenuOption> menuOptions = new();

        foreach (var diaOption in diaOptions)
        {
            MenuOption menuOption = new(diaOption.Label, diaOption.Index, diaOption.Action)
            {
                menuOptionType = MenuOptionType.dia,
                menuOptionCost = diaOption.ActionCost
            };
            menuOptions.Add(menuOption);
        }

        return menuOptions;
    }

    public static SocializeType ExecuteAction(Character player, Character personWeAreSpeakingTo, DiaActionType? actionType, object actionData)
    {
        SocializeType socialAction= SocializeType.none;
        if (actionType != null)
        {
            switch (actionType)
            {
                case DiaActionType.scriptedAction:
                    if (actionData!=null)
                    {
                        personWeAreSpeakingTo.Memory.ScriptedTaskProgress[(ScriptedTaskType)actionData] = ScriptedTaskProgressType.activated;
                    }

                    break;
                case DiaActionType.share:
                    if (actionData != null)
                    {
                        personWeAreSpeakingTo.PersonKnowledge.AddKnowledge(personWeAreSpeakingTo, player, new List<MemoryTags> { (MemoryTags)actionData });
                    }

                    break;
                case DiaActionType.action_hangout:
         
                        socialAction= SocializeType.drinking;
                    

                    break;
                case DiaActionType.action_rentRoom:
                    BaseAction.RentItem(ObjectType.bed, player, personWeAreSpeakingTo);

                    break;

            }
        }
        return socialAction;
    }
}
