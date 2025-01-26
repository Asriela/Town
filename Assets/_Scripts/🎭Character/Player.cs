using Mind;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PlayerMenuInteraction))]
[RequireComponent(typeof(PlayerControls))]
public class Player : Character
{
    [SerializeField] private GameObject _radialMenuPrefab; // Radial menu prefab
    [SerializeField] private Color _seenColor = Color.red;  // Color when the player is seen
    [SerializeField] private Color _defaultColor = Color.blue;

    private bool _isSeen = false;
    private bool _lastSeen = false;
    private GameObject _currentRadialMenu;
    private InteractionPackage _currentInteraction = null;
    public PlayerMenuInteraction MenuInteraction { get; set; }
    public PlayerControls PlayerControls { get; set; }

    private void Start()
    {
        MenuInteraction = GetComponent<PlayerMenuInteraction>();
        MenuInteraction.Initialize(this);
        PlayerControls = GetComponent<PlayerControls>();
        PlayerControls.Initialize(this);

        // Initialize with the default color

    }
    private void Update()
    {

        HandleInput();
        UpdateSeenColor();
        DoInteraction();
    }

    private void UpdateSeenColor()
    {
        if (_lastSeen != _isSeen)
        {
            _lastSeen = _isSeen;
            Appearance.ChangeColor(_isSeen ? _seenColor : _defaultColor);
        }
    }
    private void HandleInput()
    {
        if (Vitality.Dead || GameManager.Instance.CantClickOffInteractionMenu)
        { return; }
        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (MenuInteraction.NotInteractingWithMenu())
            {
                BaseAction.MoveTo(this, hit.point);
                _currentInteraction = null;
            }
        }
    }


    public void GotoAndInteractWithObject(WorldObject theObject, ObjectInteractionType interactionType)
    {
        _currentInteraction = new InteractionPackage(theObject, interactionType);

    }

    private void DoInteraction()
    {
        if (_currentInteraction == null) return;
        if (ActionsHelper.Reached(this, _currentInteraction.objectToInteractWith.transform.position, 0.3f))
        {
            BaseAction.InteractWithObject(_currentInteraction.objectToInteractWith, this, _currentInteraction.interactionType);
            _currentInteraction = null;
        }
    }
    public void SetSeenState(bool isSeen)
    {
        _lastSeen = _isSeen;
        _isSeen = isSeen;

        // Change the color based on whether the player is seen

    }

    private class InteractionPackage
    {
        public WorldObject objectToInteractWith;
        public ObjectInteractionType interactionType;

        public InteractionPackage(WorldObject objectToInteractWith, ObjectInteractionType interactionType)
        {
            this.objectToInteractWith = objectToInteractWith;
            this.interactionType = interactionType;

        }
    }

}
