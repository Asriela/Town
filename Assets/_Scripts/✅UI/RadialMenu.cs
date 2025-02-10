using UnityEngine;
using System;
using System.Collections.Generic;

using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;


public class RadialMenu : MonoBehaviour
{
    public event Action<string> OnButtonClicked;

    public VisualTreeAsset _interactionMenuTemplate;
    public VisualElement _root;
    private VisualElement _menuContainer;
    private VisualElement backgroundImage;
    private List<VisualElement> _buttons = new();
    private Character _targetCharacter = null;
    private const float _radius = 100;
    private Vector2 _hoverScale = new Vector2(1.2f, 1.2f);
    private const float _animationDuration = 0.2f;
    private UIDocument _uiDocument;


    private void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        if (_interactionMenuTemplate != null)
        {
            VisualElement interactionMenu = _interactionMenuTemplate.CloneTree();
            _root.Add(interactionMenu);
            // Get UI Document Root
            _uiDocument = GetComponent<UIDocument>();



            _menuContainer = interactionMenu.Q<VisualElement>("MenuContainer");
            _menuContainer.style.position = Position.Absolute;
            _menuContainer.style.width = 200;
            _menuContainer.style.height = 200;
            _menuContainer.style.left = new Length((1600 + 100) / 2 - 150, LengthUnit.Pixel); // Move right by 240px
            _menuContainer.style.top = new Length((900 + 200) / 2 - 500, LengthUnit.Pixel);
            _menuContainer.style.position = Position.Absolute;

            backgroundImage = new VisualElement();
            backgroundImage.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("Sprites/UI/blackSquare"));
            backgroundImage.style.position = Position.Absolute;
            backgroundImage.style.left = new Length(0, LengthUnit.Pixel);
            backgroundImage.style.top = new Length(0, LengthUnit.Pixel);
            backgroundImage.style.width = new Length(20, LengthUnit.Pixel);
            backgroundImage.style.height = new Length(20, LengthUnit.Pixel);
            backgroundImage.style.opacity = 0.99f; // Optional: Slight transparency
            _menuContainer.Add(backgroundImage);

        }
    }

    private void Update()
    {
        if (_targetCharacter != null)
        {

            Vector2 targetScreenPos = RuntimePanelUtils.CameraTransformWorldToPanel(_uiDocument.rootVisualElement.panel, _targetCharacter.transform.position, Camera.main);

            targetScreenPos.y -= 180;
            targetScreenPos.x -= 100;
            Vector2 menuPosition = _root.WorldToLocal(targetScreenPos);
            _menuContainer.style.left = 0f;// menuPosition.x;
            _menuContainer.style.top = 0f;// menuPosition.y;
            _menuContainer.transform.position = targetScreenPos;
        }

    }

    public void OpenMenu(List<string> labels, Character targetCharacter)
    {
        _menuContainer.style.display = DisplayStyle.Flex;
        _targetCharacter = targetCharacter;
        _menuContainer.Clear();
        _buttons.Clear();


        GameManager.Instance.BlockingPlayerUIOnScreen2 = true;


        // Create buttons and position them radially
        for (int i = 0; i < labels.Count; i++)
        {
            var button = CreateButton(labels[i]);
            _buttons.Add(button);
            _menuContainer.Add(button);
        }


        LayoutButtons();
        //AnimateButtonsIn();
    }

    public void HideMenu()
    {

        GameManager.Instance.BlockingPlayerUIOnScreen2 = false;
        _menuContainer.style.display = DisplayStyle.None;
        _targetCharacter=null;
        BasicFunctions.Log("🎯 CLOSE menu", LogType.radialMenu);
    }

    private VisualElement CreateButton(string label)
    {
        var button = new Button { };
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
                color = Color.black,  // Default color, will be overridden by rich text
                unityTextAlign = TextAnchor.MiddleCenter,
                fontSize = 14,
                whiteSpace = WhiteSpace.Normal,  // Allow text wrapping within the label
                overflow = Overflow.Visible,

                unityFontStyleAndWeight = FontStyle.Bold,
            }
        };
        button.Add(optionLabel);

        button.RegisterCallback<MouseEnterEvent>(evt => button.style.scale = new Scale(_hoverScale));
        button.RegisterCallback<MouseLeaveEvent>(evt => button.style.scale = new Scale(new Vector2(1f, 1f)));
        button.clicked += () => OnButtonClicked?.Invoke(label);


        return button;
    }


    private void LayoutButtons()
    {
        int count = _buttons.Count;



        for (int i = 0; i < count; i++)
        {
            float angle = ( GetPresetAngle(i, count)) * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * _radius;
            float y = Mathf.Sin(angle) * _radius;
            _buttons[i].style.left = 100f + x - 40f;
            _buttons[i].style.top = 100f + y - 20f;
        }
    }


    private float GetPresetAngle(int index, int count)
    {
        var upAngle = 180;//
        var leftAngle = 0;//co
        var rightAngle = 270;//t
        var downAngle = 90;//ch
        int ret=90;
        if (count == 1)
        {
            ret= upAngle;
        }
        if (count == 2)
        {
            ret=(index ==0 ? leftAngle : rightAngle);
        }
        if (count == 3)
        {
            switch (index)
            {
                case 0: ret = upAngle; break;
                case 1:
                    ret= leftAngle;
                    break;
                case 2:
                    ret = rightAngle;
                    break;
            }
        }
        if (count == 4)
        {
            switch (index)
            {
                case 0:
                    ret = upAngle;
                    break;
                case 1:
                    return leftAngle;
                    break;
                case 2:
                    ret = rightAngle;
                    break;
                case 3:
                    ret = downAngle;
                    break;
            }
        }
        return ret;
    }


    private void AnimateButtonsIn()
    {
        for (int i = 0; i < _buttons.Count; i++)
        {
            var button = _buttons[i];
            button.style.opacity = 1f;


            float targetX = button.style.left.value.value;
            float targetY = button.style.top.value.value;


            button.style.left = 100f;
            button.style.top = 100f;


            /* button.experimental.animation.Start(new Vector2(100f, 100f), new Vector2(targetX, targetY), (int)_animationDuration, (v) =>
             {
                 button.style.left = v.x;
                 button.style.top = v.y;
             });*/
        }
    }
}



