using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
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
                menuOptionCost = diaOption.ActionCost,
                OptionNeeds = diaOption.OptionNeeds,
                OptionKey = diaOption.OptionKey,
                IsKey = diaOption.IsKey,
                UniqueId = diaOption.UniqueId,
                OptionNumber = diaOption.OptionNumber,
                OldOption = diaOption.OldOption,
                OptionMoodReq = diaOption.OptionMoodReq
            };
            menuOptions.Add(menuOption);
        }


        return menuOptions;
    }


    public static SocializeType ExecuteAction(Character player, Character personWeAreSpeakingTo, DiaActionType? actionType, object actionData)
    {
        SocializeType socialAction = SocializeType.none;
        if (actionType != null)
        {
            switch (actionType)
            {
                case DiaActionType.scriptedAction:
                    if (actionData != null)
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
                case DiaActionType.endGame:
                    if (actionData != null)
                    {
                        GameManager.Instance.EndGameState = (GameState)actionData;

                        if (GameManager.Instance.EndGameState==GameState.lost)
                        {
                            GameManager.Instance.EndingText="You pushed Onar too far and was unable to find out what happened to Ashla.";
                        }
                        else
                        if (GameManager.Instance.EndGameState == GameState.won)
                        {
                            var onar = WorldManager.Instance.GetCharacter(CharacterName.Onar);
                            if(onar.Relationships.GetRelationshipWith(personWeAreSpeakingTo,player)<0)
                            {
                                if (player.KeyKnowledge.Keys[0]==Key.onarInnocent)
                                    GameManager.Instance.EndingText = "You used brutal force to find out that Onar was innocent and that Ashla was taken by the forest.";
                                else
                                if (player.KeyKnowledge.Keys[0] == Key.onarGuilty)
                                    GameManager.Instance.EndingText = "You used brutal force to find out that Ashla was murdered by Onar.";
                                else
                                if (player.KeyKnowledge.Keys[0] == Key.onarDead)
                                    GameManager.Instance.EndingText = "You used brutal force to find out that Ashla was murdered by Onar but Onar killed himself.";

                            }
                            else
                            {
                                if (player.KeyKnowledge.Keys[0] == Key.onarInnocent)
                                    GameManager.Instance.EndingText = "You found out that Onar was innocent and that Ashla was taken by the forest.";
                                else
                                if (player.KeyKnowledge.Keys[0] == Key.onarGuilty)
                                    GameManager.Instance.EndingText = "You found out that Ashla was murdered by Onar.";
                                else
                                if (player.KeyKnowledge.Keys[0] == Key.onarDead)
                                    GameManager.Instance.EndingText = "You found out that Ashla was murdered by Onar but Onar killed himself.";
                            }

                        }
                        GameManager.Instance.UpdateInteractionMenu(personWeAreSpeakingTo, "");
                    }

                    break;
                case DiaActionType.mood:
                    if (actionData != null)
                    {
                        var data = (MemoryTags)actionData;
                        var state = personWeAreSpeakingTo.State.VisualState[0];
                        var ok = false;
                        var currentMood = personWeAreSpeakingTo.State.VisualState[0];
                        if (data != MemoryTags.none && currentMood != data)
                        {
                            personWeAreSpeakingTo.State.SetVisualState(data);
                            GameManager.Instance.InteractionMenu.NewMood = $"@that made {personWeAreSpeakingTo.CharacterName} {data}@\n";
                        }

                    }

                    break;
                case DiaActionType.impression:
                    if (actionData != null)
                    {
                        var data = (SocialImpression)actionData;


                            var newImpression= personWeAreSpeakingTo.Impression.AddSocialImpression(data,1);
                        GameManager.Instance.InteractionMenu.NewImpression = $"*{personWeAreSpeakingTo.CharacterName} finds you {newImpression}*\n";
                    }

                    break;
                case DiaActionType.action_hangout:

                    socialAction = SocializeType.drinking;


                    break;
                case DiaActionType.action_rentRoom:
                    BaseAction.RentItem(ObjectType.bed, player, personWeAreSpeakingTo);

                    break;

            }
        }
        return socialAction;
    }
}
