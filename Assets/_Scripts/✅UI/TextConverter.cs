using Mind;
using UnityEngine;
using UnityEngine.TextCore.Text;

public static class TextConverter
{
    public static string GetRelationshipStatusText(Character target)
    {
        var ret = "neutral to you";
        var player = WorldManager.Instance.ThePlayer;
        var relationship = target.Relationships.GetRelationshipWith(target, player);
        if (relationship >= (float)ViewTowards.positive)
        {
            ret = "likes you";
        }
        if (relationship <= (float)ViewTowards.negative)
        {
            ret = "dislikes you";
        }
        return ret;
    }
}
