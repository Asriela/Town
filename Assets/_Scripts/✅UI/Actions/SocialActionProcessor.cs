using System.Linq.Expressions;
using Mind;
using UnityEngine;

public static class SocialActionProcessor
{
    private static Character currentPersonWeAreSpeakingTo;
    public static SocialImpression announceImpressionChange= SocialImpression.none;
    public static string ProcessAction(Character personWeAreSpeakingTo, ActionOption action, ref bool actionFailed)
    {

        var ret = personWeAreSpeakingTo.CharacterName switch
        {
            CharacterName.Dohlson => ProcessDohlson(personWeAreSpeakingTo, action, ref actionFailed),
            CharacterName.Onar => ProcessOnar(personWeAreSpeakingTo, action, ref actionFailed),
            _ => ProcessDohlson(personWeAreSpeakingTo, action, ref actionFailed),
        };
        return ret;
    }
    static void ChangeImpression(SocialImpression impression, int amount)
    {

        var lastImpression = currentPersonWeAreSpeakingTo.Impression.GetSocialImpression();
       var newImpression= currentPersonWeAreSpeakingTo.Impression.AddSocialImpression(impression, amount);

        //if (lastImpression != newImpression)
            announceImpressionChange= newImpression;



    }
    static void ChangeMood(MemoryTags mood)
    {
        currentPersonWeAreSpeakingTo.State.SetVisualState(mood);
    }
    static void AddToBreakDown(int amount)
        {
        currentPersonWeAreSpeakingTo.Impression.ProgressToBreakdown+= amount;

    }
    static void RemoveFromBreakDown(int amount)
    {
        currentPersonWeAreSpeakingTo.Impression.ProgressToBreakdown -= amount;
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
                ret = "'Good afternoon to you as well, what can I help with?'";
                ChangeImpression(SocialImpression.kind, 1);

                break;
            case SocializeType.bribe:
                ret = "'Good afternoon to you as well, what can I help with?'";
                ChangeImpression(SocialImpression.stuckUp, 5);

                break;
            case SocializeType.joke:
                if (mood == MemoryTags.relaxed || mood == MemoryTags.drunk)
                {
                    ret = "You tell the joke about the hairy chair 'Hahaha good one!'";
                    ChangeImpression(SocialImpression.amusing, 1);
                }
                else
                {

                    ret = "'With all do respect I am not really in the mood for a joke right now'";
                    ChangeImpression(SocialImpression.annoying, 1);
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
                if(mood == MemoryTags.scared)
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


