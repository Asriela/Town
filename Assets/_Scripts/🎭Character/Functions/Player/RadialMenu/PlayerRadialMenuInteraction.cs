using System;
using System.Collections.Generic;
using Mind;
using UnityEngine;
using UnityEngine.UIElements;
public enum SubMenu
{
    main,
    coerceActions,
    charmEmotions,
    charmActions,
    charmGive
}

public enum ActionEffects
{
    drunk,
    emotional,
    relaxed,
    tense
}
public class ActionOption
{
    public SocializeType Enum;
    public string Name;
    public string Tooltip;
    public string Emoji;
    public int Points;
    public int RelationshipImpact;
    public bool UsedUp;
    public bool IsDisabled;
    public ViewTowards RelationshipRequirement;
    public MemoryTags MoodRequirement;
    public List<MemoryTags> ActionEffects;
    public SubMenu InSubMenu;
    public Dictionary<MemoryTags, int> BonusPoints;


    public ActionOption(SocializeType enumType,  string name,  string emoji, string tooltip, int cost, int relationshipImpact, bool usedUp, bool isDisabled, ViewTowards relationshipRequirement, MemoryTags mood, SubMenu submenu, Dictionary<MemoryTags, int> bonusPoints = null, List<MemoryTags> actionEffects = null)
    {
        Enum = enumType;
        Name = name;
        Tooltip = tooltip;
        Points = cost;
        UsedUp = usedUp;
        IsDisabled = isDisabled;
        Emoji = emoji;
        RelationshipRequirement = relationshipRequirement;
        MoodRequirement = mood;
        ActionEffects= actionEffects ?? new List<MemoryTags>();
        BonusPoints = bonusPoints ?? new Dictionary<MemoryTags, int>();
        RelationshipImpact =relationshipImpact;
        InSubMenu = submenu;

    }

}
public class PlayerRadialMenuInteraction : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private RadialMenu _radialMenu;


    private bool _leftClick = false;
    private bool _rightClick = false;
    private bool _isMouseOverUI = false;
    private bool _justOpenedPieMenu = false;
    private Player _player;

    private List<ActionOption> coerceOptions = new();
    private List<ActionOption> charmOptions = new();
    private List<ActionOption> mainOptions = new();
    public void Initialize(Player player)
    {
        _player = player;
    }
    private Character _personWeAreInteractingWith;

    private void Start()
    {


        var root = _radialMenu._root;
        root.RegisterCallback<MouseEnterEvent>(e => OnMouseOverUI(true));
        root.RegisterCallback<MouseLeaveEvent>(e => OnMouseOverUI(false));

        SetupCharmOptions();
        SetupCoerceOptions();
        SetupPrimaryOptions();
    }

    private void Update()
    {
        _leftClick = false;
        _leftClick = Input.GetMouseButtonDown(0);
        _rightClick = false;
        _rightClick = Input.GetMouseButtonDown(1);
        TryOpenRadialMenu();

    }

    private void OnMouseOverUI(bool isOver)
    {
        _isMouseOverUI = isOver;

    }
    public bool NotInteractingWithMenu()
    {

        var ret = true;

        if (_justOpenedPieMenu)
        { _justOpenedPieMenu = false; return false; }

        if (_isMouseOverUI)
            return false;

        if (!_leftClick && _personWeAreInteractingWith != null)
            ret = false;
        if (GameManager.Instance.BlockingPlayerUIOnScreen2)
        {
            CloseInteractionMenu();
            ret = false;
        }

        return ret;






    }
    public void CloseInteractionMenu()
    {
        _justOpenedPieMenu = false;
        _personWeAreInteractingWith = null;
        _radialMenu.HideMenu();
        //EventManager.TriggerSwitchCameraToNormalMode();
    }


    private void TryOpenRadialMenu()
    {
        if (!_leftClick)//|| GameManager.Instance.BlockingPlayerUIOnScreen)
            return;


        // Raycast from mouse position
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Get the layer mask for a specific layer (e.g., "MyLayerName")
        LayerMask layerMask = LayerMask.GetMask("characters");

        // Perform the raycast with the layer mask
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, layerMask);



        if (hit.collider != null)
        {
            _justOpenedPieMenu = true;
            Character character = hit.collider.GetComponent<Character>();
            if (character != null && GameManager.Instance.BlockingPlayerUIOnScreen==false)
            {
                _personWeAreInteractingWith = character;
                OpenPrimaryMenu();
            }
        }
    }

    private void OpenRadialMenu(List<ActionOption> options)
    {
        EventManager.TriggerSwitchCameraToInteractionMode(_player.transform, _personWeAreInteractingWith.transform);
        _radialMenu.OpenMenu(options, _personWeAreInteractingWith);
    }

    private void OpenPrimaryMenu()
    {
        
        if (_personWeAreInteractingWith.CharacterName == CharacterName.Dohlson)
        {
            GameManager.Instance.OpenDialoguePlayer(_personWeAreInteractingWith, DialogueFileType.auto);
            CloseInteractionMenu();
        }
        else
        {
            _justOpenedPieMenu = true;


            _radialMenu.OnButtonClicked += HandlePrimarySelection;
            _radialMenu.OnButtonClicked -= HandleCoerceSelection;
            _radialMenu.OnButtonClicked -= HandleCharmSelection;
            OpenRadialMenu(mainOptions);
        }


    }


    private void HandlePrimarySelection(ActionOption actionOption)
    {
        _radialMenu.OnButtonClicked -= HandlePrimarySelection; // Unsubscribe


        switch (actionOption.Name)
        {
            case "Charm":
                OpenCharmMenu();
                break;
            case "Coerce":
                OpenCoerceMenu();
                break;
            case "Talk":
                GameManager.Instance.OpenDialoguePlayer(_personWeAreInteractingWith, DialogueFileType.auto);
                CloseInteractionMenu();
                break;
        }
    }


    private void OpenCharmMenu()
    {
        List<ActionOption> options = EvaluateOptions(charmOptions);

        _radialMenu.OnButtonClicked += HandleCharmSelection;
        OpenRadialMenu(options);
    }


    private void HandleCharmSelection(ActionOption actionOption)
    {
        _radialMenu.OnButtonClicked -= HandleCharmSelection; // Unsubscribe
        _player.RadialActionsHelper.PerformCharmAction(_personWeAreInteractingWith, actionOption);
    }


    private void OpenCoerceMenu()
    {



        List<ActionOption> options = EvaluateOptions(coerceOptions);
        _radialMenu.OnButtonClicked += HandleCoerceSelection;

        OpenRadialMenu(options);
    }


    private void HandleCoerceSelection(ActionOption actionOption)
    {
        _radialMenu.OnButtonClicked -= HandleCoerceSelection; // Unsubscribe
        _player.RadialActionsHelper.PerformCoerceAction(_personWeAreInteractingWith, actionOption);
    }


    private void SetupCharmOptions()
    {
        List<string> options = new() { "Buy Drink", "Hangout", "Hug", "Dance" };
        List<string> emoji = new() { "🍻", "💬", "🤗", "💃" };
        List<int> points = new() { 2, 1, 1, 2 };
        List<string> toolTips = new() {
            "Costs 5 coin, a quick way to get someone to like you.",
            "Takes more time out of your day but costs nothing.",
            "Not everyone is a hugger and people in distress are more receptive to hugs." ,
            "Dancing is only effective with people who already like you." };
        List<bool> isDisabled = new() { false, false, false, false };

    }

    private void SetupCoerceOptions()
    {
       
    }

    private void SetupPrimaryOptions()
    {

        /*
        mainOptions.Add(new ActionOption(
        "Charm",
        "🌼",
        "Charm them by spending time with them, it has a positive relationship impact but takes more time.",
        0,
        0,
        false,
        false,
        ViewTowards.neutral,
        MemoryTags.none,
        SubMenu.main,
        new Dictionary<MemoryTags, int> { },
        new List<MemoryTags> { }
        ));

        mainOptions.Add(new ActionOption(
        "Coerce",
        "🔪",
        "Coerce them, doesnt take up much time but has a negative relationship impact.",
        0,
        0,
        false,
        false,
        ViewTowards.neutral,
        MemoryTags.none,
        SubMenu.main,
        new Dictionary<MemoryTags, int> { },
        new List<MemoryTags> { }
        ));

        mainOptions.Add(new ActionOption(
        "Talk",
        "💬",
        "Open dialogue with the person.",
        0,
        0,
        false,
        false,
        ViewTowards.neutral,
        MemoryTags.none,
        SubMenu.main,
        new Dictionary<MemoryTags, int> { },
        new List<MemoryTags> { }
        ));
        */
    }


    private List<ActionOption> EvaluateOptions(List<ActionOption> options)
    {
        var relationship = _personWeAreInteractingWith.Relationships.GetRelationshipWith(_personWeAreInteractingWith, _player);
        BasicFunctions.Log($"❤Relationship: {relationship}", LogType.radialMenu);
        var mood = _personWeAreInteractingWith.State.VisualState[0];

        List< ActionOption > outputList = new(options);

        foreach (var option in options)
        {
            var moodPass = false;
            if (option.MoodRequirement ==MemoryTags.none || option.MoodRequirement == mood || (mood== MemoryTags.drunk && option.MoodRequirement== MemoryTags.relaxed))
                moodPass=true;
            if(moodPass==false)
            { outputList.Remove(option); }
            var relRequirement = (float)option.RelationshipRequirement;

            if (relRequirement < 0)
            {
                if (!(relationship<= relRequirement))
                { outputList.Remove(option); }

            }
            if (relRequirement > 0)
            {
                if (!(relationship >= relRequirement))
                { outputList.Remove(option); }

            }

            if(option.UsedUp )
            {
                outputList.Remove(option);
            }
        }

        return outputList;
    }
}
