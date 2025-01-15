using System;
using System.Collections.Generic;
using System.Linq;
using Mind;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using static WorldObject;

public struct MenuOption
{

    public string ButtonLabel;
    public object Data;
    public object Data2;

    public MenuOption(string buttonLabel, object data, object data2)
    {

        ButtonLabel = buttonLabel;
        Data = data;
        Data2 = data2;
    }
}

public class PlayerMenuInteraction : MonoBehaviour
{
    public enum SocialMenuState
    {
        start,
        memories,
        buy,
        sell,
        objectInteraction,
        socialAction,
        askPerson,
        askObject,
        askLocation,
        tellPerson,
        tellLocation,
        tellAboutYourself,
        talkAboutPerson,
        talkAboutPersonWhoSpokeAboutPerson,
        talkAboutPersonWhoSpokeAboutPersonsKnowledge,
        askAboutPerson,
        askAboutPersonWhoSpokeAboutPerson


    }

    private InteractionMenu _interactionMenu;


    private Character _personWeAreSpeakingTo;
    private WorldObject _selectedWorldObject;
    private bool _leftClick = false;
    private bool _justOpenedPieMenu = false;
    private Vector2 _screenPosition;
    private string _titleText = "";
    private string _passOnTitleText = "";
    private Character _knower = null;
    private Character _aboutWho = null;

    public SocialMenuState MenuState { get; set; }
    private List<MenuOption> _currentMenuOptions = new List<MenuOption> { new MenuOption("NULL", null, null) };


    public static List<SocializeType> BasicActions = new List<SocializeType> { SocializeType.greet, SocializeType.hug, SocializeType.smallTalk, SocializeType.insult };
    private Player _player;
    public void Initialize(Player player)
    {
        _player = player;
    }


    private void Start()
    {



        _interactionMenu = FindFirstObjectByType<InteractionMenu>();
        if (_interactionMenu == null)
        {
            BasicFunctions.Log("🎈PieMenu component not found in the scene.", LogType.error);
        }

        _interactionMenu.OnButtonClicked += HandleButtonClicked;


        var root = _interactionMenu.root;

        // Register mouse events on the root visual element
        root.RegisterCallback<MouseEnterEvent>(e => OnMouseOverUI(true));
        root.RegisterCallback<MouseLeaveEvent>(e => OnMouseOverUI(false));
    }

    private void Update()
    {
        InteractionMenuInteraction();

        _leftClick = false;
        _leftClick = Input.GetMouseButtonDown(0);
        //ClickedOnUI=false;


    }
    private bool isMouseOverUI = false;

    private void OnMouseOverUI(bool isOver)
    {
        isMouseOverUI = isOver;

    }
    private MenuOption Label(string label)
    {
        return new MenuOption(label, null, null);
    }

    private MenuOption Option(string label, object data, object data2)
    {
        return new MenuOption(label, data, data2);
    }



    private void HandleButtonClicked(int positionInList, string buttonLabel)
    {



        _titleText = buttonLabel;

        switch (buttonLabel)
        {

            case "Trade":

                _currentMenuOptions = new List<MenuOption>
                {
                    Label("Buy"),
                    Label("Sell"),
                    Label("Give"),
                    Label("Request")
                };
                break;
            case "Buy":
                MenuState = SocialMenuState.buy;
                var pricedItems = _personWeAreSpeakingTo.Memory.GetPricedItems();
                _currentMenuOptions = pricedItems.Select(item =>
                {
                    string labelText = item.ToString() == "bed"
                        ? "Room to Rent"
                        : item.ToString();

                    // Get the price using the method and append it to the label
                    decimal price = _personWeAreSpeakingTo.Memory.GetPrice(item);
                    string pureLabelText = labelText;
                    return Option($"{labelText}: {price:C}", pureLabelText, item);
                }).ToList();
                break;
            case "Social Action":
                MenuState = SocialMenuState.socialAction;
                _currentMenuOptions = BasicActions.Select(action => Option(action.ToString(), action, null)).ToList();
                break;

            ///=======
            ////ASK
            /// ===
            case "Ask about":

                _currentMenuOptions = new List<MenuOption>
                {
                    Label("Ask about a person"),
                    Label("Ask about a location")
                };
                break;


            case "Ask about a person":
                MenuState = SocialMenuState.askAboutPerson;
                _titleText = "So what do...";

                // MenuState = SocialMenuState.tellPerson;
                var charactersToAskAbout = _player.PersonKnowledge.GetAllCharactersPersonHasDataOn(_player);

                // Build a list of menu options from the player's MemoryTags
                _currentMenuOptions = charactersToAskAbout.Where(tag => tag != _player) // Exclude the player
                    .Select(tag =>
                    {
                        string labelText = tag.ToString();
                        if (tag == _personWeAreSpeakingTo)
                            labelText = "you";

                        // Return the menu option with the tag stored as Data
                        return Option(labelText, tag, null);
                    }).ToList();
                break;

            case "Ask about a location":
                MenuState = SocialMenuState.askLocation;


                break;
            ///=======
            ////TELL
            /// ===
            case "Talk about":
                _currentMenuOptions = new List<MenuOption>
                {
                Label("Share something about yourself"),
                Label("Talk about a person")
                //Label("Talk about a location");
                };
                break;


            case "Share something about yourself":
                MenuState = SocialMenuState.tellAboutYourself;

                // Retrieve all MemoryTags related to the player
                var playerTags = _player.PersonKnowledge.GetAllCharacterTags(_player, _player);

                // Build a list of menu options from the player's MemoryTags
                _currentMenuOptions = playerTags.Select(tag =>
                {
                    // Convert the MemoryTag to a displayable label
                    string labelText = tag.ToString(); // You can customize this based on how you want the tags displayed

                    // Return the menu option with the tag stored as Data
                    return Option(labelText, tag, null);
                }).ToList();
                break;

            case "Talk about a person":
                _titleText = "So...";
                MenuState = SocialMenuState.talkAboutPerson;
                // MenuState = SocialMenuState.tellPerson;
                var charactersToTalkAbout = _player.PersonKnowledge.GetAllCharactersWeHaveDataOn();

                // Build a list of menu options from the player's MemoryTags
                _currentMenuOptions = charactersToTalkAbout.Where(tag => tag != _player) // Exclude the player
                    .Select(tag =>
                    {
                        // Convert the MemoryTag to a displayable label
                        string labelText = tag.ToString(); // You can customize this based on how you want the tags displayed

                        // Return the menu option with the tag stored as Data
                        return Option(labelText, tag, null);
                    }).ToList();

                break;

            case "Talk about a location":
                //MenuState = SocialMenuState.tellLocation;


                break;

            default:
                _currentMenuOptions = ProcessComplexMenuOptions(positionInList);
                break;
        }




        if (_currentMenuOptions == null)
        { _interactionMenu.HideMenu(); }
        else
        { _interactionMenu.ShowMenu(_currentMenuOptions, _titleText); }
    }

    private List<MenuOption> ProcessComplexMenuOptions(int positionInList)
    {
        var chosenOption = _currentMenuOptions[positionInList];
        List<MenuOption> newOptions = null;

        string aboutWhoString = "";
        string word = "";
        MemoryTags knowledge;
        List<MemoryTags> knowledgeList;
        List<Enum> memoryTagsList;
        string doWord;
        string characterWord;

        switch (MenuState)
        {
            case SocialMenuState.memories:
                Speak(chosenOption.Data);
                break;
            case SocialMenuState.buy:


                if (chosenOption.Data == "Room to Rent")

                    BaseAction.RentItem((ObjectType)chosenOption.Data2, _player, _personWeAreSpeakingTo);

                //
                break;
            case SocialMenuState.objectInteraction:

                _player.GotoAndInteractWithObject(_selectedWorldObject, (ObjectInteractionType)chosenOption.Data);
                break;

            case SocialMenuState.tellAboutYourself:
                if (chosenOption.Data is MemoryTags memoryTag)
                {
                    memoryTagsList = new() { memoryTag };

                    SocialHelper.ShareKnowledgeAbout(_player, _personWeAreSpeakingTo, _personWeAreSpeakingTo, _player, KnowledgeType.person, memoryTagsList, null);
                    _player.Ui.Speak(DialogueHelper.GetTellDialogue(memoryTag));

                }
                else
                {
                    // Handle the case where the cast is invalid
                    Debug.LogError("Error: chosenOption.Data is not of type MemoryTags.");
                }


                //
                break;
            case SocialMenuState.askAboutPerson:

                MenuState = SocialMenuState.askAboutPersonWhoSpokeAboutPerson;
                _knower = (Character)chosenOption.Data;
                 doWord = "does";
                 characterWord = _knower.CharacterName.ToString();
                if (_knower == _personWeAreSpeakingTo)
                {
                    doWord = "do";
                    characterWord = "you";
                }
                    
                _titleText = $"So what {doWord} <b>{characterWord} think about..</b>";

                var charactersToAskAbout = _player.PersonKnowledge.GetAllCharactersPersonHasDataOn(_player);

                // Build a list of menu options from the player's MemoryTags
                newOptions = charactersToAskAbout.Where(tag => tag!=_personWeAreSpeakingTo).Select(tag =>
                {
                    string labelText = tag.ToString();
                    if (tag == _personWeAreSpeakingTo)
                        labelText = "you";
                    // Return the menu option with the tag stored as Data
                    return Option(labelText, _knower, tag);
                }).ToList();


                //
                break;
            case SocialMenuState.askAboutPersonWhoSpokeAboutPerson:

       
                _knower = (Character)chosenOption.Data;
                _aboutWho = (Character)chosenOption.Data2;
                characterWord = _knower.CharacterName.ToString();
                aboutWhoString = _aboutWho.CharacterName.ToString();
                word = "is";
                doWord = "does";
                if (_knower == _personWeAreSpeakingTo)
                {

                    word = "are";
                    doWord = "do";
                    characterWord = "you";
                }


                _passOnTitleText = $"So what {doWord} {characterWord} think about <b>{aboutWhoString}?</b>";


                SocialHelper.AskForKnowledgeAbout(_player, _personWeAreSpeakingTo, _knower, _aboutWho, KnowledgeType.person, null, true);
                _player.Ui.Speak(_passOnTitleText);


                //
                break;
            
            case SocialMenuState.talkAboutPerson:

                MenuState = SocialMenuState.talkAboutPersonWhoSpokeAboutPerson;
                _knower = (Character)chosenOption.Data;
                _titleText = $"So  <b>{_knower.CharacterName} was talking about...</b>";

                var charactersToTalkAbout = _player.PersonKnowledge.GetAllCharactersPersonHasDataOn(_knower);

                // Build a list of menu options from the player's MemoryTags
                newOptions = charactersToTalkAbout.Select(tag =>
                    {
                        string labelText = tag.ToString();
                        if (tag == _personWeAreSpeakingTo)
                            labelText = "you";
                        // Return the menu option with the tag stored as Data
                        return Option(labelText, _knower, tag);
                    }).ToList();


                //
                break;
            case SocialMenuState.talkAboutPersonWhoSpokeAboutPerson:

                MenuState = SocialMenuState.talkAboutPersonWhoSpokeAboutPersonsKnowledge;
                _knower = (Character)chosenOption.Data;
                _aboutWho = (Character)chosenOption.Data2;
                aboutWhoString = _aboutWho.CharacterName.ToString();
                word = "is";
                if (_aboutWho == _personWeAreSpeakingTo)
                {
                    aboutWhoString = "you";
                    word = "are";

                }


                _titleText = $"So {_knower.CharacterName} was talking about <b>{aboutWhoString} and said {aboutWhoString} {word}...</b>";
                _passOnTitleText = $"So {_knower.CharacterName} was talking about {aboutWhoString} and said {aboutWhoString} {word} ";

                knowledgeList = _player.PersonKnowledge.GetAllCharacterTags(_knower, _aboutWho);

                // Build a list of menu options from the player's MemoryTags
                newOptions = knowledgeList.Select(tag =>
                {

                    string labelText = tag.ToString(); // You can customize this based on how you want the tags displayed

                    // Return the menu option with the tag stored as Data
                    return Option(labelText, tag, null);
                }).ToList();


                //
                break;
            case SocialMenuState.talkAboutPersonWhoSpokeAboutPersonsKnowledge:

                knowledge = (MemoryTags)chosenOption.Data;




                memoryTagsList = new() { knowledge };

                SocialHelper.ShareKnowledgeAbout(_player, _personWeAreSpeakingTo, _knower, _aboutWho, KnowledgeType.person, memoryTagsList, null);
                _player.Ui.Speak(_passOnTitleText + knowledge.ToString());


                //
                break;
            case SocialMenuState.socialAction:
                float effectFromInteraction = _personWeAreSpeakingTo.Relationships.AddInteractionEffect((SocializeType)chosenOption.Data, _player);
                //TODO: add that if its a negative response that we dont do the action such as smalltalk which takes time to complete
                //TODO: add that actions like smalltalk take time
                //TODO: add animations here for interactions
                break;
        }


        return newOptions;
    }
    private void Speak(object messageData)
    {

    }


    private void InteractionMenuInteraction()
    {


        if (!_leftClick || GameManager.Instance.BlockingPlayerUIOnScreen)
            return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {

            _personWeAreSpeakingTo = hit.collider.GetComponent<Character>();
            if (_personWeAreSpeakingTo != null && _personWeAreSpeakingTo != _player)
            {

                _player.Movement.Stop();
                BasicFunctions.Log("Selected player", LogType.ui);
                // CameraController.Instance.EnterDialogueMode(_personWeAreSpeakingTo.transform);

                _personWeAreSpeakingTo.Reactions.PersonWeAreSpeakingTo = _player;
                _justOpenedPieMenu = true;
                _screenPosition = Camera.main.WorldToScreenPoint(hit.point);


                Vector2 clickPosition = Input.mousePosition;


                List<MenuOption> buttonLabels = new List<MenuOption>
                {
                    Label("Talk about"),
                    Label("Ask about"),
                    Label("Trade"),
                    Label("Social Action")



                };

                MenuState = SocialMenuState.start;
                _interactionMenu.ShowMenu(buttonLabels, "Social Interactions");
            }
            else
            {
                // If no character is clicked, check for a WorldObject
                _selectedWorldObject = hit.collider.GetComponent<WorldObject>();
                if (_selectedWorldObject != null)
                {
                    BasicFunctions.Log("Selected world object", LogType.ui);

                    // Set up interactions for the world object
                    _justOpenedPieMenu = true;
                    _screenPosition = Camera.main.WorldToScreenPoint(hit.point);


                    List<InteractionOption> worldObjectInteractionOptions = _selectedWorldObject.GetInteractionOptions(_player);
                    if (!worldObjectInteractionOptions.Any())
                    {
                        return;
                    }

                    // Convert InteractionOptions to MenuOption (assuming InteractionOption has a 'Label' and 'Action' method)
                    _currentMenuOptions = worldObjectInteractionOptions.Select(option =>
                    new MenuOption
                    {
                        ButtonLabel = option.Label,
                        Data = option.InteractionAction
                    }).ToList();


                    MenuState = SocialMenuState.objectInteraction;  // Assuming you have a separate menu state for WorldObjects
                    _interactionMenu.ShowMenu(_currentMenuOptions, "Social Interactions");
                }
            }

        }





    }

    public bool NotInteractingWithMenu()
    {


        if (!_leftClick && _personWeAreSpeakingTo != null)
            return false;
        if (_justOpenedPieMenu)
        { _justOpenedPieMenu = false; return false; }
        if (isMouseOverUI)
            return false;


        if (GameManager.Instance.BlockingPlayerUIOnScreen)
        {
            _interactionMenu.HideMenu();
            return false;
        }

        return true;






    }
}
