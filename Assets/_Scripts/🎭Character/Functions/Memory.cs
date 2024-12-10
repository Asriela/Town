using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ObjectTypeGameObjectPair
{
    public Mind.ObjectType objectType;
    public GameObject gameObject;
}
[Serializable]
public class TraitTypeGameObjectListPair
{
    public Mind.TraitType traitType;
    public List<GameObject> gameObjects;
}

[System.Serializable]
public class LocationTagsPair
{
    public Mind.LocationName location;
    public List<Mind.KnowledgeTag> tags = new();
}

public class Memory : MonoBehaviour
{
    public Mind.LocationName CurrentLocation { get; set; }

    public Dictionary<Mind.TargetType, GameObject> Targets { get; set; } = new();

    public Dictionary<Mind.TargetLocationType, GameObject> LocationTargets { get; set; } = new();

    [SerializeField]
    private List<LocationTagsPair> _locationKnowledge = new();

    public List<Mind.LocationName> GetLocationsByTag(params Mind.KnowledgeTag[] tags)
    {
        return _locationKnowledge
            .Where(p => tags.All(tag => p.tags.Contains(tag)))
            .Select(p => p.location)
            .ToList();
    }

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


    [SerializeField]
    private List<TraitTypeGameObjectListPair> _peopleKnowledge = new();

    public Dictionary<Mind.TraitType, List<GameObject>> PeopleKnowledge
    {
        get
        {
            Dictionary<Mind.TraitType, List<GameObject>> dictionary = new();
            foreach (var pair in _peopleKnowledge)
            {
                dictionary[pair.traitType] = pair.gameObjects;
            }
            return dictionary;
        }
    }

    [SerializeField]
    private List<ObjectTypeGameObjectPair> _possessions = new();

    public Dictionary<Mind.ObjectType, GameObject> Possessions
    {
        get
        {
            Dictionary<Mind.ObjectType, GameObject> dictionary = new();
            foreach (var pair in _possessions)
            {
                dictionary[pair.objectType] = pair.gameObject;
            }
            return dictionary;
        }
    }
}


