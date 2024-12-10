using System;
using System.Collections.Generic;
using System.Linq;
using Mind;
using Unity.VisualScripting;
using UnityEngine;
[Serializable]
public class LocationTypeGameObjectPair
{
    public Mind.LocationName locationType; // Key
    public GameObject gameObject; // Value
}


[Serializable]
public class EnumCharacterPair
{
    public Mind.CharacterName characterName; // Key
    public Character character; // Value
}

public class WorldManager : Singleton<WorldManager>
{


    [SerializeField]
    private List<LocationTypeGameObjectPair> _locations = new List<LocationTypeGameObjectPair>();

    public Dictionary<Mind.LocationName, GameObject> Locations
    {
        get
        {
            Dictionary<Mind.LocationName, GameObject> dictionary = new Dictionary<Mind.LocationName, GameObject>();
            foreach (var pair in _locations)
            {
                dictionary[pair.locationType] = pair.gameObject;
            }
            return dictionary;
        }
    }

    public float TimeOfDay = 0f;
    private void Update()
    {
        RunTimeOfDay();

    }

    private void RunTimeOfDay()
    {
        TimeOfDay = TimeOfDay < 24 ? TimeOfDay + Settings.timeOfDaySpeed : 0;
    }

    public TimeOfDayType GetTimeOfDayAsEnum()
    {
        int hour = (int)(TimeOfDay % 24);


        if (hour >= 0 && hour < 12)
        {

            return (TimeOfDayType)(hour - 1);
        }
        else
        {

            return (TimeOfDayType)(hour - 11);
        }

    }
    [SerializeField]
    private List<EnumCharacterPair> _allCharacters = new();

    public Dictionary<Mind.CharacterName, Character> AllCharacters
    {
        get
        {
            Dictionary<Mind.CharacterName, Character> dictionary = new();
            foreach (var pair in _allCharacters)
            {
                dictionary[pair.characterName] = pair.character;
            }
            return dictionary;
        }
    } 




}
