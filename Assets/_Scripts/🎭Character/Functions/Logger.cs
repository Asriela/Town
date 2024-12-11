using TMPro;
using UnityEngine;

public class Logger : MonoBehaviour
{
    public string CurrentBehaviour { get; set; }
    public string CurrentAction { get; set; }
    public string CurrentStepInAction { get; set; }

    public string CharactersInSight { get; set; }

    private TextMeshProUGUI _statsLabel;
    [SerializeField]
    private Transform _statsLabelTransform;

    private void Awake()
    {

        _statsLabelTransform = transform.Find("Canvas/StatsLabel");
        if (_statsLabelTransform != null)
        {
            _statsLabel = _statsLabelTransform.GetComponent<TextMeshProUGUI>();
        }

        if (_statsLabel == null)
        {
            Debug.LogError("StatsLabel not found or doesn't have a TextMeshPro component.");
        }
    }

    private void Update()
    {
        UpdateStatsLabel($"BEHAVIOUR:\n {CurrentBehaviour}\n-----------\nACTION:\n {CurrentAction}\n-----------\nSTEP:\n {CurrentStepInAction}\n-----------\nSEES:\n {CharactersInSight}");
    }

    public void UpdateStatsLabel(string message)
    {
        if (_statsLabel != null)
        {
            _statsLabel.text = message;
        }
    }
}
