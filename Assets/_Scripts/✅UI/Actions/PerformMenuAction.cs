﻿using System.Linq.Expressions;
using Mind;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;



public class PerformMenuAction : MonoBehaviour
{
    private float socializeTimeLeft = -1;
    public SocializeType SocialAction { get; set; } = SocializeType.none;
    private bool currentActionFailed= false;
    private ActionOption currentAction;
    private int pointsToReward = 0;
    private int relationshipImpact = 0;
    float timeAddition = 0;
    private Character personWeAreInteractingWith;
    private Player player;
    private bool socializeUntilAnimationIsOver;
    private InteractionMenu theMenu;
    private MemoryTags moodToChange;
    MemoryTags currentMood;
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


    public void PerformAction(Character target, ActionOption actionOption, InteractionMenu theMenu)
    {
        this.personWeAreInteractingWith = target;
        this.theMenu = theMenu;

        if (ProcessActionOption(actionOption, target))
        {
            SocialAction = actionOption.Enum;
        }



    }

    private bool ProcessActionOption(ActionOption actionOption, Character target)
    {
        
        var ret = true;
        int additionalPoints = 0;
        currentMood = target.State.VisualState[0];
        timeAddition= actionOption.TimeLength;
        currentAction = actionOption;
        if (actionOption.BonusPoints.ContainsKey(currentMood))
        {
            additionalPoints = actionOption.BonusPoints[currentMood];
        }
        pointsToReward = actionOption.Points + additionalPoints;
        relationshipImpact = actionOption.RelationshipImpact;

        foreach (var effect in actionOption.ActionEffects)
        {

            moodToChange = effect;

        }

        if (actionOption.AmountOfUses != -1)
        {
            target.Impression.ActionsUsesLeftCount[actionOption.Enum]--;
            if (target.Impression.ActionsUsesLeftCount[actionOption.Enum] <= 0)
                actionOption.UsedUp = true;
        }

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
                case SocializeType.greet:

                    socializeTimeLeft = 2000;
                    player.Appearance.SetSpriteAction("talk"); 
                    socializeUntilAnimationIsOver = true;
                    //personWeAreInteractingWith.Appearance.SetSpriteAction("talk");

                    WorldManager.Instance.SetRampUpSpeedOfTime(SpeedOfTime.normal);


                    break;
                case SocializeType.puthandOnShoulder:

                    socializeTimeLeft = 2000;
                    player.Appearance.SetSpriteAction("talk");
                    socializeUntilAnimationIsOver = true;
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
                default:
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
            theMenu.DoingAction = null;
            socializeTimeLeft = -1;


            player.Appearance.ResetSprite();
            WorldManager.Instance.SetSpeedOfTime(SpeedOfTime.normal);
            personWeAreInteractingWith.Appearance.ResetSprite();
            currentActionFailed=false;
            personWeAreInteractingWith.Ui.Speak(personWeAreInteractingWith, SocialActionsHelper.ProcessActionResponse(personWeAreInteractingWith, currentAction, ref currentActionFailed));
            if (personWeAreInteractingWith.Impression.ProgressToBreakdown > 10)
            {
                personWeAreInteractingWith.Impression.BrokeDown=true;
                GameManager.Instance.InteractionMenu.pastDialogue += $"^{personWeAreInteractingWith.CharacterName} has had a mental breakdown^\n";
                WorldManager.Instance.ThePlayer.MenuInteraction.ChangeDiaSection("breakdown");
            }
                if(SocialActionProcessor.announceImpressionChange!=SocialImpression.none)
            { GameManager.Instance.InteractionMenu.pastDialogue += $"*{personWeAreInteractingWith.CharacterName} finds you {SocialActionProcessor.announceImpressionChange}*\n";
                SocialActionProcessor.announceImpressionChange=SocialImpression.none;
                }
            if (currentActionFailed==false)
            ActionResolution(SocialAction, personWeAreInteractingWith);
            SocialAction = SocializeType.none;
            if (timeAddition != 0)
            {
                WorldManager.Instance.TimeProgress += timeAddition;
                if (WorldManager.Instance.TimeProgress >= WorldManager.Instance.MaxTime)
                {
                    GameManager.Instance.EndGameState = GameState.lost;
                }
         
                GameManager.Instance.UpdateInteractionMenu(personWeAreInteractingWith, "");


            }
            var currentMood= personWeAreInteractingWith.State.VisualState[0];
            if (moodToChange!=0 && moodToChange != MemoryTags.none && currentMood!= moodToChange)
            {
                GameManager.Instance.InteractionMenu.NewMood = $"@that made {personWeAreInteractingWith.CharacterName} {moodToChange}@\n";
                personWeAreInteractingWith.State.SetVisualState(moodToChange);
                GameManager.Instance.UpdateInteractionMenu(personWeAreInteractingWith, "");
            }
            timeAddition=0;




        }
    }

    private void ActionResolution(SocializeType socialAction, Character personWeAreTalkingTo)
    {
        if (currentMood == MemoryTags.angry)
            pointsToReward /= 2;
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
