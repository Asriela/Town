using System.Collections.Generic;
using System.Linq;
using Mind;
using UnityEngine;

public class Impression : MonoBehaviour
{
    public int FearTowardsPlayer { get; set; } = 20;
    public int TrustTowardsPlayer { get; set; } = 20;

    public int ProgressToBreakdown { get; set; } = 0;

    public int Confidence { get; set; } = 10;

    private Dictionary<SocialImpression, int> socialImpressionsTowardsPlayer = new();
    private SocialImpression lastImpression = SocialImpression.none;

    public SocialImpression GetSocialImpression()
    {
        if (socialImpressionsTowardsPlayer.Count == 0)
        {

            return SocialImpression.none;
        }

        var maxEntry = socialImpressionsTowardsPlayer.OrderByDescending(kv => kv.Value).FirstOrDefault();
        var endImpression = maxEntry.Key;
        if (maxEntry.Value == socialImpressionsTowardsPlayer[lastImpression])
        {
            endImpression = lastImpression;
        }

        endImpression = DetectIfOverLimmit(endImpression, socialImpressionsTowardsPlayer[maxEntry.Key]);
        return endImpression;
    }
    public string GetSocialImpressionText()
    {
        if (socialImpressionsTowardsPlayer.Count == 0)
        {

            return "Has no impression of you";
        }


        return $"Finds you {GetSocialImpression()}";

    }
    public SocialImpression AddSocialImpression(SocialImpression impression, int amount)
    {
        var ret = impression;
        lastImpression= impression;

        if (socialImpressionsTowardsPlayer.ContainsKey(impression))
        {
            socialImpressionsTowardsPlayer[impression] += amount;
        }
        else
        {
            socialImpressionsTowardsPlayer.Add(impression, amount);
        }



        return DetectIfOverLimmit(ret, socialImpressionsTowardsPlayer[impression]);

    }

    SocialImpression DetectIfOverLimmit(SocialImpression impression, int amount)
    {
        var ret = impression;
        switch (impression)
        {
            case SocialImpression.annoying:
                if (amount > 5)
                { ret = SocialImpression.infuriating; }
                break;
            case SocialImpression.scary:
                if (amount > 7)
                { ret = SocialImpression.terrifying; }
                break;
            case SocialImpression.evil:
                if (amount > 7)
                { ret = SocialImpression.monstrous; }
                break;
            case SocialImpression.amusing:
                if (amount > 7)
                { ret = SocialImpression.hilarious; }
                break;
            case SocialImpression.rude:
                if (amount > 7)
                { ret = SocialImpression.assHole; }
                break;
            case SocialImpression.boring:
                if (amount > 7)
                { ret = SocialImpression.antiSocial; }
                break;
            case SocialImpression.serious:
                if (amount > 7)
                { ret = SocialImpression.militant; }
                break;
            case SocialImpression.kind:
                if (amount > 7)
                { ret = SocialImpression.warmHearted; }
                break;
            case SocialImpression.stuckUp:
                if (amount > 7)
                { ret = SocialImpression.arrogant; }
                break;
            case SocialImpression.brave:
                if (amount > 7)
                { ret = SocialImpression.lifeOfTheParty; }
                break;
            case SocialImpression.charming:
                if (amount > 7)
                { ret = SocialImpression.lifeOfTheParty; }
                break;
            case SocialImpression.romantic:
                if (amount > 7)
                { ret = SocialImpression.sexy; }
                break;
        }
        return ret;
    }
}
public enum BaseSocial
{
    annoying,
    scary,
    amusing,
    rude,
    boring,
    serious,
    kind,
    stuckUp,
    brave,
    charming,
    romantic
}
