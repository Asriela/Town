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
            backgroundImage.style.opacity = 0.9f; // Optional: Slight transparency
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

        // Add the background image first
        menuContainer.Add(backgroundImage);

        if (!string.IsNullOrEmpty(lastMenuOption))
        {
            Label contextLabel = new Label(lastMenuOption);
            contextLabel.style.fontSize = 15; // Reduced font size
            contextLabel.style.color = new Color(1f, 1f, 1f); // Set text color to white
            contextLabel.style.marginTop = new Length(40, LengthUnit.Pixel); // Increased margin to lower the title
            contextLabel.style.marginBottom = new Length(10, LengthUnit.Pixel); // Adjusted to reduce the gap to buttons
            contextLabel.style.alignSelf = Align.Center;  // Center the context label in its container
            contextLabel.style.unityTextAlign = TextAnchor.MiddleCenter;

            // Shift the title text to the right by 50px
            contextLabel.style.marginLeft = new Length(300, LengthUnit.Pixel);

            // Set width to match the background image width and allow wrapping
            contextLabel.style.width = new Length(240, LengthUnit.Pixel);
            contextLabel.style.whiteSpace = WhiteSpace.Normal; // Enable wrapping
            contextLabel.style.overflow = Overflow.Hidden; // Prevent overflow of the text

            menuContainer.Add(contextLabel);
        }

        // Arrange buttons vertically
        for (int i = 0; i < buttonCount; i++)
        {
            string label = buttonLabels[i].ButtonLabel;
            Button button = new Button { text = label };

            // Shift the buttons to the right by 50px
            button.style.marginLeft = new Length(350, LengthUnit.Pixel);

            // Positioning buttons
            button.style.marginTop = new Length(i == 0 ? 10 : buttonSpacing, LengthUnit.Pixel); // Reduced space before buttons
            button.style.width = new Length(180, LengthUnit.Pixel);
            button.style.alignSelf = Align.Center;
            button.style.unityTextAlign = TextAnchor.MiddleLeft;
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

            menuContainer.Add(button);
        }

        // Move the menu up by the height (400px) and right by the width (240px)
        menuContainer.style.left = new Length((1600 + 100) / 2 - 150, LengthUnit.Pixel); // Move right by 240px
        menuContainer.style.top = new Length((900 + 200) / 2 - 500, LengthUnit.Pixel); // Move up by 400px

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
