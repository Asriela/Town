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
    private static List<ActionOption> charmOptions = new(SetupCharmOptions());
    private static List<ActionOption> coerceOptions = new(SetupCoerceOptions());
    private static List<ActionOption> giveOptions = new(SetupGiveOptions());

    public static List<ActionOption> GetAvailableActions(SocialActionMenuType actionMenuType, Character personWeAreSpeakingTo  )
    {
        List<ActionOption> ret = new ();

        switch (actionMenuType)
        {
            case SocialActionMenuType.charm:
                ret = charmOptions;
                break;
            case SocialActionMenuType.coerce:
                ret = coerceOptions;
                break;
            case SocialActionMenuType.give:
                ret = giveOptions;
                break;
        }

        return ret;
    }

    public static string ProcessActionResponse(Character personWeAreSpeakingTo, ActionOption action )
    {
        
        var ret = "";
        switch (action.Enum)
        {
            case SocializeType.smallTalk:
                ret= "This isnt really the time for small talk..";
                personWeAreSpeakingTo.Impression.AddSocialImpression(SocialImpression.annoying, 1);
                break;
            case SocializeType.hug:
                ret = "Get the fuck off me!";
                personWeAreSpeakingTo.Impression.AddSocialImpression(SocialImpression.annoying, 2);
                break;
        }
        return $"<color={MyColor.DarkGreyHex}>[From {action.Name}]</color> " +ret;
    }

    private static List<ActionOption> SetupGiveOptions()
    {
        var charmOptions = new List<ActionOption>();
        charmOptions.Add(new ActionOption(
            SocializeType.giveFood,
        "Give Food",
        "🍗",
        "Give them the meal you brought with you. You loose a meal but you gain a friend.",
        4,
        2,
        false,
        false,
        ViewTowards.neutral,
        MemoryTags.none,
        SubMenu.charmGive,
        new Dictionary<MemoryTags, int> { },
        new List<MemoryTags> { { MemoryTags.relaxed } }
        ));

        charmOptions.Add(new ActionOption(
             SocializeType.bribe,
        "Bribe",
        "💰",
        "Costs 40 coin and gets them to talk but it dammaged your relationship with them",
        4,
        -2,
        false,
        false,
        ViewTowards.neutral,
        MemoryTags.none,
        SubMenu.charmGive,
        new Dictionary<MemoryTags, int> { },
        new List<MemoryTags> { }
        ));

       

        charmOptions.Add(new ActionOption(SocializeType.giveAlcohol,
        "Give alcohol",
        "👐",
        "Give the person alcohol, will lead to impairment",
        4,
        1,
        false,
        false,
        ViewTowards.neutral,
        MemoryTags.none,
        SubMenu.charmGive,
        new Dictionary<MemoryTags, int> { { MemoryTags.emotional, 3 } },
        new List<MemoryTags> { { MemoryTags.drunk } }
        ));

        return charmOptions;
    }


    private static List<ActionOption> SetupCharmOptions()
    {
        var charmOptions = new List<ActionOption>();
        charmOptions.Add(new ActionOption(
        SocializeType.smallTalk,
        "Small talk",
        "💬",
        "Takes up a small amount of time but only gives 1 trust.",
        1,
        1,
        false,
        false,
        ViewTowards.neutral,
        MemoryTags.none,
        SubMenu.charmActions
        ));

        charmOptions.Add(new ActionOption(
        SocializeType.hug,
        "Hug",
        "🤗",
        "Not everyone is a hugger and people in distress are more receptive to hugs.",
        2,
        1,
        false,
        false,
        ViewTowards.positive,
        MemoryTags.none,
        SubMenu.charmActions,
        new Dictionary<MemoryTags, int> { { MemoryTags.emotional, 2 } }
        ));

    

        charmOptions.Add(new ActionOption(
             SocializeType.playChess,
        "Play Chess",
        "♟",
        "Play a game of chess, requires the person to be relaxed.",
        4,
        3,
        false,
        false,
        ViewTowards.veryPositive,
        MemoryTags.relaxed,
        SubMenu.charmActions
        ));

        charmOptions.Add(new ActionOption(SocializeType.comfort,
        "Comfort",
        "❤",
        "Comfort the person when they are emotional",
        4,
        3,
        false,
        false,
        ViewTowards.veryPositive,
        MemoryTags.emotional,
        SubMenu.charmEmotions
        ));

        charmOptions.Add(new ActionOption(SocializeType.sympathize,
        "Sympathize",
        "👐",
        "Show sympathy when they are emotional",
        2,
        1,
        false,
        false,
        ViewTowards.positive,
        MemoryTags.emotional,
        SubMenu.charmEmotions
        ));

      

        return charmOptions;
    }

    private static List<ActionOption> SetupCoerceOptions()
    {
        var coerceOptions = new List<ActionOption>();
        coerceOptions.Add(new ActionOption(SocializeType.intimidate,
        "Intimidate",
        "🔪",
        "Intimidate through foul language and body posture.",
        -2,
        -2,
        false,
        false,
        ViewTowards.neutral,
        MemoryTags.none,
        SubMenu.coerceActions,
        new Dictionary<MemoryTags, int> { },
        new List<MemoryTags> { { MemoryTags.tense } }
        ));

        coerceOptions.Add(new ActionOption(SocializeType.threaten,
        "Threaten",
        "🔪",
        "Threaten with physical violence, short term gain, long term loss as they will hate you and may report you.",
        -4,
        -4,
        false,
        false,
        ViewTowards.neutral,
        MemoryTags.none,
        SubMenu.coerceActions,
        new Dictionary<MemoryTags, int> { },
        new List<MemoryTags> { { MemoryTags.tense } }
        ));

        coerceOptions.Add(new ActionOption(SocializeType.beatUp,
        "Beat up",
        "🔪",
        "Physically beat up, its really a last resort as they will dispise you after this. ",
        -6,
        -8,
        false,
        false,
        ViewTowards.veryNegative,
        MemoryTags.none,
        SubMenu.coerceActions,
        new Dictionary<MemoryTags, int> { },
        new List<MemoryTags> { { MemoryTags.scared } }
        ));


        coerceOptions.Add(new ActionOption(SocializeType.blackmail,
        "Blackmail",
        "🔪",
        "Only available if you have dirt on someone. Current dirt: You know Onar kept Ashla locked up in his cabin.",
        -4,
        -2,
        false,
        false,
        ViewTowards.neutral,
        MemoryTags.none,
        SubMenu.coerceActions,
        new Dictionary<MemoryTags, int> { },
        new List<MemoryTags> { { MemoryTags.tense } }
        ));

        return coerceOptions;
    }
}
