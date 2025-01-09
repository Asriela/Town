using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;
using System.Linq;
using Mind;

public class Reactions : MonoBehaviour
{
    private Character _npc;
    public Character PersonWeAreSpeakingTo { get; set; }

    public void Initialize(Character npc) => _npc = npc;

    public void ReactToActionStarter(bool intendedReceiverOfMessage, Character sender, Character aboutWho, Mind.ActionType actionType, ActionPost actionPost)
    {
        StartCoroutine(ReactToAction(intendedReceiverOfMessage, sender, aboutWho, actionType, actionPost));


    }

    private IEnumerator ReactToAction(bool intendedRecieverOfMessage, Character sender, Character aboutWho, Mind.ActionType actionType, ActionPost actionPost)
    {
        if (!intendedRecieverOfMessage)
        {
            yield break;
        }


        yield return new WaitForSeconds(1f);

        switch (actionType)
        {
            case Mind.ActionType.findKnowledge:
                switch (actionPost.Parameter)
                {
                    case Mind.KnowledgeType.location:
                        //  BasicFunctions.Log("❤request for knowledge received");
                        var originalEnumList = actionPost.KnowledgeTags.Cast<Mind.KnowledgeTag>().ToArray();
                        List<Enum> newKnowledge = _npc.Memory.GetLocationsByTag(originalEnumList).Cast<Enum>().ToList();
                        var originalTags = actionPost.KnowledgeTags;
                        SocialHelper.ShareKnowledgeAbout(_npc, sender, aboutWho, Mind.KnowledgeType.location, newKnowledge, originalTags);
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
                            _npc.PersonKnowledge.AddPerson(aboutWho, tags);

                            _npc.Relationships.RecalculateMyRelationshipWithEveryone();
                            var viewAboutMemoryTag = _npc.Views.GetView(_npc, tags[0]);
                            

                            switch (viewAboutMemoryTag)
                            {
                                case ViewTowards.unforgivable:
                                    _npc.Ui.Speak("WHAT!? GET OUT!! GET OUT NOW!!");
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
                                    _npc.Ui.Speak("I like that...");
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

                        break;
                }

                break;
        }
    }


}
