﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mind;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using static InteractFeature;
using static WorldObject;

public struct MenuOption
{

    public string ButtonLabel;
    public object Data1;
    public object Data2;
    public object Data3;
    public MenuOptionType menuOptionType;
    public MemoryTags OptionNeeds;
    public Key OptionKey;
    public int menuOptionCost;
    public Key IsKey;
    public string UniqueId;
    public int OptionNumber;
    public bool OldOption;
    public MemoryTags OptionMoodReq;

    public MenuOption(string buttonLabel, object data, object data2)
    {
        
        ButtonLabel = buttonLabel;
        Data1 = data;
        Data2 = data2;
        Data3 = null;
        menuOptionType = MenuOptionType.general;
        this.menuOptionCost = 0;
        OptionNeeds = MemoryTags.none;
        IsKey=Key.none;
        OptionKey = Key.none;
        UniqueId="";
        OptionNumber=0;
        OldOption=false;
        OptionMoodReq = MemoryTags.none;
    }
}

public enum MenuOptionType
{
    general,
    dia
}

public enum SocialMenuPath
{
    shareInfoAboutThisPerson,
    askAboutThisPerson,
    shareGossip,
    shareVisualStatusOfThisPerson,
    askGossip
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
        askAboutPersonWhoSpokeAboutPerson,
        talkAboutSomeone,
        talkAboutSomeonePath,
        tellAboutPerson,
        talkPaths
    }

    private InteractionMenu _interactionMenu;


    private Character _personWeAreSpeakingTo;
    public Character PersonWeAreSpeakingTo => _personWeAreSpeakingTo;
    private WorldObject _selectedWorldObject;
    private bool _leftClick = false;
    private bool _justOpenedPieMenu = false;
    private Vector2 _screenPosition;
    private string _titleText = "";
    private string _passOnTitleText = "";
    private Character _subject = null;
    private Character _aboutWho = null;
    private DiaPackage currentDiaPackage;
    private List<MenuOption> currentDiaOptions;
    private bool closingMenu = false;

    string savedLastChosenOption;
    string savedCurrentDialogue;
    List<MenuOption> savedDiaButtons;
    string savedContextTitle;
    List<MenuOption> savedMenuButtons;

    public SocializeType SocialAction { get; set; } = SocializeType.none;

    private float socializeTimeLeft = -1;

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

        _interactionMenu.OnButtonClicked += MenuButtonPressed;


        var root = _interactionMenu.root;

        // Register mouse events on the root visual element
        root.RegisterCallback<MouseEnterEvent>(e => OnMouseOverUI(true));
        root.RegisterCallback<MouseLeaveEvent>(e => OnMouseOverUI(false));
    }

    private void Update()
    {
        HandleMenuInteraction();

        _leftClick = false;
        _leftClick = Input.GetMouseButtonDown(0);
        //ClickedOnUI=false;
        if (SocialAction != SocializeType.none)
        {
            DoSocialAction();
        }

    }

    private void DoSocialAction()
    {


        switch (SocialAction)
        {
            case SocializeType.drinking:
                if (socializeTimeLeft == -1)
                {
                    socializeTimeLeft = 10000;
                    _player.Appearance.SetSpriteAction("drinking");
                    _personWeAreSpeakingTo.Appearance.SetSpriteAction("drinking");

                    WorldManager.Instance.SetRampUpSpeedOfTime(SpeedOfTime.fast);
                }

                break;
        }





        socializeTimeLeft -= Time.timeScale;

        if (socializeTimeLeft < 0)
        {

            socializeTimeLeft = -1;
            ExecuteSocialAction(SocialAction);
            SocialAction = SocializeType.none;
            _player.Appearance.ResetSprite();
            WorldManager.Instance.SetSpeedOfTime(SpeedOfTime.normal);
            _personWeAreSpeakingTo.Appearance.ResetSprite();


            OpenInteractionMenu(savedLastChosenOption, savedCurrentDialogue, _personWeAreSpeakingTo.CharacterName.ToString(), savedDiaButtons, savedContextTitle, savedMenuButtons, _player.transform, _personWeAreSpeakingTo.transform, _personWeAreSpeakingTo);


        }

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



    private void MenuButtonPressed(int positionInList, string buttonLabel, MenuOptionType menuOptionType)
    {
        if (SocialAction != SocializeType.none)
            return;
        if (menuOptionType == MenuOptionType.dia)
        {
            HandleDiaMenuOptions(positionInList, buttonLabel);
        }
        else
        { HandleGeneralMenuOptions(positionInList, buttonLabel); }
    }
    private void HandleDiaMenuOptions(int positionInList, string buttonLabel)
    {

        currentDiaPackage = DiaReader.ChooseOption(positionInList, out DiaActionType? diaActionToExecute, out object diaActionData);
        SocialAction = DiaMenuHelper.ExecuteAction(_player, _personWeAreSpeakingTo, diaActionToExecute, diaActionData);

        if (currentDiaPackage != null)
        {

            var diaDialogue = currentDiaPackage.Dialogue;

            currentDiaOptions = DiaMenuHelper.ConvertDiaOptionToMenuOptions(currentDiaPackage.Options);

            UpdateInteractionMenu(buttonLabel, diaDialogue, currentDiaOptions, "", null);
        }


    }

    public void ChangeDiaSection(string diaSection)
    {

        currentDiaPackage = DiaReader.GotoSection( diaSection);
        //SocialAction = DiaMenuHelper.ExecuteAction(_player, _personWeAreSpeakingTo, diaActionToExecute, diaActionData);

        if (currentDiaPackage != null)
        {

            var diaDialogue = currentDiaPackage.Dialogue;

            currentDiaOptions = DiaMenuHelper.ConvertDiaOptionToMenuOptions(currentDiaPackage.Options);

            UpdateInteractionMenu("", diaDialogue, currentDiaOptions, "", null);
        }


    }
    private void HandleGeneralMenuOptions(int positionInList, string buttonLabel)
    {
        _titleText = buttonLabel;

        switch (buttonLabel)
        {
            case "open menu":
                OpenGeneralMenu();
                break;
            case "Trade":
                SetupTradeMenu();
                break;

            case "Buy":
                SetupBuyMenu();
                break;

            case "Action":
                SetupActionMenu();
                break;

            case "Talk about..":
                SetupTalkAboutMenu();
                break;

            default:
                _currentMenuOptions = ProcessComplexMenuOptions(positionInList);
                break;
        }
        if (closingMenu)
        {
            closingMenu = false;
        }
        else
        {
            UpdateInteractionMenu("", "", currentDiaOptions, _titleText, _currentMenuOptions);
        }

    }



    public void UpdateInteractionMenu(string lastChosenOption, string currentDialogue, List<MenuOption> diaButtons, string contextTitle, List<MenuOption> menuButtons)
    {
        //if (_currentMenuOptions == null || _currentMenuOptions.Count == 0)
        //  {
        // _interactionMenu.HideMenu();
        //  }
        // else
        //  {

        if (SocialAction != SocializeType.none)
        {
            if (lastChosenOption == null)
                Debug.LogWarning("lastChosenOption is null");
            if (currentDialogue == null)
                Debug.LogWarning("currentDialogue is null");
            if (diaButtons == null)
                Debug.LogWarning("diaButtons is null");
            if (contextTitle == null)
                Debug.LogWarning("contextTitle is null");
            if (menuButtons == null)
                Debug.LogWarning("menuButtons is null");
            savedLastChosenOption = lastChosenOption;
            savedCurrentDialogue = currentDialogue;
            savedDiaButtons = diaButtons;
            savedContextTitle = contextTitle;
            savedMenuButtons = menuButtons;


        }





        if (menuButtons == null)
        {
            menuButtons= new();
        }
        OpenInteractionMenu(lastChosenOption, currentDialogue, _personWeAreSpeakingTo.CharacterName.ToString(), diaButtons, contextTitle, menuButtons, _player.transform, _personWeAreSpeakingTo.transform, _personWeAreSpeakingTo);
        // }
    }
    public void UpdateInteractionMenu(string characterSpeaking, string currentDialogue)
    {

       // string newOptionLabel = "do something else..";

      //  bool optionExists = currentDiaOptions.Any(option => option.ButtonLabel == newOptionLabel);

     //   if (!optionExists)
       // {
        //    currentDiaOptions.Add(Option(newOptionLabel, currentDiaOptions.Count - 1, DiaActionType.menu));
        //}

        OpenInteractionMenu("", currentDialogue, characterSpeaking, savedDiaButtons, "", null, _player.transform, _personWeAreSpeakingTo.transform, _personWeAreSpeakingTo);

    }
    private void HandleMenuInteraction()
    {
        if (!_leftClick || GameManager.Instance.BlockingPlayerUIOnScreen)
            return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Get the layer mask for a specific layer (e.g., "MyLayerName")
        LayerMask layerMask = LayerMask.GetMask("characters");

        // Perform the raycast with the layer mask
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, layerMask);



        if (hit.collider != null)
        {
           // if (HandleCharacterInteraction(hit))
            //    return;

            HandleObjectInteraction(hit);
        }
    }

    private bool HandleCharacterInteraction(RaycastHit2D hit)
    {
        return OpenDialoguePlayer(_personWeAreSpeakingTo = hit.collider.GetComponent<Character>(), DialogueFileType.auto);

    }


    public bool OpenDialoguePlayer(Character target, DialogueFileType fileType)
    {
        _personWeAreSpeakingTo = target;
        if (_personWeAreSpeakingTo != null && _personWeAreSpeakingTo != _player)
        {
            _player.Movement.Stop();
            BasicFunctions.Log("Selected player", LogType.ui);

            _personWeAreSpeakingTo.Reactions.PersonWeAreSpeakingTo = _player;
            _justOpenedPieMenu = true;
            // _screenPosition = Camera.main.WorldToScreenPoint(hit.point);

            _currentMenuOptions.Clear();


            currentDiaPackage = DiaMenuHelper.OpenInteractionWithCharacter(_personWeAreSpeakingTo, fileType);
            var diaDialogue = currentDiaPackage.Dialogue;

            var diaOptions = DiaMenuHelper.ConvertDiaOptionToMenuOptions(currentDiaPackage.Options);


            OpenInteractionMenu("", diaDialogue, _personWeAreSpeakingTo.CharacterName.ToString(), diaOptions, "", null, _player.transform, _personWeAreSpeakingTo.transform, _personWeAreSpeakingTo);
            return true;
        }
        return false;
    }
    private void OpenGeneralMenu()
    {
        List<MenuOption> buttonLabels = new List<MenuOption>
        {
            Label("Talk about.."),
            Label("Action"),
            Label("Trade")
        };
        _titleText = "extra options";
        MenuState = SocialMenuState.start;
        _currentMenuOptions = buttonLabels;
    }
    private void HandleObjectInteraction(RaycastHit2D hit)
    {
        _selectedWorldObject = hit.collider.GetComponent<WorldObject>();
        if (_selectedWorldObject != null)
        {
            BasicFunctions.Log("Selected world object", LogType.ui);

            _justOpenedPieMenu = true;
            _screenPosition = Camera.main.WorldToScreenPoint(hit.point);

            _selectedWorldObject.ObjectActions.TryGetInteractionOptions(_player, out List<InteractionOption> worldObjectInteractionOptions);
            if (worldObjectInteractionOptions == null || !worldObjectInteractionOptions.Any())
                return;

            _currentMenuOptions = worldObjectInteractionOptions.Select(option =>
                new MenuOption
                {
                    ButtonLabel = option.Label,
                    Data1 = option.InteractionAction
                }).ToList();

            MenuState = SocialMenuState.objectInteraction;
            if (_currentMenuOptions != null)
            {
                OpenInteractionMenu("", "", "", null, "Object Interactions", _currentMenuOptions, _player.transform, _selectedWorldObject.transform, null);
            }
        }
    }

    private void OpenInteractionMenu(string lastChosenOption, string currentDialogue, string currentSpeaker, List<MenuOption> diaButtons, string contextTitle, List<MenuOption> menuButtons, Transform openerOfMenu, Transform target, Character whoWeAreSpeakingTo)
    {
        EventManager.TriggerSwitchCameraToInteractionMode(openerOfMenu, target);
        savedLastChosenOption = lastChosenOption;
        savedCurrentDialogue = currentDialogue;
        savedDiaButtons = diaButtons;
        savedContextTitle = contextTitle;
        savedMenuButtons = menuButtons;
        _interactionMenu.ShowMenu(lastChosenOption, currentDialogue, currentSpeaker, diaButtons, contextTitle, menuButtons, whoWeAreSpeakingTo);
    }

    public bool NotInteractingWithMenu()
    {


        if (!_leftClick && _personWeAreSpeakingTo != null)
            return false;
        if (_justOpenedPieMenu)
        { _justOpenedPieMenu = false; return false; }
        if (isMouseOverUI)
            return false;


        if (GameManager.Instance.BlockingPlayerUIOnScreen && GameManager.Instance.CantClickOffInteractionMenu == false && SocialAction == SocializeType.none)
        {
            CloseInteractionMenu();
            return false;
        }

        return true;






    }

    public void CloseInteractionMenu()
    {
        GameManager.Instance.CantClickOffInteractionMenu = false;
        _personWeAreSpeakingTo = null;
        _interactionMenu.HideMenu();
    }

    private void SetupTradeMenu()
    {
        _currentMenuOptions = new List<MenuOption>
    {
        Label("Buy"),
        Label("Sell"),
        Label("Give"),
        Label("Request")
    };
    }

    private void SetupBuyMenu()
    {
        MenuState = SocialMenuState.buy;
        var pricedItems = _personWeAreSpeakingTo.Memory.GetPricedItems();

        _currentMenuOptions = pricedItems.Select(item =>
        {
            string labelText = item.ToString() == "bed" ? "Room to Rent" : item.ToString();
            decimal price = _personWeAreSpeakingTo.Memory.GetPrice(item);
            return Option($"{labelText}: {price:C}", labelText, item);
        }).ToList();
    }

    private void SetupActionMenu()
    {
        MenuState = SocialMenuState.socialAction;
        _currentMenuOptions = BasicActions.Select(action => Option(action.ToString(), action, null)).ToList();
    }

    private void SetupTalkAboutMenu()
    {
        _titleText = "Talk about who?";
        MenuState = SocialMenuState.talkPaths;

        var everyoneWeKnow = _player.VisualStatusKnowledge.GetAllCharactersViewerHasVisualStatusOn(_player);
        var options = new List<MenuOption>();
        var others = new List<MenuOption>();

        foreach (var tag in everyoneWeKnow)
        {
            string labelText = tag.CharacterName.ToString();
            if (tag == _player)
            {
                labelText = "myself";
                options.Insert(0, Option(labelText, tag, null)); // Insert "myself" at the beginning
            }
            else if (tag == _personWeAreSpeakingTo)
            {
                labelText = "you";
                options.Add(Option(labelText, tag, null)); // Add "you" at the end
            }
            else
            {
                others.Add(Option(labelText, tag, null)); // Add other options to a separate list
            }
        }

        // Combine the lists: "myself", followed by others, then "you"
        options.AddRange(others);
        _currentMenuOptions = options;
    }


    private List<MenuOption> ProcessComplexMenuOptions(int positionInList)
    {
        var chosenOption = _currentMenuOptions[positionInList];
        List<MenuOption> newOptions = new();

        string aboutWhoString = "";
        string word = "";
        MemoryTags knowledge;
        List<MemoryTags> knowledgeList;
        List<Enum> memoryTagsList;
        string doWord;
        string characterWord;

        SocialMenuPath pathChosen;
        bool subjectIsWhoWeAreSpeakingTo = false;
        bool subjectIsMe = false;
        string subjectName = "";

        switch (MenuState)
        {

            case SocialMenuState.buy:


                if (chosenOption.Data1 == "Room to Rent")

                    BaseAction.RentItem((ObjectType)chosenOption.Data2, _player, _personWeAreSpeakingTo);

                
                break;
            case SocialMenuState.objectInteraction:

                _player.GotoAndInteractWithObject(_selectedWorldObject, (ObjectInteractionType)chosenOption.Data1);
                closingMenu = true;
                CloseInteractionMenu();
                break;

            case SocialMenuState.tellAboutYourself:
                if (chosenOption.Data1 is MemoryTags memoryTag)
                {
                    memoryTagsList = new() { memoryTag };

                    SocialHelper.ShareKnowledgeAbout(_player, _personWeAreSpeakingTo, _personWeAreSpeakingTo, _player, KnowledgeType.person, memoryTagsList, null);
                    _player.Ui.Speak(_player, DialogueHelper.GetTellDialogue(memoryTag, _player, _player, _player, true));

                }
                else
                {
                    // Handle the case where the cast is invalid
                    Debug.LogError("Error: chosenOption.Data is not of type MemoryTags.");
                }


                //
                break;

            case SocialMenuState.talkPaths:
                MenuState = SocialMenuState.talkAboutSomeone;
                _subject = (Character)chosenOption.Data1;
                subjectName = _subject.CharacterName.ToString();

                if (_subject == _player)
                {
                    _titleText = $"So about me, I am..";
                    MenuState = SocialMenuState.tellAboutYourself;

                    // Retrieve all MemoryTags related to the player
                    var playerTags = _player.PersonKnowledge.GetAllCharacterTags(_player, _player);

                    // Build a list of menu options from the player's MemoryTags
                    newOptions = playerTags.Select(tag =>
                    {
                        // Convert the MemoryTag to a displayable label
                        string labelText = tag.ToString(); // You can customize this based on how you want the tags displayed

                        // Return the menu option with the tag stored as Data
                        return Option(labelText, tag, null);
                    }).ToList();


                }
                else
                {
                    _titleText = $"{subjectName}..";
                    if (_subject == _personWeAreSpeakingTo)
                    {
                        newOptions.Add(Option($"Tell me about yourself..", _subject, "ask"));
                        newOptions.Add(Option($"So I know that you..", _subject, "share"));
                        newOptions.Add(Option($"So you said..", _subject, "gossip"));
                    }
                    else
                    {
                        newOptions.Add(Option($"What do you know about {subjectName}", _subject, "ask"));
                        newOptions.Add(Option($"Share info on {subjectName}", _subject, "share"));
                        newOptions.Add(Option($"Gossip about {subjectName}", _subject, "gossip"));
                    }


                }

                //
                break;

            case SocialMenuState.askAboutPersonWhoSpokeAboutPerson:


                _subject = (Character)chosenOption.Data1;
                _aboutWho = (Character)chosenOption.Data2;
                characterWord = _subject.CharacterName.ToString();
                aboutWhoString = _aboutWho.CharacterName.ToString();
                word = "is";
                doWord = "does";
                if (_subject == _personWeAreSpeakingTo)
                {

                    word = "are";
                    doWord = "do";
                    characterWord = "you";
                }

                if (_aboutWho == _player)
                { aboutWhoString = "me"; }
                _passOnTitleText = $"So what {doWord} {characterWord} think about <b>{aboutWhoString}?</b>";


                SocialHelper.AskForKnowledgeAbout(_player, _personWeAreSpeakingTo, _subject, _aboutWho, KnowledgeType.person, null, true);
                _player.Ui.Speak(_player, _passOnTitleText);


                //
                break;
            case SocialMenuState.talkAboutSomeone:

                MenuState = SocialMenuState.talkAboutSomeonePath;

                _subject = (Character)chosenOption.Data1;
                var path = (string)chosenOption.Data2;
                subjectName = _subject.CharacterName.ToString();

                subjectIsWhoWeAreSpeakingTo = _subject == _personWeAreSpeakingTo;
                subjectIsMe = _subject == _player;

                var askText = $"what can you tell me about {subjectName}..";
                var askGossipText = $"what does {subjectName} think about..";
                if (subjectIsWhoWeAreSpeakingTo)
                {
                    askText = "tell me more about you..";
                    askGossipText = $"what do you think about..";
                }
                if (subjectIsMe)
                {
                    askText = "what do you know about me?";
                }


                //TELL
                var tellText = "";
                var tellGossipText = "";


                if (subjectIsWhoWeAreSpeakingTo)
                {
                    tellText = "theres something I know about you..";
                }
                else
                if (subjectIsMe)
                {
                    tellText = "let you tell you about me...";
                }
                else
                {
                    tellText = $"So {subjectName} is...";
                    tellGossipText = $"{subjectName} said..";
                }

                switch (path)
                {
                    case "ask":


                        characterWord = _subject.CharacterName.ToString();



                        if (_subject == _personWeAreSpeakingTo)
                        {
                            _passOnTitleText = $"Tell me a bit about you?</b>";
                        }
                        else
                        { _passOnTitleText = $"So what can you tell me about {characterWord}?</b>"; }



                        SocialHelper.AskForKnowledgeAbout(_player, _personWeAreSpeakingTo, _personWeAreSpeakingTo, _subject, KnowledgeType.person, null, true);
                        _player.Ui.Speak(_player, _passOnTitleText);
                        break;
                    case "share":
                        if (subjectIsMe)
                        {
                            _titleText = $"Me...</b>";
                        }
                        else
                        {
                            _titleText = $"So  <b>{subjectName}...</b>";
                            List<MemoryTags> statuses = _player.VisualStatusKnowledge.GetVisualStatus(_player, _subject);

                            // Prepare the status string
                            string status = string.Empty;

                            // If there are at least two statuses, combine the last two with "and"
                            if (statuses.Count >= 2)
                            {
                                // Join all statuses except the last two
                                status = string.Join(" ,", statuses.Take(statuses.Count - 2).Select(s => s.ToString()));

                                // Combine the last two statuses with " and" if there are more than one
                                if (statuses.Count > 1)
                                {
                                    // Add "and" only if there were other statuses previously
                                    if (status.Length > 0)
                                    {
                                        status += " ,";
                                    }

                                    status += " " + statuses[statuses.Count - 2].ToString() + " and " + statuses[statuses.Count - 1].ToString();
                                }
                            }
                            else
                            {
                                // If there are less than two statuses, handle normally
                                status = string.Join(" ,", statuses.Select(s => s.ToString()));
                            }

                            // Handle the "you seem" case for the player
                            newOptions.Add(Option((subjectIsWhoWeAreSpeakingTo ? "you look " : $"{subjectName} looks ") + status + "..", _subject, SocialMenuPath.shareVisualStatusOfThisPerson));

                        }
                        if (_player.PersonKnowledge.DoWeHavePersonKnowledgeOn(_player, _subject))
                        {
                            newOptions.Add(Option(tellText, _subject, SocialMenuPath.shareInfoAboutThisPerson));
                        }

                        break;
                    case "gossip":

                        newOptions.Add(Option(askGossipText, _subject, SocialMenuPath.askGossip));
                        if (_player.PersonKnowledge.DoWeHavePersonKnowledge(_subject))
                        {
                            newOptions.Add(Option(tellGossipText, _subject, SocialMenuPath.shareGossip));
                        }
                        break;
                }



                break;

            case SocialMenuState.talkAboutSomeonePath:
                pathChosen = (SocialMenuPath)chosenOption.Data2;

                switch (pathChosen)
                {
                    case SocialMenuPath.shareVisualStatusOfThisPerson:
                        MenuState = SocialMenuState.tellAboutPerson;
                        List<MemoryTags> statuses = _player.VisualStatusKnowledge.GetVisualStatus(_player, _subject);

                        // Build a list of menu options from the player's MemoryTags
                        newOptions = statuses.Select(tag =>
                        {
                            // Convert the MemoryTag to a displayable label
                            string labelText = tag.ToString(); // You can customize this based on how you want the tags displayed

                            // Return the menu option with the tag stored as Data
                            return Option(labelText, tag, null);
                        }).ToList();

                        _passOnTitleText = subjectIsWhoWeAreSpeakingTo ? "you look " : $"{_subject.CharacterName} looks ";
                        break;

                    case SocialMenuPath.askGossip:
                        MenuState = SocialMenuState.askAboutPersonWhoSpokeAboutPerson;
                        _subject = (Character)chosenOption.Data1;
                        doWord = "does";
                        characterWord = _subject.CharacterName.ToString();
                        if (_subject == _personWeAreSpeakingTo)
                        {
                            doWord = "do";
                            characterWord = "you";
                        }

                        _titleText = $"So what {doWord} <b>{characterWord} think about..</b>";

                        var charactersToAskAbout = _player.VisualStatusKnowledge.GetAllCharactersViewerHasVisualStatusOn(_player);

                        // Build a list of menu options from the player's MemoryTags
                        newOptions = charactersToAskAbout
                            .Where(tag => tag != _personWeAreSpeakingTo && tag.CharacterName.ToString() != characterWord) // Filter out unwanted conditions
                            .Select(tag =>
                            {
                                string labelText = tag.CharacterName.ToString();

                                if (tag == _personWeAreSpeakingTo)
                                {
                                    labelText = "you";
                                }
                                if (tag == _player)
                                {
                                    labelText = "me";
                                }

                                // Return the menu option with the tag stored as Data
                                return Option(labelText, _subject, tag);
                            })
                            .ToList();
                        break;
                    case SocialMenuPath.askAboutThisPerson:

                        break;

                    case SocialMenuPath.shareInfoAboutThisPerson:
                        _subject = (Character)chosenOption.Data1;

                        if (_subject == _player)
                        {
                            MenuState = SocialMenuState.tellAboutYourself;

                            // Retrieve all MemoryTags related to the player
                            var playerTags = _player.PersonKnowledge.GetAllCharacterTags(_player, _player);

                            // Build a list of menu options from the player's MemoryTags
                            newOptions = playerTags.Select(tag =>
                            {
                                // Convert the MemoryTag to a displayable label
                                string labelText = tag.ToString(); // You can customize this based on how you want the tags displayed

                                // Return the menu option with the tag stored as Data
                                return Option(labelText, tag, null);
                            }).ToList();

                            var test2 = _currentMenuOptions;
                        }
                        else
                        {
                            MenuState = SocialMenuState.tellAboutPerson;
                            var knowledgeWeHaveToShare = _player.PersonKnowledge.GetAllCharacterTags(_player, _subject);

                            // Build a list of menu options from the player's MemoryTags
                            newOptions = knowledgeWeHaveToShare.Select(tag =>
                            {
                                // Convert the MemoryTag to a displayable label
                                string labelText = tag.ToString(); // You can customize this based on how you want the tags displayed

                                // Return the menu option with the tag stored as Data
                                return Option(labelText, tag, null);
                            }).ToList();
                        }


                        break;
                    case SocialMenuPath.shareGossip:
                        MenuState = SocialMenuState.talkAboutPersonWhoSpokeAboutPerson;



                        _subject = (Character)chosenOption.Data1;
                        _titleText = $"So  <b>{_subject.CharacterName} was talking about...</b>";

                        var charactersToTalkAbout = _player.PersonKnowledge.GetAllCharactersPersonHasDataOn(_subject);



                        // Build a list of menu options from the player's MemoryTags
                        newOptions.AddRange(charactersToTalkAbout.Select(tag =>
                        {
                            string labelText = tag.CharacterName.ToString();

                            if (tag == _personWeAreSpeakingTo)
                            {
                                labelText = "you";
                            }
                            if (tag == _player)
                            {
                                labelText = "me";
                            }
                            // Return the menu option with the tag stored as Data
                            return Option(labelText, _subject, tag);
                        }).ToList());

                        var test = newOptions;
                        break;
                }


                break;

            case SocialMenuState.talkAboutPersonWhoSpokeAboutPerson:

                MenuState = SocialMenuState.talkAboutPersonWhoSpokeAboutPersonsKnowledge;
                _subject = (Character)chosenOption.Data1;
                _aboutWho = (Character)chosenOption.Data2;
                aboutWhoString = _aboutWho.CharacterName.ToString();

                var aboutWhoString2 = _aboutWho.CharacterName.ToString();
                word = "is";
                if (_aboutWho == _personWeAreSpeakingTo)
                {
                    aboutWhoString = "you";
                    aboutWhoString2 = "you";
                    word = "are";

                }
                if (_aboutWho == _player)
                {
                    aboutWhoString = "me";
                    aboutWhoString2 = "I";
                    word = "am";

                }

                _titleText = $"So {_subject.CharacterName} was talking about <b>{aboutWhoString} and said {aboutWhoString2} {word}...</b>";
                _passOnTitleText = $"So {_subject.CharacterName} was talking about {aboutWhoString} and said {aboutWhoString2} {word} ";

                knowledgeList = _player.PersonKnowledge.GetAllCharacterTags(_subject, _aboutWho);

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

                knowledge = (MemoryTags)chosenOption.Data1;




                memoryTagsList = new() { knowledge };

                SocialHelper.ShareKnowledgeAbout(_player, _personWeAreSpeakingTo, _subject, _aboutWho, KnowledgeType.person, memoryTagsList, null);
                _player.Ui.Speak(_player, _passOnTitleText + knowledge.ToString());


                //
                break;

            case SocialMenuState.tellAboutPerson:

                knowledge = (MemoryTags)chosenOption.Data1;




                memoryTagsList = new() { knowledge };

                SocialHelper.ShareKnowledgeAbout(_player, _personWeAreSpeakingTo, _player, _subject, KnowledgeType.person, memoryTagsList, null);
                _player.Ui.Speak(_player, _passOnTitleText + knowledge.ToString());


                //
                break;
            case SocialMenuState.socialAction:
                ExecuteSocialAction((SocializeType)chosenOption.Data1);
                //TODO: add that if its a negative response that we dont do the action such as smalltalk which takes time to complete
                //TODO: add that actions like smalltalk take time
                //TODO: add animations here for interactions
                break;
        }


        return newOptions;
    }
    private void ExecuteSocialAction(SocializeType socialAction)
    {
        float effectFromInteraction = _personWeAreSpeakingTo.Relationships.AddInteractionEffect(socialAction, _player);

    }

}

