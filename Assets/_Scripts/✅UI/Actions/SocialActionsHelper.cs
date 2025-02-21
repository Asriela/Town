using System.Collections.Generic;
using Mind;
using NUnit.Framework;
using UnityEngine;

public enum SocialActionMenuType
{
    charm,
    coerce,
    flirt,
    give,
    none
}
public static class SocialActionsHelper
{
    private static Dictionary<SocializeType, ActionOption> charmOptions = new(SetupCharmOptions());
    private static Dictionary<SocializeType, ActionOption> coerceOptions = new(SetupCoerceOptions());
    private static Dictionary<SocializeType, ActionOption> giveOptions = new(SetupGiveOptions());

    private static readonly float veryLongTime = 90f;

    public static List<ActionOption> GetAvailableActions(SocialActionMenuType actionMenuType, Character personWeAreSpeakingTo)
    {
        List<ActionOption> ret = new();

        switch (personWeAreSpeakingTo.CharacterName)
        {
            case CharacterName.Dohlson:
                switch (actionMenuType)
                {
                    case SocialActionMenuType.charm:
                        ret.Add(charmOptions[SocializeType.smallTalk]);
                        ret.Add(charmOptions[SocializeType.hug]);
                        ret.Add(charmOptions[SocializeType.puthandOnShoulder]);
                        ret.Add(charmOptions[SocializeType.comfort]);
                        ret.Add(charmOptions[SocializeType.joke]);
                        break;
                    case SocialActionMenuType.coerce:
                        ret.Add(coerceOptions[SocializeType.insult]);

                        break;
                    case SocialActionMenuType.give:
                        ret.Add(giveOptions[SocializeType.bribe]);

                        break;
                }
                break;
            case CharacterName.Onar:
                switch (actionMenuType)
                {
                    case SocialActionMenuType.charm:
                        ret.Add(charmOptions[SocializeType.greet]);
                        ret.Add(charmOptions[SocializeType.sitAtFireplace]);
                        ret.Add(charmOptions[SocializeType.smallTalk]);
                        ret.Add(charmOptions[SocializeType.joke]);
                        ret.Add(charmOptions[SocializeType.hug]);
                        ret.Add(charmOptions[SocializeType.reassure]);
                        ret.Add(charmOptions[SocializeType.playChess]);

                        break;
                    case SocialActionMenuType.coerce:
                        ret.Add(coerceOptions[SocializeType.insult]);             
                        ret.Add(coerceOptions[SocializeType.blackmail]);
                        ret.Add(coerceOptions[SocializeType.threaten]);
                        ret.Add(coerceOptions[SocializeType.beatUp]);
                        ret.Add(coerceOptions[SocializeType.manipulate]);
                        ret.Add(coerceOptions[SocializeType.messWithTheirHead]);
                        break;
                    case SocialActionMenuType.give:
                        ret.Add(giveOptions[SocializeType.makeTea]);
                        ret.Add(giveOptions[SocializeType.bribe]);
                        ret.Add(giveOptions[SocializeType.giveAlcohol]);
                        ret.Add(giveOptions[SocializeType.giveFood]);
                        break;
                }
                break;
        }


        return ret;
    }

    public static string ProcessActionResponse(Character personWeAreSpeakingTo, ActionOption action, ref bool actionFailed)
    {

        return SocialActionProcessor.ProcessAction(personWeAreSpeakingTo, action, ref actionFailed);
    }



    private static Dictionary<SocializeType, ActionOption> SetupCharmOptions()
    {
        var charmOptions = new Dictionary<SocializeType, ActionOption>();
        charmOptions.Add(SocializeType.greet, new ActionOption(
        SocializeType.greet,
        "Greet",
        1,
        "💬",
        5,
        "Takes up a small amount of time but only gives 1 trust.",
        1,
        0,
        false,
        false,
        ViewTowards.none,
        ViewTowards.neutral,
        MemoryTags.none,
        SubMenu.charmActions,
        new Dictionary<MemoryTags, int> { },
        new List<MemoryTags> { }

        ));

        charmOptions.Add(SocializeType.smallTalk, new ActionOption(
        SocializeType.smallTalk,
        "Small talk",
        -1,
        "💬",
        10,
        "Takes up a small amount of time but only gives 2 trust.",
        1,
        1,
        false,
        false,
        ViewTowards.none,
        ViewTowards.neutral,
        MemoryTags.none,
        SubMenu.charmActions
        ));
        charmOptions.Add(SocializeType.sitAtFireplace, new ActionOption(
        SocializeType.sitAtFireplace,
        "Sit at fireplace",
        -1,
        "💬",
        30,
        "No relationship or trust bonus just relaxes the person",
        0,
        0,
        false,
        false,
        ViewTowards.none,
        ViewTowards.neutral,
        MemoryTags.none,
        SubMenu.charmActions,
                          new Dictionary<MemoryTags, int> { },
        new List<MemoryTags> { { MemoryTags.relaxed } }
        ));

        charmOptions.Add(SocializeType.joke, new ActionOption(
        SocializeType.joke,
        "Joke",
        3,
        "💬",
        5,
        "Risky but works when it works",
        2,
        1,
        false,
        false,
        ViewTowards.none,
        ViewTowards.neutral,
        MemoryTags.none,
        SubMenu.charmActions
        ));

        charmOptions.Add(SocializeType.hug, new ActionOption(
        SocializeType.hug,
        "Hug",
        -1,
        "🤗",
        5,
        "Not everyone is a hugger and people in distress are more receptive to hugs.",
        2,
        1,
        false,
        false,
                ViewTowards.none,
        ViewTowards.positive,
        MemoryTags.none,
        SubMenu.charmActions,
        new Dictionary<MemoryTags, int> { { MemoryTags.emotional, 2 } }
        ));



        charmOptions.Add(SocializeType.playChess, new ActionOption(
             SocializeType.playChess,
        "Play Chess",
        -1,
        "♟",
        30,
        "Play a game of chess, requires the person to be relaxed.",
        4,
        3,
        false,
        false,
        ViewTowards.none,
        ViewTowards.veryPositive,
        MemoryTags.relaxed,
        SubMenu.charmActions
        ));

        charmOptions.Add(SocializeType.reassure, new ActionOption(SocializeType.reassure,
        "Reassure",
        -1,
        "👐",
        5,
        "Show reassurance when they are emotional",
        2,
        1,
        false,
        false,
        ViewTowards.none,
        ViewTowards.positive,
        MemoryTags.emotional,
        SubMenu.charmEmotions
        ));

        charmOptions.Add(SocializeType.puthandOnShoulder, new ActionOption(SocializeType.puthandOnShoulder,
        "Put hand on shoulder",
        -1,
        "👐",
        1,
        "Every bit helps",
        1,
        1,
        false,
        false,
        ViewTowards.none,
        ViewTowards.positive,
        MemoryTags.emotional,
        SubMenu.charmEmotions
        ));

        charmOptions.Add(SocializeType.comfort, new ActionOption(SocializeType.comfort,
        "Comfort",
        -1,
        "❤",
        10,
        "Comfort the person when they are emotional",
        4,
        3,
        false,
        false,
        ViewTowards.none,
        ViewTowards.veryPositive,
        MemoryTags.emotional,
        SubMenu.charmEmotions
        ));





        return charmOptions;
    }

    private static Dictionary<SocializeType, ActionOption> SetupCoerceOptions()
    {
        var coerceOptions = new Dictionary<SocializeType, ActionOption>();
        coerceOptions.Add(SocializeType.insult, new ActionOption(SocializeType.insult,
        "Insult",
        -1,
        "🔪",
        1,
        "You can get by insulting someone without them hating your forever",
        -1,
        -1,
        false,
        false,
        ViewTowards.veryPositive,
        ViewTowards.neutral,
        MemoryTags.none,
        SubMenu.coerceActions,
        new Dictionary<MemoryTags, int> { },
        new List<MemoryTags> { }
        ));

        coerceOptions.Add(SocializeType.intimidate, new ActionOption(SocializeType.intimidate,
        "Intimidate",
        -1,
        "🔪",
        5,
        "Intimidate through foul language and body posture.",
        -2,
        -2,
        false,
        false,
        ViewTowards.veryPositive,
        ViewTowards.neutral,
        MemoryTags.none,
        SubMenu.coerceActions,
        new Dictionary<MemoryTags, int> { },
        new List<MemoryTags> { { MemoryTags.tense } }
        ));
        coerceOptions.Add(SocializeType.manipulate, new ActionOption(SocializeType.manipulate,
        "Manipulate",
        4,
        "🔪",
        10,
        "Lie to the person to increase your relationship with them.",
        0,
        2,
        false,
        false,
        ViewTowards.veryPositive,
        ViewTowards.neutral,
        MemoryTags.none,
        SubMenu.coerceActions,
        new Dictionary<MemoryTags, int> { },
        new List<MemoryTags> { { MemoryTags.scared } }
        ));
        coerceOptions.Add(SocializeType.threaten, new ActionOption(SocializeType.threaten,
        "Threaten",
        -1,
        "🔪",
        5,
        "Threaten with physical violence, short term gain, long term loss as they will hate you and may report you. Action can break the person.",
        -4,
        -4,
        false,
        false,
        ViewTowards.veryPositive,
        ViewTowards.neutral,
        MemoryTags.none,
        SubMenu.coerceActions,
        new Dictionary<MemoryTags, int> { },
        new List<MemoryTags> { { MemoryTags.scared } }
        ));

        coerceOptions.Add(SocializeType.beatUp, new ActionOption(SocializeType.beatUp,
        "Beat up",
        -1,
        "🔪",
        10,
        "Physically beat up, its really a last resort as they will dispise you after this. Action can break the person. ",
        -6,
        -8,
        false,
        false,
        ViewTowards.neutral,
        ViewTowards.veryNegative,
        MemoryTags.none,
        SubMenu.coerceActions,
        new Dictionary<MemoryTags, int> { },
        new List<MemoryTags> { { MemoryTags.scared } }
        ));


        coerceOptions.Add(SocializeType.blackmail, new ActionOption(SocializeType.blackmail,
        "Blackmail",
        1,
        "🔪",
        5,
        "Only available if you have dirt on someone. Current dirt: You know Onar kept Ashla locked up in his cabin.",
        -4,
        0,
        false,
        false,
        ViewTowards.extremelyPositive,
        ViewTowards.neutral,

        MemoryTags.none,
        SubMenu.coerceActions,
        new Dictionary<MemoryTags, int> { },
        new List<MemoryTags> { { MemoryTags.scared } }
        ));

        coerceOptions.Add(SocializeType.messWithTheirHead, new ActionOption(SocializeType.messWithTheirHead,
        "Psychic attack",
        1,
        "🔪",
        60,
        "Psychological attack by getting into the persons insecurities. Action can break the person.",
        -6,
        0,
        false,
        false,
         ViewTowards.neutral,
        ViewTowards.veryNegative,
        MemoryTags.none,
        SubMenu.coerceActions,
        new Dictionary<MemoryTags, int> { },
        new List<MemoryTags> { { MemoryTags.scared } }
        ));
        return coerceOptions;
    }

    private static Dictionary<SocializeType, ActionOption> SetupGiveOptions()
    {
        var charmOptions = new Dictionary<SocializeType, ActionOption>();
        charmOptions.Add(SocializeType.giveFood, new ActionOption(
            SocializeType.giveFood,
        "Give Food",
        1,
        "🍗",
        5,
        "Give them the meal you brought with you. You loose a meal but you gain a friend.",
        4,
        2,
        false,
        false,
        ViewTowards.none,
        ViewTowards.neutral,
        MemoryTags.none,
        SubMenu.charmGive,
        new Dictionary<MemoryTags, int> { },
        new List<MemoryTags> { { MemoryTags.relaxed } }
        ));
        charmOptions.Add(SocializeType.makeTea, new ActionOption(
          SocializeType.makeTea,
          "Make tea",
          3,
          "💬",
          30,
          "Small relationship impact, relaxes the person",
          2,
          1,
          false,
          false,
          ViewTowards.none,
          ViewTowards.positive,
          MemoryTags.none,
          SubMenu.charmGive,
                  new Dictionary<MemoryTags, int> { },
        new List<MemoryTags> { { MemoryTags.relaxed } }
          ));
        charmOptions.Add(SocializeType.bribe, new ActionOption(
             SocializeType.bribe,
        "Bribe",
        2,
        "💰",
        5,
        "Costs 40 coin and gets them to talk but it dammages your relationship with them",
        4,
        -1,
        false,
        false,
        ViewTowards.adore,
        ViewTowards.none,
        MemoryTags.none,
        SubMenu.charmGive,
        new Dictionary<MemoryTags, int> { },
        new List<MemoryTags> { }
        ));



        charmOptions.Add(SocializeType.giveAlcohol, new ActionOption(SocializeType.giveAlcohol,
        "Give alcohol",
        1,
        "👐",
        60,
        "Give the person alcohol, will lead to impairment",
        4,
        1,
        false,
        false,
        ViewTowards.none,
        ViewTowards.neutral,
        MemoryTags.none,
        SubMenu.charmGive,
        new Dictionary<MemoryTags, int> { { MemoryTags.emotional, 3 } },
        new List<MemoryTags> { { MemoryTags.drunk } }
        ));

        return charmOptions;
    }

}
