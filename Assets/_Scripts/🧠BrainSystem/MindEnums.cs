﻿using System;
using UnityEngine;

namespace Mind
{
    [Serializable]
    public enum TraitType : short
    {
        human,
        deathCultist,
        touchedByDarkness,
        innKeeper,
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
        findCharacter,
        findObject,
        fullfillNeed,
        patrol,
        guard,
        abduct,
        performSpell,
        socialize,
        trader,
        findKnowledge,
        gotoLocation,

    }
    [Serializable]
    public enum CharacterName : short
    {
        none,
        Belethor,
        Zerath
    }
    [Serializable]
    public enum LocationName : short
    {
        none,
        ruinsOfLazeel,
        belethorsInn
    }
    [Serializable]
    public enum TargetLocationType : short
    {
        none,
        locationOfDarkKnowledge
    }
    [Serializable]
    public enum SpellType : short
    {
        none,
        makeUndead
    }
    [Serializable]
    public enum KnowledgeTag : short
    {
        none,
        dark,
        light,
        ruins,
        inn
    }

    [Serializable]
    public enum TraderType : short
    {
        none,
        food,
        generalGoods,
        barKeeper,
        innkeeper
    }
    [Serializable]
    public enum KnowledgeType : short
    {
        none,
        location,
        person
    }

    [Serializable]
    public enum SocializeType : short
    {
        none,
        askAbout,
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
        hasTarget,
        doesNotHaveTarget,
        doesNotHaveObject,
        hasObject,
        timeOfDay,
        doesNotHaveKnowledge,
        hasKnowledge,
        atLocation,
        notAtLocation,
        hasLocationTarget,
        doesNotHaveLocationTarget

    }
    [Serializable]
    public enum TimeOfDayType : short
    {
        am1,
        am2,
        am3,
        am4,
        am5,
        am6,
        am7,
        am8,
        am9,
        am10,
        am11,
        am12,
        pm1,
        pm2,
        pm3,
        pm4,
        pm5,
        pm6,
        pm7,
        pm8,
        pm9,
        pm10,
        pm11,
        pm12,
    }

    [Serializable]
    public enum ObjectType : short
    {
        bed,
        traderDesk,
        bookOfTheDead
    }
}
