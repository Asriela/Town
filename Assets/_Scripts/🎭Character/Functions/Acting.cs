using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Mind;
using System.Data.Common;

public class Acting : MonoBehaviour
{
    private NPC _npc;
    private Dictionary<ActionType, Action<object>> _actionHandlers;
    private WorldObject _currentObjectTarget = null;
    private int _stepInAction = 0;
    private string _lastAction = "";
    private Coroutine _currentCoroutine = null;
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
        { ActionType.useObjectInInventory, param => UseObject((ObjectType)param) }
    };

    private void Update() => PerformCurrentBehavior();

    private void PerformCurrentBehavior()
    {

        if (CurrentBehavior == null)
        { return; }

        if (_actionHandlers.TryGetValue(CurrentBehavior.Action, out var action))
        {
            _npc.Logger.CurrentBehaviour = CurrentBehavior.Name;
            _npc.Logger.CurrentAction = $"{CurrentBehavior.Action} {CurrentBehavior.ActionParameter}";
            _npc.Logger.CurrentStepInAction = $"";
            if (_lastAction != CurrentBehavior.Name)
            {
                _stepInAction = 1;
                _currentCoroutine = null;
                _lastAction = CurrentBehavior.Name;
            }

            action(CurrentBehavior.ActionParameter);
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
                _currentCoroutine ??= StartCoroutine(ActionsHelper.WanderAndSearchForCharacter(_npc, targetType, true, () =>
                {
                    _stepInAction++;
                    _currentCoroutine = null;
                }, TraitType.human));

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
                _currentCoroutine ??= StartCoroutine(ActionsHelper.WanderAndSearchForObject(_npc, targetType, () =>
                {
                    _stepInAction++;
                    _currentCoroutine = null;
                }));

                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_npc);
                break;
        }

    }
    

    private void FindKnowledge(KnowledgeType knowledgeType)
    {
        //STEP 1 GOTO INNKEEPER
        _npc.Logger.CurrentStepInAction = "1 Goto nearest innkeeper";
        //remember who the local innkeeper is
        var target = _npc.Memory.GetPeopleByTag(TraitType.innKeeper).FirstOrDefault().GetComponent<Character>();


        if (ActionsHelper.Reached(_npc, target.transform.position))
        {
            //STEP 2 ASK FOR DIRECTIONS
            _npc.Logger.CurrentStepInAction = "2 Ask innkeeper for location with tags";
            List<Enum> tagsAsEnum = CurrentBehavior.ActionTags.Cast<Enum>().ToList();
            SocialHelper.AskForKnowledge(_npc, target, knowledgeType, tagsAsEnum);
            //STEP 3 LETS ASSUME WE ALWAYS WANT TO GO TO THE LOCATION WE JUST LEARNED ABOUT SO AUTOMATICALLY MAKE IT OUR TARGET
        }



    }
    private void Kill(TargetType targetType)
    {
        var target = _npc.Memory.Targets[targetType].GetComponent<Character>();

        if (ActionsHelper.Reached(_npc, target.transform.position))
        {
            target.Vitality.Die();
            _npc.Memory.Targets[targetType] = null;
        }
        //TODO: killer must only kill victim if they think the victim is alone
    }



    private void FullfillNeed(NeedType needType)
    {
        var objectToUse = _npc.Memory.Possessions[ObjectType.bed].First();

        if (ActionsHelper.Reached(_npc, objectToUse.transform.position))
        {
            _npc.Vitality.Needs[needType] = 0;
        }
    }

    private void TraderJob(TraderType traderType)
    {
        var objectToUse = _npc.Memory.Possessions[ObjectType.traderDesk].First();
        if (ActionsHelper.Reached(_npc, objectToUse.transform.position))
        {
            //TODO: extend trader behaviour here
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
                    var crops = _npc.Memory.Possessions[ObjectType.pumpkin];
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
                if (ActionsHelper.Reached(_npc, _currentObjectTarget.transform.position))
                {
                    _currentObjectTarget.CareFor(_npc);
                    _currentObjectTarget = null;
                }

                break;
            //STEP 2
            case 2:
                break;
        }


    }



    private void UseObjectInInventory(ObjectType objectType)
    {
        var objectToUse = _npc.Memory.Inventory[objectType].First();
        objectToUse.Use(_npc);
        _npc.Thinking.CalculateHighestScoringBehavior();
    }

    private void UseObject(ObjectType objectType)
    {
        var objectToUse = _npc.Memory.Possessions[objectType].First();
        if (ActionsHelper.Reached(_npc, objectToUse.transform.position))
        {
            objectToUse.Use(_npc);
            _npc.Thinking.CalculateHighestScoringBehavior();
        }

    }

    private void GotoLocation(TargetLocationType locationType)
    {
        _npc.Logger.CurrentStepInAction = $"Made it to goto location {locationType}";

        var mem = _npc.Memory;

        var locationTarget = mem.LocationTargets[locationType];
        var destination = WorldManager.Instance.Locations[locationTarget];

        if (ActionsHelper.Reached(_npc, destination.transform.position))
        {
            _npc.Movement.CurrentLocation = locationTarget;
        }
    }

}

