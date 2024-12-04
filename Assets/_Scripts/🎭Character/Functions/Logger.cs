using TMPro;
using UnityEngine;

public class Logger : MonoBehaviour
{
    public string CurrentBehaviour { get; set; }
    public string CurrentAction { get; set; }
    public string CurrentStepInAction { get; set; }

    private TextMeshPro _statsLabel;

    private void Awake()
    {

        var statsLabelTransform = transform.Find("Canvas/StatsLabel");
        if (statsLabelTransform != null)
        {
            _statsLabel = statsLabelTransform.GetComponent<TextMeshPro>();
        }

        if (_statsLabel == null)
        {
            Debug.LogError("StatsLabel not found or doesn't have a TextMeshPro component.");
        }
    }

    private void Update()
    {
        UpdateStatsLabel($"CurrentBehaviour: {CurrentBehaviour} \n CurrentAction: {CurrentAction} \n CurrentStepInAction: {CurrentStepInAction}");
    }

    public void UpdateStatsLabel(string message)
    {
        if (_statsLabel != null)
        {
            _statsLabel.text = message;
        }
    }
}
