using System.Collections.Generic;
using Mind;
using NUnit.Framework;
using UnityEngine;

public static class DiaMenuHelper
{
    public static DiaPackage OpenInteractionWithCharacter(Character characerWeAreSpeakingTo)
    {
        string charactersFileName = DiaCharacterFileHelper.GetFileName(characerWeAreSpeakingTo);
        return DiaReader.OpenNewDialogue(charactersFileName);
    }


    public static List<MenuOption> ConvertDiaOptionToMenuOptions(List<DiaOption> diaOptions)
    {
        List<MenuOption> menuOptions = new();

        foreach (var diaOption in diaOptions)
        {
            MenuOption menuOption = new(diaOption.Label, diaOption.Index, diaOption.Action)
            {
                menuOptionType = MenuOptionType.dia
            };
            menuOptions.Add(menuOption);
        }

        return menuOptions;
    }

    public static void ExecuteAction(Character player, Character personWeAreSpeakingTo, DiaActionType? actionType, ScriptedTaskType? actionData)
    {
        if (actionType != null)
        {
            switch (actionType)
            {
                case DiaActionType.scriptedAction:
                    if (actionData.HasValue)
                    {
                        personWeAreSpeakingTo.Memory.ScriptedTaskProgress[actionData.Value] = ScriptedTaskProgressType.activated;
                    }

                    break;
                }
        }
    }
}
