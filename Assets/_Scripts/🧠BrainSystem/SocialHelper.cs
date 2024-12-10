using Mind;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public static class SocialHelper
{
    public static void AskForKnowledge(Character senderOfMessage, Character recieverOfMessage, Mind.KnowledgeType knowledgeType, List<System.Enum> knowledgeTags)
    {
        ActionPost actionPost = new(knowledgeType, knowledgeTags);
        SocialMediator.PostAction(senderOfMessage, recieverOfMessage, Mind.ActionType.findKnowledge, actionPost);
    }

    public static void ShareKnowledge(Character senderOfMessage, Character recieverOfMessage, Mind.KnowledgeType knowledgeType, List<System.Enum> knowledgeTags)
    {
        ActionPost actionPost = new(knowledgeType, knowledgeTags);
        SocialMediator.PostAction(senderOfMessage, recieverOfMessage, Mind.ActionType.findKnowledge, actionPost);
    }
}

public class ActionPost
{
    public System.Enum Parameter { get; set; }
    public List<System.Enum> Tags { get; set; }

    public ActionPost(System.Enum parameter, List<System.Enum> tags)
    {
        Parameter = parameter;
        Tags = tags;
    }

}
