using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class VisualStatusKnowledge : MonoBehaviour
{
    [SerializeField]
    private List<CharacterKnowledgeTagsPair> _peopleVisualStatus = new(); // Reusing the same pair structure

    // Expose the data as a dictionary for easier lookup
    public Dictionary<Character, Dictionary<Character, List<Mind.MemoryTags>>> PeopleVisualStatus
    {
        get
        {
            return _peopleVisualStatus.ToDictionary(
                knowerPair => knowerPair.knower,
                knowerPair => knowerPair.knowledge.ToDictionary(
                    knowledgePair => knowledgePair.character,
                    knowledgePair => knowledgePair.memoryTags
                )
            );
        }
    }

    public Dictionary<Character, List<Mind.MemoryTags>> GetVisualStatusOf(Character viewer)
    {
        var viewerPair = _peopleVisualStatus.FirstOrDefault(kp => kp.knower == viewer);

        // If viewer exists, return the dictionary of the person's visual status about others
        if (viewerPair != null)
        {
            return viewerPair.knowledge.ToDictionary(k => k.character, k => k.memoryTags);
        }

        // If viewer doesn't exist, return an empty dictionary
        return new Dictionary<Character, List<Mind.MemoryTags>>();
    }

    // Retrieve a list of people who have all the specified visual status tags for a specific viewer
    public List<Character> GetPeopleByVisualStatus(Character viewer, params Mind.MemoryTags[] tags)
    {
        var viewerPair = _peopleVisualStatus.FirstOrDefault(kp => kp.knower == viewer);

        return viewerPair?.knowledge
            .Where(k => tags.All(tag => k.memoryTags.Contains(tag)))
            .Select(k => k.character)
            .ToList() ?? new List<Character>();
    }

    // Add visual status about a person with a list of memory tags
    public void AddVisualStatus(Character viewer, Character person, List<Mind.MemoryTags> tags)
    {
        var viewerPair = _peopleVisualStatus.FirstOrDefault(kp => kp.knower == viewer);

        if (viewerPair == null)
        {
            _peopleVisualStatus.Add(new CharacterKnowledgeTagsPair
            {
                knower = viewer,
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
            var statusPair = viewerPair.knowledge.FirstOrDefault(k => k.character == person);
            if (statusPair == null)
            {
                viewerPair.knowledge.Add(new CharacterMemoryTagsPair
                {
                    character = person,
                    memoryTags = new List<Mind.MemoryTags>(tags)
                });
            }
            else
            {
                foreach (var tag in tags.Where(tag => !statusPair.memoryTags.Contains(tag)))
                {
                    statusPair.memoryTags.Add(tag);
                }
            }
        }
    }

    // Retrieve visual status about a specific character
    public List<Mind.MemoryTags> GetVisualStatus(Character viewer, Character person)
    {
        var viewerPair = _peopleVisualStatus.FirstOrDefault(kp => kp.knower == viewer);
        var statusPair = viewerPair?.knowledge.FirstOrDefault(k => k.character == person);

        return statusPair?.memoryTags ?? new List<Mind.MemoryTags>();
    }

    // Check if a viewer has specific visual status about a person
    public bool HasVisualStatus(Character viewer, Character person, Mind.MemoryTags tag)
    {
        var viewerPair = _peopleVisualStatus.FirstOrDefault(kp => kp.knower == viewer);
        var statusPair = viewerPair?.knowledge.FirstOrDefault(k => k.character == person);

        return statusPair?.memoryTags.Contains(tag) ?? false;
    }

    // Remove visual status about a specific tag for a person
    public void RemoveVisualStatusTag(Character viewer, Character person, Mind.MemoryTags tag)
    {
        var viewerPair = _peopleVisualStatus.FirstOrDefault(kp => kp.knower == viewer);
        var statusPair = viewerPair?.knowledge.FirstOrDefault(k => k.character == person);

        statusPair?.memoryTags.Remove(tag);
    }

    // Retrieve a list of viewers who have all the specified visual status tags about a specific character
    public List<Character> GetViewersByVisualStatusTags(Character person, params Mind.MemoryTags[] tags)
    {
        return _peopleVisualStatus
            .Where(kp => kp.knowledge.Any(k => k.character == person && tags.All(tag => k.memoryTags.Contains(tag))))
            .Select(kp => kp.knower)
            .ToList();
    }

    public bool HasVisualStatusTag(Character person, params Mind.MemoryTags[] tags)
    {
        bool answ = false;
        foreach (var kp in _peopleVisualStatus)
        {
            if (kp.knower == person)
            {
                foreach (var k in kp.knowledge)
                {
                    Debug.Log($"Checking visual status tags for person: {person.name}");
                    foreach (var tag in tags)
                    {
                        Debug.Log($"Looking for visual status tag: {tag}, Found: {k.memoryTags.Contains(tag)}");
                        answ = k.memoryTags.Contains(tag);
                    }
                }
            }
        }

        return answ;
    }

    public List<Mind.MemoryTags> GetAllCharacterVisualStatusTags(Character viewer, Character person)
    {
        return GetVisualStatus(viewer, person);
    }

    // Retrieve all characters who have visual status data in the system
    public List<Character> GetAllCharactersWithVisualStatus()
    {
        return _peopleVisualStatus.Select(kp => kp.knower).ToList();
    }

    // Retrieve all characters a specific viewer has visual status data on
    public List<Character> GetAllCharactersViewerHasVisualStatusOn(Character viewer)
    {
        var viewerPair = _peopleVisualStatus.FirstOrDefault(kp => kp.knower == viewer);

        return viewerPair?.knowledge.Select(k => k.character).ToList() ?? new List<Character>();
    }
}
