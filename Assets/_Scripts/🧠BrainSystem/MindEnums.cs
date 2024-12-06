using System;
using UnityEngine;

namespace Mind
{
    [Serializable]
    public enum TraitType : short
    {
        human,
        murderer,
        guard,
        draugerCultist,
        jarl
    }
    [Serializable]
    public enum ActionType : short
    {
        defaultAction,
        kill,
        find,
        fullfillNeed,
        patrol,
        guard,
        abduct,
        performSpell,
        socialize
    }
    [Serializable]
    public enum SpellType : short
    {
        none,
        makeUndead
    }

    [Serializable]
    public enum SocializeType : short
    {
        none,
        drinkWith,
        eatWith,
        flirt,
        greet,
        joke
    }

    [Serializable]
    public enum TargetType : short
    {
        none,
        murderer,
        cultist,
        murderVictim,
        turnUndeadVictim,
    }
    [Serializable]
    public enum NeedType : short
    {
        none,
        sleep,
        eat,
        murder,
        makeUndead
    }
    [Serializable]
    public enum ConditionType : short
    {
        idle,
        needsTo,
        hasTarget
    }

    public enum ObjectType : short
    {
        bed
    }
}
