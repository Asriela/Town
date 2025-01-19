using System;
using UnityEngine;

public static class EventManager
{
    public static Action<Transform, Transform> OnSwitchCameraToInteractionMode;

    public static void TriggerSwitchCameraToInteractionMode(Transform t1, Transform t2)
    {
        OnSwitchCameraToInteractionMode?.Invoke(t1, t2);
    }

    public static Action OnSwitchCameraToNormalMode;

    public static void TriggerSwitchCameraToNormalMode()
    {
        OnSwitchCameraToNormalMode?.Invoke();
    }
}
