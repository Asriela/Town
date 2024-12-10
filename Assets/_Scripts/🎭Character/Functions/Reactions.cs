﻿using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class Reactions : MonoBehaviour
{
    private Character _npc;

    public void Initialize(Character npc) => _npc = npc;

    public void ReactToAction(Character sender, bool intendedRecieverOfMessage, Mind.ActionType actionType, ActionPost actionPost)
    {
        if (!intendedRecieverOfMessage)
        { return; }
        switch (actionType)
        {
            case Mind.ActionType.findKnowledge:
                switch (actionPost.Parameter)
                {
                    case Mind.KnowledgeType.location:
                        var originalEnumList = actionPost.KnowledgeTags.Cast<Mind.KnowledgeTag>().ToArray();
                        List<System.Enum> newKnowledge = _npc.Memory.GetLocationsByTag(originalEnumList).Cast<System.Enum>().ToList();
                        var originalTags = actionPost.KnowledgeTags;
                        SocialHelper.ShareKnowledge(_npc, sender, Mind.KnowledgeType.location, newKnowledge, originalTags);
                        break;
                }

                break;
            case Mind.ActionType.shareKnowledge:
                switch (actionPost.Parameter)
                {
                    case Mind.KnowledgeType.location:
                        foreach (var knowledgeItem in actionPost.KnowledgeTags)
                        {
                            Mind.LocationName location = (Mind.LocationName)Enum.Parse(typeof(Mind.LocationName), knowledgeItem.ToString());
                            List<Mind.KnowledgeTag> tags = actionPost.OriginalTags.Cast<Mind.KnowledgeTag>().ToList();

                            _npc.Memory.AddLocation(location, tags);
                        }

                        break;
                }

                break;
        }
    }


}
