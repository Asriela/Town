using UnityEngine;

public class State : MonoBehaviour
{

    private readonly Alarms _alarm = new();
    private Character _character;
    public void Initialize(Character character) => _character = character;
    private void Update()
    {
        _alarm.Run();
    }

    //alarm.Start(TimerType.wander, 5, false, 0);
    //alarm.Ended(TimerType.wander, 5, false, 0);
}
