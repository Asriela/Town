using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;


public class RadialMenu : MonoBehaviour
{
    public event Action<string> OnButtonClicked;


    public VisualTreeAsset _interactionMenuTemplate;
    public VisualElement _root;
    private VisualElement _menuContainer;
    private VisualElement _tooltipBox;
    private Label _tooltipText;
    private List<VisualElement> _buttons = new();
    private Character _targetCharacter = null;
    private const float _radius = 100;
    private Vector2 _hoverScale = new Vector2(1.2f, 1.2f);
    private UIDocument _uiDocument;


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


            // Tooltip Box Setup
            _tooltipBox = new VisualElement();
            _tooltipBox.style.backgroundColor = new Color(0, 0, 0, 0.8f);
            _tooltipBox.style.position = Position.Absolute;
            _tooltipBox.style.width = 300;
            _tooltipBox.style.height = 150; // Allow it to resize properly
            _tooltipBox.style.paddingLeft = 20;
            _tooltipBox.style.paddingRight = 20;
            _tooltipBox.style.paddingTop = 10;
            _tooltipBox.style.paddingBottom = 10;

            _tooltipBox.style.visibility = Visibility.Hidden;

            _tooltipBox.style.left = _menuContainer.resolvedStyle.left + 220;
            _tooltipBox.style.top = _menuContainer.resolvedStyle.top;



            _tooltipText = new Label
            {
                style =
                {
                    color = Color.white,
                    fontSize = 14,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    whiteSpace = WhiteSpace.Normal,
                unityFontStyleAndWeight = FontStyle.Bold,
                alignSelf =Align.Center,
                flexGrow = 1,
                flexDirection=FlexDirection.Column,
                justifyContent=Justify.Center
                }
            };
            _tooltipBox.Add(_tooltipText);
            _root.Add(_tooltipBox);
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



        }
    }


    public void OpenMenu(List<string> labels, List<string> tooltips, List<bool> isDisabled, Character targetCharacter)
    {
        if (labels.Count != tooltips.Count || labels.Count != isDisabled.Count)
        {
            Debug.LogError("Labels, tooltips, and isDisabled lists must have the same length.");
            return;
        }


        _menuContainer.style.display = DisplayStyle.Flex;
        _targetCharacter = targetCharacter;
        _menuContainer.Clear();
        _buttons.Clear();
        GameManager.Instance.BlockingPlayerUIOnScreen2 = true;


        for (int i = 0; i < labels.Count; i++)
        {
            var button = CreateButton(labels[i], tooltips[i], isDisabled[i]);
            _buttons.Add(button);
            _menuContainer.Add(button);
        }


        LayoutButtons();
    }


    public void HideMenu()
    {
        GameManager.Instance.BlockingPlayerUIOnScreen2 = false;
        _menuContainer.style.display = DisplayStyle.None;
        _tooltipBox.style.display = DisplayStyle.None;
        _targetCharacter = null;
    }


    private VisualElement CreateButton(string label, string tooltip, bool isDisabled)
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


        Label optionLabel = new Label(label)
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


        if (isDisabled)
        {
            button.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 1); // Greyed-out look
            button.RegisterCallback<MouseEnterEvent>(evt => ShowTooltip(tooltip, button));
            button.RegisterCallback<MouseLeaveEvent>(evt => HideTooltip());
            button.SetEnabled(false);
        }
        else
        {
            button.style.backgroundColor = Color.white;
            button.RegisterCallback<MouseEnterEvent>(evt => EnterButton(tooltip, button));
            button.RegisterCallback<MouseLeaveEvent>(evt => LeaveButton(button));
            button.clicked += () => OnButtonClicked?.Invoke(label);
        }


        return button;
    }

    void EnterButton(string tooltip, VisualElement button)
    {
        button.style.scale = new Scale(_hoverScale);
        ShowTooltip(tooltip, button);
    }

    void LeaveButton(VisualElement button)
    {
        button.style.scale = new Scale(new Vector2(1f, 1f));
        HideTooltip();
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
        return (count > 4 || index >= count) ? 90 : angles[index];
    }


    private void ShowTooltip(string tooltip, VisualElement button)
    {
        if (string.IsNullOrEmpty(tooltip))
            return;

        _tooltipBox.style.visibility = Visibility.Visible;
        _tooltipText.text = tooltip;
        _tooltipBox.style.display = DisplayStyle.Flex;


        Vector2 menuPos = _menuContainer.worldBound.position;

        _tooltipBox.style.left = menuPos.x+300;
        _tooltipBox.style.top = menuPos.y+30;
    }


    private void HideTooltip()
    {
        _tooltipBox.style.display = DisplayStyle.None;
    }
}
