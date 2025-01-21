using System.Collections.Generic;
using Mind;
using UnityEngine;

[CreateAssetMenu(fileName = "New WorldObjectData", menuName = "WorldObject Data", order = 1)]
public class WorldObjectData : ScriptableObject
{
    public float size;
    public float integrity;
    public float maintenance;
    public List<ObjectTrait> objectTraits = new List<ObjectTrait>();
    public List<WorldObjectFeature> features = new List<WorldObjectFeature>();
    public ObjectType objectType;
}

[System.Serializable]
public class WorldObjectFeature
{
    public ObjectFeatureType featureType;
    [SerializeField] private bool _enabled = true;  // Default to true

    public bool enabled
    {
        get => _enabled;
        set => _enabled = value;
    }
}

