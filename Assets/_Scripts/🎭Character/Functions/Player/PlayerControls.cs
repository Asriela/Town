using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;            // Maximum movement speed
    public float acceleration = 10f;       // How quickly the player accelerates
    public float deceleration = 15f;       // How quickly the player decelerates

    private Rigidbody2D rb;
    private Vector2 currentVelocity;
    private Vector2 targetVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // Ensure no gravity in a top-down game
    }

    void Update()
    {
        // Get movement input
        Vector2 inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        // Calculate target velocity based on input
        targetVelocity = inputDirection * moveSpeed;

        // Apply acceleration or deceleration
        if (inputDirection.magnitude > 0)
        {
            // Accelerate towards the target velocity
            currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.deltaTime);
        }
        else
        {
            // Decelerate smoothly to zero when no input
            currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, deceleration * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        // Move the player using the current velocity
        rb.velocity = currentVelocity;

        // Apply pixel-perfect snapping to the position after movement
        Vector3 snappedPosition = rb.position;
        snappedPosition.x = Mathf.Round(snappedPosition.x);
        snappedPosition.y = Mathf.Round(snappedPosition.y);

        // Apply the snapped position without affecting velocity
        rb.MovePosition(snappedPosition);
    }
}
