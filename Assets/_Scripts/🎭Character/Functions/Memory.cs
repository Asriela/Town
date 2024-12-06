using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectTypeGameObjectPair
{
    public Mind.ObjectType objectType; // Key
    public GameObject gameObject; // Value
}

public class Memory : MonoBehaviour
{

    public Dictionary<Mind.TargetType, GameObject> Targets { get; set; } = new Dictionary<Mind.TargetType, GameObject>();



    [SerializeField]
    private List<ObjectTypeGameObjectPair> _possessions = new List<ObjectTypeGameObjectPair>();

    // Property to access the dictionary
    public Dictionary<Mind.ObjectType, GameObject> Possessions
    {
        get
        {
            // Convert the list to a dictionary
            Dictionary<Mind.ObjectType, GameObject> dictionary = new Dictionary<Mind.ObjectType, GameObject>();
            foreach (var pair in _possessions)
            {
                dictionary[pair.objectType] = pair.gameObject;
            }
            return dictionary;
        }
    }

}
