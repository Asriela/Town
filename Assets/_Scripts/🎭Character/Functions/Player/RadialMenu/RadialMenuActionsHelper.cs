using Mind;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;


public class RadialMenuActionsHelper : MonoBehaviour
{
    private float socializeTimeLeft = -1;
    public SocializeType SocialAction { get; set; } = SocializeType.none;
    private int pointsToReward = 0;
    private Character personWeAreInteractingWith;
    private Player player;
    public void Initialize(Player player)
    {
        this.player = player;
    }
    private void Update()
    {
        DoSocialAction();
    }
    public bool HasBlackmailOn(Character target)
    {
        bool ret=false;
        // Check if we have blackmail material on this character
       // return target.HasBlackmailData();
       return ret;
    }


    public void PerformCharmAction(Character target, string action, int points)
    {
        this.personWeAreInteractingWith = target;
        pointsToReward= points;
        switch (action)
        {
            case "Buy Drink":
                SocialAction=SocializeType.drinking;
                break;
            case "Hangout":
                Debug.Log($"Hanging out with {target.name}...");
                break;
            case "Hug":
                Debug.Log($"Giving {target.name} a hug...");
                break;
            case "Dance":
                Debug.Log($"Dancing with {target.name}...");
                break;
        }
    }


    public void PerformCoerceAction(Character target, string action, int points)
    {
        this.personWeAreInteractingWith= target;
        pointsToReward = points;
        switch (action)
        {
            case "Threaten":
                Debug.Log($"Threatening {target.name}...");
                break;
            case "Blackmail":
                Debug.Log($"Blackmailing {target.name} with their secrets...");
                break;
        }
    }

    private void DoSocialAction()
    {
        if(SocialAction == SocializeType.none)return;

        player.RadialMenuInteraction.CloseInteractionMenu();

        switch (SocialAction)
        {
            case SocializeType.drinking:
                if (socializeTimeLeft == -1)
                {
                    socializeTimeLeft = 10000;
                    player.Appearance.SetSpriteAction("drinking");
                    personWeAreInteractingWith.Appearance.SetSpriteAction("drinking");

                    WorldManager.Instance.SetRampUpSpeedOfTime(SpeedOfTime.fast);
                }

                break;
        }





        socializeTimeLeft -= Time.timeScale;

        if (socializeTimeLeft < 0)
        {

            socializeTimeLeft = -1;
  
            SocialAction = SocializeType.none;
            player.Appearance.ResetSprite();
            WorldManager.Instance.SetSpeedOfTime(SpeedOfTime.normal);
            personWeAreInteractingWith.Appearance.ResetSprite();
            ActionResolution(SocialAction);

 
        }
    }

    private void ActionResolution(SocializeType socialAction)
    {
        float effectFromInteraction = personWeAreInteractingWith.Relationships.AddInteractionEffect(socialAction, player);
        if (pointsToReward < 0)
        {
            personWeAreInteractingWith.Persuasion.FearTowardsPlayer += -pointsToReward;
        }

        if (pointsToReward > 0)
        {
            personWeAreInteractingWith.Persuasion.TrustTowardsPlayer += pointsToReward;
        }
        
    }
}
