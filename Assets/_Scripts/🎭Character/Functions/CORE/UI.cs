using Mind;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    public string CurrentBehaviour { get; set; }
    public string CurrentAction { get; set; }
    public string CurrentStepInAction { get; set; }

    public string CharactersInSight { get; set; }



    private TextMeshProUGUI _statsLabel;
    private TextMeshProUGUI _speechBubbleLabel;
    private GameObject _speechBubble;
    private string _lastSpokenMessage = "";
    private float _speechBubbleLifeLeft = 0;
    [SerializeField]
    private Transform _statsLabelTransform;

    private Character _npc;
    public void Initialize(Character npc)
    {
        _npc = npc;
    }

    private void Awake()
    {
        Transform speechBubbleTransform = transform.Find("Canvas/SpeechBubble");

        if (speechBubbleTransform != null)
        {
            _speechBubble = speechBubbleTransform.gameObject;
        }

        var speechBubbleLabeTransform = transform.Find("Canvas/SpeechBubble/Speech");
        if (speechBubbleLabeTransform != null)
        {
            _speechBubbleLabel = speechBubbleLabeTransform.GetComponent<TextMeshProUGUI>();
        }


        _statsLabelTransform = transform.Find("Canvas/Panel/StatsLabel");
        if (_statsLabelTransform != null)
        {
            _statsLabel = _statsLabelTransform.GetComponent<TextMeshProUGUI>();
        }

        if (_statsLabel == null)
        {
            //Debug.LogError("StatsLabel not found or doesn't have a TextMeshPro component.");
        }
    }

    private void Update()
    {
        UpdateStatsLabel($"BEHAVIOUR:\n {CurrentBehaviour}\n-----------\nACTION:\n {CurrentAction}\n-----------\nSTEP:\n {CurrentStepInAction}\n-----------\nSEES:\n {CharactersInSight}-----------\nCOIN:\n {_npc.Vitality.Needs[NeedType.sleep]}");
        if (_speechBubbleLifeLeft > 0)
        {
            _speechBubbleLifeLeft -= 0.01f;
        }
        else
        if(_statsLabel != null )//&& _statsLabel.text == _lastSpokenMessage)
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

    public void Speak(string message)
    {
        _speechBubble.SetActive(true);
        _speechBubbleLabel.text = message;
        _lastSpokenMessage= message;
        _speechBubbleLifeLeft = 5f;
    }

    public void EndSpeech()
    {
        _speechBubble.SetActive(false); 
    }
}
