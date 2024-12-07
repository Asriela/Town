using Mind;
using Unity.VisualScripting;
using UnityEngine;

public class WorldManager : Singleton<WorldManager>
{
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

            return (TimeOfDayType)(hour + 1);
        }
        else
        {

            return (TimeOfDayType)(hour - 11);
        }

    }



}
