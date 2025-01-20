using UnityEngine;
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

    [SerializeField]
    private float buttonLeftMargin = 230;
    [SerializeField]
    private float buttonWidth = 280;

    public delegate void ButtonClicked(int positionInList, string buttonLabel);
    public event ButtonClicked OnButtonClicked;

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

            // Add the background image as a child of the menu container
            backgroundImage = new VisualElement();
            backgroundImage.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("Sprites/InteractionPanel"));
            backgroundImage.style.position = Position.Absolute;
            backgroundImage.style.left = new Length(0, LengthUnit.Pixel);
            backgroundImage.style.top = new Length(0, LengthUnit.Pixel);
            backgroundImage.style.width = new Length(480, LengthUnit.Pixel);
            backgroundImage.style.height = new Length(600, LengthUnit.Pixel);
            backgroundImage.style.opacity = 1f; // Optional: Slight transparency
            menuContainer.Add(backgroundImage);
        }
        else
        {
            Debug.LogError("InteractionMenuTemplate is not assigned in the Inspector.");
        }
    }

    public void ShowMenu(List<MenuOption> buttonLabels, string lastMenuOption)
    {
        GameManager.Instance.BlockingPlayerUIOnScreen = true;
        menuContainer.Clear();

        int buttonCount = buttonLabels.Count;
        var myRed = new Color(0.768f, 0.251f, 0.075f);

        // Add the background image first
        menuContainer.Add(backgroundImage);

        if (!string.IsNullOrEmpty(lastMenuOption))
        {
            Button contextButton = new Button();

            // Add a label to the button
            Label contextLabel = new Label(lastMenuOption)
            {
                style =
            {
                color = Color.white,
                unityTextAlign = TextAnchor.MiddleLeft,
                fontSize = 12
            }
            };

            contextButton.Add(contextLabel);

            // Apply button styling to mimic other buttons
            contextButton.style.marginLeft = new Length(130, LengthUnit.Pixel);
            contextButton.style.marginTop = new Length(20, LengthUnit.Pixel);
            contextButton.style.marginBottom = new Length(10, LengthUnit.Pixel);
            contextButton.style.width = new Length(180, LengthUnit.Pixel);
            contextButton.style.alignSelf = Align.Center;
            contextButton.style.flexDirection = FlexDirection.Row;
            contextButton.style.backgroundColor = new StyleColor(Color.clear);  // Remove button background
            contextButton.focusable = false;  // Prevent focus

            contextButton.AddToClassList("button");

            // Override click event to do nothing
            contextButton.RegisterCallback<ClickEvent>(evt => evt.StopPropagation());

            menuContainer.Add(contextButton);
        }

        // Arrange buttons vertically
        for (int i = 0; i < buttonCount; i++)
        {
            string label = buttonLabels[i].ButtonLabel;
            Button button = new Button();

            // Create a container label with different styling
            Label numberLabel = new Label($"{i + 1}.")
            {
                style =
            {
                color = Color.white,
                unityTextAlign = TextAnchor.MiddleLeft,
                fontSize = 12,
                alignSelf = Align.FlexStart  // Align to the top of the button
            }
            };

            Label textLabel = new Label(label)
            {
                style =
            {
                color = myRed,
                unityTextAlign = TextAnchor.MiddleLeft,
                fontSize = 12,
                whiteSpace = WhiteSpace.Normal, // Allow text wrapping within the label
                overflow = Overflow.Hidden, // Prevent text from overflowing horizontally
                alignSelf = Align.FlexStart // Align to the top of the button
            }
            };

            // Add labels to the button
            button.Add(numberLabel);
            button.Add(textLabel);

            // Reduced marginTop for closer buttons
            button.style.marginLeft = new Length(buttonLeftMargin, LengthUnit.Pixel); // Example left margin
            button.style.marginTop = new Length(-20, LengthUnit.Pixel); // Adjusted margin for closer buttons
            button.style.width = new Length(buttonWidth, LengthUnit.Pixel); // Fixed width for button
            button.style.alignSelf = Align.Center;
            button.style.flexDirection = FlexDirection.Row; // Ensure elements are side by side

            button.AddToClassList("button");

            // Button click handler
            int index = i;
            button.clicked += () =>
            {
                BasicFunctions.Log($"🌎Button clicked: {label}", LogType.ui);
                GameManager.Instance.UIClicked = true;
                OnButtonClicked?.Invoke(index, label);
                StartCoroutine(CheckForInputAfterDelay());
            };

            // Add button to the container
            menuContainer.Add(button);
        }

        // Move the menu up by the height (400px) and right by the width (240px)
        menuContainer.style.left = new Length((1600 + 100) / 2 - 150, LengthUnit.Pixel); // Move right by 240px
        menuContainer.style.top = new Length((900 + 200) / 2 - 500, LengthUnit.Pixel); // Move up by 400px
        menuContainer.style.flexDirection = FlexDirection.Column;
        menuContainer.style.alignItems = Align.FlexStart;
        menuContainer.style.justifyContent = Justify.FlexStart;
        // Display the menu
        menuContainer.style.display = DisplayStyle.Flex;
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
