using UnityEngine;
using UnityEngine.UIElements;

public class GameUI : MonoBehaviour
{
    private UIDocument _uiDocument;
    private Label _timeOfDaylabel;
    private void Start()
    {

        _uiDocument = GetComponent<UIDocument>();
        var root = _uiDocument.rootVisualElement;

        // Find the label by its name and set its text
        _timeOfDaylabel = root.Q<Label>("TimeOfDay");
    }

    private void Update()
    {

        if (_timeOfDaylabel != null)
        {
            _timeOfDaylabel.text = @$"Time of day: {(int)WorldManager.Instance.TimeOfDay}h Enum: {WorldManager.Instance.GetTimeOfDayAsEnum()} ";
        }

    }

}
