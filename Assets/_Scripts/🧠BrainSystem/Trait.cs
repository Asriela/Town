using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Trait", menuName = "ScriptableObjects/NewTrait")]
public class Trait : ScriptableObject
{
    public Mind.TraitType Type;


    public List<Behavior> Behaviors = new();
}

[Serializable]
public class Behavior
{
    [SerializeField] private string _name;
    [SerializeField] private string _dialogue;
    [SerializeField] private Mind.ActionType _action;
    [SerializeReference] private object _actionParameter; // Use [SerializeReference] for polymorphic serialization
    [SerializeField] private List<Condition> _conditions = new();
    [SerializeField] private int _priority;

    public string Name => _name;
    public string Dialogue => _dialogue;
    public Mind.ActionType Action => _action;
    public object ActionParameter => _actionParameter;
    public List<Condition> Conditions => _conditions;
    public int Priority => _priority;

    public Behavior(string name, string dialogue, Mind.ActionType action, List<Condition> conditions, int priority)
    {
        _name = name;
        _dialogue = dialogue;
        _action = action;
        _conditions = conditions;
        _priority = priority;
    }
}

[Serializable]
public struct Condition
{
    public Mind.ConditionType conditionType;
    [SerializeReference] public object parameter; // Use a polymorphic approach for the parameter

    public Condition(Mind.ConditionType conditionType, object parameter)
    {
        this.conditionType = conditionType;
        this.parameter = parameter;
    }
}
