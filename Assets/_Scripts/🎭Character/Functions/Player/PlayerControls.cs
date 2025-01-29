using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2.5f;            // Maximum movement speed
    public float acceleration = 10f;       // How quickly the player accelerates
    public float deceleration = 40f;       // How quickly the player decelerates

    private Rigidbody2D rb;
    private Vector2 currentVelocity;
    private Vector2 targetVelocity;

    private Player _player;
    public void Initialize(Player player)
    {
        _player = player;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // Ensure no gravity in a top-down game
    }

    void Update()
    {
     
            PlayerKeyboardMovement();
        




    }

    void PlayerKeyboardMovement()
    {
        // Get movement input
        Vector2 inputDirection = Vector2.zero;
        if (!GameManager.Instance.BlockingPlayerUIOnScreen)
        {
             inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        }


        // Calculate target velocity based on input
        targetVelocity = inputDirection * moveSpeed;
        targetVelocity = Vector2.ClampMagnitude(targetVelocity, Settings.Instance.PlayerSpeed);
        // Apply acceleration or deceleration
        if (inputDirection.magnitude > 0)
        {
            _player.Movement.IsMoving=true;
            _player.Movement.Agent.ResetPath();
            // Accelerate towards the target velocity
            currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.deltaTime);
        }
        else
        {
            _player.Movement.IsMoving = false;
            // Decelerate smoothly to zero when no input
            currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, deceleration * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        // Move the player using the current velocity

            rb.linearVelocity = currentVelocity;

        // Pixel-perfect snapping after applying velocity
        PixelSnap();
    }

    private void PixelSnap()
    {
        // Get the current position and snap to the nearest pixel
        Vector3 snappedPosition = transform.position;
        snappedPosition.x = Mathf.Round(transform.position.x * 100f) / 100f;
        snappedPosition.y = Mathf.Round(transform.position.y * 100f) / 100f;

        // Apply the snapped position without modifying velocity
        transform.position = snappedPosition;
    }
}
