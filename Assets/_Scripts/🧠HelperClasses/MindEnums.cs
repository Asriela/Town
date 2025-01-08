using System;
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
        standardTavernPatron,
        guard,
        draugerCultist,
        jarl,
        pumpkinFarmer
    }
    [Serializable]
    public enum ActionType : short
    {
        defaultAction,
        kill,
        findCharacter,
        findOccupant,
        findObject,
        fullfillNeed,
        patrol,
        guard,
        abduct,
        performSpell,
        socialize,
        trader,
        farmer,
        buyItem,
        findKnowledge,
        shareKnowledge,
        gotoLocation,
        useOneOfMyPossesions,
        useObjectInInventory,
        gotoOccupant,
        rentItem
    }
    [Serializable]
    public enum SocialActions : short
    {
        kiss,
        hug,
        kill

    }
    [Serializable]
    public enum CharacterName : short
    {
        none,
        player,
        Belethor,
        Zerath,
        Alex
    }
    [Serializable]
    public enum LocationName : short
    {
        none,
        ruinsOfLazeel,
        belethorsInn,
        AlexsPumpkinHome
    }
    [Serializable]
    public enum TargetLocationType : short
    {
        none,
        locationOfDarkKnowledge,
        tavern,
        home,
        work,
        pumpkinFarm
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
    public enum FarmerType : short
    {
        none,
        pumpkin
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
        occupant
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
        hasOccupant,
        doesNotHaveOccupant,
        doesNotHaveObject,
        hasObject,
        afterHour,
        beforeHour,
        doesNotHaveKnowledge,
        hasKnowledge,
        atLocation,
        notAtLocation,
        hasLocationTarget,
        doesNotHaveLocationTarget,
        reachedOccupant,
        hasNotReachedOccupant,
        hasEnoughCoin,
        doesNotHaveEnoughCoin,
        hasTrait,
        doesNotHaveTrait

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
        foodBowl,
        traderDesk,
        traderChair,
        bookOfTheDead,
        pumpkin,
        ale,
        barStool
    }

    [Serializable]
    public enum ObjectTrait : short
    {
        plant,
        crop,
        wilts,
        grows
    }

    [Serializable]
    public enum CoinAmount
    {
        c5 = 5,
        c10 = 10,
        c15 = 15,
        c20 = 20,
        c25 = 25,
        c30 = 30
    }
}
