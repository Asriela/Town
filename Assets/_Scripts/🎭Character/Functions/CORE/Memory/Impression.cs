using System.Collections.Generic;
using System.Linq;
using Mind;
using UnityEngine;

public class Impression : MonoBehaviour
{
    public int FearTowardsPlayer { get; set; } = 0;
    public int TrustTowardsPlayer { get; set; } = 0;

    public int ProgressToBreakdown { get; set;} =0;

    public int Confidence { get; set; } = 10;

    private Dictionary<SocialImpression, int> socialImpressionsTowardsPlayer = new();


    public SocialImpression GetSocialImpression()
    {
        if (socialImpressionsTowardsPlayer.Count == 0)
        {

            return SocialImpression.none;
        }

        var maxEntry = socialImpressionsTowardsPlayer.OrderByDescending(kv => kv.Value).FirstOrDefault();
        return maxEntry.Key;
    }
    public string GetSocialImpressionText()
    {
        if (socialImpressionsTowardsPlayer.Count == 0)
        {

            return "Has no impression of you";
        }

        var maxEntry = socialImpressionsTowardsPlayer.OrderByDescending(kv => kv.Value).FirstOrDefault();
        return $"Finds you {maxEntry.Key}";
        
    }
    public void AddSocialImpression(SocialImpression impression, int amount)
    {
        if (socialImpressionsTowardsPlayer.ContainsKey(impression))
        {
            socialImpressionsTowardsPlayer[impression] += amount;
        }
        else
        {
            socialImpressionsTowardsPlayer.Add(impression, amount);
        }

    }
}
