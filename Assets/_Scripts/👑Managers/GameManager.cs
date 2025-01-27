using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class TraitTypeTraitPair
{
    public Mind.TraitType traitType;
    public Trait trait;
}
public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private List<TraitTypeTraitPair> _traitsInPlay = new();

    public bool UIClicked { get; set; }
    public bool BlockingPlayerUIOnScreen { get; set; }

    public bool CantClickOffInteractionMenu { get; set; }

    public void UpdateInteractionMenu(Character characterSpeaking, string currentDialogue)
    {
        var characterLabel = characterSpeaking.CharacterName.ToString();
        if(characterSpeaking==WorldManager.Instance.ThePlayer) 
        {
            characterLabel = "You";
        }
        WorldManager.Instance.ThePlayer.MenuInteraction.UpdateInteractionMenu(characterLabel, currentDialogue);
    }
    public void CloseInteractionMenu()
    {
        StartCoroutine(CloseInteractionMenuDelayed());
    }
    public void OpenDialoguePlayer(Character personSpeaking)
    {
        WorldManager.Instance.ThePlayer.MenuInteraction.OpenDialoguePlayer(personSpeaking);
    }
    public Character GetPersonWeAreSpeakingTo()
    {
       return WorldManager.Instance.ThePlayer.MenuInteraction.PersonWeAreSpeakingTo;
    }
    private IEnumerator CloseInteractionMenuDelayed()
    {
        yield return null; // Wait until the next frame
        WorldManager.Instance.ThePlayer.MenuInteraction.CloseInteractionMenu();
    }
    public Character IsInDialogueMenu(Character character)
    {
        if(character is Player || character== WorldManager.Instance.ThePlayer.MenuInteraction.PersonWeAreSpeakingTo)
        {
            return character;
        }
        return null;
    }
    private void Awake()
    {
        DiaCharacterFileHelper.InitializeAccessCounts();
    }
    private void Start()
    {
        CantClickOffInteractionMenu=true;
        UIClicked = false;
        BlockingPlayerUIOnScreen = false;
    }
    public Dictionary<Mind.TraitType, Trait> TraitsInPlay
    {
        get
        {
            Dictionary<Mind.TraitType, Trait> dictionary = new();
            foreach (var pair in _traitsInPlay)
            {
                dictionary[pair.traitType] = pair.trait;
            }
            return dictionary;
        }
        set
        {
            _traitsInPlay.Clear();
            foreach (var kvp in value)
            {
                _traitsInPlay.Add(new TraitTypeTraitPair { traitType = kvp.Key, trait = kvp.Value });
            }
        }
    }
}
