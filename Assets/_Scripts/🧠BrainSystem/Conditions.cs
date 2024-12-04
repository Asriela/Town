using System;
using System.Collections.Generic;
using Mind;
using UnityEngine;


public static class Conditions
{
    public delegate bool ConditionDelegate<T>(T parameter, NPC npc);
    // Base delegate type, doesn't have to be generic.
    private static readonly Dictionary<ConditionType, Delegate> _conditionDelegates = new();

    static Conditions()
    {
        // Initialize the delegates for each condition type.
        _conditionDelegates.Add(ConditionType.needsTo, new ConditionDelegate<NeedType>(CheckNeed));
        _conditionDelegates.Add(ConditionType.hasTarget, new ConditionDelegate<TargetType>(CheckTarget));
    }

    public static bool CheckCondition(Condition condition, NPC npc)
    {
        // Look up the delegate for the given condition type
        if (_conditionDelegates.TryGetValue(condition.conditionType, out Delegate conditionDelegate))
        {
            // Now we check which type the delegate is and cast it accordingly
            switch (condition.conditionType)
            {
                case ConditionType.needsTo:
                    var needDelegate = (ConditionDelegate<NeedType>)conditionDelegate;
                    return needDelegate((NeedType)condition.parameter, npc);

                case ConditionType.hasTarget:
                    var targetDelegate = (ConditionDelegate<TargetType>)conditionDelegate;
                    return targetDelegate((TargetType)condition.parameter, npc);

                default:
                    return false;
            }
        }

        return false;
    }

    // Example condition check methods:
    private static bool CheckNeed(NeedType parameter, NPC npc)
    {
        if (npc.Needs.ContainsKey(parameter) && npc.Needs[parameter] > 80) 
        {
            return true;
        }
        return false;
    }

    private static bool CheckTarget(TargetType parameter, NPC npc)
    {
        if (npc.Memory.Targets.ContainsKey(parameter) && npc.Memory.Targets[parameter] != null) 
        {
            return true;
        }
        return false;
    }
}
