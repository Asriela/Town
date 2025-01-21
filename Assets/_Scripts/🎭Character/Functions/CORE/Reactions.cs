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
                        _npc.Ui.Speak("Let me mark that on your map...");
                        break;
                    case Mind.KnowledgeType.person:

                        if (_npc.Relationships.GetRelationshipWith(_npc, sender) >= (float)ViewTowards.positive)
                        {
                            var memTag = _npc.PersonKnowledge.GetRandomKnowledge(knower, aboutWho);
                            newKnowledge = memTag.Cast<Enum>().ToList();

                            SocialHelper.ShareKnowledgeAbout(_npc, sender, knower, aboutWho, Mind.KnowledgeType.person, newKnowledge, null);
                            if (memTag != null && memTag.Count > 0)
                            { _npc.Ui.Speak(DialogueHelper.GetTellDialogue(memTag[0])); }
                            else
                            {
                                _npc.Ui.Speak("Not much.");
                            }
                        }
                        else
                        {

                            _npc.Ui.Speak("Impress me and I will tell you more..");
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
                        // BasicFunctions.Log("❤knowledge received");
                        foreach (var knowledgeItem in actionPost.KnowledgeTags)
                        {

                            List<Mind.MemoryTags> tags = actionPost.KnowledgeTags.Cast<Mind.MemoryTags>().ToList();
                            _npc.PersonKnowledge.AddKnowledge(knower, aboutWho, tags);
                            var viewAboutMemoryTag = _npc.Views.GetView(_npc, tags[0]);

                            if (viewAboutMemoryTag != null && sender != aboutWho && _npc is not Player)
                            {
                                _npc.Relationships.AddInteractionEffect(SocializeType.tell, sender, (float)viewAboutMemoryTag);
                                _npc.Relationships.RecalculateMyRelationshipWithEveryone();
                            }



                            //if the sender is sharing info about someone else that isnt them we need to add a Interactioneffect


                            if (_npc is not Player)
                            {
                                switch (viewAboutMemoryTag)
                                {
                                    case ViewTowards.unforgivable:
                                        _npc.Ui.Speak("WHAT!? I WONT STAND FOR THIS!");
                                        break;

                                    case ViewTowards.despise:
                                        _npc.Ui.Speak("That's horrifying!");
                                        break;

                                    case ViewTowards.extremelyNegative:
                                        _npc.Ui.Speak("That's extremely upsetting.");
                                        break;

                                    case ViewTowards.veryNegative:
                                        _npc.Ui.Speak("Im not ok with that..");
                                        break;

                                    case ViewTowards.negative:
                                        _npc.Ui.Speak("I don't like that...");
                                        break;

                                    case ViewTowards.neutral:
                                        _npc.Ui.Speak("Oh ok..");
                                        break;

                                    case ViewTowards.positive:
                                        _npc.Ui.Speak("That's nice..");
                                        break;

                                    case ViewTowards.veryPositive:
                                        _npc.Ui.Speak("I really like that...");
                                        break;

                                    case ViewTowards.extremelyPositive:
                                        _npc.Ui.Speak("How wonderful!");
                                        break;

                                    case ViewTowards.adore:
                                        _npc.Ui.Speak("That's amazing!");
                                        break;

                                    case ViewTowards.obsessed:
                                        _npc.Ui.Speak("WOW! UNBELIEVABLE!");
                                        break;

                                    default:
                                        _npc.Ui.Speak("I don't know how I feel about that.");
                                        break;
                                }
                            }
                        }

                        break;
                }

                break;
        }
    }


}
