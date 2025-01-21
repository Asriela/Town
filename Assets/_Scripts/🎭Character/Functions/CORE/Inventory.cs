using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mind;
using UnityEngine.TextCore.Text;

[Serializable]
public class Inventory : MonoBehaviour
{
    [SerializeField]
    private List<ObjectTypeWorldObjectPair> _inventoryList = new();
    private Dictionary<ObjectType, List<WorldObject>> _inventoryDictionary = new();

    private Character _character;
    public void Initialize(Character character) => _character = character;

    private void Start()
    {
        PickupInitialObjectsInInventory();
    }
    private void PickupInitialObjectsInInventory()
    {
        foreach (var pair in _inventoryList)
        {
            foreach (var worldObject in pair.worldObjects)
            {
                PickUpObject(worldObject);
            }
        }
    }

    private void PickUpObject(WorldObject worldObject)
    {
        _character.Memory.AddToPossessions(worldObject.ObjectType, worldObject);
        worldObject.SetVisibility(false);
        worldObject.WhoImInPossesionOf = _character;
    }
    public Dictionary<ObjectType, List<WorldObject>> InventoryDictionary
    {
        get
        {
            if (_inventoryDictionary == null)
            {
                _inventoryDictionary = new Dictionary<ObjectType, List<WorldObject>>();
                foreach (var pair in _inventoryList)
                {
                    if (!_inventoryDictionary.ContainsKey(pair.objectType))
                    {
                        _inventoryDictionary[pair.objectType] = new List<WorldObject>();
                    }
                    _inventoryDictionary[pair.objectType].AddRange(pair.worldObjects);  // Added all worldObjects
                }
            }
            return _inventoryDictionary;
        }
        set
        {
            _inventoryList.Clear();
            foreach (var kvp in value)
            {
                _inventoryList.Add(new ObjectTypeWorldObjectPair { objectType = kvp.Key, worldObjects = kvp.Value });
            }

            _inventoryDictionary = value;
        }
    }

    public void AddToInventory(ObjectType objectType, List<WorldObject> worldObjects)
    {
        if (!_inventoryDictionary.ContainsKey(objectType))
        {
            _inventoryDictionary[objectType] = worldObjects;
        }
        else
        {
            foreach (var worldObject in worldObjects)
            {
                if (!_inventoryDictionary[objectType].Contains(worldObject))
                {
                    _inventoryDictionary[objectType].Add(worldObject);
                    worldObject.SetVisibility(false);
                }
            }
        }

        UpdateInventoryListFromDictionary();
    }
    public void AddToInventory(ObjectType objectType, WorldObject worldObject)
    {
        // Wrap the single object in a list and call the existing method
        AddToInventory(objectType, new List<WorldObject> { worldObject });
    }
    public WorldObject RemoveFromInventory(ObjectType objectType)
    {
        if (_inventoryDictionary.TryGetValue(objectType, out var worldObjects) && worldObjects.Any())
        {
            var removedObject = worldObjects[0];
            worldObjects.RemoveAt(0);

            if (!worldObjects.Any())
            {
                _inventoryDictionary.Remove(objectType);
            }

            UpdateInventoryListFromDictionary();

            return removedObject;
        }

        return null;
    }

    public WorldObject RemoveObjectFromInventory(WorldObject worldObject)
    {
        foreach (var pair in _inventoryDictionary)
        {
            if (pair.Value.Contains(worldObject))
            {
                pair.Value.Remove(worldObject);

                if (!pair.Value.Any())
                {
                    _inventoryDictionary.Remove(pair.Key);
                }

                UpdateInventoryListFromDictionary();
                return worldObject;
            }
        }

        return null;
    }

    private void UpdateInventoryListFromDictionary()
    {
        _inventoryList.Clear();
        foreach (var kvp in _inventoryDictionary)
        {
            _inventoryList.Add(new ObjectTypeWorldObjectPair { objectType = kvp.Key, worldObjects = kvp.Value });
        }
    }

    public WorldObject GetInventory(ObjectType objectType)
    {
        if (_inventoryDictionary.TryGetValue(objectType, out var objects) && objects.Any())
        {
            return objects.First();
        }

        return null;
    }
}
