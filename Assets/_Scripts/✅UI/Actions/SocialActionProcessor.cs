using System.Linq.Expressions;
using Mind;
using UnityEngine;

public static class SocialActionProcessor
{
    private static Character currentPersonWeAreSpeakingTo;
    static float smallTime = 3;
    static float mediumTime = 6;
    static float longTime = 10;
    static float firstTime = -1;
    public static SocialImpression announceImpressionChange = SocialImpression.none;
    public static string ProcessAction(Character personWeAreSpeakingTo, ActionOption action, ref bool actionFailed)
    {
        personWeAreSpeakingTo.Impression.AddToActionsPerformedCount(action.Enum);
        var ret = personWeAreSpeakingTo.CharacterName switch
        {
            CharacterName.Dohlson => ProcessDohlson(personWeAreSpeakingTo, action, ref actionFailed),
            CharacterName.Onar => ProcessOnar(personWeAreSpeakingTo, action, ref actionFailed),
            _ => ProcessDohlson(personWeAreSpeakingTo, action, ref actionFailed),
        };

        personWeAreSpeakingTo.Impression.ActionsRecord.Add(action.Enum);
        personWeAreSpeakingTo.Impression.AllInputRecord.Add(action.Enum);
        personWeAreSpeakingTo.Impression.AddActionToTimePassed(action.Enum, action.TimeLength);


        return ret;
    }
    static void ChangeImpression(SocialImpression impression, int amount)
    {

        var lastImpression = currentPersonWeAreSpeakingTo.Impression.GetSocialImpression();
        var newImpression = currentPersonWeAreSpeakingTo.Impression.AddSocialImpression(impression, amount);

        //if (lastImpression != newImpression)
        announceImpressionChange = newImpression;



    }
    static void ChangeMood(MemoryTags mood)
    {
        currentPersonWeAreSpeakingTo.State.SetVisualState(mood);
    }
    static void AddToBreakDown(int amount)
    {
        currentPersonWeAreSpeakingTo.Impression.ProgressToBreakdown += amount;

    }
    static void RemoveFromBreakDown(int amount)
    {
        currentPersonWeAreSpeakingTo.Impression.ProgressToBreakdown -= amount;
    }
    static bool LastActionIs(SocializeType action, int howFarBack)
    {
        var actionsRecord = currentPersonWeAreSpeakingTo.Impression.ActionsRecord;
        var listSize = actionsRecord.Count;
        var indexToInspect = listSize - howFarBack;
        if (listSize > 0 && indexToInspect >= 0 && actionsRecord[indexToInspect] == action)
            return true;
        else
            return false;
    }

    static bool LastInputIs(SocializeType action, int howFarBack)
    {
        var actionsRecord = currentPersonWeAreSpeakingTo.Impression.AllInputRecord;
        var listSize = actionsRecord.Count;
        var indexToInspect = listSize - howFarBack;
        if (listSize > 0 && indexToInspect >= 0 && actionsRecord[indexToInspect] == action)
            return true;
        else
            return false;
    }
    static int InputCount()
    {
        return currentPersonWeAreSpeakingTo.Impression.AllInputRecord.Count;

    }
    static float TimePassedSinceAction(SocializeType action)
    {
        var timeDic = currentPersonWeAreSpeakingTo.Impression.TimePassedSinceAction;
        if (timeDic.ContainsKey(action))
            return currentPersonWeAreSpeakingTo.Impression.TimePassedSinceAction[action];
        else
            return -1f;
    }

    static bool EnoughTimePassedSinceAction(SocializeType action, float time)
    {
        if (TimePassedSinceAction(action) == -1 || TimePassedSinceAction(action) >= time)
        {
            return true;
        }
        else
            return false;
    }

    static int GetActionCount(SocializeType action)
    {
        var countDic = currentPersonWeAreSpeakingTo.Impression.ActionsPerfomedCount;
        if (countDic.ContainsKey(action))
        {
            return countDic[action];
        }
        else
            return 0;
    }
    private static string ProcessOnar(Character personWeAreSpeakingTo, ActionOption action, ref bool actionFailed)
    {
        currentPersonWeAreSpeakingTo = personWeAreSpeakingTo;

        actionFailed = false;
        var ret = "";
        var pronoun = "he";
        var actionText = $"{action.Name} WORKED";
        var mood = personWeAreSpeakingTo.State.VisualState[0];
        switch (action.Enum)
        {
            case SocializeType.greet:
                if (InputCount() <= 1)
                {
                    ret = "You greet him. 'Good afternoon to you as well!'";
                    ChangeImpression(SocialImpression.kind, 1);
                }
                else
                {
                    ret = "'Isn't it a little too late to greet?'";
                    ChangeImpression(SocialImpression.confusing, 3);
                }


                break;
            case SocializeType.bribe:
              
                if (EnoughTimePassedSinceAction(SocializeType.bribe, smallTime))
                {
                    ChangeImpression(SocialImpression.stuckUp, 4);
                    ret = "He takes the bribe, he looks at you concerned at what you will want in return.";
                }
                else
                {
                    ChangeImpression(SocialImpression.stuckUp, 10);
                    ret = "He takes the bribe but with a kind of bitterness. Its clear he is taking it more out of fear than desire for any coin.";
                }
                break;
            case SocializeType.joke:
                var jokeCount = GetActionCount(SocializeType.joke);
                var jokeString = "You tell the joke about the " + (jokeCount == 1 ? "" : jokeCount == 2 ? "" : "");

                if (mood == MemoryTags.relaxed || mood == MemoryTags.drunk)
                {
                    if(mood == MemoryTags.drunk)
                    {
                        ret = $"{jokeString} 'Hahaha good one!'";
                        ChangeImpression(SocialImpression.amusing, 1);
                    }
                    else
                    if (EnoughTimePassedSinceAction(SocializeType.joke, smallTime))
                    {
                        if (LastActionIs(SocializeType.joke, 1))
                        {
                            ret = $"{jokeString} 'Haha..'";
                            ChangeImpression(SocialImpression.boring, 1);
                        }
                        else
                        {
                            ret = $"{jokeString} 'Hahaha good one!'";
                            ChangeImpression(SocialImpression.amusing, 1);
                        }


                    }
                    else
                    {
                        ret = $"{jokeString} 'Another joke..?'";
                        ChangeImpression(SocialImpression.annoying, 3);
                        actionFailed = true;

                    }

                    ret = $"{jokeString} 'Hahaha good one!'";
                    ChangeImpression(SocialImpression.amusing, 1);
                }
                else
                {

                    ret = "'With all do respect I am not really in the mood for a joke right now'";
                    ChangeImpression(SocialImpression.annoying, 1);
                    actionFailed = true;
                }


                break;
            case SocializeType.makeTea:

                //FIRST CHECK FOR REAL REPITITIONS
                if (EnoughTimePassedSinceAction(SocializeType.makeTea, mediumTime))
                {
                    if (LastActionIs(SocializeType.makeTea, 1))
                    {
                        ret = "You make some tea and give it to him. He smiles.";
                        ChangeImpression(SocialImpression.boring, 1);
                    }
                    else
                    {
                        ret = "You make some tea and give it to him. He smiles.";
                        ChangeImpression(SocialImpression.kind, 1);
                    }


                }
                else
                {
                    ret = "'No thank you, I think I have had enough' ";
                    ChangeImpression(SocialImpression.confusing, 3);
                    actionFailed = true;

                }


                break;
            case SocializeType.smallTalk:
                if (mood == MemoryTags.relaxed || mood == MemoryTags.drunk)
                {
                    ret = "You talk about the weather 'Yeah its quite freezing up here.'";
                    ChangeImpression(SocialImpression.kind, 1);
                }
                else
                {
                    ret = "'Not sure if you can tell but I am not really in the mood for meaningless chatter..'";
                    ChangeImpression(SocialImpression.annoying, 1);
                    actionFailed = true;

                }



                break;
            case SocializeType.insult:
                if (mood == MemoryTags.scared)
                {
                    ret = "He looks down in shame.";


                    ChangeImpression(SocialImpression.rude, 2);
                }
                else
                {
                    ret = "He squints at you. 'Is that really the person you want to be? Insulting?'";

                    actionFailed = true;
                    ChangeImpression(SocialImpression.rude, 1);
                }

                break;
            case SocializeType.threaten:
                ret = "You threaten to knock in his knees. 'I'm just an old man! Why would you say something like that!?'";


                ChangeImpression(SocialImpression.evil, 1);
                AddToBreakDown(1);
                break;
            case SocializeType.manipulate:
                var manCount = GetActionCount(SocializeType.manipulate);
                var maniString = "You whisper into his ear " + (manCount == 1 ? "'You have been so lonely in this town, its terrible that I am the only one that came to check up on you.'" :
                    manCount == 2 ? "'Is this how they treat old men, they just let you suffer alone?'" :
                   manCount == 3 ? "'I know you did nothing wrong, you are just a sweet old man.'" :
                    "'You and I know what is real, the rest of this outpost knows nothing.'"
                   );

                ret = maniString;


                ChangeImpression(SocialImpression.authoritative, 2);
                break;
            case SocializeType.intimidate:
                ret = "You take out your sword and demands that he tells you what you need to know. He looks at you with fear in his eyes.";


                ChangeImpression(SocialImpression.scary, 2);
                break;
            case SocializeType.blackmail:
                ret = "You tell him how you know that Ashla was locked up in his house and that if he doesn't co-operate you will tell the rest of the town. He looks around with fear and panick.";


                ChangeImpression(SocialImpression.scary, 2);
                break;
            case SocializeType.messWithTheirHead:
                ret = "You tell him how he is just a piece of shit child molester and that how his darkness will consume him as he rots in jail. He looks around in horror his eyes darting trembling.";


                ChangeImpression(SocialImpression.evil, 4);
                AddToBreakDown(7);
                break;
            case SocializeType.beatUp:
                ret = "You punch him in the face. He is bleeding from the mouth he looks up at you with horror.";


                ChangeImpression(SocialImpression.evil, 3);
                AddToBreakDown(3);
                break;
        }
        if (actionFailed)
        { actionText = $"{action.Name} FAILED"; }
        ret = ret.Replace('\'', '"');
        return $"<color={MyColor.DarkGreyHex}>[{actionText}]</color> " + ret;
    }
    private static string ProcessDohlson(Character personWeAreSpeakingTo, ActionOption action, ref bool actionFailed)
    {
        currentPersonWeAreSpeakingTo = personWeAreSpeakingTo;
        actionFailed = false;
        var ret = "";
        var actionText = $"{action.Name} WORKED";
        var mood = personWeAreSpeakingTo.State.VisualState[0];
        var impression = currentPersonWeAreSpeakingTo.Impression.GetSocialImpression();
        switch (action.Enum)
        {
            case SocializeType.smallTalk:
                ret = "What, you think now is the time for small talk? Do they always fry assurance agents brains before sending them out into the field?";
                actionText = "FAIL: You make an attempt at small talk.";
                actionFailed = true;
                ChangeImpression(SocialImpression.annoying, 1);
                break;
            case SocializeType.hug:
                ret = "Get the fuck off me!";
                actionText = "FAIL: You awkwardly go in for a hug.";
                actionFailed = true;
                ChangeImpression(SocialImpression.annoying, 2);
                break;
            case SocializeType.joke:
                ret = "He just stares at you.";

                actionFailed = true;
                ChangeImpression(SocialImpression.annoying, 2);
                break;
            case SocializeType.puthandOnShoulder:
                if (impression != SocialImpression.annoying && impression != SocialImpression.infuriating)
                {

                    ret = "He looks at you and while you cant make out his expression you can tell by the slightest movement of his head , he appreciated it.";
                    actionText = "You put your hand on his shoulder.";
                    ChangeMood(MemoryTags.focused);
                }
                else
                {
                    ret = "'Get your hands off me.'";
                    ChangeImpression(SocialImpression.annoying, 2);
                    actionFailed = true;
                }


                break;
        }
        if (actionFailed)
        { actionText = $"{action.Name} FAILED"; }
        ret = ret.Replace('\'', '"');
        return $"<color={MyColor.DarkGreyHex}>[{actionText}]</color> " + ret;
    }

}


