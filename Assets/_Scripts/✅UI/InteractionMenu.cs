﻿using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Collections;

public class InteractionMenu : MonoBehaviour
{
    public VisualTreeAsset interactionMenuTemplate;
    public float buttonSpacing = 2f; // Spacing between buttons
    public VisualElement root;
    private VisualElement menuContainer;
    private VisualElement backgroundImage;
    private VisualElement portraitBackImage;
    private VisualElement portraitImage;

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


    public void ShowMenu(string lastChosenOption, string currentDialogue, List<MenuOption> diaButtons, string contextTitle, List<MenuOption> menuButtons, Character personWeAreSpeakingTo)
    {
        GameManager.Instance.BlockingPlayerUIOnScreen = true;
        menuContainer.Clear();

        var myRed = new Color(0.768f, 0.251f, 0.075f);

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
            fontSize = 14
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

        // Add the pastDialogue as the first button
        pastDialogue += $"{lastChosenOption}\n{currentDialogue}\n";  // Combine the last chosen option and current dialogue
        Button pastDialogueButton = new Button();
        Label pastDialogueLabel = new Label(pastDialogue)
        {
            style =
        {
            color = Color.white,
            unityTextAlign = TextAnchor.MiddleLeft,
            fontSize = 12
        }
        };
        pastDialogueButton.Add(pastDialogueLabel);
        pastDialogueButton.style.left = new Length(buttonLeftMargin, LengthUnit.Pixel);
        pastDialogueButton.style.marginTop = new Length(buttonSpacing, LengthUnit.Pixel);
        pastDialogueButton.style.width = new Length(buttonWidth, LengthUnit.Pixel);
        pastDialogueButton.AddToClassList("button"); // Add the same class for consistent styling
        pastDialogueButton.style.backgroundColor = new StyleColor(Color.clear); // Remove button background
        pastDialogueButton.focusable = false; // Prevent focus
        pastDialogueButton.RegisterCallback<ClickEvent>(evt => evt.StopPropagation());
        menuContainer.Add(pastDialogueButton);

        // Add the diaButtons options
        if (diaButtons != null)
        {
            foreach (var diaButton in diaButtons)
            {
                Button button = CreateMenuButton(menuButtons, diaButton);
                menuContainer.Add(button);
            }
        }


        // Continue with the context button (if any)
        if (!string.IsNullOrEmpty(contextTitle))
        {
            Button contextButton = new Button();
            Label contextLabel = new Label(contextTitle)
            {
                style =
            {
                color = Color.white,
                unityTextAlign = TextAnchor.MiddleLeft,
                fontSize = 12
            }
            };

            contextButton.Add(contextLabel);
            contextButton.style.marginLeft = new Length(buttonLeftMargin, LengthUnit.Pixel);
            contextButton.style.marginTop = new Length(buttonSpacing, LengthUnit.Pixel);
            contextButton.style.width = new Length(buttonWidth, LengthUnit.Pixel);
            contextButton.AddToClassList("button"); // Add the same class for consistent styling
            contextButton.style.backgroundColor = new StyleColor(Color.clear); // Remove button background
            contextButton.focusable = false; // Prevent focus
            contextButton.RegisterCallback<ClickEvent>(evt => evt.StopPropagation());
            menuContainer.Add(contextButton);
        }

        // Arrange the menu buttons
        if (menuButtons != null)
        {
            foreach (var menuButton in menuButtons)
            {
                Button button = CreateMenuButton(menuButtons, menuButton);
                menuContainer.Add(button);
            }
        }


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
            fontSize = 12
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
    }
}
