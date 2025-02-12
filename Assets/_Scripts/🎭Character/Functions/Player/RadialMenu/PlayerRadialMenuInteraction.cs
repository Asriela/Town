using System.Collections.Generic;
using Mind;
using UnityEditor.Build.Content;
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
    public string Name;
    public string Tooltip;
    public string Emoji;
    public int Cost;
    public bool UsedUp;
    public bool IsDisabled;
    public ViewTowards RelationshipRequirement;
    public MemoryTags MoodRequirement;
    public List<ActionEffects> ActionEffects;
    public SubMenu InSubMenu;
    public Dictionary<MemoryTags, int> BonusPoints;


    public ActionOption(string name, string emoji, string tooltip, int cost, bool usedUp, bool isDisabled, ViewTowards relationshipRequirement, MemoryTags mood, SubMenu submenu, Dictionary<MemoryTags, int> bonusPoints = null, List<ActionEffects> actionEffects = null)
    {
        Name = name;
        Tooltip = tooltip;
        Cost = cost;
        UsedUp = usedUp;
        IsDisabled = isDisabled;
        Emoji = emoji;
        RelationshipRequirement = relationshipRequirement;
        MoodRequirement = mood;
        ActionEffects= actionEffects ?? new List<ActionEffects>();
        BonusPoints = bonusPoints ?? new Dictionary<MemoryTags, int>();

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
            if (character != null)
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
        _justOpenedPieMenu = true;

        List<string> options = new() { "Charm", "Coerce", "Talk" };
        List<string> emoji = new() { "🌼", "🔪", "💬" };
        List<int> points = new() { 0, 0, 0 };
        List<string> toolTips = new() { "Charm them by spending time with them, it has a positive relationship impact but takes more time.", "Coerce them, doesnt take up much time but has a negative relationship impact.", "Open dialogue with the person." };
        List<bool> isDisabled = new() { false, false, false };
        _radialMenu.OnButtonClicked += HandlePrimarySelection;
        _radialMenu.OnButtonClicked -= HandleCoerceSelection;
        _radialMenu.OnButtonClicked -= HandleCharmSelection;
        OpenRadialMenu(options);
    }


    private void HandlePrimarySelection(string selectedOption, int points)
    {
        _radialMenu.OnButtonClicked -= HandlePrimarySelection; // Unsubscribe


        switch (selectedOption)
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
        List<ActionOption> options = charmOptions;

        _radialMenu.OnButtonClicked += HandleCharmSelection;
        OpenRadialMenu(options);
    }


    private void HandleCharmSelection(string selectedOption, int points)
    {
        _radialMenu.OnButtonClicked -= HandleCharmSelection; // Unsubscribe
        _player.RadialActionsHelper.PerformCharmAction(_personWeAreInteractingWith, selectedOption, points);
    }


    private void OpenCoerceMenu()
    {



        List<ActionOption> options = coerceOptions;
        _radialMenu.OnButtonClicked += HandleCoerceSelection;

        OpenRadialMenu(options);
    }


    private void HandleCoerceSelection(string selectedOption, int points)
    {
        _radialMenu.OnButtonClicked -= HandleCoerceSelection; // Unsubscribe
        _player.RadialActionsHelper.PerformCoerceAction(_personWeAreInteractingWith, selectedOption, points);
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

        charmOptions.Add(new ActionOption(
        "Small talk",
        "💬",
        "Takes up a small amount of time but only gives 1 trust.",
        1,
        false,
        false,
        ViewTowards.neutral,
        MemoryTags.none,
        SubMenu.charmActions
        ));

        charmOptions.Add(new ActionOption(
        "Hug",
        "🤗",
        "Not everyone is a hugger and people in distress are more receptive to hugs.",
        2,
        false,
        false,
        ViewTowards.positive,
        MemoryTags.none,
        SubMenu.charmActions,
        new Dictionary<MemoryTags, int> { { MemoryTags.emotional, 2 } }
        ));

        charmOptions.Add(new ActionOption(
        "Give Food",
        "🍗",
        "Give them the meal you brought with you. You loose a meal but you gain a friend.",
        4,
        false,
        false,
        ViewTowards.negative,
        MemoryTags.none,
        SubMenu.charmGive,
        new Dictionary<MemoryTags, int> { },
        new List<ActionEffects> { { ActionEffects.relaxed } }
        ));

        charmOptions.Add(new ActionOption(
        "Play Chess",
        "♟",
        "Play a game of chess, requires the person to be relaxed.",
        4,
        false,
        false,
        ViewTowards.negative,
        MemoryTags.relaxed,
        SubMenu.charmActions
        ));

        charmOptions.Add(new ActionOption(
        "Comfort",
        "❤",
        "Comfort the person when they are emotional",
        4,
        false,
        false,
        ViewTowards.veryPositive,
        MemoryTags.emotional,
        SubMenu.charmEmotions
        ));

        charmOptions.Add(new ActionOption(
        "Sympathize",
        "👐",
        "Show sympathy when they are emotional",
        2,
        false,
        false,
        ViewTowards.positive,
        MemoryTags.emotional,
        SubMenu.charmEmotions
        ));

        charmOptions.Add(new ActionOption(
        "Give alcohol",
        "👐",
        "Give the person alcohol, will lead to impairment",
        4,
        false,
        false,
        ViewTowards.negative,
        MemoryTags.none,
        SubMenu.charmGive,
        new Dictionary<MemoryTags, int> { { MemoryTags.emotional, 3 } },
        new List<ActionEffects> { { ActionEffects.relaxed} }
        ));
    }

    private void SetupCoerceOptions()
    {
        coerceOptions.Add(new ActionOption(
        "Threaten",
        "🔪",
        "Threaten with physical violence, short term gain, long term loss as they will hate you.",
        -4,
        false,
        false,
        ViewTowards.neutral,
        MemoryTags.none,
        SubMenu.coerceActions,
        new Dictionary<MemoryTags, int> {},
        new List<ActionEffects> { { ActionEffects.tense } }
        ));

        coerceOptions.Add(new ActionOption(
        "Blackmail",
        "🔪",
        "You need dirt on someone to blackmail them.",
        -4,
        false,
        true,
        ViewTowards.neutral,
        MemoryTags.none,
        SubMenu.coerceActions,
        new Dictionary<MemoryTags, int> { },
        new List<ActionEffects> { { ActionEffects.tense } }
        ));
    }

    private void SetupPrimaryOptions()
    {
        coerceOptions.Add(new ActionOption(
        "Threaten",
        "🔪",
        "Threaten with physical violence, short term gain, long term loss as they will hate you.",
        -4,
        false,
        false,
        ViewTowards.neutral,
        MemoryTags.none,
        SubMenu.coerceActions,
        new Dictionary<MemoryTags, int> { },
        new List<ActionEffects> { { ActionEffects.tense } }
        ));

        coerceOptions.Add(new ActionOption(
        "Blackmail",
        "🔪",
        "You need dirt on someone to blackmail them.",
        -4,
        false,
        true,
        ViewTowards.neutral,
        MemoryTags.none,
        SubMenu.coerceActions,
        new Dictionary<MemoryTags, int> { },
        new List<ActionEffects> { { ActionEffects.tense } }
        ));
    }
}
