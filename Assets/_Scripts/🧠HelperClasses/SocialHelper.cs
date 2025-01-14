using Mind;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;


public static class SocialHelper
{


    public static void AskForKnowledgeAbout(Character senderOfMessage, Character recieverOfMessage, Character knower, Character aboutPerson, Mind.KnowledgeType knowledgeType, List<System.Enum> knowledgeTags)
    {

        ActionPost actionPost = new(knowledgeType, knowledgeTags, null);
        SocialMediator.PostAction(senderOfMessage, recieverOfMessage, knower,aboutPerson, Mind.ActionType.findKnowledge, actionPost);
        senderOfMessage.Ui.Speak($"Do you know of a {knowledgeType} that is {string.Join(" and ", knowledgeTags)}");

    }

    public static void ShareKnowledgeAbout(Character senderOfMessage, Character recieverOfMessage, Character knower, Character aboutPerson, KnowledgeType knowledgeType, List<System.Enum> newKnowledge, List<System.Enum> originalTags)
    {
        ActionPost actionPost = new(knowledgeType, newKnowledge, originalTags);
        SocialMediator.PostAction(senderOfMessage, recieverOfMessage, knower,aboutPerson, ActionType.shareKnowledge, actionPost);
        // senderOfMessage.Ui.Speak($"Yes let me mark it on your map");
    }

    public static void SocialAction(Character senderOfMessage, Character recieverOfMessage, SocializeType socialActionType)
    {
        ActionPost actionPost = new(socialActionType, null, null);
        // SocialMediator.PostAction(senderOfMessage, recieverOfMessage, ActionType.socialize, actionPost);

        var socialDialogue = senderOfMessage.Memory.SocialDialogue;
        if (socialDialogue != null)
        { senderOfMessage.Ui.Speak(socialDialogue); }

        float effectFromInteraction = recieverOfMessage.Relationships.AddInteractionEffect(socialActionType, senderOfMessage);

    }
}

public class ActionPost
{
    public System.Enum Parameter { get; set; }
    public List<System.Enum> KnowledgeTags { get; set; }

    public List<System.Enum> OriginalTags { get; set; }

    public ActionPost(System.Enum parameter, List<System.Enum> tags, List<System.Enum> originalTags)
    {
        Parameter = parameter;
        KnowledgeTags = tags;
        OriginalTags = originalTags;
    }

}
