﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mind;
using UnityEngine;

[Serializable]
public class ObjectTypeWorldObjectPair
{
    public Mind.ObjectType objectType;
    public List<WorldObject> worldObjects;  // Updated to List<WorldObject>
}

[Serializable]
public class GameObjectTagsPair
{
    public GameObject gameObject;
    public List<Mind.TraitType> tags = new();
}

[Serializable]
public class LocationTagsPair
{
    public Mind.LocationName location;
    public List<Mind.KnowledgeTag> tags = new();
}

[Serializable]
public class LocationTargetsPair
{
    public Mind.TargetLocationType locationType;
    public Mind.LocationName locationName;
}
[Serializable]
public class PricingPair
{
    public Mind.ObjectType objectTYpe;
    public int price;
}
public class Memory : MonoBehaviour
{
    [SerializeField]
    private int _coin;
    public int Coin
    {
        get => _coin;
        set => _coin = value;
    }
    [SerializeField]
    private List<PricingPair> _pricing;

    public int GetPrice(Mind.ObjectType objectType)
    {
        var pricingPair = _pricing.FirstOrDefault(p => p.objectTYpe == objectType);
        return pricingPair != null ? pricingPair.price : -1;
    }
    [SerializeField]
    private List<Trait> _traits = new();


    public List<Trait> Traits => _traits;

    public void AddTrait(Trait theTrait)
    {
        if (!_traits.Contains(theTrait))
        { _traits.Add(theTrait); }
    }

    public bool HasTrait(Trait theTrait)
    {
        if (_traits.Contains(theTrait))
        { return true; }

        return false;
    }

    public Character ReachedOccupant { get; set; }
    public Dictionary<Mind.TargetType, GameObject> Targets { get; set; } = new();

    public Dictionary<Mind.TraitType, Character> OccupantTargets { get; set; } = new();

    [SerializeField]
    private List<LocationTargetsPair> _locationTargetsList = new();

    public Dictionary<Mind.TargetLocationType, Mind.LocationName> LocationTargets
    {
        get
        {
            Dictionary<Mind.TargetLocationType, Mind.LocationName> dictionary = new();
            foreach (var pair in _locationTargetsList)
            {
                dictionary[pair.locationType] = pair.locationName;
            }
            return dictionary;
        }
    }
    public void AddLocationTarget(Mind.TargetLocationType locationType, Mind.LocationName locationName)
    {
        var existingPair = _locationTargetsList.FirstOrDefault(p => p.locationType == locationType);

        if (existingPair == null)
        {
            _locationTargetsList.Add(new LocationTargetsPair
            {
                locationType = locationType,
                locationName = locationName
            });
        }
        else
        {
            existingPair.locationName = locationName;
        }
    }
    public Mind.TargetLocationType LatestLocationTargetType { get; set; }

    [SerializeField]
    private List<LocationTagsPair> _locationKnowledge = new();

    public Dictionary<Mind.LocationName, List<Mind.KnowledgeTag>> LocationKnowledge
    {
        get
        {
            Dictionary<Mind.LocationName, List<Mind.KnowledgeTag>> dictionary = new();
            foreach (var pair in _locationKnowledge)
            {
                dictionary[pair.location] = pair.tags;
            }
            return dictionary;
        }
    }

    public void AddLocation(Mind.LocationName locationName, List<Mind.KnowledgeTag> tags)
    {
        var existingLocation = _locationKnowledge.FirstOrDefault(p => p.location == locationName);

        if (existingLocation == null)
        {
            _locationKnowledge.Add(new LocationTagsPair
            {
                location = locationName,
                tags = tags
            });
        }
        else
        {
            foreach (var tag in tags)
            {
                if (!existingLocation.tags.Contains(tag))
                {
                    existingLocation.tags.Add(tag);
                }
            }
        }
    }

    public List<Mind.LocationName> GetLocationsByTag(params Mind.KnowledgeTag[] tags) =>
    _locationKnowledge
        .Where(p => tags.All(tag => p.tags.Contains(tag)))
        .Select(p => p.location)
        .ToList();

    [SerializeField]
    private List<GameObjectTagsPair> _peopleKnowledge = new();

    public Dictionary<GameObject, List<Mind.TraitType>> PeopleKnowledge
    {
        get
        {
            Dictionary<GameObject, List<Mind.TraitType>> dictionary = new();
            foreach (var pair in _peopleKnowledge)
            {
                dictionary[pair.gameObject] = pair.tags;
            }
            return dictionary;
        }
    }

    public void AddPerson(GameObject person, List<Mind.TraitType> tags)
    {
        var existingPerson = _peopleKnowledge.FirstOrDefault(p => p.gameObject == person);

        if (existingPerson == null)
        {
            _peopleKnowledge.Add(new GameObjectTagsPair
            {
                gameObject = person,
                tags = tags
            });
        }
        else
        {
            foreach (var tag in tags)
            {
                if (!existingPerson.tags.Contains(tag))
                {
                    existingPerson.tags.Add(tag);
                }
            }
        }
    }

    public List<GameObject> GetPeopleByTag(params Mind.TraitType[] tags) =>
    _peopleKnowledge
        .Where(p => tags.All(tag => p.tags.Contains(tag)))
        .Select(p => p.gameObject)
        .ToList();

    // Updated: Possessions now a Dictionary with List<WorldObject> as values
    [SerializeField]
    private List<ObjectTypeWorldObjectPair> _possessionsList = new();

    private Dictionary<Mind.ObjectType, List<WorldObject>> _possessionsDictionary = new();

    public Dictionary<Mind.ObjectType, List<WorldObject>> Possessions
    {
        get
        {
            if (_possessionsDictionary == null)
            {
                _possessionsDictionary = new Dictionary<Mind.ObjectType, List<WorldObject>>();
                foreach (var pair in _possessionsList)
                {
                    if (!_possessionsDictionary.ContainsKey(pair.objectType))
                    {
                        _possessionsDictionary[pair.objectType] = new List<WorldObject>();
                    }
                    _possessionsDictionary[pair.objectType].AddRange(pair.worldObjects);  // Added all worldObjects
                }
            }
            return _possessionsDictionary;
        }
        set
        {
            _possessionsList.Clear();
            foreach (var kvp in value)
            {
                _possessionsList.Add(new ObjectTypeWorldObjectPair { objectType = kvp.Key, worldObjects = kvp.Value });
            }

            _possessionsDictionary = value;
        }
    }

    [SerializeField]
    private List<ObjectTypeWorldObjectPair> _inventoryList = new();

    private Dictionary<Mind.ObjectType, List<WorldObject>> _inventoryDictionary = new();

    public Dictionary<Mind.ObjectType, List<WorldObject>> Inventory
    {
        get
        {
            if (_inventoryDictionary == null)
            {
                _inventoryDictionary = new Dictionary<Mind.ObjectType, List<WorldObject>>();
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

    public void AddToPossessions(Mind.ObjectType objectType, List<WorldObject> worldObjects)
    {
        if (!_possessionsDictionary.ContainsKey(objectType))
        {
            _possessionsDictionary[objectType] = new List<WorldObject>();
        }

        foreach (var worldObject in worldObjects)
        {
            if (!_possessionsDictionary[objectType].Contains(worldObject))
            {
                _possessionsDictionary[objectType].Add(worldObject);
            }
        }


        UpdatePossessionsListFromDictionary();
    }
    public WorldObject RemoveFromPossessions(Mind.ObjectType objectType)
    {
        if (_possessionsDictionary.TryGetValue(objectType, out var worldObjects) && worldObjects.Any())
        {
            var removedObject = worldObjects[0];
            worldObjects.RemoveAt(0);


            if (!worldObjects.Any())
            {
                _possessionsDictionary.Remove(objectType);
            }

            UpdatePossessionsListFromDictionary();
            return removedObject;
        }


        return null;
    }
    public WorldObject RemoveObjectFromPossessions(WorldObject worldObject)
    {

        foreach (var pair in _possessionsDictionary)
        {
            if (pair.Value.Contains(worldObject))
            {
                pair.Value.Remove(worldObject);


                if (!pair.Value.Any())
                {
                    _possessionsDictionary.Remove(pair.Key);
                }


                UpdatePossessionsListFromDictionary();
                return worldObject;
            }
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

    public WorldObject RemoveFromInventory(Mind.ObjectType objectType)
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

    private void UpdatePossessionsListFromDictionary()
    {
        _possessionsList.Clear();
        foreach (var kvp in _possessionsDictionary)
        {
            _possessionsList.Add(new ObjectTypeWorldObjectPair { objectType = kvp.Key, worldObjects = kvp.Value });
        }
    }
    private void UpdatePossessionsDictionaryFromList()
    {
        // Populate the dictionary from the list if it's empty
        _possessionsDictionary.Clear();
        foreach (var pair in _possessionsList)
        {
            if (!_possessionsDictionary.ContainsKey(pair.objectType))
            {
                _possessionsDictionary[pair.objectType] = new List<WorldObject>();
            }
            _possessionsDictionary[pair.objectType].AddRange(pair.worldObjects);
        }
    }

    private void OnValidate()
    {

        UpdatePossessionsDictionaryFromList();
    }


    public void AddToInventory(Mind.ObjectType objectType, List<WorldObject> worldObjects)
    {
        if (!_inventoryDictionary.ContainsKey(objectType))
        {
            _inventoryDictionary[objectType] = new List<WorldObject>();
        }

        foreach (var worldObject in worldObjects)
        {
            if (!_inventoryDictionary[objectType].Contains(worldObject))
            {
                _inventoryDictionary[objectType].Add(worldObject);
                worldObject.SetVisibility(false);
            }
        }


        UpdateInventoryListFromDictionary();
    }


    private void UpdateInventoryListFromDictionary()
    {
        _inventoryList.Clear();
        foreach (var kvp in _inventoryDictionary)
        {
            _inventoryList.Add(new ObjectTypeWorldObjectPair { objectType = kvp.Key, worldObjects = kvp.Value });
        }
    }

    public void AddToPossessions(Mind.ObjectType objectType, WorldObject worldObject)
    {
        AddToPossessions(objectType, new List<WorldObject> { worldObject });
    }
    public void AddToInventory(Mind.ObjectType objectType, WorldObject worldObject)
    {
        AddToInventory(objectType, new List<WorldObject> { worldObject });
    }
    public WorldObject GetInventory(Mind.ObjectType objectType)
    {
        if (_inventoryDictionary.TryGetValue(objectType, out var objects) && objects.Any())
        {
            return objects.First();

        }
        else
        {
            Debug.Log("No object found in possessions.");
        }
        return null;
    }

    public WorldObject GetPossession(Mind.ObjectType objectType)
    {
        if (_possessionsDictionary.TryGetValue(objectType, out var objects) && objects.Any())
        {
            return objects.First();

        }
        else
        {
            Debug.Log("No object found in possessions.");
        }
        return null;
    }
    public List<WorldObject> GetPossessions(Mind.ObjectType objectType)
    {
        if (_possessionsDictionary.TryGetValue(objectType, out var objects) && objects.Any())
        {
            return objects;

        }
        else
        {
            Debug.Log("No object found in possessions.");
        }
        return null;
    }
}
