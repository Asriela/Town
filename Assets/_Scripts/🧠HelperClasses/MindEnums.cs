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
        pumpkinFarmer,
        newToTown,
        hatesOutsiders,
        hasNoBed,
        drunkard,
        StandardSocialAttitudes,
        hatesMages,
        mage,
        monsterKiller,
        captainOfTheGuard,
        werewolf,
        concernedWife,
        tailor,
        seamstress,
        player,
        patrol,
        doctor,
        tavernGuard,
        talkToPlayer
        
    }
    [Serializable]
    public enum SocialImpression : short
    {
        none,
        boring,
        amusing,
        annoying,
        serious,
        romantic,
        brave,
        evil,
        rude,
        scary,
        infuriating,
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
        rentItem,
        sharePersonKnowledgeAbout,
        gotoPerson,
        gotoTarget,
        completeScriptedTask,
        openDialogueWithPlayer,
        shareTag,
        becomeOccupant,
        removeAsOccupant
    }
    [Serializable]
    public enum BoolType : short
    {
        True,
        False

    }
    [Serializable]
    public enum IsClosable : short
    {
        canBeClosed,
        cantBeClosed,

    }
    [Serializable]
    public enum RemoveSocial : short
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
        Alrine,
        Zerath,
        Alex,
        Ludwar,
        Lyssara,
        Duskwither,
        Agnar,
        Talinor,
        Elara,
        Dohlson,
        Keth,
        Onar
    }
    [Serializable]
    public enum LocationName : short
    {
        none,
        ruinsOfLazeel,
        belethorsInn,
        AlexsPumpkinHome,
        guardPost,
        gate,
        lordsManor,
        tailorsHome,
        villageEntrance

    }
    [Serializable]
    public enum TargetLocationType : short
    {
        none,
        locationOfDarkKnowledge,
        tavern,
        home,
        work,
        pumpkinFarm,
        tailorsShop,
        villageEntrance
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
        innkeeper,
        innGuard
    }
    [Serializable]
    public enum DialogueFileType : short
    {
        auto,
        repeat,
        confront,
        bad,
        good,
        _1,
        _2,
        _3,
        _4,
        _5
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
        sleepWith,
        drinkWith,
        eatWith,
        flirt,
        greet,
        joke,
        insult,
        hug,
        kiss,
        smallTalk,
        drinking,
        ask,
        tell,
        giveFood,
        bribe,
        playChess,
        comfort,
        sympathize,
        intimidate,
        threaten,
        beatUp,
        blackmail,
        giveAlcohol,
        reassure,
        puthandOnShoulder,
        messWithTheirHead
    }

    [Serializable]
    public enum TargetType : short
    {
        none,
        murderer,
        cultist,
        murderVictim,
        turnUndeadVictim,
        occupant,
        seeTarget,
        knowTarget
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
        doesNotHaveTrait,
        beforeClosing,
        afterClosing,
        seesSomeoneWithTrait,
        seesSomeoneWithoutTrait,
        seesSomeoneRelatLevelAtOrAbove,
        seesSomeoneRelatLevelAtOrBelow,
        knowsAboutMemoryTag,
        hasDoneActionToday,
        hasNotDoneActionToday,
        SeePersonKnowledge,
        NotSeePersonKnowledge,
        SeePersonForm,
        NotSeePersonForm,
        knowsSomeoneWithTag,
        doesNotKnowSomeoneWithTag,
        reachedTarget,
        hasNotReachedTarget,
        completedDialogueFile,
        scriptedTaskActive,
        scriptedTaskCompleted,
        scriptedTaskNotCompleted,
        lastSpokeTo,
        reachedPerson,
        notReachedPerson,
        hasNamedOccupant,
        notHaveNamedOccupant,
        notLastSpokeTo,
        attackedPerson,
        closeToCharacter
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
    public enum ViewTowards : int
    {
        unforgivable = -25,
        despise = -10,
        extremelyNegative = -6,
        veryNegative = -3,
        negative = -1,
        neutral = 0,
        positive = 1,
        veryPositive = 3,
        extremelyPositive = 6,
        adore = 10,
        obsessed = 25

    }
    [Serializable]
    public enum ObjectType : short
    {
        none,
        bed,
        foodBowl,
        traderDesk,
        traderChair,
        bookOfTheDead,
        pumpkin,
        ale,
        barStool,
        cursedWerewolfAmulet,
        tree,
        rock,
        desk,
        chair,
        table

    }

    [Serializable]
    public enum ScriptedTaskType : short
    {
        none,
        talkToPlayer,
        takePlayerToInn,
        killedWerewolf,


    }
    [Serializable]
    public enum ScriptedTaskProgressType : short
    {
        notActive,
        activated,
        completed
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
