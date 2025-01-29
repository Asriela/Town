using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;
using System.Linq;
using Mind;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Reactions : MonoBehaviour
{
    private Character _npc;
    public Character PersonWeAreSpeakingTo { get; set; }

    public void Initialize(Character npc) => _npc = npc;

    public void ReactToActionStarter(bool intendedReceiverOfMessage, Character sender, Character knower, Character aboutWho, Mind.ActionType actionType, ActionPost actionPost)
    {
        StartCoroutine(ReactToAction(intendedReceiverOfMessage, sender, knower, aboutWho, actionType, actionPost));


    }

    private IEnumerator ReactToAction(bool intendedRecieverOfMessage, Character sender, Character knower, Character aboutWho, Mind.ActionType actionType, ActionPost actionPost)
    {
        _npc.Appearance.FlipSpriteToTarget(_npc, sender);
        if (!intendedRecieverOfMessage)
        {
            yield break;
        }

        List<Enum> newKnowledge;
        yield return new WaitForSeconds(1f);

        switch (actionType)
        {
            case Mind.ActionType.findKnowledge:

                switch (actionPost.Parameter)
                {
                    case Mind.KnowledgeType.location:

                        var knowledgeTags = actionPost.KnowledgeTags.Cast<Mind.KnowledgeTag>().ToArray();
                        newKnowledge = _npc.Memory.GetLocationsByTag(knowledgeTags).Cast<Enum>().ToList();
                        var originalTags = actionPost.KnowledgeTags;
                        SocialHelper.ShareKnowledgeAbout(_npc, sender, knower, aboutWho, Mind.KnowledgeType.location, newKnowledge, originalTags);
                        _npc.Ui.Speak(null,"Let me mark that on your map...");
                        break;
                    case Mind.KnowledgeType.person:

                      
                        if (_npc.Relationships.GetRelationshipWith(_npc, sender) >= (float)ViewTowards.positive)
                        {
                            var memTag = _npc.PersonKnowledge.GetRandomKnowledge(knower, aboutWho);
                            newKnowledge = memTag.Cast<Enum>().ToList();

                            SocialHelper.ShareKnowledgeAbout(_npc, sender, knower, aboutWho, Mind.KnowledgeType.person, newKnowledge, null);
                            if (memTag != null && memTag.Count > 0)
                            { _npc.Ui.Speak(_npc, DialogueHelper.GetTellDialogue(memTag[0], _npc,knower, aboutWho, aboutWho == WorldManager.Instance.ThePlayer)); }
                            else
                            {
                                _npc.Ui.Speak(_npc, "Not much." );
                            }
                        }
                        else
                        {

                            _npc.Ui.Speak(_npc, "Impress me and I will tell you more..");
                        }


                        break;
                }

                break;
            case Mind.ActionType.shareKnowledge:
                switch (actionPost.Parameter)
                {
                    case Mind.KnowledgeType.location:
                        // BasicFunctions.Log("❤knowledge received");
                        foreach (var knowledgeItem in actionPost.KnowledgeTags)
                        {
                            Mind.LocationName location = (Mind.LocationName)Enum.Parse(typeof(Mind.LocationName), knowledgeItem.ToString());
                            List<Mind.KnowledgeTag> tags = actionPost.OriginalTags.Cast<Mind.KnowledgeTag>().ToList();

                            _npc.Memory.AddLocation(location, tags);
                            _npc.Memory.AddLocationTarget(_npc.Memory.LatestLocationTargetType, location);


                        }

                        break;
                    case Mind.KnowledgeType.person:
                        foreach (var knowledgeItem in actionPost.KnowledgeTags)
                        {
                            List<Mind.MemoryTags> tags = actionPost.KnowledgeTags.Cast<Mind.MemoryTags>().ToList();
                            _npc.PersonKnowledge.AddKnowledge(knower, aboutWho, tags);

                            // Get relationship between the NPC and the sender
                            var relationshipState = (ViewTowards)(int)_npc.Relationships.GetRelationshipWith(_npc, sender);

                            if ( _npc is not Player)
                            {
                                // Check if there's a scripted dialogue response
                                var dialogueResponse = _npc.DialogueResponsesToPersonInformation.GetDialogue(aboutWho, tags[0], relationshipState);
                                var viewAboutMemoryTag = _npc.Views.GetView(_npc, tags[0]);

                                if (dialogueResponse != null)
                                {
                                    // Call scripted response handler
                                    HandleScriptedResponse(sender, aboutWho, tags[0], relationshipState);
                                }
                                else
                                {
                                    // Handle default response and relationship impact
                                    HandleDefaultRelationshipImpact(viewAboutMemoryTag, sender);
                                    HandleDefaultResponse(viewAboutMemoryTag);
                                }
                            }
                        }
                        break;
                }

                break;
        }
    }
    private void HandleScriptedResponse(Character sender, Character aboutWho, Mind.MemoryTags memoryTag, ViewTowards relationship)
    {
        string dialogue = _npc.DialogueResponsesToPersonInformation.GetDialogue(aboutWho, memoryTag, relationship);
        _npc.Ui.Speak(_npc,dialogue);

        // Get the response details to handle relationship impact and output tags
        var characterDialogues = _npc.DialogueResponsesToPersonInformation.PeopleDialogues[aboutWho];
        var memoryTagDialogue = characterDialogues.FirstOrDefault(view => view.memoryTags == memoryTag);
        var response = memoryTagDialogue.dialogueResponses.FirstOrDefault(d => d.relationshipState == relationship);

        if (response != null)
        {
            // Apply scripted relationship impact
            _npc.Relationships.AddInteractionEffect(SocializeType.tell, sender, response.relImpactOnMessenger);

            // Store output tags for future use
            foreach (var tagPair in response.tagsInDialogue)
            {
                // We use the knower and tagAboutWho from the CharacterMemoryTagPair to correctly associate the knowledge
                _npc.PersonKnowledge.AddKnowledge(tagPair.knower, tagPair.tagAboutWho, new List<Mind.MemoryTags> { tagPair.tag });
            }
        }
    }

    private void HandleDefaultRelationshipImpact(ViewTowards? view, Character sender)
    {
        if (view != null)
        {
            _npc.Relationships.AddInteractionEffect(SocializeType.tell, sender, (float)view);
            _npc.Relationships.RecalculateMyRelationshipWithEveryone();
        }
    }
    private void HandleDefaultResponse(ViewTowards? view)
    {
        if (_npc is not Player)
        {
            string response = view switch
            {
                ViewTowards.unforgivable => "WHAT!? I WON'T STAND FOR THIS!",
                ViewTowards.despise => "I HATE this!",
                ViewTowards.extremelyNegative => "I REALLY dont like that",
                ViewTowards.veryNegative => "I'm dont like that...",
                ViewTowards.negative => "uhhh...",
                ViewTowards.neutral => "Oh, okay...",
                ViewTowards.positive => "That's nice...",
                ViewTowards.veryPositive => "I like that...",
                ViewTowards.extremelyPositive => "I really like that!",
                ViewTowards.adore => "Oh! I love it!",
                ViewTowards.obsessed => "WOW! UNBELIEVABLE!",
                _ => "Oh ok.."
            };

            _npc.Ui.Speak(_npc, response);
        }
    }
}
