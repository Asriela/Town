using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform target;          // Reference to the player's transform
    public float smoothSpeed = 0.125f; // Smoothing factor for camera movement
    public Vector3 offset;            // Offset of the camera relative to the target
    public float fixedZPosition = -10f; // Fixed Z position for the camera in 2D

    private void FixedUpdate()
    {
        if (target == null)
            return;

        // Calculate the desired position with the offset
        Vector3 desiredPosition = target.position + offset;

        // Keep the Z position fixed for 2D
        desiredPosition.z = fixedZPosition;

        // Smoothly interpolate between the current position and the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Update the camera's position
        transform.position = smoothedPosition;
    }
}
