using System;
using System.Collections.Generic;
using System.Linq;
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

public class Memory : MonoBehaviour
{
    public Dictionary<Mind.TargetType, GameObject> Targets { get; set; } = new();

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

    private Dictionary<Mind.ObjectType, List<WorldObject>> _possessionsDictionary;

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

    private Dictionary<Mind.ObjectType, List<WorldObject>> _inventoryDictionary;

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
}
