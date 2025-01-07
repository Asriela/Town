using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;
using System.Linq;

public class Reactions : MonoBehaviour
{
    private Character _npc;
    public Character PersonWeAreSpeakingTo { get; set; }

    public void Initialize(Character npc) => _npc = npc;

    public void ReactToActionStarter(Character sender, bool intendedReceiverOfMessage, Mind.ActionType actionType, ActionPost actionPost)
    {
        StartCoroutine(ReactToAction( sender, intendedReceiverOfMessage, actionType, actionPost));


    }

    private IEnumerator ReactToAction(Character sender, bool intendedRecieverOfMessage, Mind.ActionType actionType, ActionPost actionPost)
    {
        if (!intendedRecieverOfMessage)
        {
            yield break; 
        }


        yield return new WaitForSeconds(3f); 

        switch (actionType)
        {
            case Mind.ActionType.findKnowledge:
                switch (actionPost.Parameter)
                {
                    case Mind.KnowledgeType.location:
                        //  print("❤request for knowledge received");
                        var originalEnumList = actionPost.KnowledgeTags.Cast<Mind.KnowledgeTag>().ToArray();
                        List<Enum> newKnowledge = _npc.Memory.GetLocationsByTag(originalEnumList).Cast<Enum>().ToList();
                        var originalTags = actionPost.KnowledgeTags;
                        SocialHelper.ShareKnowledge(_npc, sender, Mind.KnowledgeType.location, newKnowledge, originalTags);
                        break;
                }

                break;
            case Mind.ActionType.shareKnowledge:
                switch (actionPost.Parameter)
                {
                    case Mind.KnowledgeType.location:
                        // print("❤knowledge received");
                        foreach (var knowledgeItem in actionPost.KnowledgeTags)
                        {
                            Mind.LocationName location = (Mind.LocationName)Enum.Parse(typeof(Mind.LocationName), knowledgeItem.ToString());
                            List<Mind.KnowledgeTag> tags = actionPost.OriginalTags.Cast<Mind.KnowledgeTag>().ToList();

                            _npc.Memory.AddLocation(location, tags);
                            _npc.Memory.AddLocationTarget(_npc.Memory.LatestLocationTargetType, location);

                          
                        }

                        break;
                }

                break;
        }
    }


}
