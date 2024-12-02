using System;
using System.Collections.Generic;
using UnityEngine;

public class Trait : ScriptableObject
{
    public Mind.TraitType Type { get; set; }
    private List<Behavior> _behaviors = new();

    public List<Behavior> Behaviors
    {
        get => _behaviors;
        set => _behaviors = value ?? new List<Behavior>();
    }
}

[Serializable]
public class Behavior
{
    public string Name { get; set; }
    public string Text { get; set; }
    public Action Action { get; set; }
    public Func<bool> Condition { get; set; }
    public int Priority { get; set; }

    public Behavior(string name, string text, Action action, Func<bool> condition, int priority)
    {
        Name = name;
        Text = text;
        Action = action;
        Condition = condition;
        Priority = priority;
    }
}
