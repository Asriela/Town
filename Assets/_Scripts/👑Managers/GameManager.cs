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

    private void Start()
    {
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
