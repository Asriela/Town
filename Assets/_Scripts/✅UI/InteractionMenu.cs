using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using Mind;
using UnityEngine.Windows;
using System.Linq.Expressions;


public class InteractionMenu : MonoBehaviour
{
    public VisualTreeAsset interactionMenuTemplate;
    public float buttonSpacing = 2f; // Spacing between buttons
    public VisualElement root;
    public VisualElement menuContainer;
    public VisualElement menuContainer2;
    private VisualElement backgroundImage;
    private VisualElement sideBarBack;
    public VisualElement actionsBackPanel;
    private VisualElement portraitImage;
    public VisualElement ActionInfoPanel;
    public VisualElement ActionImage;
    public VisualElement EndGameBack;
    public Label TooltipText;
    private Button dialogue;
    public Button statText;
    public Button charmMenuButton;
    public Button coerceMenuButton;
    public Button giveMenuButton;
    public ActionOption DoingAction=null;
    string reportNewKey="";

    private List<Button> dialogueOptionButtons = new();
    private float scrolldown = 0;
    private float scrolldown2 = 0;
    private int standardFontSize = 13;
    private List<string> chosenOptions = new List<string>();
    private VisualElement selectRectangle;
    private string redOn = MyColor.GreyHex;
    private string greenOn = MyColor.GreyHex;
    public Label statLabel;
    public SocialActionMenuType ActionsMenuTypeWeAreIn = SocialActionMenuType.charm;
    private SocialActionMenuType lastActionsMenu = SocialActionMenuType.none;
    public List<Button> ActionButtonList = new();
    public List<Label> ActionLabelList = new();
    [SerializeField]
    private float buttonLeftMargin = 230 - 200;
    [SerializeField]
    private float buttonWidth = 280;
    bool fastScroll=false;
    public delegate void ButtonClicked(int positionInList, string buttonLabel, MenuOptionType menuOptionType);
    public event ButtonClicked OnButtonClicked;

    public string pastDialogue =  $"<color=#7F807A>" ; // New field to store the past dialogue

    public Label WinLooseLabel { get; set; }
    public Label WinLooseDetailsLabel { get; internal set; }
    public VisualElement TimeBar { get; internal set; }
    public VisualElement TimeBarBack { get; internal set; }
    public VisualElement TimeBarNext { get; internal set; }
    public float TimeBarWidth { get; internal set; }
    public VisualElement TimePanel { get; internal set; }
    public Label TimeText { get; internal set; }
    public string NewImpression { get; internal set; }
    public string NewMood { get; internal set; }

    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        if (interactionMenuTemplate != null)
        {
            VisualElement interactionMenu2 = interactionMenuTemplate.CloneTree();
            root.Add(interactionMenu2);
            VisualElement interactionMenu = interactionMenuTemplate.CloneTree();
            root.Add(interactionMenu);

            // Get references to the container
            menuContainer2 = interactionMenu2.Q<VisualElement>("MenuContainer");
            menuContainer = interactionMenu.Q<VisualElement>("MenuContainer");

            menuContainer2.style.position = Position.Absolute;

            menuContainer2.style.width = new Length(1000, LengthUnit.Pixel);
            menuContainer2.style.height = new Length(596, LengthUnit.Pixel);
            menuContainer2.style.display = DisplayStyle.None;
            menuContainer2.style.left = new Length(550, LengthUnit.Pixel);
            menuContainer2.style.left = new Length((1600 + 100) / 2 - 150, LengthUnit.Pixel); // Move right by 240px
            menuContainer2.style.top = new Length((900 + 200) / 2 - 550, LengthUnit.Pixel); // Move up by 400px
            menuContainer2.style.flexDirection = FlexDirection.Column;
            menuContainer2.style.alignItems = Align.FlexStart;
            menuContainer2.style.justifyContent = Justify.FlexStart;
            menuContainer2.style.overflow = Overflow.Visible;
            // Display the menu
            menuContainer2.style.display = DisplayStyle.Flex;



            menuContainer.style.position = Position.Absolute;
            menuContainer.style.width = new Length(440 + 20, LengthUnit.Pixel);
            menuContainer.style.height = new Length(596, LengthUnit.Pixel);
            menuContainer.style.display = DisplayStyle.None;
            menuContainer.style.left = new Length(550, LengthUnit.Pixel);
            menuContainer.style.left = new Length((1600 + 100) / 2 - 150, LengthUnit.Pixel); // Move right by 240px
            menuContainer.style.top = new Length((900 + 200) / 2 - 550, LengthUnit.Pixel); // Move up by 400px
            menuContainer.style.flexDirection = FlexDirection.Column;
            menuContainer.style.alignItems = Align.FlexStart;
            menuContainer.style.justifyContent = Justify.FlexStart;
            menuContainer.style.overflow = Overflow.Hidden;
            menuContainer.style.paddingRight = new Length(20, LengthUnit.Pixel);
            // Display the menu
            menuContainer.style.display = DisplayStyle.Flex;

            selectRectangle = new VisualElement();
            selectRectangle.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("Sprites/square"));
            selectRectangle.style.position = Position.Absolute;
            selectRectangle.style.left = new Length(0, LengthUnit.Pixel);
            selectRectangle.style.top = new Length(-30, LengthUnit.Pixel);
            selectRectangle.style.width = new Length(400, LengthUnit.Pixel);
            selectRectangle.style.height = new Length(20, LengthUnit.Pixel);
            selectRectangle.style.opacity = 0.99f; // Optional: Slight transparency

            menuContainer2.Add(selectRectangle);



            // Add the background image as a child of the menu container
            backgroundImage = new VisualElement();
            var texture = Resources.Load<Texture2D>("Sprites/InteractionPanel");
            backgroundImage.style.backgroundImage = new StyleBackground(texture);
            backgroundImage.style.position = Position.Absolute;
            backgroundImage.style.left = new Length(80, LengthUnit.Pixel);
            backgroundImage.style.top = new Length(-30, LengthUnit.Pixel);
            backgroundImage.style.width = new Length(616 / 2 + 61 + 16, LengthUnit.Pixel);
            backgroundImage.style.height = texture.height;
            backgroundImage.style.overflow = Overflow.Visible;
            backgroundImage.style.opacity = 0.99f; // Optional: Slight transparency
            //backgroundImage.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
            menuContainer2.Add(backgroundImage);

            var portraitWidth = 63 * 2;
            // Add the portrait background image
            sideBarBack = new VisualElement();
            sideBarBack.style.backgroundColor = MyColor.GreyBack;
            sideBarBack.style.position = Position.Absolute;
            sideBarBack.style.left = new Length(-78, LengthUnit.Pixel); // Left of the menu background
            sideBarBack.style.top = new Length(00, LengthUnit.Pixel);
            sideBarBack.style.width = new Length(portraitWidth + 50, LengthUnit.Pixel); // Scale factor for portrait
            sideBarBack.style.height = new Length((49 * 6) + 5 + 20, LengthUnit.Pixel); // Scale factor for portrait
            sideBarBack.style.opacity = 0.99f;
            sideBarBack.style.borderTopLeftRadius = 10;
            sideBarBack.style.borderTopRightRadius = 10;
            sideBarBack.style.borderBottomLeftRadius = 10;
            sideBarBack.style.borderBottomRightRadius = 10;
            menuContainer2.Add(sideBarBack);



            MenuHelper.SetupActionMenuElements(this, ref TooltipText, ref ActionImage, ref ActionInfoPanel, ref actionsBackPanel, ref menuContainer2, portraitWidth);
            MenuHelper.SetupWinLooseElements(this);
            // Add the actual portrait image
            portraitImage = new VisualElement();
            portraitImage.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("Sprites/portraits/portraitTalinor"));
            portraitImage.style.position = Position.Absolute;
            portraitImage.style.left = new Length(-68, LengthUnit.Pixel); // Align with the back portrait
            portraitImage.style.top = new Length(10, LengthUnit.Pixel);
            portraitImage.style.width = portraitWidth;
            portraitImage.style.height = portraitWidth;
            menuContainer2.Add(portraitImage);


        }
        else
        {
            Debug.LogError("InteractionMenuTemplate is not assigned in the Inspector.");
        }


    }
    private void Start()
    {
        HideMenu();
    }
    private void Update()
    {
        if (ActionsMenuTypeWeAreIn != lastActionsMenu)
        {
            var charm = ActionsMenuTypeWeAreIn == SocialActionMenuType.charm ? "S" : "";
            var give = ActionsMenuTypeWeAreIn == SocialActionMenuType.give ? "S" : "";
            var coercion = ActionsMenuTypeWeAreIn == SocialActionMenuType.coerce ? "S" : "";
            charmMenuButton.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("Sprites/UI/menuCharm" + $"{charm}"));
            giveMenuButton.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("Sprites/UI/menuGive" + give));
            coerceMenuButton.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("Sprites/UI/menuCoercion" + coercion));
            lastActionsMenu = ActionsMenuTypeWeAreIn;
        }

        if (dialogue != null)
        {
            if (scrolldown > 0)
            {

                dialogue.style.marginTop = 160 + 90 + scrolldown;
                if(scrolldown2!=0)
                foreach (var item in dialogueOptionButtons)
                {
                    item.style.top = 240 + 90 + scrolldown2;
                }
                scrolldown -= 1;
                scrolldown2 -= fastScroll ? 2 : 1;

            }
       
   
        }
    }


    string GetStatString(Character personWeAreSpeakingTo)
    {
        var trust = personWeAreSpeakingTo.Impression.TrustTowardsPlayer;
        var fear = personWeAreSpeakingTo.Impression.FearTowardsPlayer;
        var relationship = TextConverter.GetRelationshipStatusText(personWeAreSpeakingTo);
        var mood = personWeAreSpeakingTo.State.VisualState[0];
        var impression = personWeAreSpeakingTo.Impression.GetSocialImpressionText();
        return $"<color={greenOn}>TRUST {trust}</color>\n<color={redOn}>FEAR {fear}</color>\n-----------\n<color=#A0A0A0>RELATIONSHIP</color>\n{relationship}</color>\n<color=#A0A0A0>MOOD</color>\n{mood}\n<color=#A0A0A0>IMPRESSION</color>\n{impression} ";

    }
    public void ShowMenu(string lastChosenOption, string currentDialogue, string currentSpeaker, List<MenuOption> dialogueOptions, string contextTitle, List<MenuOption> menuButtons, Character personWeAreSpeakingTo)
    {

        lastActionsMenu = SocialActionMenuType.none;
        #region START
        if (currentDialogue != "")
        {
            fastScroll=true;
            scrolldown = 20;
            scrolldown2 = 20;
        }
        else
        {
            fastScroll=false;
            scrolldown = 1f;
            scrolldown2 = 0;
        }

        menuContainer2.style.width = new Length(1000, LengthUnit.Pixel);
        GameManager.Instance.BlockingPlayerUIOnScreen = true;
        menuContainer.Clear();
        menuContainer2.Clear();
        var endGameState= GameManager.Instance.EndGameState;
        if (endGameState  != GameState.none)
        {
            MenuHelper.WinLooseScreen(this, endGameState);
            return;
        }
        // Add the background image first
        menuContainer.Add(backgroundImage);

        MenuHelper.Portrait(personWeAreSpeakingTo, menuContainer2, sideBarBack, portraitImage);
        if(DoingAction!=null)
        MenuHelper.ActionText(DoingAction,personWeAreSpeakingTo, menuContainer, scrolldown);


        var trust = personWeAreSpeakingTo.Impression.TrustTowardsPlayer;
        var fear = personWeAreSpeakingTo.Impression.FearTowardsPlayer;
        var relationship = TextConverter.GetRelationshipStatusText(personWeAreSpeakingTo);
        var mood = personWeAreSpeakingTo.State.VisualState[0];
        var impression = personWeAreSpeakingTo.Impression.GetSocialImpressionText();

        MenuHelper.StatText(personWeAreSpeakingTo, menuContainer2, ref statText, ref statLabel, greenOn, redOn, trust, fear, relationship, mood, impression);
        #endregion

        #region ACTIONS MENU
        MenuHelper.ActionsMenu(this, ActionsMenuTypeWeAreIn, personWeAreSpeakingTo);


        #endregion

        #region DIALOGUE
        //////DIALOGUE

        string strippedPastDialogue = pastDialogue;
        if (lastChosenOption != "")
        { strippedPastDialogue = MyColor.StripColorTags(pastDialogue); }

        pastDialogue =
                  $"{strippedPastDialogue}";



        pastDialogue = MyColor.WrapTextInPurpleTag(pastDialogue);

        // Conditionally add "YOU-{lastChosenOption}" if it's not an empty string
        if (lastChosenOption != "" && lastChosenOption != "CONTINUE >")
        {
            pastDialogue += @$"<color=#B3B4BC>YOU-""{lastChosenOption}""</color>" + "\n" + "\n" + "()";
        }

        pastDialogue += NewImpression;

        pastDialogue += NewMood;

        pastDialogue += reportNewKey;

        if(NewMood!="")
        pastDialogue = MyColor.WrapTextInDarkYellowTag(pastDialogue) + "\n";
        NewMood = "";

        if (reportNewKey!="")
        pastDialogue = MyColor.WrapTextInAquaTag(pastDialogue)+"\n";
        reportNewKey = "";

        pastDialogue = MyColor.WrapTextInYellowTag(pastDialogue);
        NewImpression = "";
        // Add the speaker and current dialogue in white

        var newDialogue="";
        if (currentDialogue != "")
        {
            newDialogue = @$"<color=#FFFFFF>{currentSpeaker.ToUpper()}</color><color=#D5D6C8>- " +
                Regex.Replace(currentDialogue, @"^#(.*?)#(.*)", @$"  <color={MyColor.PaleWhiteHex}>$1</color>"+ @"""$2""") +
                "\n\n";


        }
        newDialogue = newDialogue.Replace("\n", "");
        if (newDialogue != "")
        {
            pastDialogue += newDialogue + "\n" + "\n";
        }


        // Create the button and add the label
        dialogue = new Button();
        var removestars = System.Text.RegularExpressions.Regex.Replace(pastDialogue, @"\*", string.Empty);
        pastDialogue.Replace("()", "");
        removestars=removestars.Replace("()","\n");

        // Add a label to the button
        Label dialogueLabel = new Label($"<color=#7F807A>"+removestars + "</color>")
        {
            style =
            {
                color = Color.white,  // Default color, will be overridden by rich text
                unityTextAlign = TextAnchor.MiddleLeft,
                fontSize = standardFontSize,
                whiteSpace = WhiteSpace.Normal,  // Allow text wrapping within the label
                overflow = Overflow.Hidden,
                paddingBottom = new Length(5, LengthUnit.Pixel),  // Prevent cutting off text
                paddingTop = new Length(20, LengthUnit.Pixel),
            }
        };
        dialogue.Add(dialogueLabel);

        // Apply button styling to mimic other buttons
        dialogue.style.position = Position.Absolute; // Set position to absolute
        dialogue.style.left = new Length(130 + 120 + 60 - 19 - 9 + 50, LengthUnit.Pixel);
        dialogue.style.top = new Length(40 + scrolldown, LengthUnit.Pixel); // Set initial top position
        dialogue.style.width = new Length(340, LengthUnit.Pixel);
        dialogue.style.flexDirection = FlexDirection.ColumnReverse;  // Makes new content push upward

        dialogue.style.overflow = Overflow.Hidden;  // Prevents content from pushing layout

        dialogue.style.alignItems = Align.FlexStart;  // Ensure text starts from the top
        dialogue.style.backgroundColor = new StyleColor(Color.clear);  // Remove button background
        dialogue.focusable = false;  // Prevent focus
        dialogue.style.left = 80 + 22;

        dialogue.style.top = 220;





        dialogue.style.marginTop = 0;
        dialogue.style.paddingTop = new Length(5, LengthUnit.Pixel);
        dialogue.AddToClassList("button");

        // Override click event to do nothing
        dialogue.RegisterCallback<ClickEvent>(evt => evt.StopPropagation());

        menuContainer.Add(dialogue);

        // Adjust the top position dynamically based on content height
        dialogue.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            if (currentDialogue != "")
                dialogue.style.top = new Length(90 + scrolldown - dialogue.resolvedStyle.height, LengthUnit.Pixel);
            else
                dialogue.style.top = new Length(90 - dialogue.resolvedStyle.height, LengthUnit.Pixel);
        });

        #endregion

        #region DIALOGUE OPTIONS
        if (DoingAction == null)
        {
            var player = WorldManager.Instance.ThePlayer;
            // Add the dialogue options
            if (dialogueOptions != null)
            {
                var buttonCount = dialogueOptions.Count;
                dialogueOptionButtons.Clear();



                // Arrange buttons vertically
                int trueIndex = 0;
                for (int i = 0; i < buttonCount; i++)
                {
                    var optionIsDisabled = false;
                    if (dialogueOptions[i].menuOptionCost > trust || -dialogueOptions[i].menuOptionCost > fear)
                    {
                        optionIsDisabled = true;
                    }
                    var theirMood = personWeAreSpeakingTo.State.VisualState[0];
                    if (dialogueOptions[i].OptionMoodReq!=MemoryTags.none && dialogueOptions[i].OptionMoodReq != theirMood)
                    {
                        optionIsDisabled = true;
                    }
                    if (dialogueOptions[i].ButtonLabel == "do something else.." && menuButtons != null)
                    {
                        BasicFunctions.Log($"💥Button skipped: {dialogueOptions[i].ButtonLabel} because of do something else", LogType.dia);
                        continue;
                    }

                    if (chosenOptions.Contains(dialogueOptions[i].UniqueId))
                    {
                        BasicFunctions.Log($"💥Button skipped: {dialogueOptions[i].ButtonLabel} is already chosen", LogType.dia);
                        continue;
                    }

                    
                    var moodReq = dialogueOptions[i].OptionNeeds;
                    if (moodReq != MemoryTags.none && moodReq != theirMood)
                    {
                        BasicFunctions.Log($"💥Button skipped: {dialogueOptions[i].ButtonLabel} mood didnt match", LogType.dia);
                        continue;
                    }
                    var optionKey = dialogueOptions[i].OptionKey;
                    var playerKeys = player.KeyKnowledge.Keys;
                    var hasKey = false;
                    if (optionKey == "" || optionKey == null || playerKeys.Count > 0)
                    { hasKey = true; }
                    if (hasKey == false)
                    {
                        BasicFunctions.Log($"💥Button skipped: {dialogueOptions[i].ButtonLabel} key didnt match", LogType.dia);
                        continue;
                    }
                    var costText = "";
                    int cost = dialogueOptions[i].menuOptionCost;
                    if (cost != 0)
                    {
                        if (cost > 0)
                            costText = $"[Trust: {Math.Abs(cost)}] ";
                        else
                            costText = $"[Fear: {Math.Abs(cost)}] ";
                    }
                    string label = dialogueOptions[i].ButtonLabel.TrimStart('-');
                    string originalLabel = label;
                    Button button2 = new Button();
                    button2.style.left = new Length(200, LengthUnit.Pixel);

                    button2.visible = false;
                    menuContainer.Add(button2);

                    Button button3 = new Button();
                    button3.style.left = new Length(200, LengthUnit.Pixel);

                    button3.visible = false;
                    menuContainer.Add(button3);
                    Button button = new Button();
                    dialogueOptionButtons.Add(button);
                    VisualElement selectRect = new VisualElement();
                    selectRect.style.position = Position.Absolute;
                    selectRect.style.left = new Length(0, LengthUnit.Pixel);
                    selectRect.style.top = new Length(5, LengthUnit.Pixel);
                    selectRect.style.width = new Length(95, LengthUnit.Pixel);
                    selectRect.style.height = new Length(24, LengthUnit.Pixel);
                    selectRect.style.flexGrow = 0;
                    selectRect.style.flexShrink = 0;
                    selectRect.style.flexBasis = new Length(0, LengthUnit.Pixel);
                    selectRect.style.overflow = Overflow.Visible;
                    // Create a container label with different styling
                    var numberText = $"{trueIndex + 1}.-";
                    if (label == "")
                    {
                        numberText = "";
                        label = "CONTINUE >";
                        selectRect.style.backgroundColor = MyColor.CyanBack;
                        selectRect.style.width = new Length(320, LengthUnit.Pixel);
                        selectRect.style.height = new Length(36, LengthUnit.Pixel);

                    }
                    else
                    {
                        label = Regex.Replace(label, @"\d", "");
                        label = label.Replace("-", "");
                    }
                    Label numberLabel = new Label(numberText)
                    {
                        style =
                    {
                        color =optionIsDisabled ? Color.grey : Color.white,
                        unityTextAlign = TextAnchor.MiddleLeft,
                        fontSize = standardFontSize,
                        alignSelf = Align.FlexStart , // Align to the top of the button
                        unityFontStyleAndWeight = FontStyle.Normal
                    }
                    };
                    var kindOfCyan = dialogueOptions[i].OldOption ? MyColor.DarkCyan : MyColor.Cyan;

                  Label textLabel = new Label(costText + label)
                    {
                        style =
                    {
                        color = optionIsDisabled ? Color.grey : label=="CONTINUE >" ? Color.white : cost == 0 ? kindOfCyan : cost < 0 ? MyColor.Red : MyColor.Green,  // Default color
             
                        unityTextAlign = TextAnchor.MiddleLeft,
                        fontSize = standardFontSize,
                        whiteSpace = WhiteSpace.Normal, // Allow text wrapping within the label
                        overflow = Overflow.Visible, // Prevent text from overflowing horizontally
                        alignSelf = Align.FlexStart, // Align to the top of the button
                        unityFontStyleAndWeight = FontStyle.Normal
                    }
                    };
                    if (textLabel.text != "CONTINUE >")
                    {
                        textLabel.text = textLabel.text.Replace(">", "");
                        textLabel.text = textLabel.text.Replace("*", "");
                        numberLabel.text = numberLabel.text.Replace(">", "");
                        numberLabel.text = numberLabel.text.Replace("*", "");
                    }
                    //textLabel.transform.scale = new Vector3(0.8f, 1f, 1f);
                    //selectRect.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("Sprites/square"));

                    // Register the MouseEnter and MouseLeave events to change the color on hover
                    button.RegisterCallback<MouseEnterEvent>(evt =>
                    {
                        if (!optionIsDisabled)
                        {
                            textLabel.style.color = cost == 0 ? Color.white : cost < 0 ? Color.white : Color.white; // Change to white on hover
                            selectRect.style.backgroundColor = new StyleColor(cost == 0 ? MyColor.CyanBack : cost < 0 ? MyColor.Red : MyColor.GreenBack);
                        }

                        if (cost < 0)
                        {
                            redOn = MyColor.RedHex;
                        }

                        if (cost > 0)
                            greenOn = MyColor.GreenHex;
                        if (cost == 0)
                        {
                            selectRect.style.width = new Length(320, LengthUnit.Pixel);
                            selectRect.style.height = new Length(36, LengthUnit.Pixel);
                        }
                        else
                        {
                            statLabel.text = GetStatString(personWeAreSpeakingTo);
                        }

                        // button.style.
                    });

                    button.RegisterCallback<MouseLeaveEvent>(evt =>
                    {
                        if (label != "CONTINUE >" && optionIsDisabled == false)
                        {
                          
                            textLabel.style.color = cost == 0 ? kindOfCyan : cost < 0 ? MyColor.Red : MyColor.Green;  // Change back to the original red color when not hovered

                            selectRect.style.backgroundColor = Color.clear;
                        }

                        redOn = MyColor.GreyHex;
                        greenOn = MyColor.GreyHex;
                        statLabel.text = GetStatString(personWeAreSpeakingTo);
                    });
                    button.style.backgroundColor = Color.clear;
                    // Add labels to the button
                    button.Add(selectRect);
                    button.Add(numberLabel);
                    button.Add(textLabel);


                    // Reduced marginTop for closer buttons
                    button.style.marginLeft = new Length(buttonLeftMargin - 30 + 6 + 8, LengthUnit.Pixel); // Example left margin
                    button.style.marginTop = -30;// Adjusted margin for closer buttons

                    button.style.marginBottom = new Length(-20, LengthUnit.Pixel);
                    button.style.width = new Length(buttonWidth + 20, LengthUnit.Pixel); // Fixed width for button
                    button.style.alignSelf = Align.Center;
                    button.style.flexDirection = FlexDirection.Row; // Ensure elements are side by side
                    button.style.top = 311;//260;
                    button.style.left = new Length(-100 + 40, LengthUnit.Pixel);
                    button.style.paddingTop = new Length(-10, LengthUnit.Pixel);
                    button.style.paddingBottom = new Length(0, LengthUnit.Pixel);
                    button.style.paddingLeft = new Length(0, LengthUnit.Pixel);
                    button.style.paddingRight = new Length(0, LengthUnit.Pixel);

                    button.style.height = StyleKeyword.Auto; // Fit height to content

                    button.style.borderTopLeftRadius = 0;
                    button.style.borderTopRightRadius = 0;
                    button.style.borderBottomLeftRadius = 0;
                    button.style.borderBottomRightRadius = 0;
                    button.style.overflow = Overflow.Visible;
                    button.AddToClassList("button");

                    // Button click handler
                    var diaAction = (DiaActionType)dialogueOptions[i].Data2;
                    int index = i;
                    var uniqueId = dialogueOptions[i].UniqueId;
                    var finalLabel = label;
                    var finalMenuOptionType = MenuOptionType.dia;
                    var newIndex = trueIndex;

                    if (diaAction == DiaActionType.menu)
                    {
                        finalLabel = "open menu";
                        finalMenuOptionType = MenuOptionType.general;
                    }
                    var key = dialogueOptions[i].IsKey;
                    var keyText = TextConverter.GetKeyText(key);
                    button.clicked += () =>
                    {
                        if (optionIsDisabled)
                            return;
                        BasicFunctions.Log($"🌎Button clicked: {label}", LogType.ui);
                        GameManager.Instance.UIClicked = true;
                        if (cost > 0)
                        {
                            personWeAreSpeakingTo.Impression.TrustTowardsPlayer -= cost;
                        }
                        if (cost < 0)
                        {
                            personWeAreSpeakingTo.Impression.FearTowardsPlayer += cost;
                        }

                        if (key != Key.none)
                        {
                            player.KeyKnowledge.Keys.Add(key);
                            reportNewKey = $"$Found key info: {keyText}$\n";

                        }


                        chosenOptions.Add(uniqueId);
                        OnButtonClicked?.Invoke(index, finalLabel, finalMenuOptionType);
                        StartCoroutine(CheckForInputAfterDelay());
                    };

                    // Add button to the container
                    menuContainer.Add(button);
                    trueIndex++;
                }
            }
        }
    
           


        #endregion










        // Continue with the context button (if any)
        if (!string.IsNullOrEmpty(contextTitle))
        {
            Button contextButton = new Button();

            // Add a label to the button
            Label contextLabel = new Label(contextTitle)
            {
                style =
                {
                    color = Color.white,
                    unityTextAlign = TextAnchor.MiddleLeft,
                    fontSize = standardFontSize
                }
            };
            contextButton.Add(contextLabel);

            // Apply button styling to mimic other buttons
            contextButton.style.marginLeft = new Length(130 + 120, LengthUnit.Pixel);
            contextButton.style.marginTop = new Length(20, LengthUnit.Pixel);
            contextButton.style.top = 240;
            contextButton.style.marginBottom = new Length(10, LengthUnit.Pixel);
            contextButton.style.width = new Length(300, LengthUnit.Pixel);
            contextButton.style.alignSelf = Align.Center;
            contextButton.style.flexDirection = FlexDirection.Row;
            contextButton.style.backgroundColor = new StyleColor(Color.clear);  // Remove button background
            contextButton.focusable = false;  // Prevent focus

            contextButton.AddToClassList("button");

            // Override click event to do nothing
            contextButton.RegisterCallback<ClickEvent>(evt => evt.StopPropagation());

            menuContainer.Add(contextButton);
        }
        if (menuButtons != null)
        {
            var buttonCount = menuButtons.Count;

            // Arrange buttons vertically
            for (int i = 0; i < buttonCount; i++)
            {

                string label = menuButtons[i].ButtonLabel;
                Button button = new Button();
                dialogueOptionButtons.Add(button);
                // Create a container label with different styling
                Label numberLabel = new Label($"{i + 1}.")
                {
                    style =
                    {
                        color = Color.white,
                        unityTextAlign = TextAnchor.MiddleLeft,
                        fontSize = standardFontSize,
                        alignSelf = Align.FlexStart  // Align to the top of the button
                    }
                };

                Label textLabel = new Label(label)
                {
                    style =
                    {
                        color = MyColor.Red,  // Default color
                        unityTextAlign = TextAnchor.MiddleLeft,
                        fontSize = standardFontSize,
                        whiteSpace = WhiteSpace.Normal, // Allow text wrapping within the label
                        overflow = Overflow.Hidden, // Prevent text from overflowing horizontally
                        alignSelf = Align.FlexStart, // Align to the top of the button
                        unityFontStyleAndWeight = FontStyle.Bold
                    }
                };

                // Register the MouseEnter and MouseLeave events to change the color on hover
                button.RegisterCallback<MouseEnterEvent>(evt =>
                {
                    textLabel.style.color = Color.white;  // Change to white on hover
                });

                button.RegisterCallback<MouseLeaveEvent>(evt =>
                {
                    textLabel.style.color = MyColor.Red;  // Change back to the original red color when not hovered
                });

                // Add labels to the button
                button.Add(numberLabel);
                button.Add(textLabel);

                // Reduced marginTop for closer buttons
                button.style.marginLeft = new Length(buttonLeftMargin - 30 + 6 + 8, LengthUnit.Pixel); // Example left margin
                button.style.marginTop = new Length(5, LengthUnit.Pixel); // Adjusted margin for closer buttons

                button.style.marginBottom = new Length(-20, LengthUnit.Pixel);
                button.style.width = new Length(buttonWidth, LengthUnit.Pixel); // Fixed width for button
                button.style.alignSelf = Align.Center;
                button.style.flexDirection = FlexDirection.Row; // Ensure elements are side by side
                button.style.top = 210;


                button.AddToClassList("button");

                // Button click handler

                int index = i;

                button.clicked += () =>
                {
                    BasicFunctions.Log($"🌎Button clicked: {label}", LogType.ui);
                    GameManager.Instance.UIClicked = true;
                    OnButtonClicked?.Invoke(index, label, MenuOptionType.general);
                    StartCoroutine(CheckForInputAfterDelay());
                };

                // Add button to the container
                menuContainer.Add(button);
            }
        }
        // Arrange the menu buttons
        /*  if (menuButtons != null)
          {
              foreach (var menuButton in menuButtons)
              {
                  Button button = CreateMenuButton(menuButtons, menuButton);
                  menuContainer.Add(button);
              }
          }*/

        // Display the menu
        menuContainer.style.display = DisplayStyle.Flex;
        menuContainer2.style.display = DisplayStyle.Flex;
    }

    private Button CreateMenuButton(List<MenuOption> menuButtons, MenuOption menuOption)
    {
        Button button = new Button();
        Label label = new Label(menuOption.ButtonLabel)
        {
            style =
            {
                color = Color.white,
                unityTextAlign = TextAnchor.MiddleLeft,
                fontSize = 16
            }
        };
        button.Add(label);
        button.style.marginLeft = new Length(buttonLeftMargin, LengthUnit.Pixel);
        button.style.marginTop = new Length(buttonSpacing, LengthUnit.Pixel);
        button.style.width = new Length(buttonWidth, LengthUnit.Pixel);
        button.AddToClassList("button");

        // Track the button's index using the menuOptions list index
        int buttonIndex = (int)menuOption.Data1;

        button.clicked += () =>
        {
            BasicFunctions.Log($"🌎 Button clicked: {menuOption.ButtonLabel}", LogType.ui);
            GameManager.Instance.UIClicked = true;

            // Now call the event with the index from the list
            OnButtonClicked?.Invoke(buttonIndex, menuOption.ButtonLabel, menuOption.menuOptionType);
            StartCoroutine(CheckForInputAfterDelay());
        };

        return button;
    }

    private IEnumerator CheckForInputAfterDelay()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return null;
        }
        GameManager.Instance.UIClicked = false;
    }

    public void HideMenu()
    {
        EventManager.TriggerSwitchCameraToNormalMode();
        GameManager.Instance.BlockingPlayerUIOnScreen = false;
        menuContainer.style.display = DisplayStyle.None;
        menuContainer2.style.display = DisplayStyle.None;
        pastDialogue = "";
    }


}
