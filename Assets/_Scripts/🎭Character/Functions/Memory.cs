using System.Collections.Generic;
using UnityEngine;

public class Memory : MonoBehaviour
{

    public Dictionary<Mind.TargetType, GameObject> Targets { get; set; } = new Dictionary<Mind.TargetType, GameObject>();
}
