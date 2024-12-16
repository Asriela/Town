using UnityEngine;
using UnityEngine.AI;


public class Player : Character
{
    [SerializeField] private GameObject _radialMenuPrefab; // Radial menu prefab
    [SerializeField] private Color _seenColor = Color.red;  // Color when the player is seen
    [SerializeField] private Color _defaultColor = Color.blue;
    private SpriteRenderer _spriteRenderer;
    private bool _isSeen = false;
    private bool _lastSeen = false;
    private GameObject _currentRadialMenu;

    private void Start()
    {

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

            // Check if we clicked on an enemy (with a 2D collider)
            if (hit.collider != null && hit.collider.CompareTag("NPC"))
            {
                ShowRadialMenu(hit.collider.gameObject, hit.point);
            }
            // Check if we clicked on the ground (no collider hit, or just background)
            else
            {
                MoveToLocation(hit.point);
                HideRadialMenu();
            }
        }
    }

    private void MoveToLocation(Vector3 targetPosition) => Movement.MoveTo(targetPosition);

    private void ShowRadialMenu(GameObject enemy, Vector3 position)
    {
        // Destroy the existing radial menu if there is one
        if (_currentRadialMenu != null)
        {
            Destroy(_currentRadialMenu);
        }

        // Instantiate a new radial menu at the click position
        _currentRadialMenu = Instantiate(_radialMenuPrefab, position, Quaternion.identity);

        // Optionally: Pass the enemy to the radial menu for context-based actions
        RadialMenu menu = _currentRadialMenu.GetComponent<RadialMenu>();
        if (menu != null)
        {
            menu.SetTarget(enemy);
        }
    }

    private void HideRadialMenu()
    {
        if (_currentRadialMenu != null)
        {
            Destroy(_currentRadialMenu);
            _currentRadialMenu = null;
        }
    }

    public void SetSeenState(bool isSeen)
    {
        _lastSeen = _isSeen;
        _isSeen = isSeen;

        // Change the color based on whether the player is seen

    }

}
