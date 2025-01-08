using Mind;
using UnityEngine;

public enum StateType
{
    normal,
    sleeping,
    inCombat,
    inConversation,
    dead
}
public class State : MonoBehaviour
{

    private readonly Alarms _alarm = new();


    [SerializeField]
    private StateType _currentState;

    public StateType CurrentState => _currentState;

    private Character _character;
    public void Initialize(Character character) => _character = character;


    private void Update()
    {
        _alarm.Run();
        RunState();
        ResetStateToNormal();
    }


    public void SetState(StateType stateType)
    {

        _currentState = stateType;
        _alarm.Start(TimerType.setNewState, 5, false, 0);

        switch (stateType)
        {
            case StateType.normal:
                _character.Appearance.State = AppearanceState.standing;
                break;
            case StateType.sleeping:
                _character.Appearance.State = AppearanceState.lyingDown;
                break;
            case StateType.dead:
                _character.Appearance.State = AppearanceState.dead;
                break;
        }

    }

    private void ResetStateToNormal()
    {
        if (_character is not Player && _alarm.Ended(TimerType.setNewState))
        {
            SetState(StateType.normal);
        }
    }


    private void RunState()
    {
        switch (_currentState)
        {
            case StateType.sleeping:
                _character.Vitality.Needs[NeedType.sleep] -= 0.1f;
                break;
        }

    }
    //
    //alarm.Ended(TimerType.wander, 5, false, 0);
}
