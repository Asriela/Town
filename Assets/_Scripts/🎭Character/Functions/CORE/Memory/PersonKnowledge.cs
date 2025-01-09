using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;


[Serializable]
public class CharacterTagsPair
{
    public Character character;
    public List<Mind.MemoryTags> tags = new();
}

//TODO: split up tags so its easier to sort

public class PersonKnowledge : MonoBehaviour
{
    [SerializeField]
    private List<CharacterTagsPair> _peopleKnowledge = new();

    public Dictionary<Character, List<Mind.MemoryTags>> PeopleKnowledge
    {
        get
        {
            Dictionary<Character, List<Mind.MemoryTags>> dictionary = new();
            foreach (var pair in _peopleKnowledge)
            {
                dictionary[pair.character] = pair.tags;
            }
            return dictionary;
        }
    }

    public void AddPerson(Character person, List<Mind.MemoryTags> tags)
    {
        var existingPerson = _peopleKnowledge.FirstOrDefault(p => p.character == person);

        if (existingPerson == null)
        {
            _peopleKnowledge.Add(new CharacterTagsPair
            {
                character = person,
                tags = tags
            });
        }
        else
        {
            foreach (var tag in tags)
            {
                if (!existingPerson.tags.Contains(tag))
                {
                    existingPerson.tags.Add(tag);
                }
            }
        }
    }

    public List<Character> GetPeopleByTag(params Mind.MemoryTags[] tags) =>
        _peopleKnowledge
            .Where(p => tags.All(tag => p.tags.Contains(tag)))
            .Select(p => p.character)
            .ToList();

    public List<Mind.MemoryTags> GetAllCharacterTags(Character character)
    {
        // Find the CharacterTagsPair for the given character
        var characterTagsPair = _peopleKnowledge.FirstOrDefault(p => p.character == character);

        // Return the tags if found, otherwise return an empty list
        return characterTagsPair != null ? characterTagsPair.tags : new List<Mind.MemoryTags>();
    }
}
