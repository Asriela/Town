using System.Collections.Generic;
using UnityEngine;

public class ObjectActions : MonoBehaviour
{
    private WorldObject _worldObject;

    public ObjectActions(WorldObject worldObject)
    {
        _worldObject = worldObject;
    }

    // TryRenting Action
    public bool TryRenting(Character buyer, Location location, float hours)
    {
        var rentable = _worldObject.GetFeature<IRentable>();
        if (rentable != null)
        {
            rentable.StartRenting(buyer, location, hours);
            return true; // Success
        }
        else
        {
            Debug.LogWarning("This object is not rentable.");
            return false; // Failure
        }
    }

    // Interact Action
    public bool TryInteractWithObject(Character userOfObject, ObjectInteractionType interactionType)
    {
        var interactFeature = _worldObject.GetFeature<InteractFeature>();
        if (interactFeature != null)
        {
            interactFeature.Interact(userOfObject, interactionType);
            return true; // Success
        }
        else
        {
            Debug.LogWarning("This object is not interactable.");
            return false; // Failure
        }
    }

    // Get Interaction Options
    public bool TryGetInteractionOptions(Character userOfObject, out List<InteractFeature.InteractionOption> interactionOptions)
    {
        var interactFeature = _worldObject.GetFeature<InteractFeature>();
        if (interactFeature != null)
        {
            interactionOptions = interactFeature.GetInteractionOptions(userOfObject);
            return true; // Success
        }
        else
        {
            interactionOptions = null;
            Debug.LogWarning("This object is not interactable.");
            return false; // Failure
        }
    }
}
