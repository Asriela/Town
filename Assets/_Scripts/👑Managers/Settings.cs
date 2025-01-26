using UnityEngine;

public class Settings : Singleton<Settings>
{
    [SerializeField] private bool _allNPCsShowLogUI = false;
    public bool AllNPCsShowLogUI
    {
        get => _allNPCsShowLogUI;
        set => _allNPCsShowLogUI = value;
    }

    [SerializeField] private float _interactionDistance = 0.6f;
    public float InteractionDistance
    {
        get => _interactionDistance;
        set => _interactionDistance = value;
    }

    [SerializeField] private float _playerSpeed = 7;
    public float PlayerSpeed
    {
        get => _playerSpeed;
        set => _playerSpeed = value;
    }


    [SerializeField] private float _worldSpeed = 2;
    public float WorldSpeed
    {
        get => _worldSpeed;
        set => _worldSpeed = value;
    }
}


