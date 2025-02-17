using Mind;
using UnityEngine;

public static class SocialActionProcessor
{
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


    private static string ProcessDohlson(Character personWeAreSpeakingTo, ActionOption action, ref bool actionFailed)
    {
        var ret = "";
        var actionText= $"From {action.Name}";
        switch (action.Enum)
        {
            case SocializeType.smallTalk:
                ret = "What, you think now is the time for small talk? Do they always fry assurance agents brains before sending them out into the field?";
                actionText = "FAIL: You make an attempt at small talk.";
                actionFailed = true;
                personWeAreSpeakingTo.Impression.AddSocialImpression(SocialImpression.annoying, 1);
                break;
            case SocializeType.hug:
                ret = "Get the fuck off me!";
                actionText="FAIL: You awkwardly go in for a hug.";
                actionFailed = true;
                personWeAreSpeakingTo.Impression.AddSocialImpression(SocialImpression.annoying, 2);
                break;
            case SocializeType.puthandOnShoulder:
                ret = "He looks at you and while you cant make out his expression you can tell by the slightest movement of his head , he appreciated it.";
                actionText = "You put your hand on his shoulder.";

                personWeAreSpeakingTo.Impression.AddSocialImpression(SocialImpression.annoying, 2);
                break;
        }
        return $"<color={MyColor.DarkGreyHex}>[{actionText}]</color> " + ret;
    }
    private static string ProcessOnar(Character personWeAreSpeakingTo, ActionOption action, ref bool actionFailed)
    {
        actionFailed = false;
        var ret = "";
        var actionText = $"From {action.Name}";
        switch (action.Enum)
        {
            case SocializeType.insult:
                ret = "He squints at you. 'Is that really the person you want to be? Insulting?'";
                actionText = "FAIL: You try to insult.";
                actionFailed=true;
                personWeAreSpeakingTo.Impression.AddSocialImpression(SocialImpression.rude, 1);
                break;
            case SocializeType.threaten:
                ret = "I'm just an old man! Why are you threatening me?!";
                actionText = "You threaten to punch his legs in.";
               
                personWeAreSpeakingTo.Impression.AddSocialImpression(SocialImpression.scary, 2);
                break;
            case SocializeType.beatUp:
                ret = "He is bleeding from the mouth he looks up at you with horror.";
                actionText = "You punch him in the face.";
    
                personWeAreSpeakingTo.Impression.AddSocialImpression(SocialImpression.evil, 2);
                break;
        }
        return $"<color={MyColor.DarkGreyHex}>[{actionText}]</color> " + ret;
    }
}


