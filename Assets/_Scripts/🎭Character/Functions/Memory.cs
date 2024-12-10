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

    public List<GameObject> GetPeopleByTag(params Mind.TraitType[] tags) =>
    _peopleKnowledge
    .Where(p => tags.All(tag => p.tags.Contains(tag)))
    .Select(p => p.gameObject)
    .ToList();


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


