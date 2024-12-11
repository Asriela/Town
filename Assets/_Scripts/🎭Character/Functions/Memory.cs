using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ObjectTypeWorldObjectPair
{
    public Mind.ObjectType objectType;
    public WorldObject worldObject;
}
[Serializable]
public class GameObjectTagsPair
{
    public GameObject gameObject;
    public List<Mind.TraitType> tags = new();
}

[System.Serializable]
public class LocationTagsPair
{
    public Mind.LocationName location;
    public List<Mind.KnowledgeTag> tags = new();
}

public class Memory : MonoBehaviour
{


    public Dictionary<Mind.TargetType, GameObject> Targets { get; set; } = new();

    public Dictionary<Mind.TargetLocationType, Mind.LocationName> LocationTargets { get; set; } = new();
    public Mind.TargetLocationType LatestLocationTargetType { get; set; }

    [SerializeField]
    private List<LocationTagsPair> _locationKnowledge = new();



    public Dictionary<Mind.LocationName, List<Mind.KnowledgeTag>> LocationKnowledge
    {
        get
        {
            Dictionary<Mind.LocationName, List<Mind.KnowledgeTag>> dictionary = new();
            {
                foreach (var pair in _locationKnowledge)
                {
                    dictionary[pair.location] = pair.tags;
                }
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


    [SerializeField]
    private List<ObjectTypeWorldObjectPair> _possessions = new();

    private Dictionary<Mind.ObjectType, WorldObject> _possessionsDictionary;

    public Dictionary<Mind.ObjectType, WorldObject> Possessions
    {
        get
        {
            if (_possessionsDictionary == null)
            {

                _possessionsDictionary = new Dictionary<Mind.ObjectType, WorldObject>();
                foreach (var pair in _possessions)
                {
                    _possessionsDictionary[pair.objectType] = pair.worldObject;
                }
            }
            return _possessionsDictionary;
        }
        set
        {
            _possessions.Clear();
            foreach (var kvp in value)
            {
                _possessions.Add(new ObjectTypeWorldObjectPair { objectType = kvp.Key, worldObject = kvp.Value });
            }

            _possessionsDictionary = value;
        }
    }

    //TODO: make inventory items follow player
    //TODO: maybe move inventory to a different class
    [SerializeField]
    private List<ObjectTypeWorldObjectPair> _inventory = new();

    private Dictionary<Mind.ObjectType, WorldObject> _inventoryDictionary;

    public Dictionary<Mind.ObjectType, WorldObject> Inventory
    {
        get
        {
            if (_inventoryDictionary == null)
            {
                _inventoryDictionary = new Dictionary<Mind.ObjectType, WorldObject>();
                foreach (var pair in _inventory)
                {
                    _inventoryDictionary[pair.objectType] = pair.worldObject;
                }
            }
            return _inventoryDictionary;
        }
        set
        {
            _inventory.Clear();
            foreach (var kvp in value)
            {
                _inventory.Add(new ObjectTypeWorldObjectPair { objectType = kvp.Key, worldObject = kvp.Value });
            }

            _inventoryDictionary = value;
        }
    }
}


