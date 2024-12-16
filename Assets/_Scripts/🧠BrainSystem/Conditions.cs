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
        _conditionDelegates.Add(ConditionType.doesNotHaveTarget, new ConditionDelegate<TargetType>(CheckTarget));
        _conditionDelegates.Add(ConditionType.hasObject, new ConditionDelegate<ObjectType>(CheckHasObject));
        _conditionDelegates.Add(ConditionType.doesNotHaveObject, new ConditionDelegate<ObjectType>(CheckHasObject));
        _conditionDelegates.Add(ConditionType.beforeHour, new ConditionDelegate<TimeOfDayType>(CheckTimeOfDay));
        _conditionDelegates.Add(ConditionType.afterHour, new ConditionDelegate<TimeOfDayType>(CheckTimeOfDay));
        _conditionDelegates.Add(ConditionType.atLocation, new ConditionDelegate<TargetLocationType>(AtLocation));
        _conditionDelegates.Add(ConditionType.hasLocationTarget, new ConditionDelegate<TargetLocationType>(CheckLocationTarget));
        _conditionDelegates.Add(ConditionType.doesNotHaveLocationTarget, new ConditionDelegate<TargetLocationType>(CheckLocationTarget));
    }

    public static bool CheckCondition(Condition condition, NPC npc)
    {
        // Look up the delegate for the given condition type
        if (_conditionDelegates.TryGetValue(condition.conditionType, out Delegate conditionDelegate))
        {
            // Now we check which type the delegate is and cast it accordingly
            switch (condition.conditionType)
            {

                case ConditionType.afterHour:
                    var beforeHourDelegate = (ConditionDelegate<TimeOfDayType>)conditionDelegate;
                    return beforeHourDelegate((TimeOfDayType)condition.parameter, npc, true);
                case ConditionType.beforeHour:
                    var afterHourDelegate = (ConditionDelegate<TimeOfDayType>)conditionDelegate;
                    return afterHourDelegate((TimeOfDayType)condition.parameter, npc, true);

                case ConditionType.needsTo:
                    var needDelegate = (ConditionDelegate<NeedType>)conditionDelegate;
                    return needDelegate((NeedType)condition.parameter, npc, true);

                case ConditionType.hasTarget:
                    var targetDelegate = (ConditionDelegate<TargetType>)conditionDelegate;
                    return targetDelegate((TargetType)condition.parameter, npc, true);

                case ConditionType.doesNotHaveTarget:
                    var targetDelegateNot = (ConditionDelegate<TargetType>)conditionDelegate;
                    return targetDelegateNot((TargetType)condition.parameter, npc, false);


                case ConditionType.hasObject:
                    var objectDelegate = (ConditionDelegate<ObjectType>)conditionDelegate;
                    return objectDelegate((ObjectType)condition.parameter, npc, true);

                case ConditionType.doesNotHaveObject:
                    var objectNotDelegate = (ConditionDelegate<ObjectType>)conditionDelegate;
                    return objectNotDelegate((ObjectType)condition.parameter, npc, false);
                case ConditionType.atLocation:
                    var atLocation = (ConditionDelegate<TargetLocationType>)conditionDelegate;
                    return atLocation((TargetLocationType)condition.parameter, npc, true);
                case ConditionType.notAtLocation:
                    var atLocationNot = (ConditionDelegate<TargetLocationType>)conditionDelegate;
                    return atLocationNot((TargetLocationType)condition.parameter, npc, false);
                case ConditionType.hasLocationTarget:
                    var locationTargetDelegate = (ConditionDelegate<TargetLocationType>)conditionDelegate;
                    return locationTargetDelegate((TargetLocationType)condition.parameter, npc, true);
                case ConditionType.doesNotHaveLocationTarget:
                    var locationTargetNotDelegate = (ConditionDelegate<TargetLocationType>)conditionDelegate;
                    return locationTargetNotDelegate((TargetLocationType)condition.parameter, npc, false);
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
    private static bool CheckLocationTarget(TargetLocationType parameter, NPC npc, bool trueStatement)
    {
        bool ret;
        if (npc.Memory.LocationTargets.ContainsKey(parameter))
        {
            Debug.Log($"🔰Location target: {npc.Memory.LocationTargets[parameter]}");
        }

        if (npc.Memory.LocationTargets.ContainsKey(parameter) && npc.Memory.LocationTargets[parameter] != LocationName.none)
        {
            ret = trueStatement;
        }
        else
        { ret = !trueStatement; }

        if (ret != trueStatement)
        { npc.Memory.LatestLocationTargetType = parameter; }

        return ret;
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

        if (trueStatement && WorldManager.Instance.GetTimeOfDayAsEnum() >= parameter)
        {
            return true;
        }
        if (!trueStatement && WorldManager.Instance.GetTimeOfDayAsEnum() < parameter)
        {
            return true;
        }
        return false;
    }
    private static bool AtLocation(TargetLocationType parameter, NPC npc, bool trueStatement)
    {
        if (!npc.Memory.LocationTargets.ContainsKey(parameter))
        { return !trueStatement; }
        var locationFromMemory = npc.Memory.LocationTargets[parameter];

        Debug.Log($"⭐current location : {npc.Movement.CurrentLocation} locationFromMemory: {locationFromMemory}");

        if (npc.Movement.CurrentLocation != LocationName.none && npc.Movement.CurrentLocation == locationFromMemory)
        { return trueStatement; }
        return !trueStatement;
    }
}
