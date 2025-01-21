using System;
using System.Collections.Generic;
using UnityEngine;
public enum ObjectInteractionType : short
{
    use,
    careFor
}
public class InteractFeature : MonoBehaviour, IInteractable
{
    private WorldObject _worldObject;
    private readonly List<InteractionOption> _interactionOptions = new();

    public void Setup(WorldObject worldObject)
    {
        _worldObject = worldObject;
    }

    public void Interact(Character character, ObjectInteractionType interactionType)
    {
        // Check the interaction type and execute based on the available features
        switch (interactionType)
        {
            case ObjectInteractionType.use:
                // Check if the object has the UseFeature
                var useFeature = _worldObject.GetFeature<UseFeature>();
                if (useFeature != null)
                {
                    useFeature.Use(character);
                }
                break;

            case ObjectInteractionType.careFor:
                // Check if the object has the UpkeepFeature
                var upkeepFeature = _worldObject.GetFeature<UpkeepFeature>();
                if (upkeepFeature != null)
                {
                    upkeepFeature.Maintain(character);
                }
                break;

            default:
                throw new NotSupportedException("Interaction type not supported");
        }
    }

    // Get a list of available interaction options based on the features present on the object
    public List<InteractionOption> GetInteractionOptions(Character character)
    {
        var world = _worldObject;
        _interactionOptions.Clear();

        // Check for features and add relevant interaction options
        if (_worldObject.HasFeature<UseFeature>())
        {
            _interactionOptions.Add(new InteractionOption("Use", ObjectInteractionType.use));
        }

        if (_worldObject.HasFeature<UpkeepFeature>())
        {
            _interactionOptions.Add(new InteractionOption("Maintain", ObjectInteractionType.careFor));
        }

        return _interactionOptions;
    }

    // InteractionOption class to define the available interactions
    public class InteractionOption
    {
        public string Label { get; }
        public ObjectInteractionType InteractionAction { get; }

        public InteractionOption(string label, ObjectInteractionType interactionAction)
        {
            Label = label;
            InteractionAction = interactionAction;
        }
    }
}
