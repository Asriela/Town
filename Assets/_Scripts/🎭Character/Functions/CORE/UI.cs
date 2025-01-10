using Mind;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    public string CurrentBehaviour { get; set; }
    public string CurrentAction { get; set; }
    public string CurrentStepInAction { get; set; }

    public string CharactersInSight { get; set; }


    private TextMeshProUGUI _actionLabel;
    private TextMeshProUGUI _statsLabel;
    private TextMeshProUGUI _speechBubbleLabel;
    private GameObject _speechBubble;
    private string _lastSpokenMessage = "";
    private float _speechBubbleLifeLeft = 0;
    [SerializeField]
    private Transform _statsLabelTransform;
    [SerializeField]
    private Transform _actionLabelTransform;
    [SerializeField]
    private Transform _statsPanelTransform;
    [SerializeField]
    private Transform _actionPanellTransform;
    [SerializeField]
    private Transform _speechPanel;
    [SerializeField]
    private Transform _speechText;
    [SerializeField]
    private Character _character;
    public void Initialize(Character character)
    {
        _character = character;
        if (_character is not Player && Settings.Instance.AllNPCsShowLogUI == false)
        {
            // _statsLabel.gameObject.SetActive(false);
            // _statsPanelTransform.gameObject.SetActive(false);
        }

    }

    private void Awake()
    {
        Transform speechBubbleTransform = _speechPanel;

        if (speechBubbleTransform != null)
        {
            _speechBubble = speechBubbleTransform.gameObject;
        }

        var speechBubbleLabeTransform = _speechText;
        if (speechBubbleLabeTransform != null)
        {
            _speechBubbleLabel = speechBubbleLabeTransform.GetComponent<TextMeshProUGUI>();
        }



        if (_statsLabelTransform != null)
        {
            _statsLabel = _statsLabelTransform.GetComponent<TextMeshProUGUI>();
        }



        if (_actionLabelTransform != null)
        {
            _actionLabel = _actionLabelTransform.GetComponent<TextMeshProUGUI>();
        }


    }

    private void Update()
    {
        UpdateStatsLabel($"BEHAVIOUR:\n {CurrentBehaviour}\n-----------\nACTION:\n {CurrentAction}\n-----------\nSTEP:\n {CurrentStepInAction}\n-----------\nSEES:\n {CharactersInSight}-----------\nCOIN:\n {_character.Vitality.Needs[NeedType.sleep]}");
        if (_character is Player)
        { UpdateActionLabel($"{_character.State.CurrentState}"); }
        else
        { UpdateActionLabel($"{CurrentAction}"); }

        if (_speechBubbleLifeLeft > 0)
        {
            _speechBubbleLifeLeft -= 0.01f;
        }
        else
        if (_statsLabel != null)//&& _statsLabel.text == _lastSpokenMessage)
        {
            EndSpeech();
        }
    }
    public void UpdateStatsLabel(string message)
    {
        if (_statsLabel != null)
        {
            _statsLabel.text = message;
        }
    }

    private void UpdateActionLabel(string message)
    {
        if (_actionLabel != null)
        {
            _actionLabel.text = message;
        }
    }
    public void Speak(string message)
    {
        _speechBubble.SetActive(true);
        _speechBubbleLabel.text = message;
        _lastSpokenMessage = message;
        _speechBubbleLifeLeft = 5f;
    }

    public void EndSpeech()
    {
        _speechBubble.SetActive(false);
    }
}
