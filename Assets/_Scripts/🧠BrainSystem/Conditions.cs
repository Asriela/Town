﻿using System;
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
        _conditionDelegates.Add(ConditionType.needsTo, new ConditionDelegate<Mind.NeedType>(CheckNeed));
        _conditionDelegates.Add(ConditionType.hasTarget, new ConditionDelegate<Mind.TargetType>(CheckTarget));
        _conditionDelegates.Add(ConditionType.doesNotHaveTarget, new ConditionDelegate<Mind.NeedType>(CheckNeed));
        _conditionDelegates.Add(ConditionType.hasObject, new ConditionDelegate<Mind.ObjectType>(CheckHasObject));
        _conditionDelegates.Add(ConditionType.doesNotHaveObject, new ConditionDelegate<Mind.ObjectType>(CheckHasObject));
        _conditionDelegates.Add(ConditionType.timeOfDay, new ConditionDelegate<Mind.TimeOfDayType>(CheckTimeOfDay));
        _conditionDelegates.Add(ConditionType.atLocation, new ConditionDelegate<Mind.TargetLocationType>(AtLocation));
        _conditionDelegates.Add(ConditionType.hasLocationTarget, new ConditionDelegate<Mind.TargetLocationType>(CheckLocationTarget));
        _conditionDelegates.Add(ConditionType.doesNotHaveLocationTarget, new ConditionDelegate<Mind.TargetLocationType>(CheckLocationTarget));
    }

    public static bool CheckCondition(Condition condition, NPC npc)
    {
        // Look up the delegate for the given condition type
        if (_conditionDelegates.TryGetValue(condition.conditionType, out Delegate conditionDelegate))
        {
            // Now we check which type the delegate is and cast it accordingly
            switch (condition.conditionType)
            {

                case ConditionType.timeOfDay:
                    var timeDelegate = (ConditionDelegate<Mind.TimeOfDayType>)conditionDelegate;
                    return timeDelegate((Mind.TimeOfDayType)condition.parameter, npc, true);

                case ConditionType.needsTo:
                    var needDelegate = (ConditionDelegate<Mind.NeedType>)conditionDelegate;
                    return needDelegate((Mind.NeedType)condition.parameter, npc, true);

                case ConditionType.hasTarget:
                    var targetDelegate = (ConditionDelegate<Mind.TargetType>)conditionDelegate;
                    return targetDelegate((Mind.TargetType)condition.parameter, npc, true);



                case ConditionType.hasObject:
                    var objectDelegate = (ConditionDelegate<Mind.ObjectType>)conditionDelegate;
                    return objectDelegate((Mind.ObjectType)condition.parameter, npc, true);

                case ConditionType.doesNotHaveObject:
                    var objectNotDelegate = (ConditionDelegate<Mind.ObjectType>)conditionDelegate;
                    return objectNotDelegate((Mind.ObjectType)condition.parameter, npc, false);
                case ConditionType.atLocation:
                    var atLocation = (ConditionDelegate<Mind.TargetLocationType>)conditionDelegate;
                    return atLocation((Mind.TargetLocationType)condition.parameter, npc, true);
                case ConditionType.notAtLocation:
                    var atLocationNot = (ConditionDelegate<Mind.TargetLocationType>)conditionDelegate;
                    return atLocationNot((Mind.TargetLocationType)condition.parameter, npc, false);
                case ConditionType.hasLocationTarget:
                    var locationTargetDelegate = (ConditionDelegate<Mind.TargetLocationType>)conditionDelegate;
                    return locationTargetDelegate((Mind.TargetLocationType)condition.parameter, npc, true);
                case ConditionType.doesNotHaveLocationTarget:
                    var locationTargetNotDelegate = (ConditionDelegate<Mind.TargetLocationType>)conditionDelegate;
                    return locationTargetNotDelegate((Mind.TargetLocationType)condition.parameter, npc, false);
                default:
                    return false;
            }
        }

        return false;
    }

    // Example condition check methods:
    private static bool CheckNeed(Mind.NeedType parameter, NPC npc, bool trueStatement)
    {
        if (npc.Vitality.Needs.ContainsKey(parameter) && npc.Vitality.Needs[parameter] > 80)
        {
            return true;
        }
        return false;
    }

    private static bool CheckTarget(Mind.TargetType parameter, NPC npc, bool trueStatement)
    {
        if (npc.Memory.Targets.ContainsKey(parameter) && npc.Memory.Targets[parameter] != null)
        {
            return trueStatement;
        }
        return !trueStatement;
    }
    private static bool CheckLocationTarget(Mind.TargetLocationType parameter, NPC npc, bool trueStatement)
    {
        bool ret;
        if (npc.Memory.LocationTargets.ContainsKey(parameter))
            Debug.Log($"🔰Location target: {npc.Memory.LocationTargets[parameter]}");
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

    private static bool CheckHasObject(Mind.ObjectType parameter, NPC npc, bool trueStatement)
    {
        if (npc.Memory.Possessions.ContainsKey(parameter) && npc.Memory.Possessions[parameter] != null)
        {
            return trueStatement;
        }
        return !trueStatement;
    }
    private static bool CheckTimeOfDay(Mind.TimeOfDayType parameter, NPC npc, bool trueStatement)
    {

        if (WorldManager.Instance.GetTimeOfDayAsEnum() == parameter)
        {
            return trueStatement;
        }
        return !trueStatement;
    }
    private static bool AtLocation(Mind.TargetLocationType parameter, NPC npc, bool trueStatement)
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
