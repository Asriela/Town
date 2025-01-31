using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Mind;
using System.Data.Common;
using static UnityEngine.GraphicsBuffer;

public class Actions : MonoBehaviour
{
    private NPC _npc;
    private Dictionary<ActionType, Action<object>> _actionHandlers;
    private WorldObject _currentObjectTarget = null;
    public int StepInAction { get; set; } = 0;
    private string _lastAction = "";
    private float _waitBeforeAction = 0;
    private ActionType _doingActionTypeOf;

    public void Initialize(NPC npc) => _npc = npc;
    public Behavior CurrentBehavior { get; set; }

    private void Awake() =>
    _actionHandlers = new Dictionary<ActionType, Action<object>>
    {
        { ActionType.findCharacter, param => FindCharacter((TargetType)param) },
        { ActionType.findObject, param => FindObject((ObjectType)param) },
        { ActionType.fullfillNeed, param => FullfillNeed((NeedType)param) },
        { ActionType.kill, param => Kill((TargetType)param) },
        { ActionType.trader, param => TraderJob((TraderType)param) },
        { ActionType.farmer, param => FarmerJob((FarmerType)param) },
        { ActionType.findKnowledge, param => FindKnowledge((KnowledgeType)param) },
        { ActionType.gotoLocation, param => GotoLocation((TargetLocationType)param) },
        { ActionType.gotoPerson , param => GotoPerson((CharacterName)param) },
        { ActionType.useOneOfMyPossesions, param => UseOneOfMyPossesions((ObjectType)param) },
        { ActionType.useObjectInInventory, param => UseObjectInInventory((ObjectType)param) },
        { ActionType.findOccupant, param => FindOccupant((TraitType)param) },
        { ActionType.gotoOccupant, param => GotoOccupant((TraitType)param) },
        { ActionType.buyItem, param => BuyAnItem((ObjectType)param,_npc.Memory.ReachedOccupant) },
        { ActionType.rentItem, param => RentAnItem((ObjectType)param,_npc.Memory.ReachedOccupant) },
        { ActionType.socialize, param => DoSocialActionWithSomeone((SocializeType)param,_npc.Memory.SocialTarget) },
        { ActionType.sharePersonKnowledgeAbout , param => sharePersonKnowledge((CharacterName)param) },
        { ActionType.gotoTarget , param => GotoTarget((TargetType)param) },
        { ActionType.completeScriptedTask , param => MarkScriptedTaskCompleted((ScriptedTaskType)param) },
        { ActionType.openDialogueWithPlayer , param => OpenDialogueWithPlayer((DialogueFileType)param) },
        { ActionType.shareTag , param => ShareMemoryTag((MemoryTags)param) }

    };

    private void Update() => PerformCurrentBehavior();

    private void PerformCurrentBehavior()
    {
        if(_npc.State.ActionState==StateType.dead)
        { return; }
        if (CurrentBehavior == null)
        { return; }

        if (_actionHandlers.TryGetValue(CurrentBehavior.Action, out var action))
        {
            _npc.Ui.CurrentBehaviour = CurrentBehavior.Name;
            _npc.Ui.CurrentAction = $"{CurrentBehavior.Action} {CurrentBehavior.ActionParameter}";
            _npc.Ui.CurrentStepInAction = $"";
            if (_lastAction != CurrentBehavior.Name)
            {
                StepInAction = 0;
                _doingActionTypeOf = CurrentBehavior.Action;
                    
                _lastAction = CurrentBehavior.Name;
                _npc.Movement.Stop();

                if (CurrentBehavior.Dialogue != "")
                {
                    _waitBeforeAction = 4;
                    if (CurrentBehavior.Action == ActionType.socialize || CurrentBehavior.Action == ActionType.sharePersonKnowledgeAbout)
                    {
                        _npc.Memory.SocialDialogue = CurrentBehavior.Dialogue;
                    }
                    else
                    {
                        _npc.Ui.Speak(_npc,CurrentBehavior.Dialogue); 

                    }
                }
                else
                {
                    _waitBeforeAction = 2;
                }
            }

            if (StepInAction == 0 && _waitBeforeAction <= 0)
            {
                StepInAction++;
                //_npc.Ui.EndSpeech();

            }
            
            action(CurrentBehavior.ActionParameter);

            _waitBeforeAction -= 0.01f;
        }
        else
        {
            Debug.LogWarning($"No handler defined for ActionType: {CurrentBehavior.Action}");
        }
    }
    //TODO: add dynamic tags  when searching for a character

    private void OpenDialogueWithPlayer(DialogueFileType fileType)
    {
        if(GameManager.Instance.BlockingPlayerUIOnScreen==false)
        if(_npc.Memory.GetScriptedTaskProgress(ScriptedTaskType.talkToPlayer)!= ScriptedTaskProgressType.completed)
        {
            _npc.Memory.ScriptedTaskProgress[ScriptedTaskType.talkToPlayer] = ScriptedTaskProgressType.completed;

            if (fileType == DialogueFileType.confront)
            {
                GameManager.Instance.CantClickOffInteractionMenu=true;
            }
            GameManager.Instance.OpenDialoguePlayer(_npc, fileType);
        }

    }
    private void ShareMemoryTag(MemoryTags memTag)
    {
           var whoToShareWith=WorldManager.Instance.GetCharacter(_npc.Memory.ReachedPerson);

        if (whoToShareWith!=null)
        {
            whoToShareWith.PersonKnowledge.AddKnowledge(whoToShareWith, _npc, new List<Mind.MemoryTags> { memTag });
            whoToShareWith.Memory.LastSpokeTo=_npc.CharacterName;
            _npc.Memory.LastSpokeTo = whoToShareWith.CharacterName;
        }

    }
    
    private void FindCharacter(TargetType targetType)
    {
       
        switch (StepInAction)
        {
            case 1:
                if (ActionsHelper.WanderAndSearchForCharacter(_npc as NPC, targetType, true, TraitType.human))
                {
                    IncrementStepInAction();

                }

                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_npc, _doingActionTypeOf);
                break;
        }
    }
    private void MarkScriptedTaskCompleted(ScriptedTaskType taskType)=> _npc.Memory.ScriptedTaskProgress[taskType] = ScriptedTaskProgressType.completed;
    private void FindOccupant(TraitType traitType)
    {

        switch (StepInAction)
        {
            case 1:
                var locationObject = WorldManager.Instance.GetLocation(_npc.Movement.CurrentLocation);
                var occupant = locationObject.GetOccupant(traitType);
                if (occupant != null)
                {
                    _npc.Memory.OccupantTargets[traitType] = occupant;
                    _npc.Memory.Targets[TargetType.occupant] = occupant;
                    IncrementStepInAction();
                }
                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_npc, _doingActionTypeOf);
                break;
        }
    }

    private void sharePersonKnowledge(CharacterName aboutWho)
    {
        switch (StepInAction)
        {
            case 1:
                var bestie = _npc.Relationships.GetHighestRelationship(_npc);
                var aboutWhoCharacter = WorldManager.Instance.AllCharacters[aboutWho];

                if (bestie != null && ActionsHelper.Reached(_npc, bestie.transform.position, 3f))
                {
                    var knowledge = _npc.PersonKnowledge.GetAllKnowledge(_npc, aboutWhoCharacter);
                    SocialHelper.ShareKnowledgeAbout(_npc, bestie, bestie, aboutWhoCharacter, KnowledgeType.person, knowledge.Cast<Enum>().ToList(), null);
                    _npc.Ui.Speak(_npc,$"The stranger is a mage");
                    bestie.Memory.LastSpokeTo= _npc.CharacterName;
                    _npc.Memory.LastSpokeTo = bestie.CharacterName;
                    IncrementStepInAction();
                }
                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_npc, _doingActionTypeOf);
                break;
        }
    }

    private void GotoOccupant(TraitType traitType)
    {

        switch (StepInAction)
        {
            case 1:
                var occupant = _npc.Memory.OccupantTargets[traitType];
                if (occupant != null && ActionsHelper.Reached(_npc, occupant.transform.position, 3f))
                {
                    _npc.Memory.ReachedOccupant = occupant;
                    IncrementStepInAction();
                }
                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_npc, _doingActionTypeOf);
                break;
        }
    }

    private void GotoTarget(TargetType targetType)
    {
        switch (StepInAction)
        {
            case 1:
                var target = _npc.Memory.Targets[targetType];
                if (target != null && ActionsHelper.Reached(_npc, target.transform.position, 3f))
                {
                    _npc.Memory.ReachedTarget = target;
                    IncrementStepInAction();
                }
                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_npc, _doingActionTypeOf);
                break;
        }
    }
    private void FindObject(ObjectType targetType)
    {

        switch (StepInAction)
        {
            case 1:
                if (ActionsHelper.WanderAndSearchForObject(_npc as NPC, targetType))
                {
                    StepInAction = 2;
                }

                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_npc, _doingActionTypeOf);
                break;
        }

    }


    private void FindKnowledge(KnowledgeType knowledgeType)
    {
        //STEP 1 GOTO INNKEEPER

        switch (StepInAction)
        {
            case 1:
                _npc.Ui.CurrentStepInAction = "1 Goto nearest innkeeper";
                //remember who the local innkeeper is
                var target = _npc.PersonKnowledge.GetPeopleByTag(_npc,MemoryTags.innKeeper).FirstOrDefault();


                if (ActionsHelper.Reached(_npc, target.transform.position, 2f))
                {
                    //STEP 2 ASK FOR DIRECTIONS

                    _npc.Ui.CurrentStepInAction = "2 Ask innkeeper for location with tags";
                    List<Enum> tagsAsEnum = CurrentBehavior.ActionTags.Cast<Enum>().ToList();

                    //TODO: ask for specific person if its person knowledge
                    SocialHelper.AskForKnowledgeAbout(_npc, target, _npc,_npc, knowledgeType, tagsAsEnum,false);
                    IncrementStepInAction();

                }

                break;
            case 2:
                //WAIT FOR RESPONSE
                break;
        }
    }


    private void DoSocialActionWithSomeone(SocializeType socializeActionType, Character personToSocializeWith)
    {
        switch (StepInAction)
        {
            case 1:
                _npc.Ui.CurrentStepInAction = "1 Goto nearest innkeeper";
                //remember who the local innkeeper is



                if (ActionsHelper.Reached(_npc, personToSocializeWith.transform.position, 2f))
                {

                    SocialHelper.SocialAction(_npc, personToSocializeWith, socializeActionType);
                    IncrementStepInAction();

                }

                break;
            case 2:
                //WAIT FOR RESPONSE
                break;
        }
    }

    public void BuyAnItem(ObjectType objectType, Character seller)
    {

        switch (StepInAction)
        {
            case 1:

                var buyer = _npc;

                _npc.Movement.Stop();
                if (seller != null && ActionsHelper.Reached(_npc, seller.transform.position, 2.7f))
                {

                    BaseAction.BuyItem(objectType, buyer, seller);

                    IncrementStepInAction();


                }
                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_npc, _doingActionTypeOf);
                break;
        }
    }

    public void RentAnItem(ObjectType objectType, Character seller)
    {

        switch (StepInAction)
        {
            case 1:

                var buyer = _npc;

                _npc.Movement.Stop();

                BaseAction.RentItem(objectType, buyer, seller);


                IncrementStepInAction();



                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_npc, _doingActionTypeOf);
                break;
        }
    }

    private void Kill(TargetType targetType)
    {

        switch (StepInAction)
        {
            case 1:

                if (_npc.Memory.Targets[targetType] is { } target &&
                    ActionsHelper.Reached(_npc, target.transform.position, 3f))
                {
                    BaseAction.HurtSomeone(_npc, target, 100);
                    _npc.Memory.Targets[targetType] = null;
                    IncrementStepInAction();
                    _npc.Memory.LastWeAttacked= target.CharacterName;
                }

                //TODO: killer must only kill victim if they think the victim is alone
                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_npc, _doingActionTypeOf);
                break;
        }
    }


    private void FullfillNeed(NeedType needType)
    {

        switch (StepInAction)
        {
            case 1:

                var objectToUse = _npc.Memory.GetPossession(ObjectType.bed);
                if (objectToUse != null)
                {
                    _npc.Ui.CurrentStepInAction = $"going to bed";
                    if (ActionsHelper.Reached(_npc, objectToUse.transform.position, 1f))
                    {
                        _npc.Ui.CurrentStepInAction = $"sleeping";
                        BaseAction.InteractWithObject(objectToUse, _npc, ObjectInteractionType.use);

                        IncrementStepInAction();
                    }
                }

                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_npc, _doingActionTypeOf);
                break;
        }

    }

    private void TraderJob(TraderType traderType)
    {
        WorldObject objectToUse=null;
        switch (StepInAction)
        {
            case 1:
                switch (traderType)
                {
                    case TraderType.innkeeper:
                         objectToUse = _npc.Memory.GetPossession(ObjectType.traderChair);
                        if (ActionsHelper.Reached(_npc, objectToUse.transform.position, 3f))
                        {
                            _npc.Appearance.FaceLeft();
                            //TODO: extend trader behaviour here
                        }

                        break;
                    case TraderType.innGuard:
                         objectToUse = _npc.Memory.GetPossession(ObjectType.traderChair);
                        if (ActionsHelper.Reached(_npc, objectToUse.transform.position, 3f))
                        {
                            _npc.Appearance.FaceLeft();
                            //TODO: extend trader behaviour here
                        }

                        break;
                }

                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_npc, _doingActionTypeOf);
                break;
        }
    }

    private void FarmerJob(FarmerType farmerType)
    {

        switch (StepInAction)
        {
            //STEP 1
            case 1:
                if (_currentObjectTarget == null)
                {
                    var crops = _npc.Memory.GetPossessions(ObjectType.pumpkin);
                    foreach (var crop in crops)
                    {
                        if (crop.Maintenance < 80)
                        {
                            _currentObjectTarget = crop;
                            break;
                        }
                    }

                    if (_currentObjectTarget == null)
                    { StepInAction = 2; }
                }
                else
                if (ActionsHelper.Reached(_npc, _currentObjectTarget.transform.position, 1f))
                {
                    BaseAction.InteractWithObject(_currentObjectTarget, _npc, ObjectInteractionType.careFor);
                    _currentObjectTarget = null;
                }

                break;
            //STEP 2
            case 2:
                ActionsHelper.EndThisBehaviour(_npc, _doingActionTypeOf);
                break;
        }


    }



    private void UseObjectInInventory(ObjectType objectType)
    {

        switch (StepInAction)
        {
            //STEP 1
            case 1:
                var objectToUse = _npc.Memory.GetPossession(objectType);
                BaseAction.InteractWithObject(objectToUse, _npc, ObjectInteractionType.use);
                _npc.Ui.CurrentStepInAction = "drinking";


                ActionsHelper.EndThisBehaviour(_npc, _doingActionTypeOf);
                break;
            //STEP 2
            case 2:
                ActionsHelper.EndThisBehaviour(_npc, _doingActionTypeOf);
                break;
        }
    }

    private void UseOneOfMyPossesions(ObjectType objectType)
    {

        var objectToUse = _npc.Memory.Possessions[objectType].First();
        switch (StepInAction)
        {
            //STEP 1
            case 1:
                if (ActionsHelper.Reached(_npc, objectToUse.transform.position, 0.3f))
                {
                    IncrementStepInAction();


                }

                break;
            //STEP 2
            case 2:
                BaseAction.InteractWithObject(objectToUse, _npc, ObjectInteractionType.use);
                break;

        }

    }


    private void GotoLocation(TargetLocationType locationType)
    {

        switch (StepInAction)
        {
            //STEP 1
            case 1:
                _npc.Ui.CurrentStepInAction = $"Made it to goto location {locationType}";

                var mem = _npc.Memory;

                var locationTarget = mem.LocationTargets[locationType];
                var destination = WorldManager.Instance.Locations[locationTarget];

                if (ActionsHelper.Reached(_npc, destination.transform.position, 1f))
                {
                    _npc.Movement.CurrentLocation = locationTarget;
                    _npc.Memory.OccupantTargets.Clear();
                    IncrementStepInAction();
                }

                break;
            //STEP 2
            case 2: 
                ActionsHelper.EndThisBehaviour(_npc, _doingActionTypeOf);
                break;
        }
    }

    private void GotoPerson(CharacterName character)
    {

        switch (StepInAction)
        {
            //STEP 1
            case 1:

                var gotoPerson = WorldManager.Instance.AllCharacters[character];


                if (ActionsHelper.Reached(_npc, gotoPerson.transform.position, 1f))
                {

                    _npc.Memory.ReachedPerson= gotoPerson.CharacterName;
                    IncrementStepInAction();
                }

                break;
            //STEP 2
            case 2:
                ActionsHelper.EndThisBehaviour(_npc, _doingActionTypeOf);
                break;
        }
    }

    

    private void IncrementStepInAction()
    {

        StepInAction++;

    }
}

