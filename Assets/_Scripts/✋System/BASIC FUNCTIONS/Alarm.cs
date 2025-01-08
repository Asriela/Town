using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Alarm
{
    public float TimeLength { get; set; }
    public bool Triggered { get; set; }
    public bool Loops { get; set; }

    public bool Paused { get; set; }
    public float EndTime { get; set; }
    public int Count { get; set; }

    public int Ended { get; set; }


    public Alarm(float TimeLength, bool Triggered, bool Loops, float EndTime)
    {
        this.TimeLength = TimeLength;
        this.Triggered = Triggered;
        this.Loops = Loops;
        this.EndTime = EndTime;
        this.Paused = false;
        Ended = 0;
        Count = 0;
    }
}

public class Alarms
{
    private static float globalTimer = 0;

    Dictionary<TimerType, Alarm> alarmsList = new Dictionary<TimerType, Alarm>();

    public bool initialized;
    public bool HasBeenSetup { get; set; } = false;

    public float Left(TimerType timerType)
    {
        float timeLength = alarmsList[timerType].EndTime - globalTimer;
        return timeLength;
    }

    public float Total(TimerType timerType)
    {
        float timeLength = alarmsList[timerType].EndTime;
        return timeLength;
    }

    public float Global()
    {
        float timeLength = globalTimer;
        return timeLength;
    }
    public void Start(TimerType timerType, float timeLength, bool loop, float firstLength)
    {

        if (alarmsList.ContainsKey(timerType))
        {
  
            alarmsList[timerType].Triggered = false;
            alarmsList[timerType].TimeLength = timeLength;
            if (loop)
            {

                alarmsList[timerType].EndTime = 1;
            }
            else
            { alarmsList[timerType].EndTime = globalTimer + timeLength; }
            alarmsList[timerType].Loops = loop;
        }
        else
        {
  
            var endTime = 0f;
            var triggered = false;
            if (loop)
            {
                endTime = globalTimer + 2;
                triggered = true;
            }
            else
            {
                endTime = globalTimer + timeLength;
            }


            alarmsList.Add(timerType, new Alarm(timeLength, triggered, loop, endTime));
        }
        BasicFunctions.Log("Created new Alarm: " + timerType + $": {alarmsList[timerType].EndTime}", LogType.alarms);
    }
    public void Run()
    {
        var keys = alarmsList.Keys.ToArray();
        //
        foreach (var key in keys)
        {
            TimerType tempType = key;
            // BasicFunctions.Log($"NOW {key} : {alarmsList[key].EndTime}", LogType.alarms);








            if (alarmsList[key].Triggered && !alarmsList[key].Paused)
            {




                if (alarmsList[key].Loops)
                {
                    BasicFunctions.Log($"LOOPING ALARM {tempType} FINNISHED [{alarmsList[key].Count}] => next alarm at {alarmsList[key].EndTime}", LogType.alarms);
                    alarmsList[key].EndTime = globalTimer + alarmsList[key].TimeLength;
                    alarmsList[key].Count++;
                    BasicFunctions.Log($"LOOPING ALARM {tempType} end time is: {alarmsList[key].EndTime}", LogType.alarms);


                }
                else
                {
                    BasicFunctions.Log("NON LOOPING ALARM FINNISHED " + alarmsList[key].Count, LogType.alarms);
                    alarmsList[key].EndTime = -1;

                }

            }

            alarmsList[key].Triggered = (globalTimer > alarmsList[key].EndTime) && !(alarmsList[key].EndTime == -1);

        }
    }
    public void Pause(TimerType timerType)
    {
        if (alarmsList.ContainsKey(timerType))
        { alarmsList[timerType].Paused = true; }
    }

    public void UnPause(TimerType timerType)
    {
        if (alarmsList.ContainsKey(timerType))
        { alarmsList[timerType].Paused = false; }
    }
    public bool Ended(TimerType timerType)
    {
        //if (alarmsList[timerType].Triggered)

        if (alarmsList.ContainsKey(timerType))
        {
            var ret = false;


            ret = alarmsList[timerType].Triggered;
            if (ret)
            { BasicFunctions.Log($"ALARM {timerType}, ENDED CHECK IS TRUE", LogType.alarms); }

            return ret;

        }

        else
        { return false; }
    }
}
