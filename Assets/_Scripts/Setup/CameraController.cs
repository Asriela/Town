using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform target;          // Reference to the player's transform
    public float smoothSpeed = 0.125f; // Smoothing factor for camera movement
    public Vector3 offset;            // Offset of the camera relative to the target
    public float fixedZPosition = -10f; // Fixed Z position for the camera in 2D

    private enum CameraMode
    {
        NormalMode,
        InteractionMode
    }

    private CameraMode currentMode = CameraMode.NormalMode;

    private Transform interactionTransform1;
    private Transform interactionTransform2;

    private void OnEnable()
    {
        // Subscribe to the event
        EventManager.OnSwitchCameraToInteractionMode += SwitchToInteractionMode;
        EventManager.OnSwitchCameraToNormalMode += SwitchToNormalMode;


    }

    private void OnDisable()
    {
        // Unsubscribe from the event
        EventManager.OnSwitchCameraToInteractionMode -= SwitchToInteractionMode;
        EventManager.OnSwitchCameraToNormalMode -= SwitchToNormalMode;
    }

    private void FixedUpdate()
    {
        if (currentMode == CameraMode.NormalMode)
        {
            FollowTarget();
        }
        else if (currentMode == CameraMode.InteractionMode)
        {
            PositionForInteraction();
        }
    }

    private void FollowTarget()
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

    private void PositionForInteraction()
    {
        if (interactionTransform1 == null || interactionTransform2 == null)
            return;

        // Calculate the midpoint between the two interaction transforms
        Vector3 midpoint = (interactionTransform1.position + interactionTransform2.position) / 2;

        // Adjust Y position to match interactionTransform2
        midpoint.y = interactionTransform2.position.y;

        // Offset the camera to the left to leave space for the interaction menu
        Vector3 interactionOffset = new Vector3(5f, 0f, fixedZPosition);

        // Smoothly move the camera to the desired interaction position
        Vector3 desiredPosition = midpoint + interactionOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    }

    private void SwitchToInteractionMode(Transform t1, Transform t2)
    {
        // Set the interaction transforms
        interactionTransform1 = t1;
        interactionTransform2 = t2;

        // Switch the mode
        currentMode = CameraMode.InteractionMode;
    }

    public void SwitchToNormalMode()
    {
        // Reset the mode to normal
        currentMode = CameraMode.NormalMode;
    }
}
