using Mind;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static Unity.Burst.Intrinsics.Arm;

public static class TextConverter
{
    public static string GetKeyText(string key)
    {
        var ret="";
        if (key!="")
        {
          
                ret= "Ashla was taken by broken magic";
         
        }
        return ret;
    }
    public static string GetRelationshipStatusText(Character target)
    {
        var ret = "neutral";
        var player = WorldManager.Instance.ThePlayer;
        var relationship = target.Relationships.GetRelationshipWith(target, player);

        if (relationship <= (float)ViewTowards.unforgivable)
        {
            ret = "finds you unforgivable";
        }
        else if (relationship <= (float)ViewTowards.despise)
        {
            ret = "hates you";
        }
        else if (relationship <= (float)ViewTowards.extremelyNegative)
        {
            ret = "dispises you";
        }
        else if (relationship <= (float)ViewTowards.veryNegative)
        {
            ret = "really dislikes you";
        }
        else if (relationship <= (float)ViewTowards.negative)
        {
            ret = "dislikes you";
        }
        else if (relationship >= (float)ViewTowards.obsessed)
        {
            ret = "is obsessed with you";
        }
        else if (relationship >= (float)ViewTowards.adore)
        {
            ret = "loves you";
        }
        else if (relationship >= (float)ViewTowards.extremelyPositive)
        {
            ret = "adores you";
        }
        else if (relationship >= (float)ViewTowards.veryPositive)
        {
            ret = "really likes you";
        }
        else if (relationship >= (float)ViewTowards.positive)
        {
            ret = "likes you";
        }

        return ret + $" {relationship}";
    }
    public static string GetStatString(string trust, string fear, string relationship, string mood, string impression, string greenOn, string redOn)
    {
        return $"<color={greenOn}>TRUST {trust}</color>\n<color={redOn}>FEAR {fear}</color>\n-----------\n<color=#A0A0A0>RELATIONSHIP</color>\n{relationship}</color>\n<color=#A0A0A0>MOOD</color>\n{mood}\n<color=#A0A0A0>IMPRESSION</color>\n{impression} ";

    }
    public static string ChangeSocialInteractionToText(SocializeType type, string character)
    {
        var ret = "";
        switch (type)
        {
            case SocializeType.drinking:
                ret = $"drinking with {character}";
                break;
        }
        return ret.ToUpper();
    }
}
