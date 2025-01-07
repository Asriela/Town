
using UnityEngine;
using UnityEngine.Rendering.Universal; // Ensure this is used for Light2D components

public class GlobalLightController : MonoBehaviour
{

    private Light2D globalLight;




    public Color dayColor = new Color(1f, 1f, 1f); // White light for the daytime
    public Color nightColor = new Color(0.2f, 0.2f, 0.5f); // Blue-ish light for night

    private float _transitionSpeed=1f;

    void Start()
    {
        // Get the Light2D component from the GameObject
        globalLight = GetComponent<Light2D>();

        if (globalLight == null)
        {
            Debug.LogError("Light2D component not found on this GameObject.");
            return;
        }
    }

    void Update()
    {
        if (globalLight != null)
        {

            UpdateLightColorBasedOnTime(WorldManager.Instance.TimeOfDay);
        }
    }


    void UpdateLightColorBasedOnTime(float time)
    {


        if (time >= 6f && time <= 18f) 
        {
            globalLight.color = Color.Lerp(globalLight.color, dayColor, Time.deltaTime * _transitionSpeed);
        }
        else 
        {
            globalLight.color = Color.Lerp(globalLight.color, nightColor, Time.deltaTime * _transitionSpeed);
        }
    }
}

