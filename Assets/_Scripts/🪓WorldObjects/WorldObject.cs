
using Mind;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectFeatureType
{
    UseFeature,
    UpkeepFeature,
    InteractionFeature,
    GrowFeature,
    RentFeature,
    EffectsFeature
}

public class WorldObject : MonoBehaviour
{
    [SerializeField]
    private WorldObjectData worldObjectData; // Reference to ScriptableObject
    public WorldObjectData WorldObjectData => worldObjectData;

    protected List<ObjectTrait> objectTraits = new List<ObjectTrait>();
    public List<ObjectTrait> ObjectTraits => objectTraits;

    private UseFeature _useFeature;
    private UpkeepFeature _upkeepFeature;
    private GrowFeature _growFeature;
    private InteractFeature _interactFeature;
    private RentFeature _rentFeature;
    private EffectsFeature _effectsFeature;
    private ObjectActions _objectActions; // The new ObjectActions
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        if (worldObjectData != null)
        {
            // Initialize properties from ScriptableObject
            InitializeFromData(worldObjectData);
        }

        _objectActions = new ObjectActions(this); // Initialize ObjectActions
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        _spriteRenderer.sortingOrder = Mathf.RoundToInt((-transform.position.y) * 100);
    }

    private void InitializeFromData(WorldObjectData data)
    {
        // Set size, integrity, and maintenance from ScriptableObject
        Size = data.size;
        Integrity = data.integrity;
        Maintenance = data.maintenance;

        // Set object traits from ScriptableObject
        objectTraits.AddRange(data.objectTraits);

        // Add features dynamically based on ScriptableObject
        foreach (var feature in data.features)
        {
            if (feature.enabled)
            {
                switch (feature.featureType)
                {
                    case ObjectFeatureType.UseFeature:
                        _useFeature = gameObject.AddComponent<UseFeature>();
                        _useFeature.Setup(this); // Setup with WorldObject reference
                        break;
                    case ObjectFeatureType.UpkeepFeature:
                        _upkeepFeature = gameObject.AddComponent<UpkeepFeature>();
                        _upkeepFeature.Setup(this);
                        break;
                    case ObjectFeatureType.InteractionFeature:
                        _interactFeature = gameObject.AddComponent<InteractFeature>();
                        _interactFeature.Setup(this);
                        break;
                    case ObjectFeatureType.GrowFeature:
                        _growFeature = gameObject.AddComponent<GrowFeature>();
                        _growFeature.Setup(this);
                        break;
                    case ObjectFeatureType.RentFeature:
                        _rentFeature = gameObject.AddComponent<RentFeature>();
                        _rentFeature.Setup(this);
                        break;
                    case ObjectFeatureType.EffectsFeature:
                        _effectsFeature = gameObject.AddComponent<EffectsFeature>();
                        _effectsFeature.Setup(this);
                        break;
                }
            }
        }
    }

    // New Methods to Access the Actions
    public ObjectActions ObjectActions => _objectActions;

    public float Integrity { get; set; }
    public float Size { get; set; }
    public float Maintenance { get; set; }

    // Feature-related methods...
    public bool HasFeature<T>() where T : class
    {
        return GetComponent<T>() != null;
    }

    public T GetFeature<T>() where T : class
    {
        return GetComponent<T>();
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetVisibility(bool isVisible)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = isVisible;
        }
    }
    protected Character whoIAmInPossesionOf;

    public Character WhoImInPossesionOf
    {
        get => whoIAmInPossesionOf;
        set => whoIAmInPossesionOf = value;
    }
    // Now access ObjectType from WorldObjectData
    public ObjectType ObjectType => worldObjectData?.objectType ?? ObjectType.none; // Default to None if not set
}
