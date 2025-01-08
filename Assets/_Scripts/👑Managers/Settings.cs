using UnityEngine;

public class Settings : Singleton<Settings>
{
    [SerializeField] private float _timeOfDaySpeed = 0.001f;
    public float TimeOfDaySpeed
    {
        get => _timeOfDaySpeed;
        set => _timeOfDaySpeed = value;
    }
}


