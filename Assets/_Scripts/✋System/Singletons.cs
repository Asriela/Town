using UnityEngine;

public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // Attempt to find an existing instance
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    Debug.LogError($"No instance of {typeof(T)} found in the scene.");
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    protected virtual void OnApplicationQuit()
    {
        _instance = null;
    }
}

public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
{
    [SerializeField] private bool _persistent = false;

    protected override void Awake()
    {
        base.Awake();

        if (_persistent && Instance == this)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
