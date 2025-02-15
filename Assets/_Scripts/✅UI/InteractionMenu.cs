using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using Mind;
using UnityEngine.Windows;


public class InteractionMenu : MonoBehaviour
{
    public VisualTreeAsset interactionMenuTemplate;
    public float buttonSpacing = 2f; // Spacing between buttons
    public VisualElement root;
    private VisualElement menuContainer;
    private VisualElement menuContainer2;
    private VisualElement backgroundImage;
    private VisualElement sideBarBack;
    private VisualElement portraitImage;
    private Button dialogue;
    private Button statText; 
    private List<Button> dialogueOptionButtons = new();
    private float scrolldown = 0;
    private float scrolldown2 = 0;
    private int standardFontSize=14;
    private List<string> chosenOptions = new List<string>();
    private VisualElement selectRectangle;
    private string redOn=MyColor.GreyHex;
    private string greenOn = MyColor.GreyHex;
    private Label statLabel;

    [SerializeField]
    private float buttonLeftMargin = 230-200;
    [SerializeField]
    private float buttonWidth = 280;

    public delegate void ButtonClicked(int positionInList, string buttonLabel, MenuOptionType menuOptionType);
    public event ButtonClicked OnButtonClicked;

    private string pastDialogue = ""; // New field to store the past dialogue

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
            menuContainer= interactionMenu.Q<VisualElement>("MenuContainer");

            menuContainer2.style.position = Position.Absolute;
            menuContainer2.style.width = new Length(50, LengthUnit.Pixel);
            menuContainer2.style.height = new Length(596, LengthUnit.Pixel);
            menuContainer2.style.display = DisplayStyle.None;
            menuContainer2.style.left = new Length(550, LengthUnit.Pixel);
            menuContainer2.style.left = new Length((1600 + 100) / 2 - 150, LengthUnit.Pixel); // Move right by 240px
            menuContainer2.style.top = new Length((900 + 200) / 2 - 500, LengthUnit.Pixel); // Move up by 400px
            menuContainer2.style.flexDirection = FlexDirection.Column;
            menuContainer2.style.alignItems = Align.FlexStart;
            menuContainer2.style.justifyContent = Justify.FlexStart;
            menuContainer2.style.overflow = Overflow.Visible;
            // Display the menu
            menuContainer2.style.display = DisplayStyle.Flex;

            menuContainer.style.position = Position.Absolute;
            menuContainer.style.width = new Length(440+20, LengthUnit.Pixel);
            menuContainer.style.height = new Length(596, LengthUnit.Pixel);
            menuContainer.style.display = DisplayStyle.None;
            menuContainer.style.left = new Length(550, LengthUnit.Pixel);
            menuContainer.style.left = new Length((1600 + 100) / 2 - 150, LengthUnit.Pixel); // Move right by 240px
            menuContainer.style.top = new Length((900 + 200) / 2 - 500, LengthUnit.Pixel); // Move up by 400px
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
            backgroundImage.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("Sprites/InteractionPanel"));
            backgroundImage.style.position = Position.Absolute;
            backgroundImage.style.left = new Length(0, LengthUnit.Pixel);
            backgroundImage.style.top = new Length(-30, LengthUnit.Pixel);
            backgroundImage.style.width = new Length(480, LengthUnit.Pixel);
            backgroundImage.style.height = new Length(630, LengthUnit.Pixel);
            backgroundImage.style.opacity = 0.99f; // Optional: Slight transparency

            menuContainer2.Add(backgroundImage);

            var portraitWidth = 63 * 2;
            // Add the portrait background image
            sideBarBack = new VisualElement();
            sideBarBack.style.backgroundColor= MyColor.GreyBack;
            sideBarBack.style.position = Position.Absolute;
            sideBarBack.style.left = new Length(-78, LengthUnit.Pixel); // Left of the menu background
            sideBarBack.style.top = new Length(00, LengthUnit.Pixel);
            sideBarBack.style.width = new Length(portraitWidth+50, LengthUnit.Pixel); // Scale factor for portrait
            sideBarBack.style.height = new Length((49 * 6)+5+20, LengthUnit.Pixel); // Scale factor for portrait
            sideBarBack.style.opacity = 0.99f;
            sideBarBack.style.borderTopLeftRadius = 10;
            sideBarBack.style.borderTopRightRadius = 10;
            sideBarBack.style.borderBottomLeftRadius = 10;
            sideBarBack.style.borderBottomRightRadius = 10;
            menuContainer2.Add(sideBarBack);



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
        if (dialogue != null)
        {
            if (scrolldown > 0)
            {

                dialogue.style.marginTop = 160 + 90 + scrolldown;
                foreach (var item in dialogueOptionButtons)
                {
                    item.style.top = 240 + 90 + scrolldown2;
                }
                scrolldown -= 1;
               scrolldown2 -= 2;
            
            }
        }
    }
    private string StripColorTags(string input)
    {
        return System.Text.RegularExpressions.Regex.Replace(input, "<color[^>]*?>|</color>", string.Empty);


    }
    string GetStatString(int trust, int fear, string relationship, MemoryTags mood, string impression)
    {
        return $"<color={greenOn}>TRUST {trust}</color>\n<color={redOn}>FEAR {fear}</color>\n-----------\n<color=#A0A0A0>RELATIONSHIP</color>\n{relationship}</color>\n<color=#A0A0A0>MOOD</color>\n{mood}\n<color=#A0A0A0>IMPRESSION</color>\n{impression} ";

    }

    public void ShowMenu(string lastChosenOption, string currentDialogue, string currentSpeaker, List<MenuOption> dialogueOptions, string contextTitle, List<MenuOption> menuButtons, Character personWeAreSpeakingTo)
    {

            scrolldown = 20;
        scrolldown2 = 20;
            

        GameManager.Instance.BlockingPlayerUIOnScreen = true;
        menuContainer.Clear();
        menuContainer2.Clear();


        // Add the background image first
        menuContainer.Add(backgroundImage);

        if (personWeAreSpeakingTo != null)
        {
            // Add the portrait images
            menuContainer2.Add(sideBarBack);
            menuContainer2.Add(portraitImage);

            // Update the portrait image with the character's name
            portraitImage.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>($"Sprites/portraits/portrait{personWeAreSpeakingTo.CharacterName}"));

            // Create a separate container for the name button (this will not affect the menu options layout)
            VisualElement nameButtonContainer = new VisualElement();
            nameButtonContainer.style.position = Position.Absolute;
            nameButtonContainer.style.top = new Length(110, LengthUnit.Pixel); // Position below the portrait
            nameButtonContainer.style.left = new Length(-70, LengthUnit.Pixel); // Align to the left margin
            nameButtonContainer.style.width = new Length(180, LengthUnit.Pixel);

            // Add a button below the portrait image with the character's name
            Button nameButton = new Button();
            Label nameLabel = new Label(personWeAreSpeakingTo.CharacterName.ToString())
            {
                style =
                {
                    color = Color.white,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    fontSize = 16,
                }
            };
            nameButton.Add(nameLabel);

            nameButton.style.marginTop = new Length(10, LengthUnit.Pixel); // Adjust this margin if necessary
            nameButton.style.backgroundColor = new StyleColor(Color.clear);  // Transparent background
            nameButton.focusable = false;  // Prevent focus

            nameButton.AddToClassList("button");

            // Override click event to do nothing
            nameButton.RegisterCallback<ClickEvent>(evt => evt.StopPropagation());

            // Add the name button to the container
            nameButtonContainer.Add(nameButton);

            // Add the container for the name button to the menu (it won't affect the other button layout)
          //  menuContainer2.Add(nameButtonContainer);
        }
        var currentPlayerSocialAction= GameManager.Instance.GetPlayersCurrentSocialAction();
        if (currentPlayerSocialAction != SocializeType.none)
        {
            string socialActionText=$"<color=#7F807A>INTERACTION PAUSED BECAUSE YOU ARE BUSY \n{ChangeSocialInteractionToText(currentPlayerSocialAction, currentSpeaker)}</color>";
            // Create the button and add the label
            var socialAction = new Button();

            // Add a label to the button
            Label socialActionLabel = new Label(socialActionText)
            {
                style =
            {
                color = Color.white,  // Default color, will be overridden by rich text
                unityTextAlign = TextAnchor.MiddleCenter,
                fontSize = 17,
                whiteSpace = WhiteSpace.Normal,  // Allow text wrapping within the label
                overflow = Overflow.Hidden,
                paddingBottom = new Length(5, LengthUnit.Pixel),  // Prevent cutting off text
                paddingTop = new Length(20, LengthUnit.Pixel),
            }
            };
            socialAction.Add(socialActionLabel);

            // Apply button styling to mimic other buttons
            socialAction.style.position = Position.Absolute; // Set position to absolute
            socialAction.style.left = new Length(130 + 120 + 60 - 19 - 9, LengthUnit.Pixel);
            socialAction.style.top = new Length(90 + scrolldown, LengthUnit.Pixel); // Set initial top position
            socialAction.style.width = new Length(340, LengthUnit.Pixel);
            socialAction.style.flexDirection = FlexDirection.ColumnReverse;  // Makes new content push upward

            socialAction.style.overflow = Overflow.Hidden;  // Prevents content from pushing layout

            socialAction.style.alignItems = Align.FlexStart;  // Ensure text starts from the top
            socialAction.style.backgroundColor = new StyleColor(Color.clear);  // Remove button background
            socialAction.focusable = false;  // Prevent focus
            socialAction.style.left = 80;
            socialAction.style.top = 220;





            socialAction.style.marginTop = 160 + 40;
            socialAction.style.paddingTop = new Length(5, LengthUnit.Pixel);
            socialAction.AddToClassList("button");

            // Override click event to do nothing
            socialAction.RegisterCallback<ClickEvent>(evt => evt.StopPropagation());

            menuContainer.Add(socialAction);

            // Adjust the top position dynamically based on content height
            socialAction.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                socialAction.style.top = new Length(90 + scrolldown - socialAction.resolvedStyle.height, LengthUnit.Pixel);
            });
            return;
        }

        statText = new Button();//red: C03F13 green 50AA7C
        var trust = personWeAreSpeakingTo.Impression.TrustTowardsPlayer;
        var fear = personWeAreSpeakingTo.Impression.FearTowardsPlayer;
        var relationship=TextConverter.GetRelationshipStatusText(personWeAreSpeakingTo);
        var mood = personWeAreSpeakingTo.State.VisualState[0];
        var impression = personWeAreSpeakingTo.Impression.GetSocialImpressionText();

        var statsString =GetStatString(trust, fear, relationship, mood,impression);
        // Add a label to the button
         statLabel = new Label(statsString)
        {
            style =
            {

                unityTextAlign = TextAnchor.MiddleLeft,
                fontSize = 12,
                whiteSpace = WhiteSpace.Normal,  // Allow text wrapping within the label
                overflow = Overflow.Hidden,
                paddingBottom = new Length(5, LengthUnit.Pixel),  // Prevent cutting off text
                paddingTop = new Length(20, LengthUnit.Pixel),
            }
        };
        statText.Add(statLabel);

        // Apply button styling to mimic other buttons
        statText.style.position = Position.Absolute; // Set position to absolute
        statText.style.width = new Length(340, LengthUnit.Pixel);
        statText.style.flexDirection = FlexDirection.Column;  // Makes new content push upward


        statText.style.alignItems = Align.FlexStart;  // Ensure text starts from the top
        statText.style.backgroundColor = new StyleColor(Color.clear);  // Remove button background
        statText.focusable = false;  // Prevent focus
        statText.style.left = -80;
        statText.style.top = 120;





        statText.style.marginTop = 0;
        statText.style.paddingTop = new Length(5, LengthUnit.Pixel);
        statText.AddToClassList("statStyle");

        // Override click event to do nothing
        statText.RegisterCallback<ClickEvent>(evt => evt.StopPropagation());

        menuContainer2.Add(statText);

        //////DIALOGUE
        string strippedPastDialogue= pastDialogue;
        if (lastChosenOption != "")
        { strippedPastDialogue = StripColorTags(pastDialogue); }

        pastDialogue = $"<color=#7F807A>" +
                  $"{strippedPastDialogue}";

        pastDialogue = MyColor.WrapTextInPurpleTag(pastDialogue);

        // Conditionally add "YOU-{lastChosenOption}" if it's not an empty string
        if (lastChosenOption!="")
        {
            pastDialogue += @$"<color=#B3B4BC>YOU-""{lastChosenOption}""</color>" + "\n\n";
        }

        // Add the speaker and current dialogue in white
        if (currentDialogue != "")
        {
            pastDialogue += @$"<color=#FFFFFF>{currentSpeaker.ToUpper()}</color><color=#D5D6C8>- " +
                Regex.Replace(currentDialogue, @"^#(.*?)#(.*)", @"  <color=#a0a095>$1</color>"+"\n"+@"""$2""") +
                "</color>\n\n";


        }


        // Create the button and add the label
        dialogue = new Button();
        var removestars= System.Text.RegularExpressions.Regex.Replace(pastDialogue, @"\*", string.Empty);
        
        // Add a label to the button
        Label dialogueLabel = new Label(removestars)
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
        dialogue.style.left = new Length(130 + 120 + 60 - 19 - 9, LengthUnit.Pixel);
        dialogue.style.top = new Length(40 + scrolldown, LengthUnit.Pixel); // Set initial top position
        dialogue.style.width = new Length(340, LengthUnit.Pixel);
        dialogue.style.flexDirection = FlexDirection.ColumnReverse;  // Makes new content push upward

        dialogue.style.overflow = Overflow.Hidden;  // Prevents content from pushing layout

        dialogue.style.alignItems = Align.FlexStart;  // Ensure text starts from the top
        dialogue.style.backgroundColor = new StyleColor(Color.clear);  // Remove button background
        dialogue.focusable = false;  // Prevent focus
        dialogue.style.left = 80;
        dialogue.style.top = 220;





        dialogue.style.marginTop = 0 ;
        dialogue.style.paddingTop= new Length(5, LengthUnit.Pixel);
        dialogue.AddToClassList("button");

        // Override click event to do nothing
        dialogue.RegisterCallback<ClickEvent>(evt => evt.StopPropagation());

        menuContainer.Add(dialogue);

        // Adjust the top position dynamically based on content height
        dialogue.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            dialogue.style.top = new Length(90 + scrolldown - dialogue.resolvedStyle.height, LengthUnit.Pixel);
        });

   var player =WorldManager.Instance.ThePlayer;
        // Add the dialogue options
        if (dialogueOptions != null)
        {
            var buttonCount = dialogueOptions.Count;
            dialogueOptionButtons.Clear();
            // Arrange buttons vertically
            int trueIndex = 0;
            for (int i = 0; i < buttonCount; i++)
            {
                if (dialogueOptions[i].menuOptionCost > trust || -dialogueOptions[i].menuOptionCost > fear)
                {BasicFunctions.Log($"💥Button skipped: {dialogueOptions[i].ButtonLabel} because of trust/fear", LogType.dia);
                    continue;}
                if (dialogueOptions[i].ButtonLabel=="do something else.." && menuButtons!=null)
                { BasicFunctions.Log($"💥Button skipped: {dialogueOptions[i].ButtonLabel} because of do something else", LogType.dia);
                    continue;}

                if(chosenOptions.Contains(dialogueOptions[i].UniqueId))
                { BasicFunctions.Log($"💥Button skipped: {dialogueOptions[i].ButtonLabel} is already chosen", LogType.dia);
                    continue; }

                var theirMood= personWeAreSpeakingTo.State.VisualState[0];
                var moodReq = dialogueOptions[i].OptionNeeds;
                if (moodReq != MemoryTags.none && moodReq !=theirMood)
                    {
                    BasicFunctions.Log($"💥Button skipped: {dialogueOptions[i].ButtonLabel} mood didnt match", LogType.dia);
                    continue; }
                var optionKey= dialogueOptions[i].OptionKey;
                var playerKeys= player.KeyKnowledge.Keys;
                var hasKey=false;
                if (optionKey == "" || optionKey == null || playerKeys.Count>0)
                    { hasKey=true;}
                if(hasKey==false)
                {
                    BasicFunctions.Log($"💥Button skipped: {dialogueOptions[i].ButtonLabel} key didnt match", LogType.dia);
                    continue;
                }
                var costText = "";
                int cost = dialogueOptions[i].menuOptionCost;
                if (cost != 0)
                {
                    if(cost>0)
                    costText =$"[Trust: {Math.Abs(cost)}] ";
                    else
                        costText = $"[Fear: {Math.Abs(cost)}] ";
                }
                string label = dialogueOptions[i].ButtonLabel.TrimStart('-');
                string originalLabel= label;
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
                // Create a container label with different styling
                Label numberLabel = new Label($"{trueIndex + 1}.-")
                {
                    style =
                    {
                        color = Color.white,
                        unityTextAlign = TextAnchor.MiddleLeft,
                        fontSize = standardFontSize,
                        alignSelf = Align.FlexStart , // Align to the top of the button
                        unityFontStyleAndWeight = FontStyle.Normal
                    }
                };

                Label textLabel = new Label(costText+label)
                {
                    style =
                    {
                        color = cost == 0 ? MyColor.Cyan : cost < 0 ? MyColor.Red : MyColor.Green,  // Default color
                        unityTextAlign = TextAnchor.MiddleLeft,
                        fontSize = standardFontSize,
                        whiteSpace = WhiteSpace.Normal, // Allow text wrapping within the label
                        overflow = Overflow.Visible, // Prevent text from overflowing horizontally
                        alignSelf = Align.FlexStart, // Align to the top of the button
                        unityFontStyleAndWeight = FontStyle.Normal
                    }
                };

                //textLabel.transform.scale = new Vector3(0.8f, 1f, 1f);
                //selectRect.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("Sprites/square"));
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
                // Register the MouseEnter and MouseLeave events to change the color on hover
                button.RegisterCallback<MouseEnterEvent>(evt =>
                {
                    textLabel.style.color = cost == 0 ? Color.white : cost < 0 ? Color.white : Color.white; // Change to white on hover
                
                        selectRect.style.backgroundColor = new StyleColor(cost == 0 ? MyColor.CyanBack : cost < 0 ? MyColor.Red : MyColor.GreenBack);
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
                        statLabel.text = GetStatString(trust, fear, relationship, mood, impression);
                    }

                    // button.style.
                });

                button.RegisterCallback<MouseLeaveEvent>(evt =>
                {
                    textLabel.style.color = cost == 0 ? MyColor.Cyan : cost< 0 ? MyColor.Red : MyColor.Green;  // Change back to the original red color when not hovered
                    selectRect.style.backgroundColor = Color.clear;
                    redOn= MyColor.GreyHex;
                    greenOn = MyColor.GreyHex;
                    statLabel.text = GetStatString(trust, fear, relationship, mood, impression);
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
                button.style.width = new Length(buttonWidth+20, LengthUnit.Pixel); // Fixed width for button
                button.style.alignSelf = Align.Center;
                button.style.flexDirection = FlexDirection.Row; // Ensure elements are side by side
                button.style.top = 260;
                button.style.left = new Length(-100, LengthUnit.Pixel);
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
                var uniqueId= dialogueOptions[i].UniqueId;
                var finalLabel =label;
                var finalMenuOptionType=MenuOptionType.dia;
                var newIndex= trueIndex;

                if (diaAction == DiaActionType.menu)
                {
                    finalLabel="open menu";
                    finalMenuOptionType=MenuOptionType.general;
                }
                var key= dialogueOptions[i].IsKey;
                var keyText= TextConverter.GetKeyText(key);
                button.clicked += () =>
                {
                    BasicFunctions.Log($"🌎Button clicked: {label}", LogType.ui);
                    GameManager.Instance.UIClicked = true;
                    if (cost > 0)
                    {
                        personWeAreSpeakingTo.Impression.TrustTowardsPlayer-= cost;
                    }
                    if (cost < 0)
                    {
                        personWeAreSpeakingTo.Impression.FearTowardsPlayer += cost;
                    }

                    if(key != "")
                    {
                        player.KeyKnowledge.Keys.Add(key);
                        pastDialogue += $"*Found key info: {keyText}*\n";
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

    string ChangeSocialInteractionToText(SocializeType type, string character)
    {
        var ret="";
        switch (type)
        {
            case SocializeType.drinking:
                ret= $"drinking with {character}";
                break;
        }
        return ret.ToUpper();
    }
}
