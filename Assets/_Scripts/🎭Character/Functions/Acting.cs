using System.Collections.Generic;
using System;
using UnityEngine;
using Mind;

public class Acting : MonoBehaviour
{
    private NPC _npc;
    private Dictionary<Mind.ActionType, Action<object>> _actionHandlers;
    public void Initialize(NPC npc) => _npc = npc;
    public Behavior CurrentBehavior { get; set; }

    private void Awake() =>
    _actionHandlers = new Dictionary<Mind.ActionType, Action<object>>
    {
        { Mind.ActionType.findCharacter, param => FindCharacter((Mind.TargetType)param) },
        { Mind.ActionType.findObject, param => FindObject((Mind.ObjectType)param) },
        { Mind.ActionType.fullfillNeed, param => FullfillNeed((Mind.NeedType)param) },
        { Mind.ActionType.kill, param => Kill((Mind.TargetType)param) },
        { Mind.ActionType.trader, param => TraderJob((Mind.TraderType)param) },
        { Mind.ActionType.findKnowledge, param => FindKnowledge((Mind.KnowledgeType)param) },
        { Mind.ActionType.gotoLocation, param => GotoLocation((Mind.TargetLocationType)param) }
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

            action(CurrentBehavior.ActionParameter);
        }
        else
        {
            Debug.LogWarning($"No handler defined for ActionType: {CurrentBehavior.Action}");
        }
    }

    private void FindCharacter(Mind.TargetType targetType)
    {
        StartCoroutine(ActionsHelper.WanderAndSearch(_npc, targetType, true, Mind.TraitType.human));
    }
    private void FindObject(Mind.ObjectType targetType)
    {
        //  StartCoroutine(ActionsHelper.WanderAndSearch(_npc, targetType, true, Mind.TraitType.human));

    }
    private void FindKnowledge(Mind.KnowledgeType knowledgeType)
    {
        //  StartCoroutine(ActionsHelper.WanderAndSearch(_npc, targetType, true, Mind.TraitType.human));
        // CurrentBehavior.ActionTags;

    }
    private void Kill(Mind.TargetType targetType)
    {
        var target = _npc.Memory.Targets[targetType].GetComponent<Character>();

        if (ActionsHelper.Reached(_npc, target.transform.position))
        {
            target.Vitality.Die();
            _npc.Memory.Targets[targetType] = null;
        }

    }

    private void FullfillNeed(Mind.NeedType needType)
    {
        var objectToUse = _npc.Memory.Possessions[ObjectType.bed];
        if (ActionsHelper.Reached(_npc, objectToUse.transform.position))
        {
            _npc.Vitality.Needs[needType] = 0;
        }
    }

    private void TraderJob(Mind.TraderType traderType)
    {
        var objectToUse = _npc.Memory.Possessions[ObjectType.traderDesk];
        if (ActionsHelper.Reached(_npc, objectToUse.transform.position))
        {

        }
    }

    private void GotoLocation(Mind.TargetLocationType locationType)
    {
        var locationTarget = _npc.Memory.LocationTargets[locationType];
        var destination = WorldManager.Instance.Locations[locationTarget];

        if (ActionsHelper.Reached(_npc, destination.transform.position))
        {
            _npc.Movement.CurrentLocation = locationTarget;
        }
    }

}
