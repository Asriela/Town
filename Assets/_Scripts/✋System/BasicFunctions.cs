
using System.Collections.Generic;

using System.Linq;

using UnityEngine;


public enum TimerType
{
    initialAction,
    physicsStep,
    performance,
    stateMachine,
    spawnInterval,
    spawnInterval2,
    actionLength,
    sideActionLength,
    FindTarget,
    life,
    moveOn,
    stateMachineLoop,
    wander,
    grumpy,
    setNewState
}
public enum LogType : short
{
    alarms,
    nearest,
    action,
    state,
    timedFunction,
    game,
    error,
    file,
    ui,
    weird,
    player,
    step,
    test,
    basic,
    memory,
    thinking
}

public static class BasicFunctions
{
    #region ALARM


    public static float globalTimer = 0;

    #endregion

    #region LOG


    private static Dictionary<LogType, bool> LogTypesOn = new Dictionary<LogType, bool>()
    {
        { LogType.alarms,           BinaryBool(1)},
        { LogType.test,           BinaryBool(1)},
        { LogType.nearest,          BinaryBool(0)},
        { LogType.action,           BinaryBool(0)},
        { LogType.timedFunction,    BinaryBool(0)},
        { LogType.state,            BinaryBool(0)},
        { LogType.game,             BinaryBool(0)},
        { LogType.file,             BinaryBool(0)},
        { LogType.error,            BinaryBool(1)},
        { LogType.ui,               BinaryBool(0)},
        { LogType.player,           BinaryBool(0)},
        { LogType.step,             BinaryBool(0)},
        { LogType.weird,            BinaryBool(0)},
        { LogType.memory,           BinaryBool(0)},
        { LogType.basic,             BinaryBool(0)},
        { LogType.thinking,            BinaryBool(0)}
    };


    public static void Log(string text, LogType type)
    {
        if (type == LogType.error || type == LogType.ui)
        {
            text = "ERROR! " + text;
            if (LogTypesOn[type] == true)
            { Debug.LogError(text);}
        }
        else
        if (type == LogType.step)
        {
            text = "🐾 " + text;
            if (LogTypesOn[type] == true)
            {Debug.Log(text);}
        }
        else
        if (type == LogType.weird)
        {
            text = "🎃 " + text;
            if (LogTypesOn[type] == true)
            { Debug.Log(text);}
        }
        else
        if (LogTypesOn[type] == true)
        {Debug.Log(text);}
    }

    #endregion

    #region TRICKS

    public static float ClampMin(float clampValue, float inputValue)
    {
        if (inputValue < clampValue)
            inputValue = clampValue;
        return inputValue;
    }
    public static bool BinaryBool(int binary)
        => binary != 0;
    
    #endregion
}

