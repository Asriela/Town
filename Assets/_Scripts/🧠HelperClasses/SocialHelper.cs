﻿using Mind;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public static class SocialHelper
{


    public static void ShareKnowledgeAbout(Character senderOfMessage, Character recieverOfMessage, Character aboutPerson, KnowledgeType knowledgeType, List<System.Enum> newKnowledge, List<System.Enum> originalTags)
    {
        ActionPost actionPost = new(knowledgeType, newKnowledge, originalTags);
        SocialMediator.PostAction(senderOfMessage, recieverOfMessage, aboutPerson, ActionType.shareKnowledge, actionPost);

    }

    public static void ShareKnowledgeAbout(Character senderOfMessage, Character recieverOfMessage, Character aboutPerson, KnowledgeType knowledgeType, List<System.Enum> newKnowledge)
    {
        ActionPost actionPost = new(knowledgeType, newKnowledge, null);
        SocialMediator.PostAction(senderOfMessage, recieverOfMessage, aboutPerson, ActionType.shareKnowledge, actionPost);

    }
    
    public static void SocialAction(Character senderOfMessage, Character recieverOfMessage, SocializeType socialActionType)
    {
        ActionPost actionPost = new(socialActionType, null, null);
       // SocialMediator.PostAction(senderOfMessage, recieverOfMessage, ActionType.socialize, actionPost);
        var socialDialogue = senderOfMessage.Memory.SocialDialogue;
        if (socialDialogue != null)
        { senderOfMessage.Ui.Speak(socialDialogue); }

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
