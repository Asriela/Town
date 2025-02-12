using Mind;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;


public class RadialMenuActionsHelper : MonoBehaviour
{
    private float socializeTimeLeft = -1;
    public SocializeType SocialAction { get; set; } = SocializeType.none;
    private int pointsToReward = 0;
    private int relationshipImpact =0;
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
        bool ret = false;
        // Check if we have blackmail material on this character
        // return target.HasBlackmailData();
        return ret;
    }


    public void PerformCharmAction(Character target, ActionOption actionOption)
    {
        this.personWeAreInteractingWith = target;

        if (ProcessActionOption(actionOption, target))
        {
            switch (actionOption.Name)
            {
                case "Give alcohol":
                    SocialAction = SocializeType.drinking;
                    break;
                case "Small talk":
                    SocialAction = SocializeType.smallTalk;
                    break;
                case "Give Food":
                    SocialAction = SocializeType.giveFood;
                    break;
                case "Hug":
                    SocialAction = SocializeType.hug;
                    break;
            }
        }



    }

    private bool ProcessActionOption(ActionOption actionOption, Character target)
    {
        var ret = true;
        int additionalPoints = 0;
        var mood = target.State.VisualState[0];
        if (actionOption.BonusPoints.ContainsKey(mood))
        {
            additionalPoints = actionOption.BonusPoints[mood];
        }
        pointsToReward = actionOption.Points + additionalPoints;
        relationshipImpact = actionOption.RelationshipImpact;

        foreach (var effect in actionOption.ActionEffects)
        {

            target.State.VisualState[0] = effect;

        }



        actionOption.UsedUp = true;
        return ret;
    }

    public void PerformCoerceAction(Character target, ActionOption actionOption)
    {
        this.personWeAreInteractingWith = target;
        if (ProcessActionOption(actionOption, target))
        {
            switch (actionOption.Name)
            {
                case "Threaten":
                    Debug.Log($"Threatening {target.name}...");
                    break;
                case "Blackmail":
                    Debug.Log($"Blackmailing {target.name} with their secrets...");
                    break;
            }
        }
    }

    private void DoSocialAction()
    {
        if (SocialAction == SocializeType.none)
            return;

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

            case SocializeType.smallTalk:
                if (socializeTimeLeft == -1)
                {
                    socializeTimeLeft = 2000;
                    player.Appearance.SetSpriteAction("talking");
                    personWeAreInteractingWith.Appearance.SetSpriteAction("talking");

                    WorldManager.Instance.SetRampUpSpeedOfTime(SpeedOfTime.fast);
                }

                break;
        }





        socializeTimeLeft -= Time.timeScale;

        if (socializeTimeLeft < 0)
        {

            socializeTimeLeft = -1;


            player.Appearance.ResetSprite();
            WorldManager.Instance.SetSpeedOfTime(SpeedOfTime.normal);
            personWeAreInteractingWith.Appearance.ResetSprite();
            ActionResolution(SocialAction);
            SocialAction = SocializeType.none;


        }
    }

    private void ActionResolution(SocializeType socialAction)
    {
        float effectFromInteraction = personWeAreInteractingWith.Relationships.AddInteractionEffect(socialAction, player, relationshipImpact);
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
