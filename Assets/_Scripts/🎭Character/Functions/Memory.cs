using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectTypeGameObjectPair
{
    public Mind.ObjectType objectType; // Key
    public GameObject gameObject; // Value
}
[Serializable]
public class TraitTypeGameObjectListPair
{
    public Mind.TraitType traitType;
    public List<GameObject> gameObjects;
}

public class Memory : MonoBehaviour
{
    public Mind.LocationName CurrentLocation { get; set; }

    public Dictionary<Mind.TargetType, GameObject> Targets { get; set; } = new Dictionary<Mind.TargetType, GameObject>();

    [SerializeField]
    private List<TraitTypeGameObjectListPair> _peopleKnowledge = new List<TraitTypeGameObjectListPair>();

    public Dictionary<Mind.TraitType, List<GameObject>> PeopleKnowledge
    {
        get
        {
            Dictionary<Mind.TraitType, List<GameObject>> dictionary = new Dictionary<Mind.TraitType, List<GameObject>>();
            foreach (var pair in _peopleKnowledge)
            {
                dictionary[pair.traitType] = pair.gameObjects;
            }
            return dictionary;
        }
    }

    [SerializeField]
    private List<ObjectTypeGameObjectPair> _possessions = new List<ObjectTypeGameObjectPair>();

    public Dictionary<Mind.ObjectType, GameObject> Possessions
    {
        get
        {
            Dictionary<Mind.ObjectType, GameObject> dictionary = new Dictionary<Mind.ObjectType, GameObject>();
            foreach (var pair in _possessions)
            {
                dictionary[pair.objectType] = pair.gameObject;
            }
            return dictionary;
        }
    }
}


