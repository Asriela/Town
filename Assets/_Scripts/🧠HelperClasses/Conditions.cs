﻿using System;
using System.Collections.Generic;
using Mind;
using Unity.VisualScripting;
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
        _conditionDelegates.Add(ConditionType.hasOccupant, new ConditionDelegate<TraitType>(CheckOccupantTarget));
        _conditionDelegates.Add(ConditionType.doesNotHaveOccupant, new ConditionDelegate<TraitType>(CheckOccupantTarget));
        _conditionDelegates.Add(ConditionType.hasObject, new ConditionDelegate<ObjectType>(CheckHasObject));
        _conditionDelegates.Add(ConditionType.doesNotHaveObject, new ConditionDelegate<ObjectType>(CheckHasObject));
        _conditionDelegates.Add(ConditionType.beforeHour, new ConditionDelegate<TimeOfDayType>(CheckTimeOfDay));
        _conditionDelegates.Add(ConditionType.afterHour, new ConditionDelegate<TimeOfDayType>(CheckTimeOfDay));
        _conditionDelegates.Add(ConditionType.atLocation, new ConditionDelegate<TargetLocationType>(AtLocation));
        _conditionDelegates.Add(ConditionType.notAtLocation, new ConditionDelegate<TargetLocationType>(AtLocation));
        _conditionDelegates.Add(ConditionType.hasLocationTarget, new ConditionDelegate<TargetLocationType>(CheckLocationTarget));
        _conditionDelegates.Add(ConditionType.doesNotHaveLocationTarget, new ConditionDelegate<TargetLocationType>(CheckLocationTarget));
        _conditionDelegates.Add(ConditionType.reachedOccupant, new ConditionDelegate<TraitType>(ReachedOccupant));
        _conditionDelegates.Add(ConditionType.hasNotReachedOccupant, new ConditionDelegate<TraitType>(ReachedOccupant));
        _conditionDelegates.Add(ConditionType.hasEnoughCoin, new ConditionDelegate<CoinAmount>(CheckCoin));
        _conditionDelegates.Add(ConditionType.doesNotHaveEnoughCoin, new ConditionDelegate<CoinAmount>(CheckCoin));
        _conditionDelegates.Add(ConditionType.hasTrait, new ConditionDelegate<TraitType>(CheckTrait));
        _conditionDelegates.Add(ConditionType.doesNotHaveTrait, new ConditionDelegate<TraitType>(CheckTrait));
    }

    public static bool CheckCondition(Condition condition, NPC npc)
    {
        // Look up the delegate for the given condition type
        if (_conditionDelegates.TryGetValue(condition.conditionType, out var conditionDelegate))
        {
            // Now we check which type the delegate is and cast it accordingly


            return condition.conditionType switch
            {
                ConditionType.afterHour => ConditionHandler<TimeOfDayType>((ConditionDelegate<TimeOfDayType>)conditionDelegate, condition.parameter, npc, true),

                ConditionType.beforeHour => ConditionHandler<TimeOfDayType>((ConditionDelegate<TimeOfDayType>)conditionDelegate, condition.parameter, npc, false),

                ConditionType.needsTo => ConditionHandler<NeedType>((ConditionDelegate<NeedType>)conditionDelegate, condition.parameter, npc, true),

                ConditionType.hasTarget => ConditionHandler<TargetType>((ConditionDelegate<TargetType>)conditionDelegate, condition.parameter, npc, true),

                ConditionType.doesNotHaveTarget => ConditionHandler<TargetType>((ConditionDelegate<TargetType>)conditionDelegate, condition.parameter, npc, false),

                ConditionType.hasOccupant => ConditionHandler<TraitType>((ConditionDelegate<TraitType>)conditionDelegate, condition.parameter, npc, true),

                ConditionType.doesNotHaveOccupant => ConditionHandler<TraitType>((ConditionDelegate<TraitType>)conditionDelegate, condition.parameter, npc, false),

                ConditionType.hasObject => ConditionHandler<ObjectType>((ConditionDelegate<ObjectType>)conditionDelegate, condition.parameter, npc, true),

                ConditionType.doesNotHaveObject => ConditionHandler<ObjectType>((ConditionDelegate<ObjectType>)conditionDelegate, condition.parameter, npc, false),

                ConditionType.atLocation => ConditionHandler<TargetLocationType>((ConditionDelegate<TargetLocationType>)conditionDelegate, condition.parameter, npc, true),

                ConditionType.notAtLocation => ConditionHandler<TargetLocationType>((ConditionDelegate<TargetLocationType>)conditionDelegate, condition.parameter, npc, false),

                ConditionType.hasLocationTarget => ConditionHandler<TargetLocationType>((ConditionDelegate<TargetLocationType>)conditionDelegate, condition.parameter, npc, true),

                ConditionType.doesNotHaveLocationTarget => ConditionHandler<TargetLocationType>((ConditionDelegate<TargetLocationType>)conditionDelegate, condition.parameter, npc, false),

                ConditionType.reachedOccupant => ConditionHandler<TraitType>((ConditionDelegate<TraitType>)conditionDelegate, condition.parameter, npc, true),

                ConditionType.hasNotReachedOccupant => ConditionHandler<TraitType>((ConditionDelegate<TraitType>)conditionDelegate, condition.parameter, npc, false),

                ConditionType.hasEnoughCoin => ConditionHandler<CoinAmount>((ConditionDelegate<CoinAmount>)conditionDelegate, condition.parameter, npc, true),

                ConditionType.doesNotHaveEnoughCoin => ConditionHandler<CoinAmount>((ConditionDelegate<CoinAmount>)conditionDelegate, condition.parameter, npc, false),
                ConditionType.hasTrait => ConditionHandler<TraitType>((ConditionDelegate<TraitType>)conditionDelegate, condition.parameter, npc, true),

                ConditionType.doesNotHaveTrait => ConditionHandler<TraitType>((ConditionDelegate<TraitType>)conditionDelegate, condition.parameter, npc, false),

                _ => false
            };

        }

        return false;
    }

    private static bool ConditionHandler<T>(ConditionDelegate<T> conditionDelegate, object parameter, NPC npc, bool trueStatement) => conditionDelegate((T)parameter, npc, trueStatement);

    // Example condition check methods:
    private static bool CheckNeed(NeedType parameter, NPC npc, bool trueStatement)
    {
        if (npc.Vitality.Needs.ContainsKey(parameter) && npc.Vitality.Needs[parameter] > 80)
        {
            return true;
        }
        return false;
    }

    private static bool CheckOccupantTarget(TraitType parameter, NPC npc, bool trueStatement)
    {

        if (npc.Memory.OccupantTargets.ContainsKey(parameter) && npc.Memory.OccupantTargets[parameter] != null)
        {
            return trueStatement;
        }
        return !trueStatement;
    }

    private static bool CheckTarget(TargetType parameter, NPC npc, bool trueStatement)
    {

        if (npc.Memory.Targets.ContainsKey(parameter) && npc.Memory.Targets[parameter] != null)
        {
            return trueStatement;
        }
        return !trueStatement;
    }
    private static bool CheckTrait(TraitType parameter, NPC npc, bool trueStatement)
    {

        if (npc.Memory.HasTrait(GameManager.Instance.TraitsInPlay[parameter]))
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
    private static bool CheckCoin(CoinAmount parameter, NPC npc, bool trueStatement)
    {

        if (trueStatement && npc.Memory.Coin >= (int)parameter)
        {
            return true;
        }
        if (!trueStatement && npc.Memory.Coin < (int)parameter)
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
    private static bool ReachedOccupant(TraitType parameter, NPC npc, bool trueStatement)
    {
        //check if we have a reached occupant and if its trait matches what we are looking for
        var occupantToReach = npc.Memory.ReachedOccupant;
        if (occupantToReach != null && occupantToReach.Memory.HasTrait(GameManager.Instance.TraitsInPlay[parameter]))
        { return trueStatement; }

        return !trueStatement;
    }


}
