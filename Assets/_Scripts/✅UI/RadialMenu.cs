using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.TextCore.Text;
using static UnityEngine.GraphicsBuffer;


public class RadialMenu : MonoBehaviour
{
    public event Action<ActionOption> OnButtonClicked;


    public VisualTreeAsset _interactionMenuTemplate;
    public VisualElement _root;
    private VisualElement _menuContainer;
    private VisualElement _tooltipBox;
    private VisualElement _tooltipBoxBack;
    private VisualElement portraitBackImage;
    private VisualElement actionImage;
    private Label _tooltipText;
    private Label _tooltipTextTitle;
    private Label _tooltipEmoji;
    private List<VisualElement> _buttons = new();
    private Character _targetCharacter = null;
    private const float _radius = 140;
    private Vector2 _hoverScale = new Vector2(1.2f, 1.2f);
    private UIDocument _uiDocument;
    [SerializeField] private StyleFontDefinition emojiFont;
    [SerializeField] private Font emojiFont2;
    // New UI Elements
    private VisualElement _backPanel;
    private Label _characterName;
    private Label _trustMeter;
    private Label _fearMeter;
    private Label _relationshipStatus;
    private Label _moodStatus;
    private Label _impressionText;

    private bool _inOpeningMenu = false;
    private void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;


        if (_interactionMenuTemplate != null)
        {
            VisualElement interactionMenu = _interactionMenuTemplate.CloneTree();
            _root.Add(interactionMenu);
            _uiDocument = GetComponent<UIDocument>();


            _menuContainer = interactionMenu.Q<VisualElement>("MenuContainer");
            _menuContainer.style.position = Position.Absolute;
            _menuContainer.style.width = 200;
            _menuContainer.style.height = 200;


            // Back Panel
            _backPanel = new VisualElement();
            _backPanel.style.backgroundColor = new Color(0, 0, 0, 0.9f);
            _backPanel.style.position = Position.Absolute;
            _backPanel.style.width = 370;
            _backPanel.style.height = 400;
            _backPanel.style.borderTopLeftRadius = 10;
            _backPanel.style.borderTopRightRadius = 10;
            _backPanel.style.borderBottomLeftRadius = 10;
            _backPanel.style.borderBottomRightRadius = 10;
            _root.Add(_backPanel);

            var portraitWidth = 63 * 2;
            // Add the portrait background image
            portraitBackImage = new VisualElement();
            portraitBackImage.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("Sprites/portraits/portraitBack"));
            portraitBackImage.style.position = Position.Absolute;
            portraitBackImage.style.left = 210;// Left of the menu background
            portraitBackImage.style.top = 70;
            portraitBackImage.style.width = portraitWidth; // Scale factor for portrait
            portraitBackImage.style.height = portraitWidth; // Scale factor for portrait
            portraitBackImage.style.opacity = 1;
            portraitBackImage.style.unityTextAlign = TextAnchor.MiddleLeft;
            _backPanel.Add(portraitBackImage);

            // Character Name
            _characterName = new Label
            {
                style =
                {
                    color = Color.white,
                    fontSize = 20,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    marginBottom = 5
                }
            };
            _backPanel.Add(_characterName);

            _impressionText = new Label
            {
                style =
                {
                    color = Color.grey,
                    fontSize = 14,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    marginBottom = 5,
                    top = -10
                }
            };
            _backPanel.Add(_impressionText);
            // Trust and Fear Meters
            _trustMeter = new Label
            {
                style =
                {
                    color = MyColor.Green,
                    fontSize = 16,
                    unityTextAlign = TextAnchor.MiddleLeft,
                    marginBottom = 2,
                    top =130
                }
            };
            _backPanel.Add(_trustMeter);


            _fearMeter = new Label
            {
                style =
                {
                    color = MyColor.Red,
                    fontSize = 16,
                    unityTextAlign = TextAnchor.MiddleLeft,
                    marginBottom = 5,
                     top =130
                }
            };
            _backPanel.Add(_fearMeter);


            // Relationship and Mood Status
            _relationshipStatus = new Label
            {
                style =
                {
                    color = Color.white,
                    fontSize = 14,
                    unityTextAlign = TextAnchor.MiddleLeft,
                    marginBottom = 2,
                     top =130
                }
            };
            _backPanel.Add(_relationshipStatus);


            _moodStatus = new Label
            {
                style =
                {
                    color = Color.white,
                    fontSize = 14,
                    unityTextAlign = TextAnchor.MiddleLeft,
                    top =130
                }
            };
            _backPanel.Add(_moodStatus);

            _backPanel.style.display = DisplayStyle.None;
            // Tooltip Box Setup
            _tooltipBoxBack = new VisualElement();
            _tooltipBoxBack.style.backgroundColor = Color.white;
            _tooltipBoxBack.style.position = Position.Absolute;
            _tooltipBoxBack.style.width = 200;
            _tooltipBoxBack.style.height = 300;
            _tooltipBoxBack.style.paddingLeft = 20;
            _tooltipBoxBack.style.paddingRight = 20;
            _tooltipBoxBack.style.paddingTop = 10;
            _tooltipBoxBack.style.paddingBottom = 10;
            _tooltipBoxBack.style.flexDirection = FlexDirection.Column;
            _tooltipBoxBack.style.alignItems = Align.Center;
            _tooltipBoxBack.style.justifyContent = Justify.Center;
            _tooltipBoxBack.style.visibility = Visibility.Hidden;
            _tooltipBoxBack.style.left = _menuContainer.resolvedStyle.left + 220;
            _tooltipBoxBack.style.top = _menuContainer.resolvedStyle.top;
            _tooltipBoxBack.style.borderTopLeftRadius = 5;
            _tooltipBoxBack.style.borderTopRightRadius = 5;
            _tooltipBoxBack.style.borderBottomLeftRadius = 5;
            _tooltipBoxBack.style.borderBottomRightRadius = 5;
            _root.Add(_tooltipBoxBack);
            _tooltipBox = new VisualElement();
            _tooltipBox.style.backgroundColor = Color.white;
            _tooltipBox.style.position = Position.Absolute;
            _tooltipBox.style.width = 200;
            _tooltipBox.style.height = 300;
            _tooltipBox.style.paddingLeft = 20;
            _tooltipBox.style.paddingRight = 20;
            _tooltipBox.style.paddingTop = 10;
            _tooltipBox.style.paddingBottom = 10;
            _tooltipBox.style.flexDirection = FlexDirection.Column;
            _tooltipBox.style.alignItems = Align.Center;
            _tooltipBox.style.justifyContent = Justify.Center;
            _tooltipBox.style.visibility = Visibility.Hidden;
            _tooltipBox.style.left = _menuContainer.resolvedStyle.left + 220;
            _tooltipBox.style.top = _menuContainer.resolvedStyle.top;
            _tooltipBox.style.borderTopLeftRadius = 5;
            _tooltipBox.style.borderTopRightRadius = 5;
            _tooltipBox.style.borderBottomLeftRadius = 5;
            _tooltipBox.style.borderBottomRightRadius = 5;

            var picWidth = 166;
            var picHeight = 147;
            // Add the portrait background image
            actionImage = new VisualElement();
            actionImage.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("Sprites/portraits/portraitBack"));
            actionImage.style.position = Position.Absolute;
            actionImage.style.left = 20;// Left of the menu background
            actionImage.style.top = 50;
            actionImage.style.width = picWidth; // Scale factor for portrait
            actionImage.style.height = picHeight; // Scale factor for portrait
            actionImage.style.opacity = 1;
            actionImage.style.unityTextAlign = TextAnchor.MiddleCenter;
            _tooltipBox.Add(actionImage);

            _tooltipTextTitle = new Label
            {
                style =
                {
                    color = Color.black,
                    fontSize = 18,
                    unityTextAlign = TextAnchor.UpperCenter,
                    whiteSpace = WhiteSpace.Normal,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    alignSelf = Align.Center,
                    flexGrow = 0,
                    marginBottom = 2
                }
            };
            _tooltipBox.Add(_tooltipTextTitle);
            _tooltipEmoji = new Label
            {
                style =
                {
                    color = Color.white,
                    fontSize = 18,
                    unityTextAlign = TextAnchor.UpperCenter,
                    whiteSpace = WhiteSpace.Normal,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    alignSelf = Align.Center,
                    flexGrow = 0,
                    marginBottom = 2,
                    unityFontDefinition  = new StyleFontDefinition(emojiFont2)
}
            };
            _tooltipBox.Add(_tooltipEmoji);

            _tooltipText = new Label
            {
                style =
                {
                    color = Color.black,
                    fontSize = 14,
                    unityTextAlign = TextAnchor.LowerCenter,
                    whiteSpace = WhiteSpace.Normal,
                    alignSelf = Align.Center,
                    flexGrow = 1
                }
            };
            _tooltipBox.Add(_tooltipText);
            _root.Add(_tooltipBox);

            VisualElement _statusContainer = new VisualElement
            {
                style =
                {
                    position = Position.Absolute,
                    right = 0, // Push to the right side of the panel
                    width = Length.Percent(50), // Only take the right half
                    flexDirection = FlexDirection.Column,
                    left = 230,
                    top = 70,
                }
            };
            _backPanel.Add(_statusContainer);

            // Update labels to keep text left-aligned
            _trustMeter.style.unityTextAlign = TextAnchor.MiddleLeft;
            _fearMeter.style.unityTextAlign = TextAnchor.MiddleLeft;
            _relationshipStatus.style.unityTextAlign = TextAnchor.MiddleLeft;
            _moodStatus.style.unityTextAlign = TextAnchor.MiddleLeft;
            _statusContainer.style.left=210;

            // Add labels to the new container instead of _backPanel
            _statusContainer.Add(_trustMeter);
            _statusContainer.Add(_fearMeter);
            _statusContainer.Add(_relationshipStatus);
            _statusContainer.Add(_moodStatus);
        }
    }

    private void Update()
    {
        if (_targetCharacter != null)
        {
            Vector2 targetScreenPos = RuntimePanelUtils.CameraTransformWorldToPanel(
                _uiDocument.rootVisualElement.panel,
                _targetCharacter.transform.position,
                Camera.main
            );


            targetScreenPos.y -= 180;
            targetScreenPos.x -= 100;
            Vector2 menuPosition = _root.WorldToLocal(targetScreenPos);
            _menuContainer.style.left = menuPosition.x;
            _menuContainer.style.top = menuPosition.y;

            _backPanel.style.left = menuPosition.x+260+40+40;
            _backPanel.style.top = menuPosition.y-40;



            Vector2 menuPos = _menuContainer.worldBound.position;


            _tooltipBox.style.left = menuPos.x + 300+40;
            _tooltipBox.style.top = menuPos.y + 30;
        }
    }

    public void OpenMenu(List<ActionOption> options,  Character targetCharacter)
    {


        portraitBackImage.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>($"Sprites/portraits/portrait{targetCharacter.CharacterName}"));
        _tooltipBoxBack.style.display = DisplayStyle.Flex;
        _menuContainer.style.display = DisplayStyle.Flex;
        _backPanel.style.display = DisplayStyle.Flex;
        _targetCharacter = targetCharacter;
        _menuContainer.Clear();
        _buttons.Clear();
        GameManager.Instance.BlockingPlayerUIOnScreen2 = true;

        var player=WorldManager.Instance.ThePlayer;
        _characterName.text = targetCharacter.CharacterName.ToString();
        _impressionText.text = targetCharacter.Impression.GetSocialImpressionText();
        _trustMeter.text = $"TRUST: {targetCharacter.Impression.TrustTowardsPlayer}";
        _fearMeter.text = $"FEAR: {targetCharacter.Impression.FearTowardsPlayer}";
        _relationshipStatus.text = $"<color=#969696>RELATIONSHIP:</color>\n{TextConverter.GetRelationshipStatusText(targetCharacter)}";
        _moodStatus.text = $"<color=#969696>MOOD:</color>\n{targetCharacter.State.VisualState[0]}";

        _inOpeningMenu=false;
        for (int i = 0; i < options.Count; i++)
        {
            var button = CreateButton(options[i], targetCharacter);
            _buttons.Add(button);
            _menuContainer.Add(button);
            if(options[i].Name =="Talk")
                _inOpeningMenu = true;
        }


        
        _tooltipBox.style.backgroundColor = MyColor.PurpleBack;

        _tooltipBox.style.visibility = Visibility.Visible;
        _tooltipText.text = "";
        _tooltipTextTitle.text = "";
        _tooltipBox.style.display = DisplayStyle.Flex;



        LayoutButtons();
    }

    public void HideMenu()
    {
        GameManager.Instance.BlockingPlayerUIOnScreen2 = false;
        _menuContainer.style.display = DisplayStyle.None;
        _tooltipBox.style.display = DisplayStyle.None;
        _backPanel.style.display = DisplayStyle.None;
        _tooltipBoxBack.style.display = DisplayStyle.None;
        _targetCharacter = null;
    }


    private VisualElement CreateButton(ActionOption actionOption, Character target)
    {
        var button = new Button();
        button.style.width = 80;
        button.style.height = 40;
        //button.style.borderRadius = 10;
        button.style.backgroundColor = Color.white;
        button.style.color = Color.black;
        button.style.unityTextAlign = TextAnchor.MiddleCenter;
        button.style.position = Position.Absolute;
        button.style.opacity = 1f; // Start hidden for animation
        button.style.borderBottomLeftRadius = 20;
        button.style.borderBottomRightRadius = 20;
        button.style.borderTopRightRadius = 20;
        button.style.borderTopLeftRadius = 20;
        button.style.flexGrow = 1;
        button.style.flexShrink = 0;
        button.style.width = StyleKeyword.Auto;
        button.style.maxWidth = StyleKeyword.None;


        Label optionLabel = new Label(actionOption.Name)
        {
            style =
            {
                color = Color.black,
                unityTextAlign = TextAnchor.MiddleCenter,
                fontSize = 14,
                whiteSpace = WhiteSpace.Normal,  // Allow text wrapping within the label
                overflow = Overflow.Visible,

                unityFontStyleAndWeight = FontStyle.Bold,
            }
        };


        button.Add(optionLabel);


        if (actionOption.IsDisabled)
        {
            button.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 1); // Greyed-out look
            button.RegisterCallback<MouseEnterEvent>(evt => ShowTooltip(actionOption, button, target));
            button.RegisterCallback<MouseLeaveEvent>(evt => HideTooltip(target));
            button.SetEnabled(false);
        }
        else
        {
            button.style.backgroundColor = Color.white;
            button.RegisterCallback<MouseEnterEvent>(evt => EnterButton(actionOption, button, target));
            button.RegisterCallback<MouseLeaveEvent>(evt => LeaveButton(button, target));
            button.clicked += () => OnButtonClicked?.Invoke(actionOption);
        }


        return button;
    }

    void EnterButton(ActionOption actionOption, VisualElement button , Character target)
    {
        button.style.scale = new Scale(_hoverScale);
        ShowTooltip(actionOption, button, target);
    }

    void LeaveButton(VisualElement button, Character target)
    {
        button.style.scale = new Scale(new Vector2(1f, 1f));
        HideTooltip(target);
    }

    private void LayoutButtons()
    {
        int count = _buttons.Count;
        for (int i = 0; i < count; i++)
        {
            float angle = GetPresetAngle(i, count) * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * _radius;
            float y = Mathf.Sin(angle) * _radius;
            _buttons[i].style.left = 100f + x - 40f;
            _buttons[i].style.top = 100f + y - 20f;
        }
    }


    private float GetPresetAngle(int index, int count)
    {
        int[] angles = { 180, 0, 270, 90 }; // Up, Left, Right, Down
        return (count > 4 || index >= count) ? (360/ count)* index : angles[index];
    }


    private void ShowTooltip(ActionOption actionOption, VisualElement button, Character targetCharacter)
    {
        var tooltip = actionOption.Tooltip;
        var title = actionOption.Name;
        var points = actionOption.Points;
        if (string.IsNullOrEmpty(tooltip))
            return;

        _tooltipBox.style.backgroundColor = _inOpeningMenu==false ? Color.white : MyColor.PurpleBack;
        actionImage.style.visibility = Visibility.Visible;
        var actionTitle= title.ToLower().Replace(" ", "");
        actionImage.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>($"Sprites/actions/{actionTitle}"));
        if (actionTitle == "givealcohol")
        {
            actionImage.style.width = 107;
            actionImage.style.height = 147;
        }
        else
        {
            actionImage.style.width = 166;
            actionImage.style.height = 147;
        }

        _tooltipBox.style.visibility = Visibility.Visible;
        _tooltipText.text = tooltip;
        _tooltipEmoji.text ="";// emoji;
        _tooltipTextTitle.text = title;
        _tooltipText.style.color = _inOpeningMenu? Color.white : Color.black;
        _tooltipTextTitle.style.color = _inOpeningMenu? Color.white : Color.black;
        var addTrust= points > 0 ? $" + {points}" : "";
        var addFear = points < 0 ? $" + {-points}" : "";
        _trustMeter.text = $"TRUST: {targetCharacter.Impression.TrustTowardsPlayer}{addTrust}";
        _fearMeter.text = $"FEAR: {targetCharacter.Impression.FearTowardsPlayer}{addFear}";

        var relAddition="";
        if (actionOption.RelationshipImpact > 0)
            relAddition = $" + {actionOption.RelationshipImpact}";
        else if (actionOption.RelationshipImpact < 0)
            relAddition = $" - {-actionOption.RelationshipImpact}";

        _relationshipStatus.text = $"<color=#969696>RELATIONSHIP:</color>\n{TextConverter.GetRelationshipStatusText(targetCharacter)}{relAddition}";

        var moodAddition="";
        if (actionOption.ActionEffects.Count != 0)
        {
            moodAddition=$" => {actionOption.ActionEffects[0]}";
        }

        _moodStatus.text = $"<color=#969696>MOOD:</color>\n{targetCharacter.State.VisualState[0]}{moodAddition}";
        _tooltipBox.style.display = DisplayStyle.Flex;


        Vector2 menuPos = _menuContainer.worldBound.position;


        _tooltipBox.style.left = menuPos.x + 300;
        _tooltipBox.style.top = menuPos.y + 30;
    }
    private void HideTooltip(Character targetCharacter)
    {

        ColorUtility.TryParseHtmlString("#464568", out Color color);
        _tooltipBox.style.backgroundColor = color;
        _tooltipText.text = "";
        _tooltipTextTitle.text = "";
        actionImage.style.visibility = Visibility.Hidden;
        _tooltipEmoji.text = "";
        _trustMeter.text = $"TRUST: {targetCharacter.Impression.TrustTowardsPlayer}";
        _fearMeter.text = $"FEAR: {targetCharacter.Impression.FearTowardsPlayer}";
        _moodStatus.text = $"<color=#969696>MOOD:</color>\n{targetCharacter.State.VisualState[0]}";
        _relationshipStatus.text = $"<color=#969696>RELATIONSHIP:</color>\n{TextConverter.GetRelationshipStatusText(targetCharacter)}";

    }
}
