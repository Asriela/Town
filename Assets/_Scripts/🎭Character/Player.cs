using Mind;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PlayerMenuInteraction))]
[RequireComponent(typeof(PlayerRadialMenuInteraction))]
[RequireComponent(typeof(RadialMenuActionsHelper))]
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
    public PlayerRadialMenuInteraction RadialMenuInteraction { get; set; }
    public RadialMenuActionsHelper RadialActionsHelper { get; set; }
    public PlayerControls PlayerControls { get; set; }

    private void Start()
    {
        MenuInteraction = GetComponent<PlayerMenuInteraction>();
        MenuInteraction.Initialize(this);
        RadialMenuInteraction = GetComponent<PlayerRadialMenuInteraction>();
        RadialMenuInteraction.Initialize(this);
        PlayerControls = GetComponent<PlayerControls>();
        PlayerControls.Initialize(this);
        RadialActionsHelper = GetComponent<RadialMenuActionsHelper>();
        RadialActionsHelper.Initialize(this);
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
        if (Vitality.Dead || GameManager.Instance.CantClickOffInteractionMenu || GameManager.Instance.GetPlayersCurrentSocialAction()!=SocializeType.none)
        { return; }
        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            // Raycast from mouse position
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Get the layer mask for a specific layer (e.g., "MyLayerName")
            LayerMask layerMask = LayerMask.GetMask("characters");

            // Perform the raycast with the layer mask
            RaycastHit2D hit2 = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, layerMask);



    

            if (hit2.collider == null && MenuInteraction.NotInteractingWithMenu() && RadialMenuInteraction.NotInteractingWithMenu())
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
