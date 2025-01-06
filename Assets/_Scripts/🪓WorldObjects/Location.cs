using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class TraitOccupantPair
{
    public Mind.TraitType traitType;
    public List<Character> characters;
}


[System.Serializable]
public class ObjectTypeObjectPair
{
    public Mind.ObjectType objectType;
    public List<WorldObject> worldObject;
}

public class Location : MonoBehaviour
{

    [SerializeField]
    private List<ObjectTypeObjectPair> _possessions = new();

    public List<ObjectTypeObjectPair> Possession => _possessions;


    public void AddPosession(Mind.ObjectType objectType, WorldObject worldObject)
    {

        var pair = _possessions.Find(p => p.objectType == objectType);

        if (pair == null)
        {

            _possessions.Add(new ObjectTypeObjectPair
            {
                objectType = objectType,
                worldObject = new List<WorldObject> { worldObject }
            });
        }
        else
        {

            if (!pair.worldObject.Contains(worldObject))
            {
                pair.worldObject.Add(worldObject);
            }
        }
    }



    public WorldObject RemoveFromPossessions(Mind.ObjectType objectType)
    {
        var pair = _possessions.FirstOrDefault(p => p.objectType == objectType);

        if (pair != null && pair.worldObject.Any())
        {
            // Remove the first worldObject from the pair's worldObjects list
            var removedObject = pair.worldObject[0];
            pair.worldObject.RemoveAt(0);

            // If the worldObjects list is empty after removal, remove the pair from _possessions
            if (!pair.worldObject.Any())
            {
                _possessions.Remove(pair);
            }

            // Return the removed worldObject
            return removedObject;
        }

        // Return null if no matching pair or object was found
        return null;
    }


    public WorldObject GetPossession(Mind.ObjectType objectType)
        => _possessions
            .FirstOrDefault(p => p.objectType == objectType)?
            .worldObject
            .FirstOrDefault();

    [SerializeField]
    private List<TraitOccupantPair> _occupants = new();

    public List<TraitOccupantPair> Occupants => _occupants;


    public void AddOccupant(Mind.TraitType traitType, Character character)
    {

        var pair = _occupants.Find(p => p.traitType == traitType);

        if (pair == null)
        {

            _occupants.Add(new TraitOccupantPair
            {
                traitType = traitType,
                characters = new List<Character> { character }
            });
        }
        else
        {

            if (!pair.characters.Contains(character))
            {
                pair.characters.Add(character);
            }
        }
    }


    public void RemoveOccupant(Mind.TraitType traitType, Character character)
    {
        var pair = _occupants.Find(p => p.traitType == traitType);

        if (pair != null)
        {
            pair.characters.Remove(character);


            if (pair.characters.Count == 0)
            {
                _occupants.Remove(pair);
            }
        }
    }


    public Character GetOccupant(Mind.TraitType traitType)
        => _occupants
            .FirstOrDefault(p => p.traitType == traitType)?
            .characters
            .FirstOrDefault();

}


