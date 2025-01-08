using UnityEngine;

public class Settings : Singleton<Settings>
{
    [SerializeField] private bool _allNPCsShowLogUI = false;
    public bool AllNPCsShowLogUI
    {
        get => _allNPCsShowLogUI;
        set => _allNPCsShowLogUI = value;
    }
}


