using System.Collections.Generic;
using Mind;
using UnityEngine;
using UnityEngine.UIElements;


public class PlayerRadialMenuInteraction : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private RadialMenu _radialMenu;


    private bool _leftClick = false;
    private bool _rightClick = false;
    private bool _isMouseOverUI = false;
    private bool _justOpenedPieMenu = false;
    private Player _player;
    public void Initialize(Player player)
    {
        _player = player;
    }
    private Character _personWeAreInteractingWith;

    private void Start()
    {


        var root = _radialMenu._root;
        root.RegisterCallback<MouseEnterEvent>(e => OnMouseOverUI(true));
        root.RegisterCallback<MouseLeaveEvent>(e => OnMouseOverUI(false));
    }

    private void Update()
    {
        _leftClick = false;
        _leftClick = Input.GetMouseButtonDown(0);
        _rightClick = false;
        _rightClick = Input.GetMouseButtonDown(1);
        TryOpenRadialMenu();

    }

    private void OnMouseOverUI(bool isOver)
    {
        _isMouseOverUI = isOver;

    }
    public bool NotInteractingWithMenu()
    {

        var ret=true;

        if (_justOpenedPieMenu)
        { _justOpenedPieMenu = false; return false; }

        if (_isMouseOverUI)
            return false;

        if (!_leftClick && _personWeAreInteractingWith != null)
            ret= false;
        if (GameManager.Instance.BlockingPlayerUIOnScreen2)
        {
            CloseInteractionMenu();
            ret= false;
        }

        return ret;






    }
    public void CloseInteractionMenu()
    {
        _justOpenedPieMenu=false;
        _personWeAreInteractingWith = null;
        _radialMenu.HideMenu();
    }


    private void TryOpenRadialMenu()
    {
        if (!_leftClick)//|| GameManager.Instance.BlockingPlayerUIOnScreen)
            return;


        // Raycast from mouse position
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Get the layer mask for a specific layer (e.g., "MyLayerName")
        LayerMask layerMask = LayerMask.GetMask("characters");

        // Perform the raycast with the layer mask
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, layerMask);



        if (hit.collider != null)
        {
            _justOpenedPieMenu=true;
            Character character = hit.collider.GetComponent<Character>();
            if (character != null)
            {
                _personWeAreInteractingWith = character;
                OpenPrimaryMenu();
            }
        }
    }


    private void OpenPrimaryMenu()
    {
        _justOpenedPieMenu = true;

        List<string> options = new() { "Charm", "Coerce", "Talk" };
        _radialMenu.OnButtonClicked += HandlePrimarySelection;
        _radialMenu.OnButtonClicked -= HandleCoerceSelection;
        _radialMenu.OnButtonClicked -= HandleCharmSelection;
        _radialMenu.OpenMenu(options, _personWeAreInteractingWith);
    }


    private void HandlePrimarySelection(string selectedOption)
    {
        _radialMenu.OnButtonClicked -= HandlePrimarySelection; // Unsubscribe


        switch (selectedOption)
        {
            case "Charm":
                OpenCharmMenu();
                break;
            case "Coerce":
                OpenCoerceMenu();
                break;
            case "Talk":
                Debug.Log("Opening dialogue interaction...");
                break;
        }
    }


    private void OpenCharmMenu()
    {
        List<string> options = new() { "Buy Drink", "Hangout", "Hug", "Dance" };
        _radialMenu.OnButtonClicked += HandleCharmSelection;
        _radialMenu.OpenMenu(options, _personWeAreInteractingWith);
    }


    private void HandleCharmSelection(string selectedOption)
    {
        _radialMenu.OnButtonClicked -= HandleCharmSelection; // Unsubscribe
        _player.RadialActionsHelper.PerformCharmAction(_personWeAreInteractingWith, selectedOption);
    }


    private void OpenCoerceMenu()
    {
        List<string> options = new() { "Threaten", "Blackmail" };


        /*   if (_actionsHelper.HasBlackmailOn(_personWeAreInteractingWith))
           {
               options.Add("Blackmail");
           }
        */

        _radialMenu.OnButtonClicked += HandleCoerceSelection;
        _radialMenu.OpenMenu(options, _personWeAreInteractingWith);
    }


    private void HandleCoerceSelection(string selectedOption)
    {
        _radialMenu.OnButtonClicked -= HandleCoerceSelection; // Unsubscribe
        _player.RadialActionsHelper.PerformCoerceAction(_personWeAreInteractingWith, selectedOption);
    }
}
