using System;
using System.Collections.Generic;
using Mind;
using UnityEngine;


public static class Conditions
{
    public delegate bool ConditionDelegate<T>(T parameter, NPC npc, bool reverse);
    // Base delegate type, doesn't have to be generic.
    private static readonly Dictionary<ConditionType, Delegate> _conditionDelegates = new();

    static Conditions()
    {
        // Initialize the delegates for each condition type.
        _conditionDelegates.Add(ConditionType.needsTo, new ConditionDelegate<NeedType>(CheckNeed));
        _conditionDelegates.Add(ConditionType.hasTarget, new ConditionDelegate<TargetType>(CheckTarget));
        _conditionDelegates.Add(ConditionType.doesNotHaveTarget, new ConditionDelegate<NeedType>(CheckNeed));
        _conditionDelegates.Add(ConditionType.hasObject, new ConditionDelegate<ObjectType>(CheckHasObject));
        _conditionDelegates.Add(ConditionType.doesNotHaveObject, new ConditionDelegate<ObjectType>(CheckHasObject));
        _conditionDelegates.Add(ConditionType.timeOfDay, new ConditionDelegate<TimeOfDayType>(CheckTimeOfDay));
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
                    return needDelegate((NeedType)condition.parameter, npc, true);

                case ConditionType.hasTarget:
                    var targetDelegate = (ConditionDelegate<TargetType>)conditionDelegate;
                    return targetDelegate((TargetType)condition.parameter, npc, true);

                case ConditionType.doesNotHaveTarget:
                    var targetNotDelegate = (ConditionDelegate<TargetType>)conditionDelegate;
                    return targetNotDelegate((TargetType)condition.parameter, npc, false);

                case ConditionType.hasObject:
                    var objectDelegate = (ConditionDelegate<ObjectType>)conditionDelegate;
                    return objectDelegate((ObjectType)condition.parameter, npc, true);

                case ConditionType.doesNotHaveObject:
                    var objectNotDelegate = (ConditionDelegate<ObjectType>)conditionDelegate;
                    return objectNotDelegate((ObjectType)condition.parameter, npc, false);
                default:
                    return false;
            }
        }

        return false;
    }

    // Example condition check methods:
    private static bool CheckNeed(NeedType parameter, NPC npc, bool trueStatement)
    {
        if (npc.Vitality.Needs.ContainsKey(parameter) && npc.Vitality.Needs[parameter] > 80)
        {
            return true;
        }
        return false;
    }

    private static bool CheckTarget(TargetType parameter, NPC npc, bool trueStatement)
    {
        if (npc.Memory.Targets.ContainsKey(parameter) && npc.Memory.Targets[parameter] != null)
        {
            return trueStatement;
        }
        return !trueStatement;
    }

    private static bool CheckHasObject(ObjectType parameter, NPC npc, bool trueStatement)
    {
        if (npc.Memory.Possessions.ContainsKey(parameter) && npc.Memory.Possessions[parameter] != null)
        {
            return trueStatement;
        }
        return !trueStatement;
    }
    private static bool CheckTimeOfDay(TimeOfDayType parameter, NPC npc, bool trueStatement)
    {
        if (WorldManager.Instance.GetTimeOfDayAsEnum() == parameter)
        {
            return trueStatement;
        }
        return !trueStatement;
    }

}
