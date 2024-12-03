using UnityEngine;

public class RadialMenu : MonoBehaviour
{
    private GameObject target;

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
        // Customize menu options based on the target
    }

    public void Attack()
    {
        Debug.Log($"Attacking {target.name}");
        Destroy(gameObject);
    }

    public void Inspect()
    {
        Debug.Log($"Inspecting {target.name}");
        Destroy(gameObject);
    }

    public void Cancel()
    {
        Destroy(gameObject);
    }
}
