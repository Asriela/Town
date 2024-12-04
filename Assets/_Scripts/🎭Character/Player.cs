using UnityEngine;
using UnityEngine.AI;

public class Player : Character
{
    [SerializeField] private GameObject radialMenuPrefab; // Radial menu prefab
    [SerializeField] private Color seenColor = Color.red;  // Color when the player is seen
    [SerializeField] private Color defaultColor = Color.blue;
    private SpriteRenderer _spriteRenderer;
    private bool _isSeen = false;
    private GameObject currentRadialMenu;

    protected override void Start()
    {
        base.Start();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // Initialize with the default color
        _spriteRenderer.color = defaultColor;
    }
        private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
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

    private void MoveToLocation(Vector3 targetPosition)
    {
        MoveTo(targetPosition);
    }

    private void ShowRadialMenu(GameObject enemy, Vector3 position)
    {
        // Destroy the existing radial menu if there is one
        if (currentRadialMenu != null)
        {
            Destroy(currentRadialMenu);
        }

        // Instantiate a new radial menu at the click position
        currentRadialMenu = Instantiate(radialMenuPrefab, position, Quaternion.identity);

        // Optionally: Pass the enemy to the radial menu for context-based actions
        RadialMenu menu = currentRadialMenu.GetComponent<RadialMenu>();
        if (menu != null)
        {
            menu.SetTarget(enemy);
        }
    }

    private void HideRadialMenu()
    {
        if (currentRadialMenu != null)
        {
            Destroy(currentRadialMenu);
            currentRadialMenu = null;
        }
    }

    public void SetSeenState(bool isSeen)
    {
        _isSeen = isSeen;

        // Change the color based on whether the player is seen
        if (_isSeen)
        {
            _spriteRenderer.color = seenColor;  // Change color to 'seen' color
        }
        else
        {
            _spriteRenderer.color = defaultColor;  // Revert to default color
        }
    }

}
