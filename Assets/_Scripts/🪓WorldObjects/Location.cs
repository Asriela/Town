using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Location : MonoBehaviour
{
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


[System.Serializable]
public class TraitOccupantPair
{
    public Mind.TraitType traitType;
    public List<Character> characters;
}
