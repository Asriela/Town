using System.Collections.Generic;
using System.Linq;
using Mind;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;
using static WorldObject;

public struct MenuOption
{

    public string ButtonLabel;
    public object Data;
    public object Data2;

    public MenuOption(string buttonLabel, object data, object data2)
    {

        ButtonLabel = buttonLabel;
        Data = data;
        Data2 = data2;
    }
}

public class PlayerMenuInteraction : MonoBehaviour
{
    public enum SocialMenuState
    {
        start,
        memories,
        buy,
        sell,
        objectInteraction

    }

    private InteractionMenu _interactionMenu;

    private Character _character;
    private Character _personWeAreSpeakingTo;
    private bool _leftClick = false;
    private bool _justOpenedPieMenu = false;
    private Vector2 _screenPosition;

    public SocialMenuState MenuState { get; set; }
    private List<MenuOption> _currentMenuOptions = new List<MenuOption> { new MenuOption("NULL", null,null) };


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
        return new MenuOption(label, null, null);
    }

    private MenuOption Option(string label, object data, object data2)
    {
        return new MenuOption(label, data, data2);
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
            case "Buy":
                MenuState = SocialMenuState.buy;
                var pricedItems = _personWeAreSpeakingTo.Memory.GetPricedItems();
                _currentMenuOptions = pricedItems.Select(item =>
                {
                    string labelText = item.ToString() == "bed"
                        ? "Room to Rent"
                        : item.ToString();

                    // Get the price using the method and append it to the label
                    decimal price = _personWeAreSpeakingTo.Memory.GetPrice(item);
                    string pureLabelText = labelText;
                    return Option($"{labelText}: {price:C}", pureLabelText, item);
                }).ToList();
                break;
            case "Action":

                _currentMenuOptions = BasicActions.Select(action => Option(action.ToString(), null,null)).ToList();
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




        if (_currentMenuOptions == null)
        { _interactionMenu.HideMenu();}
        else
        {_interactionMenu.ShowMenu(_currentMenuOptions);}
    }

    private List<MenuOption> ProcessComplexMenuOptions(int positionInList)
    {
        var chosenOption = _currentMenuOptions[positionInList ];
        List<MenuOption> newOptions = null;

        switch (MenuState)
        {
            case SocialMenuState.memories:
                Speak(chosenOption.Data);
                break;
            case SocialMenuState.buy:


               if (chosenOption.Data == "Room to Rent")
                    _character.Acting.RentItem((ObjectType)chosenOption.Data2,  _personWeAreSpeakingTo);

                //
                break;
            case SocialMenuState.objectInteraction:


                if (chosenOption.Data == "Room to Rent")
                    _character.Acting.RentItem((ObjectType)chosenOption.Data2, _personWeAreSpeakingTo);
                var option = (InteractionOption)chosenOption.Data;
                //option.Execute(_character);
                //
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
   
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            
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
            else
            {
                // If no character is clicked, check for a WorldObject
                WorldObject clickedWorldObject = hit.collider.GetComponent<WorldObject>();
                if (clickedWorldObject != null)
                {
                    Debug.Log("HIT on WorldObject");

                    // Set up interactions for the world object
                    _justOpenedPieMenu = true;
                    _screenPosition = Camera.main.WorldToScreenPoint(hit.point);


                    List<InteractionOption> worldObjectInteractionOptions = clickedWorldObject.GetInteractionOptions(_character);
                    if (!worldObjectInteractionOptions.Any())
                    {
                        return;
                    }

                    // Convert InteractionOptions to MenuOption (assuming InteractionOption has a 'Label' and 'Action' method)
                        List<MenuOption> menuOptions = worldObjectInteractionOptions.Select(option =>
                        new MenuOption
                        {
                            ButtonLabel = option.Label,
                            Data = option
                        }).ToList();


                    MenuState = SocialMenuState.objectInteraction;  // Assuming you have a separate menu state for WorldObjects
                    _interactionMenu.ShowMenu(menuOptions);
                }
            }
        
    }





    }

    public bool NotInteractingWithMenu()
    {


        if (!_leftClick && _personWeAreSpeakingTo!=null)
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
