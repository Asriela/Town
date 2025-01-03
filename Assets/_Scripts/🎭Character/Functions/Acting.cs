﻿using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Mind;
using System.Data.Common;
using static UnityEngine.GraphicsBuffer;

public class Acting : MonoBehaviour
{
    private NPC _npc;
    private Dictionary<ActionType, Action<object>> _actionHandlers;
    private WorldObject _currentObjectTarget = null;
    private int _stepInAction = 0;
    private string _lastAction = "";
    private float _waitBeforeAction = 0;

    public void Initialize(NPC npc) => _npc = npc;
    public Behavior CurrentBehavior { get; set; }

    private void Awake() =>
    _actionHandlers = new Dictionary<ActionType, Action<object>>
    {
        { ActionType.findCharacter, param => FindCharacter((TargetType)param) },
        { ActionType.findObject, param => FindObject((ObjectType)param) },
        { ActionType.fullfillNeed, param => FullfillNeed((NeedType)param) },
        { ActionType.kill, param => Kill((TargetType)param) },
        { ActionType.trader, param => TraderJob((TraderType)param) },
        { ActionType.farmer, param => FarmerJob((FarmerType)param) },
        { ActionType.findKnowledge, param => FindKnowledge((KnowledgeType)param) },
        { ActionType.gotoLocation, param => GotoLocation((TargetLocationType)param) },
        { ActionType.useObject, param => UseObject((ObjectType)param) },
        { ActionType.useObjectInInventory, param => UseObjectInInventory((ObjectType)param) },
        { ActionType.findOccupant, param => FindOccupant((TraitType)param) },
        { ActionType.gotoOccupant, param => GotoOccupant((TraitType)param) },
        { ActionType.buyItem, param => BuyItem((ObjectType)param) }
    };

    private void Update() => PerformCurrentBehavior();

    private void PerformCurrentBehavior()
    {

        if (CurrentBehavior == null)
        { return; }

        if (_actionHandlers.TryGetValue(CurrentBehavior.Action, out var action))
        {
            _npc.Ui.CurrentBehaviour = CurrentBehavior.Name;
            _npc.Ui.CurrentAction = $"{CurrentBehavior.Action} {CurrentBehavior.ActionParameter}";
            _npc.Ui.CurrentStepInAction = $"";
            if (_lastAction != CurrentBehavior.Name)
            {
                _stepInAction = 0;
                
                _lastAction = CurrentBehavior.Name;
                _npc.Movement.Stop();
                _npc.Ui.EndSpeech();
                if (CurrentBehavior.Dialogue!="")
                {
                    _waitBeforeAction = 4;
                    _npc.Ui.Speak(CurrentBehavior.Dialogue);
                }
                else
                {
                    _waitBeforeAction = 2;
                }
            }

            if (_stepInAction == 0 && _waitBeforeAction <= 0)
            {
                _stepInAction++;
                _npc.Ui.EndSpeech();

            }
            action(CurrentBehavior.ActionParameter);

            _waitBeforeAction -= 0.01f;
        }
        else
        {
            Debug.LogWarning($"No handler defined for ActionType: {CurrentBehavior.Action}");
        }
    }
    //TODO: add dynamic tags  when searching for a character
    private void FindCharacter(TargetType targetType)
    {
        switch (_stepInAction)
        {
            case 1:
                if (ActionsHelper.WanderAndSearchForCharacter(_npc, targetType, true, TraitType.human))
                {
                    _stepInAction++;

                }

                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_npc);
                break;
        }
    }
    private void FindOccupant(TraitType traitType)
    {
        switch (_stepInAction)
        {
            case 1:
                var locationObject = WorldManager.Instance.GetLocation(_npc.Movement.CurrentLocation);
                var occupant = locationObject.GetOccupant(traitType);
                if (occupant != null)
                {
                    _npc.Memory.OccupantTargets[traitType] = occupant;
                    _stepInAction++;
                }
                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_npc);
                break;
        }
    }
    private void GotoOccupant(TraitType traitType)
    {
        switch (_stepInAction)
        {
            case 1:
                var occupant = _npc.Memory.OccupantTargets[traitType];
                if (occupant != null && ActionsHelper.Reached(_npc, occupant.transform.position, 3f))
                {
                    _npc.Memory.ReachedOccupant = occupant;
                    _stepInAction++;
                }
                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_npc);
                break;
        }
    }
    private void FindObject(ObjectType targetType)
    {

        switch (_stepInAction)
        {
            case 1:
                if (ActionsHelper.WanderAndSearchForObject(_npc, targetType))
                {
                    _stepInAction = 2;
                }

                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_npc);
                break;
        }

    }


    private void FindKnowledge(KnowledgeType knowledgeType)
    {
        //STEP 1 GOTO INNKEEPER

        switch (_stepInAction)
        {
            case 1:
                _npc.Ui.CurrentStepInAction = "1 Goto nearest innkeeper";
                //remember who the local innkeeper is
                var target = _npc.Memory.GetPeopleByTag(TraitType.innKeeper).FirstOrDefault().GetComponent<Character>();


                if (ActionsHelper.Reached(_npc, target.transform.position, 2f))
                {
                    //STEP 2 ASK FOR DIRECTIONS

                    _npc.Ui.CurrentStepInAction = "2 Ask innkeeper for location with tags";
                    List<Enum> tagsAsEnum = CurrentBehavior.ActionTags.Cast<Enum>().ToList();
                    SocialHelper.AskForKnowledge(_npc, target, knowledgeType, tagsAsEnum);
                    _stepInAction++;
 
                }

                break;
            case 2:
        //WAIT FOR RESPONSE
                break;
        }
    }


    private void BuyItem(ObjectType objectType)
    {

        switch (_stepInAction)
        {
            case 1:
                var seller = _npc.Memory.ReachedOccupant;
                var buyer = _npc;

                _npc.Movement.Stop();
                if (seller != null && ActionsHelper.Reached(_npc, seller.transform.position, 2.7f))
                {
                    var price = seller.Memory.GetPrice(objectType);
                    if (buyer.Memory.Coin >= price && seller.Memory.Possessions.ContainsKey(objectType))
                    {
                        var boughtItem = seller.Memory.RemoveFromPossessions(objectType);
                        buyer.Memory.AddToPossessions(objectType, boughtItem);
                        buyer.Memory.AddToInventory(objectType, boughtItem);
                        buyer.Memory.Coin -= price;
                    }

                    _stepInAction++;


                }
                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_npc);
                break;
        }
    }



    private void Kill(TargetType targetType)
    {
        switch (_stepInAction)
        {
            case 1:

                if (_npc.Memory.Targets[targetType].GetComponent<Character>() is { } target &&
                    ActionsHelper.Reached(_npc, target.transform.position, 1f))
                {
                    target.Vitality.Die();
                    _npc.Memory.Targets[targetType] = null;
                    _stepInAction++;
                }

                //TODO: killer must only kill victim if they think the victim is alone
                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_npc);
                break;
        }
    }


    private void FullfillNeed(NeedType needType)
    {
        switch (_stepInAction)
        {
            case 1:
                if (_npc.Memory.GetPossession(ObjectType.bed) is { } objectToUse && ActionsHelper.Reached(_npc, objectToUse.transform.position, 0.2f))
                {
                    _npc.Vitality.Needs[needType] = 0;
                    _stepInAction++;
                }
                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_npc);
                break;
        }

    }

    private void TraderJob(TraderType traderType)
    {
        switch (_stepInAction)
        {
            case 1:
                var objectToUse = _npc.Memory.GetPossession(ObjectType.traderChair);
                if (ActionsHelper.Reached(_npc, objectToUse.transform.position, 0.2f))
                {
                    //TODO: extend trader behaviour here
                }

                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_npc);
                break;
        }
    }

    private void FarmerJob(FarmerType farmerType)
    {

        switch (_stepInAction)
        {
            //STEP 1
            case 1:
                if (_currentObjectTarget == null)
                {
                    var crops = _npc.Memory.GetPossessions(ObjectType.pumpkin);
                    foreach (var crop in crops)
                    {
                        if (crop.Care < 80)
                        {
                            _currentObjectTarget = crop;
                            break;
                        }
                    }

                    if (_currentObjectTarget == null)
                    { _stepInAction = 2; }
                }
                else
                if (ActionsHelper.Reached(_npc, _currentObjectTarget.transform.position, 1f))
                {
                    _currentObjectTarget.CareFor(_npc);
                    _currentObjectTarget = null;
                }

                break;
            //STEP 2
            case 2:
                ActionsHelper.EndThisBehaviour(_npc);
                break;
        }


    }



    private void UseObjectInInventory(ObjectType objectType)
    {
        switch (_stepInAction)
        {
            //STEP 1
            case 1:
                var objectToUse = _npc.Memory.GetPossession(objectType);
                objectToUse.Use(_npc, out bool destroyObject);
                _npc.Ui.CurrentStepInAction = "drinking";
                if (destroyObject)
                {
                    ActionsHelper.DestroyObject(_npc, objectToUse);
                }

                ActionsHelper.EndThisBehaviour(_npc);
                break;
            //STEP 2
            case 2:
                ActionsHelper.EndThisBehaviour(_npc);
                break;
        }
    }

    private void UseObject(ObjectType objectType)
    {
        var objectToUse = _npc.Memory.Possessions[objectType].First();
        switch (_stepInAction)
        {
            //STEP 1
            case 1:
                if (ActionsHelper.Reached(_npc, objectToUse.transform.position, 0.3f))
                {
                    _stepInAction++;


                }

                break;
            //STEP 2
            case 2:
                objectToUse.Use(_npc, out bool destroyObject);
                if (destroyObject)
                {
                    ActionsHelper.DestroyObject(_npc, objectToUse);
                }
                break;

        }

    }


    private void GotoLocation(TargetLocationType locationType)
    {
        switch (_stepInAction)
        {
            //STEP 1
            case 1:
                _npc.Ui.CurrentStepInAction = $"Made it to goto location {locationType}";

                var mem = _npc.Memory;

                var locationTarget = mem.LocationTargets[locationType];
                var destination = WorldManager.Instance.Locations[locationTarget];

                if (ActionsHelper.Reached(_npc, destination.transform.position, 1f))
                {
                    _npc.Movement.CurrentLocation = locationTarget;
                    _npc.Memory.OccupantTargets.Clear();
                }

                break;
            //STEP 2
            case 2:
                ActionsHelper.EndThisBehaviour(_npc);
                break;
        }
    }
}

