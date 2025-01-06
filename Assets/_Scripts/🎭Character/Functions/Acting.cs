using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Mind;
using System.Data.Common;
using static UnityEngine.GraphicsBuffer;

public class Acting : MonoBehaviour
{
    private Character _character;
    private Dictionary<ActionType, Action<object>> _actionHandlers;
    private WorldObject _currentObjectTarget = null;
    public int StepInAction { get; set; } = 0;
    private string _lastAction = "";
    private float _waitBeforeAction = 0;

    public void Initialize(Character character) => _character = character;
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
        { ActionType.useObject, param => UseObject((ObjectType)param) },
        { ActionType.useObjectInInventory, param => UseObjectInInventory((ObjectType)param) },
        { ActionType.findOccupant, param => FindOccupant((TraitType)param) },
        { ActionType.gotoOccupant, param => GotoOccupant((TraitType)param) },
        { ActionType.buyItem, param => BuyItem((ObjectType)param,_character.Memory.ReachedOccupant) },
        { ActionType.rentItem, param => RentItem((ObjectType)param,_character.Memory.ReachedOccupant) }
    };

    private void Update() => PerformCurrentBehavior();

    private void PerformCurrentBehavior()
    {

        if (CurrentBehavior == null)
        { return; }

        if (_actionHandlers.TryGetValue(CurrentBehavior.Action, out var action))
        {
            _character.Ui.CurrentBehaviour = CurrentBehavior.Name;
            _character.Ui.CurrentAction = $"{CurrentBehavior.Action} {CurrentBehavior.ActionParameter}";
            _character.Ui.CurrentStepInAction = $"";
            if (_lastAction != CurrentBehavior.Name)
            {
                StepInAction = 0;
                
                _lastAction = CurrentBehavior.Name;
                _character.Movement.Stop();
                _character.Ui.EndSpeech();
                if (CurrentBehavior.Dialogue!="")
                {
                    _waitBeforeAction = 4;
                    _character.Ui.Speak(CurrentBehavior.Dialogue);
                }
                else
                {
                    _waitBeforeAction = 2;
                }
            }

            if (StepInAction == 0 && _waitBeforeAction <= 0)
            {
                StepInAction++;
                _character.Ui.EndSpeech();

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
    private void FindCharacter(TargetType targetType)
    {
        PlayerStepInActionCorrection();
        switch (StepInAction)
        {
            case 1:
                if (ActionsHelper.WanderAndSearchForCharacter(_character as NPC, targetType, true, TraitType.human))
                {
                    IncrementStepInAction();

                }

                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_character);
                break;
        }
    }
    private void FindOccupant(TraitType traitType)
    {
        PlayerStepInActionCorrection();
        switch (StepInAction)
        {
            case 1:
                var locationObject = WorldManager.Instance.GetLocation(_character.Movement.CurrentLocation);
                var occupant = locationObject.GetOccupant(traitType);
                if (occupant != null)
                {
                    _character.Memory.OccupantTargets[traitType] = occupant;
                    IncrementStepInAction();
                }
                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_character);
                break;
        }
    }
    private void GotoOccupant(TraitType traitType)
    {
        PlayerStepInActionCorrection();
        switch (StepInAction)
        {
            case 1:
                var occupant = _character.Memory.OccupantTargets[traitType];
                if (occupant != null && ActionsHelper.Reached(_character, occupant.transform.position, 3f))
                {
                    _character.Memory.ReachedOccupant = occupant;
                    IncrementStepInAction();
                }
                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_character);
                break;
        }
    }
    private void FindObject(ObjectType targetType)
    {
        PlayerStepInActionCorrection();
        switch (StepInAction)
        {
            case 1:
                if (ActionsHelper.WanderAndSearchForObject(_character as NPC, targetType))
                {
                    StepInAction = 2;
                }

                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_character);
                break;
        }

    }


    private void FindKnowledge(KnowledgeType knowledgeType)
    {
        //STEP 1 GOTO INNKEEPER
        PlayerStepInActionCorrection();
        switch (StepInAction)
        {
            case 1:
                _character.Ui.CurrentStepInAction = "1 Goto nearest innkeeper";
                //remember who the local innkeeper is
                var target = _character.Memory.GetPeopleByTag(TraitType.innKeeper).FirstOrDefault().GetComponent<Character>();


                if (ActionsHelper.Reached(_character, target.transform.position, 2f))
                {
                    //STEP 2 ASK FOR DIRECTIONS

                    _character.Ui.CurrentStepInAction = "2 Ask innkeeper for location with tags";
                    List<Enum> tagsAsEnum = CurrentBehavior.ActionTags.Cast<Enum>().ToList();
                    SocialHelper.AskForKnowledge(_character, target, knowledgeType, tagsAsEnum);
                    IncrementStepInAction();
 
                }

                break;
            case 2:
        //WAIT FOR RESPONSE
                break;
        }
    }


    public void BuyItem(ObjectType objectType,Character seller)
    {
        PlayerStepInActionCorrection();
        switch (StepInAction)
        {
            case 1:

                var buyer = _character;

                _character.Movement.Stop();
                if (seller != null && ActionsHelper.Reached(_character, seller.transform.position, 2.7f))
                {
                    var price = seller.Memory.GetPrice(objectType);
                    if (seller.Memory.Possessions.ContainsKey(objectType) && ActionsHelper.FinancialTransaction(buyer, seller, price) )
                    {
                        var boughtItem = seller.Memory.RemoveFromPossessions(objectType);
                        buyer.Memory.AddToPossessions(objectType, boughtItem);
                        buyer.Memory.AddToInventory(objectType, boughtItem);

                    }

                    IncrementStepInAction();


                }
                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_character);
                break;
        }
    }

    public void RentItem(ObjectType objectType, Character seller)
    {
        PlayerStepInActionCorrection();
        switch (StepInAction)
        {
            case 1:

                var buyer = _character;

                _character.Movement.Stop();

                    int price = seller.Memory.GetPrice(objectType);
                    var workLocation = WorldManager.Instance.GetLocation(seller.Memory.GetLocationTarget(TargetLocationType.work));
                    var canGetPossession = workLocation.GetPossession(objectType);

                if (canGetPossession == null)
                {
                    seller.Ui.Speak($"There are no more rooms available");
                }
                else if (!ActionsHelper.FinancialTransaction(buyer, seller, price))
                {
                    seller.Ui.Speak($"That's not enough coin");
                }
                else
                {

                    var boughtItem = workLocation.RemoveFromPossessions(objectType);
                    buyer.Memory.AddToPossessions(objectType, boughtItem);
                    boughtItem.StartRenting(_character, workLocation, 24);
                    seller.Ui.Speak($"Here is the key to your room");

                }

                IncrementStepInAction();


                
                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_character);
                break;
        }
    }

    private void Kill(TargetType targetType)
    {
        PlayerStepInActionCorrection();
        switch (StepInAction)
        {
            case 1:

                if (_character.Memory.Targets[targetType].GetComponent<Character>() is { } target &&
                    ActionsHelper.Reached(_character, target.transform.position, 1f))
                {
                    target.Vitality.Die();
                    _character.Memory.Targets[targetType] = null;
                    IncrementStepInAction();
                }

                //TODO: killer must only kill victim if they think the victim is alone
                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_character);
                break;
        }
    }


    private void FullfillNeed(NeedType needType)
    {
        PlayerStepInActionCorrection();
        switch (StepInAction)
        {
            case 1:
                if (_character.Memory.GetPossession(ObjectType.bed) is { } objectToUse && ActionsHelper.Reached(_character, objectToUse.transform.position, 0.2f))
                {
                    _character.Vitality.Needs[needType] = 0;
                    IncrementStepInAction();
                }
                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_character);
                break;
        }

    }

    private void TraderJob(TraderType traderType)
    {
        PlayerStepInActionCorrection();
        switch (StepInAction)
        {
            case 1:
                var objectToUse = _character.Memory.GetPossession(ObjectType.traderChair);
                if (ActionsHelper.Reached(_character, objectToUse.transform.position, 0.2f))
                {
                    //TODO: extend trader behaviour here
                }

                break;
            case 2:
                ActionsHelper.EndThisBehaviour(_character);
                break;
        }
    }

    private void FarmerJob(FarmerType farmerType)
    {
        PlayerStepInActionCorrection();
        switch (StepInAction)
        {
            //STEP 1
            case 1:
                if (_currentObjectTarget == null)
                {
                    var crops = _character.Memory.GetPossessions(ObjectType.pumpkin);
                    foreach (var crop in crops)
                    {
                        if (crop.Care < 80)
                        {
                            _currentObjectTarget = crop;
                            break;
                        }
                    }

                    if (_currentObjectTarget == null)
                    { StepInAction = 2; }
                }
                else
                if (ActionsHelper.Reached(_character, _currentObjectTarget.transform.position, 1f))
                {
                    _currentObjectTarget.CareFor(_character);
                    _currentObjectTarget = null;
                }

                break;
            //STEP 2
            case 2:
                ActionsHelper.EndThisBehaviour(_character);
                break;
        }


    }



    private void UseObjectInInventory(ObjectType objectType)
    {
        PlayerStepInActionCorrection();
        switch (StepInAction)
        {
            //STEP 1
            case 1:
                var objectToUse = _character.Memory.GetPossession(objectType);
                objectToUse.Use(_character, out bool destroyObject);
                _character.Ui.CurrentStepInAction = "drinking";
                if (destroyObject)
                {
                    ActionsHelper.DestroyObject(_character, objectToUse);
                }

                ActionsHelper.EndThisBehaviour(_character);
                break;
            //STEP 2
            case 2:
                ActionsHelper.EndThisBehaviour(_character);
                break;
        }
    }

    private void UseObject(ObjectType objectType)
    {
        PlayerStepInActionCorrection();
        var objectToUse = _character.Memory.Possessions[objectType].First();
        switch (StepInAction)
        {
            //STEP 1
            case 1:
                if (ActionsHelper.Reached(_character, objectToUse.transform.position, 0.3f))
                {
                    IncrementStepInAction();


                }

                break;
            //STEP 2
            case 2:
                objectToUse.Use(_character, out bool destroyObject);
                if (destroyObject)
                {
                    ActionsHelper.DestroyObject(_character, objectToUse);
                }
                break;

        }

    }


    private void GotoLocation(TargetLocationType locationType)
    {
        PlayerStepInActionCorrection();
        switch (StepInAction)
        {
            //STEP 1
            case 1:
                _character.Ui.CurrentStepInAction = $"Made it to goto location {locationType}";

                var mem = _character.Memory;

                var locationTarget = mem.LocationTargets[locationType];
                var destination = WorldManager.Instance.Locations[locationTarget];

                if (ActionsHelper.Reached(_character, destination.transform.position, 1f))
                {
                    _character.Movement.CurrentLocation = locationTarget;
                    _character.Memory.OccupantTargets.Clear();
                    IncrementStepInAction();
                }

                break;
            //STEP 2
            case 2:
                ActionsHelper.EndThisBehaviour(_character);
                break;
        }
    }


    private void PlayerStepInActionCorrection()
    {
        if (_character is Player)
        {
            StepInAction = StepInAction==0 ? 1 : StepInAction;
        }
    }

    private void IncrementStepInAction()
    {
        if (_character is Player)
        {
            StepInAction = 0;
        }
        else
        {
            StepInAction ++;
        }
    }
}

