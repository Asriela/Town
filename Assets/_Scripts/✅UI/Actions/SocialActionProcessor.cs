using Mind;
using UnityEngine;

public static class SocialActionProcessor
{
    private static Character currentPersonWeAreSpeakingTo;
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
    static void ChangeImpression(SocialImpression impression , int amount)
    {
        if (currentPersonWeAreSpeakingTo.Impression.GetSocialImpression()!= impression)
            GameManager.Instance.InteractionMenu.pastDialogue += $"*{currentPersonWeAreSpeakingTo.CharacterName} finds you {impression}*\n";
        currentPersonWeAreSpeakingTo.Impression.AddSocialImpression(impression, amount);

    }
    static void ChangeMood(MemoryTags mood)
    {
        currentPersonWeAreSpeakingTo.State.SetVisualState(mood);
    }
    private static string ProcessOnar(Character personWeAreSpeakingTo, ActionOption action, ref bool actionFailed)
    {
        currentPersonWeAreSpeakingTo = personWeAreSpeakingTo;
        actionFailed = false;
        var ret = "";
        var actionText = $"{action.Name} WORKED";
        var mood= personWeAreSpeakingTo.State.VisualState[0];
        switch (action.Enum)
        {
            case SocializeType.greet:
                ret = "'Good afternoon to you as well, what can I do you for?'";


                break;
            case SocializeType.smallTalk:
                if(mood==MemoryTags.distant || mood== MemoryTags.tense  || mood== MemoryTags.tense)
                {
                    ret="'Not sure if you can tell but I am not really in the mood for meaningless chatter..'";
                    ChangeImpression(SocialImpression.annoying, 1);
                    actionFailed = true;
                }
                else
                {

                    ret = "You talk about the weather 'Yeah its quite freezing up here.'";

                }

        

                break;
            case SocializeType.insult:
                ret = "He squints at you. 'Is that really the person you want to be? Insulting?'";

                actionFailed = true;
                ChangeImpression(SocialImpression.rude, 1);
                break;
            case SocializeType.threaten:
                ret = "You threaten to knock in his knees. 'I'm just an old man! Why would you say something like that!?'";


                ChangeImpression(SocialImpression.scary, 2);
                break;
            case SocializeType.beatUp:
                ret = "You punch him in the face. He is bleeding from the mouth he looks up at you with horror.";


                ChangeImpression(SocialImpression.evil, 2);
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
                actionText="FAIL: You awkwardly go in for a hug.";
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


