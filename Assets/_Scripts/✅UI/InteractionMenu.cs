using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Collections;
using static UnityEditor.Progress;

public class InteractionMenu : MonoBehaviour
{
    public VisualTreeAsset interactionMenuTemplate;
    public float buttonSpacing = 2f; // Spacing between buttons
    public VisualElement root;
    private VisualElement menuContainer;
    private VisualElement backgroundImage;
    private VisualElement portraitBackImage;
    private VisualElement portraitImage;
    private Button dialogue;
    private List<Button> dialogueOptionButtons = new();
    private float scrolldown = 0;
    private float scrolldown2 = 0;
    private int standardFontSize=14;

    [SerializeField]
    private float buttonLeftMargin = 230;
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
            VisualElement interactionMenu = interactionMenuTemplate.CloneTree();
            root.Add(interactionMenu);

            // Get references to the container
            menuContainer = interactionMenu.Q<VisualElement>("MenuContainer");
            menuContainer.style.position = Position.Absolute;
            menuContainer.style.width = new Length(240, LengthUnit.Pixel);
            menuContainer.style.display = DisplayStyle.None;
            menuContainer.style.left = new Length(650, LengthUnit.Pixel);
            menuContainer.style.left = new Length((1600 + 100) / 2 - 150, LengthUnit.Pixel); // Move right by 240px
            menuContainer.style.top = new Length((900 + 200) / 2 - 500, LengthUnit.Pixel); // Move up by 400px
            menuContainer.style.flexDirection = FlexDirection.Column;
            menuContainer.style.alignItems = Align.FlexStart;
            menuContainer.style.justifyContent = Justify.FlexStart;

            // Display the menu
            menuContainer.style.display = DisplayStyle.Flex;

            // Add the background image as a child of the menu container
            backgroundImage = new VisualElement();
            backgroundImage.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("Sprites/InteractionPanel"));
            backgroundImage.style.position = Position.Absolute;
            backgroundImage.style.left = new Length(0, LengthUnit.Pixel);
            backgroundImage.style.top = new Length(0, LengthUnit.Pixel);
            backgroundImage.style.width = new Length(480, LengthUnit.Pixel);
            backgroundImage.style.height = new Length(600, LengthUnit.Pixel);
            backgroundImage.style.opacity = 0.99f; // Optional: Slight transparency
            menuContainer.Add(backgroundImage);

            var portraitWidth = 39 * 3;
            // Add the portrait background image
            portraitBackImage = new VisualElement();
            portraitBackImage.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("Sprites/portraits/portraitBack"));
            portraitBackImage.style.position = Position.Absolute;
            portraitBackImage.style.left = new Length(-38, LengthUnit.Pixel); // Left of the menu background
            portraitBackImage.style.top = new Length(20, LengthUnit.Pixel);
            portraitBackImage.style.width = new Length(portraitWidth, LengthUnit.Pixel); // Scale factor for portrait
            portraitBackImage.style.height = new Length(49 * 3, LengthUnit.Pixel); // Scale factor for portrait
            portraitBackImage.style.opacity = 0.99f;
            menuContainer.Add(portraitBackImage);

            // Add the actual portrait image
            portraitImage = new VisualElement();
            portraitImage.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("Sprites/portraits/portraitTalinor"));
            portraitImage.style.position = Position.Absolute;
            portraitImage.style.left = new Length(-38, LengthUnit.Pixel); // Align with the back portrait
            portraitImage.style.top = new Length(10, LengthUnit.Pixel);
            portraitImage.style.width = new Length(portraitWidth, LengthUnit.Pixel); // Scale factor X 7
            portraitImage.style.height = new Length(portraitWidth, LengthUnit.Pixel); // Scale factor Y 7
            menuContainer.Add(portraitImage);
        }
        else
        {
            Debug.LogError("InteractionMenuTemplate is not assigned in the Inspector.");
        }
    }

    private void Update()
    {
        if (dialogue != null)
        {
            if (scrolldown > 0)
            {

                dialogue.style.marginTop = 160 + 40 + scrolldown;
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

    public void ShowMenu(string lastChosenOption, string currentDialogue, string currentSpeaker, List<MenuOption> dialogueOptions, string contextTitle, List<MenuOption> menuButtons, Character personWeAreSpeakingTo)
    {

            scrolldown = 0;
        scrolldown2 = 0;
            

        GameManager.Instance.BlockingPlayerUIOnScreen = true;
        menuContainer.Clear();

        var myRed = new Color(0.768f, 0.251f, 0.075f);
        var myGrey = new Color(0.478f, 0.494f, 0.518f);

        // Add the background image first
        menuContainer.Add(backgroundImage);

        if (personWeAreSpeakingTo != null)
        {
            // Add the portrait images
            menuContainer.Add(portraitBackImage);
            menuContainer.Add(portraitImage);

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
            menuContainer.Add(nameButtonContainer);
        }

        //////DIALOGUE
        string strippedPastDialogue= pastDialogue;
        if (lastChosenOption != "")
        { strippedPastDialogue = StripColorTags(pastDialogue); }

        pastDialogue = $"<color=#7F807A>" +
                  $"{strippedPastDialogue}";

        // Conditionally add "YOU-{lastChosenOption}" if it's not an empty string
        if (lastChosenOption!="")
        {
            pastDialogue += @$"<color=#B3B4BC>YOU-""{lastChosenOption}""</color>" + "\n\n";
        }

        // Add the speaker and current dialogue in white
        if (currentDialogue != "")
        {
            pastDialogue += @$"<color=#FFFFFF>{currentSpeaker.ToUpper()}</color><color=#D5D6C8>-""{currentDialogue}""</color>" +
            "</color>" + "\n\n";
        }


        // Create the button and add the label
        dialogue = new Button();

        // Add a label to the button
        Label dialogueLabel = new Label(pastDialogue)
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
        dialogue.style.top = new Length(90 + scrolldown, LengthUnit.Pixel); // Set initial top position
        dialogue.style.width = new Length(340, LengthUnit.Pixel);
        dialogue.style.flexDirection = FlexDirection.ColumnReverse;  // Makes new content push upward

        dialogue.style.overflow = Overflow.Hidden;  // Prevents content from pushing layout

        dialogue.style.alignItems = Align.FlexStart;  // Ensure text starts from the top
        dialogue.style.backgroundColor = new StyleColor(Color.clear);  // Remove button background
        dialogue.focusable = false;  // Prevent focus
        dialogue.style.left = 80;
        dialogue.style.top = 220;





            dialogue.style.marginTop = 160 + 40;
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

        // Add the dialogue options
        if (dialogueOptions != null)
        {
            var buttonCount = dialogueOptions.Count;
            dialogueOptionButtons.Clear();
            // Arrange buttons vertically
            for (int i = 0; i < buttonCount; i++)
            {
                if (dialogueOptions[i].ButtonLabel=="do something else.." && menuButtons!=null)
                { continue;}
                string label = dialogueOptions[i].ButtonLabel;
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
                        color = myRed,  // Default color
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
                    textLabel.style.color = myRed;  // Change back to the original red color when not hovered
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
                button.style.top = 260 ;


                    button.AddToClassList("button");

                // Button click handler
                var diaAction = (DiaActionType)dialogueOptions[i].Data2;
                int index = i;

                var finalLabel =label;
                var finalMenuOptionType=MenuOptionType.dia;


                if (diaAction == DiaActionType.menu)
                {
                    finalLabel="open menu";
                    finalMenuOptionType=MenuOptionType.general;
                }
                button.clicked += () =>
                {
                    BasicFunctions.Log($"🌎Button clicked: {label}", LogType.ui);
                    GameManager.Instance.UIClicked = true;
                    OnButtonClicked?.Invoke(index, finalLabel, finalMenuOptionType);
                    StartCoroutine(CheckForInputAfterDelay());
                };

                // Add button to the container
                menuContainer.Add(button);
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
                        color = myRed,  // Default color
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
                    textLabel.style.color = myRed;  // Change back to the original red color when not hovered
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
        pastDialogue = "";
    }
}
