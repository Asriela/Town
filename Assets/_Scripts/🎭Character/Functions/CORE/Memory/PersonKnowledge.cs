using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

[Serializable]
public class CharacterMemoryTagsPair
{
    public Character character;                 // The character
    public List<Mind.MemoryTags> memoryTags = new(); // List of memory tags associated with the character
}

[Serializable]
public class CharacterKnowledgeTagsPair
{
    public Character knower;                             // The character with knowledge
    public List<CharacterMemoryTagsPair> knowledge = new(); // Knowledge about other characters
}

public class PersonKnowledge : MonoBehaviour
{
    private Character _character;
    public void Initialize(Character character) => _character = character;
    [SerializeField]
    private List<CharacterKnowledgeTagsPair> _peopleKnowledge = new();

    // Expose the data as a dictionary for easier lookup
    public Dictionary<Character, Dictionary<Character, List<Mind.MemoryTags>>> PeopleKnowledge
    {
        get
        {
            return _peopleKnowledge.ToDictionary(
                knowerPair => knowerPair.knower,
                knowerPair => knowerPair.knowledge.ToDictionary(
                    knowledgePair => knowledgePair.character,
                    knowledgePair => knowledgePair.memoryTags
                )
            );
        }
    }
    public Dictionary<Character, List<Mind.MemoryTags>> GetKnowledgeOf(Character knower)
    {
        var knowerPair = _peopleKnowledge.FirstOrDefault(kp => kp.knower == knower);

        // If knower exists, return the dictionary of the person's knowledge about others
        if (knowerPair != null)
        {
            return knowerPair.knowledge.ToDictionary(k => k.character, k => k.memoryTags);
        }

        // If knower doesn't exist, return an empty dictionary
        return new Dictionary<Character, List<Mind.MemoryTags>>();
    }
    // Retrieve a list of people who have all the specified memory tags for a specific knower
    public List<Character> GetPeopleByTag(Character knower, params Mind.MemoryTags[] tags)
    {
        var knowerPair = _peopleKnowledge.FirstOrDefault(kp => kp.knower == knower);

        return knowerPair?.knowledge
            .Where(k => tags.All(tag =>
                k.memoryTags.Any(memoryTag => _character.Generalizations.IsTagA(memoryTag, tag))  // Check if the tag is under the generalization
            ))
            .Select(k => k.character)
            .ToList() ?? new List<Character>();
    }
    // Add knowledge about a person with a list of memory tags
    public void AddKnowledge(Character knower, Character person, List<Mind.MemoryTags> tags)
    {
        var knowerPair = _peopleKnowledge.FirstOrDefault(kp => kp.knower == knower);

        if (knowerPair == null)
        {
            _peopleKnowledge.Add(new CharacterKnowledgeTagsPair
            {
                knower = knower,
                knowledge = new List<CharacterMemoryTagsPair>
                {
                    new CharacterMemoryTagsPair
                    {
                        character = person,
                        memoryTags = new List<Mind.MemoryTags>(tags)
                    }
                }
            });
        }
        else
        {
            var knowledgePair = knowerPair.knowledge.FirstOrDefault(k => k.character == person);
            if (knowledgePair == null)
            {
                knowerPair.knowledge.Add(new CharacterMemoryTagsPair
                {
                    character = person,
                    memoryTags = new List<Mind.MemoryTags>(tags)
                });
            }
            else
            {
                foreach (var tag in tags.Where(tag => !knowledgePair.memoryTags.Contains(tag)))
                {
                    knowledgePair.memoryTags.Add(tag);
                }
            }
        }
    }

    // Retrieve knowledge about a specific character
    public List<Mind.MemoryTags> GetKnowledge(Character knower, Character person)
    {
        var knowerPair = _peopleKnowledge.FirstOrDefault(kp => kp.knower == knower);
        var knowledgePair = knowerPair?.knowledge.FirstOrDefault(k => k.character == person);

        return knowledgePair?.memoryTags ?? new List<Mind.MemoryTags>();
    }

    public List<Mind.MemoryTags> GetRandomKnowledge(Character knower, Character person)
    {
        // Find the knower pair from the knowledge collection
        var knowerPair = _peopleKnowledge.FirstOrDefault(kp => kp.knower == knower);

        // If no knower pair is found, return an empty list
        if (knowerPair == null)
        {
            return new List<Mind.MemoryTags>();
        }

        // Find the knowledge pair for the given person
        var knowledgePair = knowerPair.knowledge.FirstOrDefault(k => k.character == person);

        // If no knowledge pair is found or memory tags are empty, return an empty list
        if (knowledgePair == null || knowledgePair.memoryTags.Count == 0)
        {
            return new List<Mind.MemoryTags>();
        }

        // Randomly select a memory tag from the knowledge
        var randomIndex = UnityEngine.Random.Range(0, knowledgePair.memoryTags.Count);

        // Return the randomly selected memory tag as a list (if you need it wrapped in a list)
        return new List<Mind.MemoryTags> { knowledgePair.memoryTags[randomIndex] };
    }


    // Check if a knower has specific knowledge about a person
    public bool HasKnowledge(Character knower, Character person, Mind.MemoryTags tag)
    {
        var knowerPair = _peopleKnowledge.FirstOrDefault(kp => kp.knower == knower);
        var knowledgePair = knowerPair?.knowledge.FirstOrDefault(k => k.character == person);

        return knowledgePair?.memoryTags.Contains(tag) ?? false;
    }

    // Remove knowledge about a specific tag for a person
    public void RemoveKnowledgeTag(Character knower, Character person, Mind.MemoryTags tag)
    {
        var knowerPair = _peopleKnowledge.FirstOrDefault(kp => kp.knower == knower);
        var knowledgePair = knowerPair?.knowledge.FirstOrDefault(k => k.character == person);

        knowledgePair?.memoryTags.Remove(tag);
    }

    // Retrieve a list of knowers who have all the specified tags about a specific character
    public List<Character> GetKnowersByTags(Character person, params Mind.MemoryTags[] tags)
    {
        return _peopleKnowledge
            .Where(kp => kp.knowledge.Any(k => k.character == person && tags.All(tag => k.memoryTags.Contains(tag))))
            .Select(kp => kp.knower)
            .ToList();
    }
    public bool HasMemoryTag(Character person, params Mind.MemoryTags[] tags)
    {
        bool answ = false;
        foreach (var kp in _peopleKnowledge)
        {
            if (kp.knower == person)
            {
                foreach (var k in kp.knowledge)
                {


                    Debug.Log($"Checking tags for person: {person.name}");
                    foreach (var tag in tags)
                    {
                        Debug.Log($"Looking for tag: {tag}, Found: {k.memoryTags.Contains(tag)}");
                        answ= k.memoryTags.Contains(tag);
                    }
                }
            }
        } 



        return answ;

    }
    public List<Mind.MemoryTags> GetAllCharacterTags(Character knower, Character person)
    {
        return GetKnowledge(knower, person);
    }


    // Retrieve all characters who have data in the system
    public List<Character> GetAllCharactersWeHaveDataOn()
    {
        return _peopleKnowledge.Select(kp => kp.knower).ToList();
    }

    // Retrieve all characters a specific knower has data about
    public List<Character> GetAllCharactersPersonHasDataOn(Character knower)
    {
        var knowerPair = _peopleKnowledge.FirstOrDefault(kp => kp.knower == knower);

        return knowerPair?.knowledge.Select(k => k.character).ToList() ?? new List<Character>();
    }
    public bool DoWeHavePersonKnowledge(Character knower)
    {
        // Retrieve the knower's knowledge
        var knowerPair = _peopleKnowledge.FirstOrDefault(kp => kp.knower == knower);

        // If knower exists and has knowledge data, return true, meaning the knower has knowledge about someone
        if (knowerPair != null && knowerPair.knowledge.Any())
        {
            return true;
        }

        // If the knower has no knowledge, return false
        return false;
    }
    public bool DoWeHavePersonKnowledgeOn(Character knower, Character aboutwho)
    {
        // Retrieve the knower's knowledge
        var knowerPair = _peopleKnowledge.FirstOrDefault(kp => kp.knower == knower);

        // If knower exists and has knowledge about the specific person
        if (knowerPair != null)
        {
            // Check if the knower has knowledge about the specified person
            var knowledgePair = knowerPair.knowledge.FirstOrDefault(k => k.character == aboutwho);

            // If knowledge exists for that person, return true
            if (knowledgePair != null && knowledgePair.memoryTags.Any())
            {
                return true;
            }
        }

        // If the knower doesn't have knowledge about the person, return false
        return false;
    }

}
