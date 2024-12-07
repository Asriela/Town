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
        { Mind.ActionType.findCharacter, param => Find((Mind.TargetType)param) },
        { Mind.ActionType.fullfillNeed, param => FullfillNeed((Mind.NeedType)param) },
        { Mind.ActionType.kill, param => Kill((Mind.TargetType)param) }
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

    private void Find(Mind.TargetType targetType)
    {
        StartCoroutine(ActionsHelper.WanderAndSearch(_npc, targetType, true,Mind.TraitType.human));

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
}
