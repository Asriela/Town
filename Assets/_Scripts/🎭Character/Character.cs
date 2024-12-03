using Mind;
using System.Collections.Generic;
using UnityEngine;

public class Character : Movement
{
    private Dictionary<NeedType, float> _needs = new();

    public Dictionary<NeedType, float> Needs
    {
        get => _needs;
        set => _needs = value;
    }

    private Dictionary<TargetType, GameObject> _targets = new();

    public Dictionary<TargetType, GameObject> Targets
    {
        get => _targets;
        set => _targets = value;
    }

}
