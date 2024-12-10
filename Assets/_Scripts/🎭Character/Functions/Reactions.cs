using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class Reactions : MonoBehaviour
{
    private Character _npc;

    public void Initialize(Character npc) => _npc = npc;

    public void ReactToAction(Character sender, bool intendedRecieverOfMessage, Mind.ActionType actionType, ActionPost actionPost)
    {

        switch (actionType)
        {
            case Mind.ActionType.findKnowledge:
                switch (actionPost.Parameter)
                {
                    case Mind.KnowledgeType.location:
                        Mind.KnowledgeTag[] originalEnumList = actionPost.Tags.Cast<Mind.KnowledgeTag>().ToArray();
                        List<System.Enum> knowledge = _npc.Memory.GetLocationsByTag(originalEnumList).Cast<System.Enum>().ToList();
                        SocialHelper.ShareKnowledge(_npc, sender, Mind.KnowledgeType.location, knowledge);
                        break;
                }

                break;
        }
    }


}
