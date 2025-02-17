using Mind;
using UnityEngine;



public class PerformMenuAction : MonoBehaviour
{
    private float socializeTimeLeft = -1;
    public SocializeType SocialAction { get; set; } = SocializeType.none;
    private bool currentActionFailed= false;
    private int pointsToReward = 0;
    private int relationshipImpact = 0;
    private Character personWeAreInteractingWith;
    private Player player;
    private bool socializeUntilAnimationIsOver;
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


    public void PerformAction(Character target, ActionOption actionOption, bool actionFailed)
    {
        this.personWeAreInteractingWith = target;
        currentActionFailed= actionFailed;

        if (ProcessActionOption(actionOption, target))
        {
            SocialAction = actionOption.Enum;
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


    private void DoSocialAction()
    {
        if (SocialAction == SocializeType.none)
            return;

        player.RadialMenuInteraction.CloseInteractionMenu();
        if (socializeTimeLeft == -1)
        {
            socializeUntilAnimationIsOver=false;
            switch (SocialAction)
            {
                case SocializeType.drinking:

                    socializeTimeLeft = 5000;
                    player.Appearance.SetSpriteAction("drinking");
                    //personWeAreInteractingWith.Appearance.SetSpriteAction("drinking");

                    WorldManager.Instance.SetRampUpSpeedOfTime(SpeedOfTime.fast);


                    break;

                case SocializeType.smallTalk:

                    socializeTimeLeft = 1000;
                    player.Appearance.SetSpriteAction("talk");
                    //personWeAreInteractingWith.Appearance.SetSpriteAction("talk");

                    WorldManager.Instance.SetRampUpSpeedOfTime(SpeedOfTime.fast);


                    break;

                case SocializeType.giveFood:

                    socializeTimeLeft = 2000;
                    player.Appearance.SetSpriteAction("give");

                    //personWeAreInteractingWith.Appearance.SetSpriteAction("talk");
                    socializeUntilAnimationIsOver = true;
                    WorldManager.Instance.SetRampUpSpeedOfTime(SpeedOfTime.normal);


                    break;

                case SocializeType.bribe:

                    socializeTimeLeft = 2000;
                    player.Appearance.SetSpriteAction("give");
                    socializeUntilAnimationIsOver=true;
                    //personWeAreInteractingWith.Appearance.SetSpriteAction("talk");

                    WorldManager.Instance.SetRampUpSpeedOfTime(SpeedOfTime.normal);


                    break;

                case SocializeType.hug:

                    socializeTimeLeft = 2000;
                    player.Appearance.SetSpriteAction("talk");

                    //personWeAreInteractingWith.Appearance.SetSpriteAction("talk");
                    socializeUntilAnimationIsOver = true;
                    WorldManager.Instance.SetRampUpSpeedOfTime(SpeedOfTime.normal);


                    break;
                case SocializeType.comfort:

                    socializeTimeLeft = 2000;
                    player.Appearance.SetSpriteAction("talk");

                    //personWeAreInteractingWith.Appearance.SetSpriteAction("talk");
                    socializeUntilAnimationIsOver = true;
                    WorldManager.Instance.SetRampUpSpeedOfTime(SpeedOfTime.normal);


                    break;
                case SocializeType.sympathize:
                    socializeTimeLeft = 2000;
                    player.Appearance.SetSpriteAction("talk");
                    //personWeAreInteractingWith.Appearance.SetSpriteAction("talk");
                    socializeUntilAnimationIsOver = true;
                    break;
                case SocializeType.playChess:
                    socializeTimeLeft = 2000;
                    player.Appearance.SetSpriteAction("talk");
                    //personWeAreInteractingWith.Appearance.SetSpriteAction("talk");
                    socializeUntilAnimationIsOver = true;
                    break;
                case SocializeType.intimidate:
                    socializeTimeLeft = 2000;
                    player.Appearance.SetSpriteAction("talk");
                    //personWeAreInteractingWith.Appearance.SetSpriteAction("talk");
                    socializeUntilAnimationIsOver = true;
                    break;
                case SocializeType.threaten:
                    socializeTimeLeft = 2000;
                    player.Appearance.SetSpriteAction("talk");
                    //personWeAreInteractingWith.Appearance.SetSpriteAction("talk");
                    socializeUntilAnimationIsOver = true;
                    break;
                case SocializeType.beatUp:
                    socializeTimeLeft = 2000;
                    player.Appearance.SetSpriteAction("talk");
                    //personWeAreInteractingWith.Appearance.SetSpriteAction("talk");
                    socializeUntilAnimationIsOver = true;
                    break;
                case SocializeType.insult:
                    socializeTimeLeft = 2000;
                    player.Appearance.SetSpriteAction("talk");
                    //personWeAreInteractingWith.Appearance.SetSpriteAction("talk");
                    socializeUntilAnimationIsOver = true;
                    break;
                case SocializeType.messWithTheirHead:
                    socializeTimeLeft = 2000;
                    player.Appearance.SetSpriteAction("talk");
                    //personWeAreInteractingWith.Appearance.SetSpriteAction("talk");
                    socializeUntilAnimationIsOver = true;
                    break;
                case SocializeType.blackmail:
                    socializeTimeLeft = 2000;
                    player.Appearance.SetSpriteAction("talk");
                    //personWeAreInteractingWith.Appearance.SetSpriteAction("talk");
                    socializeUntilAnimationIsOver = true;
                    break;
            }
        }
        if (socializeUntilAnimationIsOver && player.Appearance.HasAnimationEnded())
            socializeTimeLeft = -1;



        socializeTimeLeft -= Time.timeScale;

        if (socializeTimeLeft < 0)
        {

            socializeTimeLeft = -1;


            player.Appearance.ResetSprite();
            WorldManager.Instance.SetSpeedOfTime(SpeedOfTime.normal);
            personWeAreInteractingWith.Appearance.ResetSprite();
            if(currentActionFailed==false)
            ActionResolution(SocialAction, personWeAreInteractingWith);
            SocialAction = SocializeType.none;


        }
    }

    private void ActionResolution(SocializeType socialAction, Character personWeAreTalkingTo)
    {
        float effectFromInteraction = personWeAreInteractingWith.Relationships.AddInteractionEffect(socialAction, player, relationshipImpact);
        if (pointsToReward < 0)
        {
            personWeAreInteractingWith.Impression.FearTowardsPlayer += -pointsToReward;
        }

        if (pointsToReward > 0)
        {
            personWeAreInteractingWith.Impression.TrustTowardsPlayer += pointsToReward;
        }

        GameManager.Instance.UpdateInteractionMenu(personWeAreTalkingTo, "");
    }
}
