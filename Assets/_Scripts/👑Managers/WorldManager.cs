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


    public enum SpeedOfTime
{
    normal,
    fast,
    veryFast
}

public class WorldManager : Singleton<WorldManager>
{

    public event Action OnMidnight;

    [SerializeField] private float _timeOfDaySpeed = 0.001f;
    public float TimeOfDaySpeed
    {
        get => _timeOfDaySpeed;
        set => _timeOfDaySpeed = value;
    }
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

    [SerializeField]
    private Player _thePlayer;


    public float TotalHoursPassed { get; set; } = 0;
    public float TimeThatsChanged { get; set; } = 0;

    public Player ThePlayer => _thePlayer;

    private float _lastTimeOfDay = 0;

    public float TimeOfDay
    {
        get => _timeOfDay;
        set => _timeOfDay = value;
    }
    private void Update() => RunTimeOfDay();

    private void Start()
    {
        _timeOfDay = _startingTime;
        _lastTimeOfDay = _timeOfDay;
        Time.timeScale= Settings.Instance.WorldSpeed;
    }

    private void RunTimeOfDay()
    {
        TimeOfDay = TimeOfDay < 24 ? TimeOfDay + _timeOfDaySpeed* Time.timeScale : 0;
        TimeThatsChanged = TimeOfDay - _lastTimeOfDay;
        _lastTimeOfDay = TimeOfDay;
        TotalHoursPassed += TimeThatsChanged;

        RenderSettings.ambientLight = new Color(0.1f, 0.1f, 0.3f);

        if (_lastTimeOfDay > TimeOfDay)
        {
            OnMidnight?.Invoke();
        }
    }

    public void SetSpeedOfTime(SpeedOfTime speedOfTime)
    {
        switch (speedOfTime)
        {

            case SpeedOfTime.normal:
                Time.timeScale = Settings.Instance.WorldSpeed;
                break;
            case SpeedOfTime.fast:
                Time.timeScale = 15f;
                break;
            case SpeedOfTime.veryFast:
                Time.timeScale = 30.0f;
                break;
        }

    }
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
