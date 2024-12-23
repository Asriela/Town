using System.Collections.Generic;
using System.Linq;
using Mind;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

public struct MenuOption
{

    public string ButtonLabel;
    public object Data;

    public MenuOption(string buttonLabel, object data)
    {

        ButtonLabel = buttonLabel;
        Data = data;
    }
}

public class MenuInteraction : MonoBehaviour
{
    public enum SocialMenuState
    {
        start,
        memories

    }

    private InteractionMenu _interactionMenu;

    private Character _character;
    private Character _personWeAreSpeakingTo;
    private bool _leftClick = false;
    private bool _justOpenedPieMenu = false;
    private Vector2 _screenPosition;

    public SocialMenuState MenuState;
    private List<MenuOption> _currentMenuOptions = new List<MenuOption> { new MenuOption("NULL", null) };


    public static List<BasicAction> BasicActions = new List<BasicAction> { BasicAction.hug, BasicAction.kiss, BasicAction.kill };

    public void Initialize(Character character)
    {
        _character = character;
    }

    private void Start()
    {



        _interactionMenu = FindFirstObjectByType<InteractionMenu>();
        if (_interactionMenu == null)
        {
            Debug.LogError("🎈PieMenu component not found in the scene.");
        }
        else
        {
            Debug.Log("🎈PieMenu FOUND.");
        }
        _interactionMenu.OnButtonClicked += HandleButtonClicked;
         

        var root = _interactionMenu.root;

        // Register mouse events on the root visual element
        root.RegisterCallback<MouseEnterEvent>(e => OnMouseOverUI(true));
        root.RegisterCallback<MouseLeaveEvent>(e => OnMouseOverUI(false));
    }

    private void Update()
    {
        InteractionMenuInteraction();

        _leftClick = false;
        _leftClick = Input.GetMouseButtonDown(0);
        //ClickedOnUI=false;


    }
    private bool isMouseOverUI = false;

    private void OnMouseOverUI(bool isOver)
    {
        isMouseOverUI = isOver;
        print($"over: {isMouseOverUI}");
    }
    private MenuOption Label(string label)
    {
        return new MenuOption(label, null);
    }

    private MenuOption Option(string label, object data)
    {
        return new MenuOption(label, data);
    }



    private void HandleButtonClicked(int positionInList, string buttonLabel)
    {





        switch (buttonLabel)
        {
            case "Trade":

                _currentMenuOptions = new List<MenuOption>
                {
                    Label("Buy"),
                    Label("Sell"),
                    Label("Give"),
                    Label("Request")
                };
                break;

            case "Action":

                _currentMenuOptions = BasicActions.Select(action => Option(action.ToString(), null)).ToList();
                break;
            case "Social":

                _currentMenuOptions = new List<MenuOption>
                {
                    Label("Ask"),
                    Label("Tell"),
                    Label("Hang out")
                };
                break;



            default:
                _currentMenuOptions = ProcessComplexMenuOptions(positionInList);
                break;
        }






        _interactionMenu.ShowMenu(_currentMenuOptions);
    }

    private List<MenuOption> ProcessComplexMenuOptions(int positionInList)
    {
        var chosenOption = _currentMenuOptions[positionInList - 1];
        List<MenuOption> newOptions = null;

        switch (MenuState)
        {
            case SocialMenuState.memories:
                Speak(chosenOption.Data);
                break;
        }


        return newOptions;
    }
    private void Speak(object messageData)
    {

    }


    private void InteractionMenuInteraction()
    {

   
        if (!_leftClick || GameManager.Instance.BlockingPlayerUIOnScreen)
            return;
        Debug.Log("✅ STAGE 1");
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            Debug.Log("⚡ STAGE 2");
            _personWeAreSpeakingTo = hit.collider.GetComponent<Character>();
            if (_personWeAreSpeakingTo != null && _personWeAreSpeakingTo != _character)
            {

                _character.Movement.Stop();
                Debug.Log("HIT");
                // CameraController.Instance.EnterDialogueMode(_personWeAreSpeakingTo.transform);

                _personWeAreSpeakingTo.Reactions.PersonWeAreSpeakingTo = _character;
                _justOpenedPieMenu = true;
                _screenPosition = Camera.main.WorldToScreenPoint(hit.point);


                Vector2 clickPosition = Input.mousePosition;


                List<MenuOption> buttonLabels = new List<MenuOption>
                {
                    Label("Trade"),
                    Label("Action"),
                    Label("Social")


                };

                MenuState = SocialMenuState.start;
                _interactionMenu.ShowMenu(buttonLabels);
            }
        }





    }

    public bool NotInteractingWithMenu()
    {


        if (!_leftClick && !_personWeAreSpeakingTo==null)
            return false;
        if (_justOpenedPieMenu)
        { _justOpenedPieMenu = false; return false; }
        if (isMouseOverUI)
            return false;


        if (GameManager.Instance.BlockingPlayerUIOnScreen)
        {
            _interactionMenu.HideMenu();
            return false;
        }

        return true;






    }
}
