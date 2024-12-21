using System;
using System.Collections.Generic;
using Mind;
using UnityEngine;
[Serializable]
public class LocationTypeGameObjectPair
{
    public LocationName locationType; // Key
    public Location gameObject; // Value
}


[Serializable]
public class EnumCharacterPair
{
    public CharacterName characterName; // Key
    public Character character; // Value
}

public class WorldManager : Singleton<WorldManager>
{


    [SerializeField]
    private List<LocationTypeGameObjectPair> _locations = new();

    public Dictionary<LocationName, Location> Locations
    {
        get
        {
            Dictionary<LocationName, Location> dictionary = new();
            foreach (var pair in _locations)
            {
                dictionary[pair.locationType] = pair.gameObject;
            }
            return dictionary;
        }
    }

    public Location GetLocation(LocationName locationName) =>Locations[locationName];
    

    [SerializeField]
    private float _timeOfDay;

    [SerializeField]
    private float _startingTime;

    public float TimeOfDay
    {
        get => _timeOfDay;
        set => _timeOfDay = value;
    }
    private void Update() => RunTimeOfDay();

    private void Start()
    {
        _timeOfDay = _startingTime;
    }

    private void RunTimeOfDay() => TimeOfDay = TimeOfDay < 24 ? TimeOfDay + Settings.timeOfDaySpeed : 0;

    public TimeOfDayType GetTimeOfDayAsEnum()
    {
        int hour = (int)(TimeOfDay % 24);

        return hour switch
        {

            >= 0 and < 12 =>
                 (TimeOfDayType)(hour - 1),
            _ => (TimeOfDayType)(hour - 1)

        };
    }
    [SerializeField]
    private List<EnumCharacterPair> _allCharacters = new();

    public Dictionary<CharacterName, Character> AllCharacters
    {
        get
        {
            Dictionary<CharacterName, Character> dictionary = new();
            foreach (var pair in _allCharacters)
            {
                dictionary[pair.characterName] = pair.character;
            }
            return dictionary;
        }
    }




}
