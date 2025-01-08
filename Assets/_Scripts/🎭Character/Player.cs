using Mind;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PlayerMenuInteraction))]
public class Player : Character
{
    [SerializeField] private GameObject _radialMenuPrefab; // Radial menu prefab
    [SerializeField] private Color _seenColor = Color.red;  // Color when the player is seen
    [SerializeField] private Color _defaultColor = Color.blue;
    private SpriteRenderer _spriteRenderer;
    private bool _isSeen = false;
    private bool _lastSeen = false;
    private GameObject _currentRadialMenu;

    public PlayerMenuInteraction MenuInteraction { get; set; }

    private void Start()
    {
        MenuInteraction = GetComponent<PlayerMenuInteraction>();
        MenuInteraction.Initialize(this);
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // Initialize with the default color
        _spriteRenderer.color = _defaultColor;
    }
    private void Update()
    {

        HandleInput();
        UpdateSeenColor();
    }

    private void UpdateSeenColor()
    {
        if (_lastSeen != _isSeen)
        {
            _lastSeen = _isSeen;
            _spriteRenderer.color = _isSeen ? _seenColor : _defaultColor;
        }
    }
    private void HandleInput()
    {
        if (Vitality.Dead)
        { return; }
        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (MenuInteraction.NotInteractingWithMenu())
            {
                BaseAction.MoveTo(this, hit.point);

            }
        }
    }




    public void SetSeenState(bool isSeen)
    {
        _lastSeen = _isSeen;
        _isSeen = isSeen;

        // Change the color based on whether the player is seen

    }

}
