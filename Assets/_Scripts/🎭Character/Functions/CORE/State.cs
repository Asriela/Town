﻿using Mind;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
    private StateType _actionState;

    [SerializeField]
    private List<MemoryTags> _visualState = new();

    [SerializeField]
    private List<MemoryTags> _formstate = new();

    private bool resetTime = false;

    public StateType ActionState { get=>_actionState; set=> _actionState=value;}

    public List<MemoryTags> VisualState => _visualState;

    public List<MemoryTags> FormState => _formstate;

    private Character _character;
    public void Initialize(Character character) => _character = character;


    private void Update()
    {
        _alarm.Run();
        RunState();
        if(_actionState!= StateType.dead)
        ResetStateToNormal();
    }


    public void SetState(StateType stateType)
    {

        _actionState = stateType;
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

    public void SetVisualState(params MemoryTags[] memTags)
    {
        _visualState.Clear();
        _visualState.AddRange(memTags);

    }

    public void SetFormState(params MemoryTags[] memTags)
    {
        _formstate.Clear();
        _formstate.AddRange(memTags);

    }
    public bool IsForm(Mind.MemoryTags memoryTag, Character personWhoIsLookingAtForm)
    {
        var thsiPerson = _character;
        foreach (var tag in _formstate)
        {
            if (tag == memoryTag || personWhoIsLookingAtForm.Generalizations.IsTagA(tag, memoryTag))
            {
                return true;
            }
        }
        return false;
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

        if (_character == WorldManager.Instance.ThePlayer && resetTime)
        {
            resetTime = false;
            WorldManager.Instance.SetSpeedOfTime(SpeedOfTime.normal);
        }
        switch (_actionState)
        {
            case StateType.sleeping:
                if (_character == WorldManager.Instance.ThePlayer)
                {
                    resetTime = true;
                   WorldManager.Instance.SetSpeedOfTime(SpeedOfTime.veryFast);
                    
                }
                _character.Vitality.Needs[NeedType.sleep] -= 0.1f;
                break;
        }

    }
    //
    //alarm.Ended(TimerType.wander, 5, false, 0);
}
